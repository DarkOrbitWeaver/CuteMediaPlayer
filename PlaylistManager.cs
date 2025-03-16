﻿using System;
using System.Collections.Generic;
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

        private Playlist SelectPlaylistDialog()
        {
            // No playlists exist
            if (allPlaylists.Count == 0)
            {
                MessageBox.Show("Create a playlist first!");
                return null;
            }

            using (PlaylistDialog dialog = new PlaylistDialog())
            {
                // Add playlist names to listbox
                dialog.PlaylistListBox.Items.AddRange(allPlaylists.Select(p => p.Name).ToArray());

                // Show the dialog
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    //  Check if user actually selected something
                    if (dialog.PlaylistListBox.SelectedIndex == -1)
                    {
                        MessageBox.Show("Please select a playlist first!");
                        return null;
                    }

                    // Return safe selected playlist
                    return allPlaylists[dialog.PlaylistListBox.SelectedIndex];
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
            allPlaylists.Clear();
            Directory.CreateDirectory(playlistsFolder);

            foreach (var file in Directory.GetFiles(playlistsFolder, "*.m3u"))
            {
                try
                {
                    var pl = new Playlist
                    {
                        Name = Path.GetFileNameWithoutExtension(file),
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

            // Clear existing items
            customPlaylistsPanel.ClearTracks();

            // Add playlist items
            foreach (var playlist in allPlaylists)
            {
                // Add playlist as a special item
                customPlaylistsPanel.AddTrack($"PLAYLIST:{playlist.Name}");
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


        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Get the selected track
            CustomTrackItem selectedItem = GetSelectedTrackItem();
            if (selectedItem == null) return;

            int index = currentPlaylist.Tracks.IndexOf(selectedItem.FilePath);
            if (index != -1)
            {
                // Remove from both playlist and control
                currentPlaylist.Tracks.RemoveAt(index);
                SyncPlaylistToUI();
            }
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
        private void menuSavedPlaylists_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Check if the right-clicked item is a playlist
            var item = GetSelectedTrackItem();
            deleteToolStripMenuItem.Enabled = (item != null) && item.FilePath.StartsWith("PLAYLIST:");
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedItem = GetSelectedTrackItem();
            if (selectedItem == null) return;

            // Confirm deletion with cute message 💬
            DialogResult result = MessageBox.Show("Delete this playlist forever?",
                "So sad... 😢",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
 
                try
                {
                    int index = allPlaylists.FindIndex(p => p.Name == selectedItem.Title);
                    if (index != -1)
                    {
                        allPlaylists.RemoveAt(index);
                        customPlaylistsPanel.ClearTracks();
                        allPlaylists.ForEach(pl => customPlaylistsPanel.AddTrack($"PLAYLIST:{pl.Name}"));
                        SaveAllPlaylists();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting playlist: {ex.Message}");
                    return;
                }

            }
        }

        // when i right click song and play it
        private void playToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Get the selected track
            CustomTrackItem selectedItem = GetSelectedTrackItem();
            if (selectedItem == null) return;

            int index = currentPlaylist.Tracks.IndexOf(selectedItem.FilePath);
            if (index != -1)
            {
                // Update current track index
                currentTrackIndex = index;

                // Play the selected track
                PlayCurrentTrack();

                // Update UI
                UpdateButtonStates();
                UpdateButtonImages();
            }
        }


        // keep the playlist in sync
        private void SyncPlaylistToUI()
        {
            try
            {
                // Update custom playlist panel
                customPlaylistPanel.SetTracks(currentPlaylist.Tracks);

                // Highlight current track if playing
                if (currentTrackIndex >= 0 && currentTrackIndex < currentPlaylist.Tracks.Count)
                {
                    customPlaylistPanel.SelectTrack(currentTrackIndex);
                }

                // Make sure playlist reference is in sync
                playlist = currentPlaylist.Tracks;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error syncing playlist: {ex.Message}");
            }
        }

        // adding songs to a playlist via the right click
        private void addToPlaylistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Get the selected track
            CustomTrackItem selectedItem = GetSelectedTrackItem();
            if (selectedItem == null) return;

            // Get selected song path
            string selectedFile = selectedItem.FilePath;

            // Select target playlist
            Playlist target = SelectPlaylistDialog();
            if (target == null) return;

            // Check for existing
            if (!target.Tracks.Contains(selectedFile))
            {
                // Add to existing tracks
                target.Tracks.Add(selectedFile);
                SaveAllPlaylists();
                MessageBox.Show("Added to playlist!");
            }
            else
            {
                MessageBox.Show("Song already exists in this playlist!");
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
            Playlist target = SelectPlaylistDialog();
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
            Playlist target = SelectPlaylistDialog();
            if (target == null) return;

            // Check if already exists
            if (!target.Tracks.Contains(currentFile))
            {
                // Add to existing tracks (this is the fix - we're adding to the existing list)
                target.Tracks.Add(currentFile);
                SaveAllPlaylists();
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

            // 📦 Create context menu for tracks
            customPlaylistPanel.ContextMenuStrip = menuPlaylist;

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

        // 🔍 Gets selected track item from EITHER playlist or track context menu
        private CustomTrackItem GetSelectedTrackItem()
        {
            // Check both possible context menu sources
            var sourcePanel = menuPlaylist.SourceControl ?? menuSavedPlaylists.SourceControl;

            if (sourcePanel is CustomPlaylistPanel panel)
            {
                // Get click position relative to the panel
                Point clickPos = panel.PointToClient(Cursor.Position);

                // Find control at click position
                Control clickedControl = panel.GetChildAtPoint(clickPos);

                // Return if it's a track item
                return clickedControl as CustomTrackItem;
            }
            return null;
        }




        // for the new ui
        private void CustomPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Select the track under the cursor
                CustomPlaylistPanel panel = (CustomPlaylistPanel)sender;
                foreach (Control control in panel.Controls)
                {
                    if (control is CustomTrackItem && control.Bounds.Contains(e.Location))
                    {
                        // Ensure the track is selected before showing context menu
                        CustomTrackItem item = (CustomTrackItem)control;

                        // Force selection on right-click
                        item.SetPlaying(true); // Visual feedback

                        // Show different context menu based on which panel was clicked
                        if (panel == customPlaylistPanel)
                        {
                            menuPlaylist.Show(panel, e.Location);
                        }
                        else
                        {
                            menuSavedPlaylists.Show(panel, e.Location);
                        }
                        break;
                    }
                }
            }
        }

    }
}
