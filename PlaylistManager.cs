using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuteMediaPlayer
{
    public partial class Form1
    {

        private List<string> playlist = new List<string>();
        private int currentTrackIndex = -1;
        private List<Playlist> allPlaylists = new List<Playlist>();
        private Playlist currentPlaylist = new Playlist { Name = "Default" };
        private string playlistsFolder = Path.Combine(Application.StartupPath, "Playlists");
        // shuffle 
        private List<string> originalPlaylistOrder = new List<string>();
        private bool isShuffled = false;


        public class Playlist
        {
            public string Name { get; set; }
            public List<string> Tracks { get; set; } = new List<string>();
        }

        private Playlist SelectPlaylistDialog(string promptText = "")
        {
            // No playlists exist
            if (allPlaylists.Count == 0)
            {
                MessageBox.Show("Create a playlist first!");
                return null;
            }

            using (PlaylistDialog dialog = new PlaylistDialog(promptText))
            {
                // Pass the entire playlists collection instead of just names
                dialog.PopulatePlaylists(allPlaylists);

                // Show the dialog
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // Get the selected playlist
                    string selectedName = dialog.SelectedPlaylist;

                    // Find the corresponding playlist
                    if (!string.IsNullOrEmpty(selectedName))
                    {
                        return allPlaylists.FirstOrDefault(p => p.Name == selectedName);
                    }
                    else
                    {
                        MessageBox.Show("Please select a playlist first!");
                        return null;
                    }
                }
            }
            return null;
        }


        private void SaveAllPlaylists()
        {
            // Create folder if missing
            Directory.CreateDirectory(playlistsFolder);

            // Save each playlist
            foreach (var pl in allPlaylists)
            {
                File.WriteAllLines(Path.Combine(playlistsFolder, $"{pl.Name}.m3u"), pl.Tracks);
            }
        }

        private void LoadAllPlaylists()
        {
            bool wasUsingDefault = (currentPlaylist.Name == "Default");
            allPlaylists.Clear();
            Directory.CreateDirectory(playlistsFolder);

            foreach (var file in Directory.GetFiles(playlistsFolder, "*.m3u"))
            {
                try
                {
                    var pl = new Playlist
                    {
                        // Trim the playlist name to remove leading/trailing spaces
                        Name = Path.GetFileNameWithoutExtension(file).Trim(),
                        Tracks = File.ReadAllLines(file)
                                      .Where(line => File.Exists(line))
                                      .ToList()
                    };
                    allPlaylists.Add(pl);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading playlist '{file}': {ex.Message}");
                }
            }

            // Refresh the playlists panel
            customPlaylistsPanel.ClearTracks();
            foreach (var playlist in allPlaylists)
            {
                customPlaylistsPanel.AddTrack($"PLAYLIST:{playlist.Name}|{playlist.Tracks.Count}");
            }

            if (wasUsingDefault)
            {
                currentPlaylist = allPlaylists.FirstOrDefault(p => p.Name == "Default")
                                ?? new Playlist { Name = "Default" };
            }
        }

        private void NewPlaylist_Click(object sender, EventArgs e)
        {
            string baseName = "";
            string finalName = "";
            bool isValidName = false;

            // Keep asking for a name until valid or user cancels
            while (!isValidName)
            {
                using (var nameDialog = new NewPlaylistDialog())
                {
                    // Pre-fill the dialog with last generated name
                    nameDialog.PlaylistName = finalName;

                    // User clicked Cancel
                    if (nameDialog.ShowDialog() != DialogResult.OK) return;

                    baseName = nameDialog.PlaylistName.Trim();

                    // Empty name check
                    if (string.IsNullOrWhiteSpace(baseName))
                    {
                        baseName = "My Playlist";
                        finalName = baseName;
                        MessageBox.Show("Playlist name cannot be empty!");
                        continue;
                    }

                    // Check for duplicates
                    finalName = baseName;
                    int counter = 1;
                    bool nameExists;

                    do
                    {
                        nameExists = false;
                        // Simple loop instead of LINQ
                        foreach (Playlist p in allPlaylists)
                        {
                            if (string.Equals(p.Name, finalName, StringComparison.OrdinalIgnoreCase))
                            {
                                nameExists = true;
                                finalName = $"{baseName}_{counter}";
                                counter++;
                                break;
                            }
                        }
                    } while (nameExists);

                    // If name was changed
                    if (finalName != baseName)
                    {
                        DialogResult choice = MessageBox.Show(
                            $"Name taken! Use '{finalName}' instead?",
                            "Oops!",
                            MessageBoxButtons.YesNoCancel);

                        if (choice == DialogResult.Yes) isValidName = true;
                        else if (choice == DialogResult.Cancel) return;
                        // Else (No) - loop again
                    }
                    else
                    {
                        isValidName = true;
                    }
                }
            }

            // Create new playlist
            var newPl = new Playlist { Name = finalName };
            allPlaylists.Add(newPl);
            customPlaylistsPanel.AddTrack($"PLAYLIST:{finalName}");

            // Ask to add files
            DialogResult addFiles = MessageBox.Show(
                "Add files to this playlist now?",
                "Add Files",
                MessageBoxButtons.YesNo);

            if (addFiles == DialogResult.Yes)
            {
                using OpenFileDialog fileDialog = new OpenFileDialog()
                {
                    Title = "Select Media Files 🎧",
                    Multiselect = true,
                    Filter = FilesFilter
                };

                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Add files with simple duplicate check
                    foreach (string file in fileDialog.FileNames)
                    {
                        bool duplicateFound = false;
                        foreach (string track in newPl.Tracks)
                        {
                            if (string.Equals(track, file, StringComparison.OrdinalIgnoreCase))
                            {
                                duplicateFound = true;
                                break;
                            }
                        }

                        if (!duplicateFound) newPl.Tracks.Add(file);
                    }

                    // Save only if files added
                    if (newPl.Tracks.Count > 0) SaveAllPlaylists();
                }
                else
                {
                    // Remove empty playlist if user cancels file selection
                    // (No direct replacement needed - handled by customPlaylistsPanel)
                }
            }

            // refresh ui
            SyncPlaylistToUI();
        }


        // shuffle 
        private void shuffle_Click(object sender, EventArgs e)
        {
            isShuffled = !isShuffled;

            if (isShuffled)
            {
                if (originalPlaylistOrder.Count == 0)
                    originalPlaylistOrder = new List<string>(currentPlaylist.Tracks);

                // Shuffle currentPlaylist.Tracks directly
                currentPlaylist.Tracks = currentPlaylist.Tracks
                    .OrderBy(x => random.Next())
                    .ToList();

                playlist = currentPlaylist.Tracks; // Sync reference
                shuffle.BackgroundImage = Properties.Resources.ShuffleIcon;

            }
            else
            {
                currentPlaylist.Tracks = new List<string>(originalPlaylistOrder);
                playlist = currentPlaylist.Tracks; // Sync reference    
                shuffle.BackgroundImage = Properties.Resources.DisabledShuffleIcon;

            }

            customPlaylistPanel.SetTracks(currentPlaylist.Tracks);
            SaveAllPlaylists();
            SyncPlaylistToUI();
        }

        private void openPlaylist_Click(object sender, EventArgs e)
        {
            // 🎵 Toggle playlist visibility
            panelPlaylist.Visible = !panelPlaylist.Visible;
        }

        // playlist managment


        // keep the playlist in sync
        private void SyncPlaylistToUI()
        {
            try
            {

                if (currentPlaylist == null)
                {
                    currentPlaylist = new Playlist { Name = "Default" };
                }

                // Update custom playlist panel
                customPlaylistPanel.SetTracks(currentPlaylist.Tracks);

                // Highlight current track if playing
                if (currentTrackIndex >= 0 && currentTrackIndex < currentPlaylist.Tracks.Count)
                {
                    customPlaylistPanel.SelectTrack(currentTrackIndex);
                    UpdatePlaylistTitleDisplay();
                }

                // Make sure playlist reference is in sync
                playlist = currentPlaylist.Tracks;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error syncing playlist: {ex.Message}");
            }
        }


        // adding songs to a playlist
        private void btnAddToPlaylist_Click(object sender, EventArgs e)
        {
            // Check if current playlist has tracks
            if (currentPlaylist.Tracks.Count == 0)
            {
                MessageBox.Show("Current playlist is empty!");
                return;
            }

            // Let user choose a target playlist
            Playlist target = SelectPlaylistDialog("This will add all \"Now Playing\"  to the selected playlist");
            if (target == null) return; // User canceled

            // Add each track from current playlist to target (skip duplicates)
            foreach (string file in currentPlaylist.Tracks)
            {
                if (!target.Tracks.Contains(file))
                {
                    target.Tracks.Add(file);
                }
            }

            // Save changes
            SaveAllPlaylists();
            MessageBox.Show($"Added to '{target.Name}'! (¬_¬\")");
        }

        //
        private void btnAddCurrentToPlaylist_Click(object sender, EventArgs e)
        {
            // Check if any track is playing
            if (currentTrackIndex == -1 || playlist.Count == 0)
            {
                MessageBox.Show("No track is playing!");
                return;
            }

            // Get current track
            string currentFile = playlist[currentTrackIndex];

            // Select target playlist
            Playlist target = SelectPlaylistDialog("Add Currently playing track to a playlist:");
            if (target == null) return;

            // Check if already exists
            if (!target.Tracks.Contains(currentFile))
            {
                // Add to existing tracks 
                target.Tracks.Add(currentFile);
                SaveAllPlaylists();
                LoadAllPlaylists();
                MessageBox.Show("Added current song to playlist!");
            }
            else
            {
                MessageBox.Show("Song already exists in this playlist!");
            }
        }

        // ui
        private void InitializeCustomPlaylistControls()
        {
            // 🎵 Create custom playlist panel for Now Playing
            customPlaylistPanel = new CustomPlaylistPanel();
            customPlaylistPanel.Dock = DockStyle.Fill;
            customPlaylistPanel.TrackSelected += CustomPlaylistPanel_TrackSelected;
            customPlaylistPanel.AddToPlaylistRequested += CustomPlaylistPanel_AddToPlaylistRequested;


            // 🗂️ Create custom playlists panel for My Playlists
            customPlaylistsPanel = new CustomPlaylistPanel();
            customPlaylistsPanel.Dock = DockStyle.Fill;
            customPlaylistsPanel.TrackSelected += CustomPlaylistsPanel_TrackSelected;
            customPlaylistsPanel.ContextMenuStrip = menuSavedPlaylists;

            // ➕ Replace the ListBox controls with our custom panels
            tabCurrent.Controls.Add(customPlaylistPanel);
            tabLibrary.Controls.Add(customPlaylistsPanel);

            // 🔄 Set tab order
            tabCurrent.Controls.SetChildIndex(customPlaylistPanel, 0);
            tabLibrary.Controls.SetChildIndex(customPlaylistsPanel, 0);

            customPlaylistPanel.AddSongsClicked += (s, e) =>
            {
                OpenToolStripMenuItemHelper(); // 🔗 Connect to existing open dialog
            };

            customPlaylistsPanel.PlaylistDeleted += (s, e) =>
            {
 
                CustomTrackItem item = e as CustomTrackItem;
                Debug.WriteLine("file path" + item?.FilePath.StartsWith("P"));

                if (item != null && item.FilePath.StartsWith("PLAYLIST:"))
                {
                    try
                    {
                        int index = allPlaylists.FindIndex(p => string.Equals(p.Name, item.Title, StringComparison.OrdinalIgnoreCase));

                        if (index != -1 && allPlaylists[index].Name == currentPlaylist.Name)
                        {
                            // Reset current playlist if deleting active one
                            currentPlaylist = new Playlist { Name = "Default" };
                            playlist = currentPlaylist.Tracks;
                            currentTrackIndex = -1; // Reset selection
                        }

                        if (index != -1)
                        {
                            string playlistPath = Path.Combine(playlistsFolder, $"{allPlaylists[index].Name}.m3u");
                            if (File.Exists(playlistPath))
                            {
                                File.Delete(playlistPath); // 🗑️ Delete the file
                            }
                            allPlaylists.RemoveAt(index); // Remove from list

                            // Force-refresh the playlists panel
                            customPlaylistsPanel.ClearTracks();

                            foreach (var pl in allPlaylists)
                            {
                                customPlaylistsPanel.AddTrack($"PLAYLIST:{pl.Name}");
                            }

                            customPlaylistsPanel.Refresh(); // ⚡ Force UI update
                            SaveAllPlaylists();
                            LoadAllPlaylists(); // Reload all playlists properly
                            SyncPlaylistToUI();
                            UpdateButtonStates();
                            UpdateButtonImages();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting playlist: {ex.Message}");
                    }
                }
            };
            customPlaylistPanel.Controls.OfType<CustomTrackItem>().ToList().ForEach(item =>
            {
                item.AddToPlaylistRequested += (s, e) =>
                {
                    CustomTrackItem trackItem = s as CustomTrackItem;
                    if (trackItem != null)
                    {
                        // Use existing logic to add track to playlist
                        Playlist target = SelectPlaylistDialog();
                        if (target != null && !target.Tracks.Contains(trackItem.FilePath))
                        {
                            target.Tracks.Add(trackItem.FilePath);
                            SaveAllPlaylists();
                            MessageBox.Show("Added to playlist!");
                        }
                    }
                };
            });
            customPlaylistPanel.TrackRemovedFromPlaylist += (sender, trackPath) =>
            {
                if (currentPlaylist != null && currentPlaylist.Tracks.Contains(trackPath))
                {
                    currentPlaylist.Tracks.Remove(trackPath);
                    SaveAllPlaylists(); // 💾 Save to file
                    SyncPlaylistToUI(); // 🔄 Update UI
                }
            };
        }


        // 🎯 Handle track selection in Now Playing list
        private void CustomPlaylistPanel_TrackSelected(object sender, CustomTrackItem track)
        {
            // Find the index of the selected track
            int index = currentPlaylist.Tracks.IndexOf(track.FilePath);
            if (index != -1)
            {
                // Update current track index
                currentTrackIndex = index;

                // Play the selected track
                PlayCurrentTrack();

                // Update UI
                customPlaylistPanel.SelectTrack(currentTrackIndex);
            }
        }

        // 🎯 Handle playlist selection in My Playlists
        private void CustomPlaylistsPanel_TrackSelected(object sender, CustomTrackItem track)
        {

            if (!track.FilePath.StartsWith("PLAYLIST:")) return; // Safety check

            // Find the index of the selected playlist
            int index = -1;
            for (int i = 0; i < allPlaylists.Count; i++)
            {
                if (allPlaylists[i].Name == track.Title)
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                // Load selected playlist
                currentPlaylist = allPlaylists[index];
                playlist = currentPlaylist.Tracks;

                // Update UI
                customPlaylistPanel.SetTracks(currentPlaylist.Tracks);
                UpdatePlaylistTitleDisplay();

                // 🎵 Auto-play first track if playlist isn't empty
                if (playlist.Count > 0)
                {
                    currentTrackIndex = 0; // Start from first track
                    PlayCurrentTrack();   // Play it!
                    customPlaylistPanel.SelectTrack(currentTrackIndex);
                }

                // Update UI
                tabControl1.SelectedTab = tabCurrent;
                isShuffled = false;
                shuffle.BackColor = Color.FromArgb(254, 184, 195);
                UpdateButtonStates();
                UpdateButtonImages();
            }
        }

        private void CustomPlaylistPanel_AddToPlaylistRequested(object sender, CustomTrackItem track)
        {
            // Use existing logic from SelectPlaylistDialog
            Playlist target = SelectPlaylistDialog();

            if (target != null)
            {
                // Check if track already exists in the playlist
                if (!target.Tracks.Contains(track.FilePath))
                {
                    target.Tracks.Add(track.FilePath);
                    SaveAllPlaylists();
                    MessageBox.Show($"Added '{track.Title}' to playlist '{target.Name}'!");
                }
                else
                {
                    MessageBox.Show("This track is already in the selected playlist!");
                }
            }
        }

        //
        private void UpdatePlaylistTitleDisplay()
        {
            if (currentPlaylist != null)
            {
                // Show the current playlist name and track count
                int trackCount = currentPlaylist.Tracks.Count;
                string trackText = trackCount == 1 ? "track" : "tracks"; // Proper pluralization

                lblCurrentPlaylist.Text = $"Playlist Name: {currentPlaylist.Name} ({trackCount} {trackText})";
            }
            else
            {
                // No playlist loaded
                lblCurrentPlaylist.Text = "";
            }
        }

    }
}
