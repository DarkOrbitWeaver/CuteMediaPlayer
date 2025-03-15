using System;
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

            listSavedPlaylists.Items.AddRange(allPlaylists.Select(p => p.Name).ToArray());
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
            listSavedPlaylists.Items.Add(finalName);

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
                    allPlaylists.Remove(newPl);
                    listSavedPlaylists.Items.Remove(finalName);
                }
            }
        }

        // this plays the selected track
        private void listPlaylist_DoubleClick(object sender, EventArgs e)
        {
            if (listPlaylist.SelectedIndex != -1)
            {
                // Simply set currentTrackIndex to the selected index
                currentTrackIndex = listPlaylist.SelectedIndex;
                PlayCurrentTrack();
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listPlaylist.SelectedIndex == -1) return;

            // Remove from both playlist and listbox
            playlist.RemoveAt(listPlaylist.SelectedIndex);
            listPlaylist.Items.RemoveAt(listPlaylist.SelectedIndex);
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

            listPlaylist.Items.Clear();
            listPlaylist.Items.AddRange(currentPlaylist.Tracks.Select(Path.GetFileName).ToArray());
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
            deleteToolStripMenuItem.Enabled = (listSavedPlaylists.SelectedIndex != -1);
        }
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listSavedPlaylists.SelectedIndex == -1) return;

            // Confirm deletion with cute message 💬
            DialogResult result = MessageBox.Show("Delete this playlist forever?",
                "So sad... 😢",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
 
                try
                {
                    var pl = allPlaylists[listSavedPlaylists.SelectedIndex];

                    // Delete file
                    File.Delete(Path.Combine(playlistsFolder, $"{pl.Name}.m3u"));

                    // Update UI
                    allPlaylists.RemoveAt(listSavedPlaylists.SelectedIndex);
                    listSavedPlaylists.Items.RemoveAt(listSavedPlaylists.SelectedIndex);
                    SaveAllPlaylists();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting playlist: {ex.Message}");
                    return;
                }

            }
        }

        // okay when i double click a playlist it switch to it and load it 
        private void listSavedPlaylists_DoubleClick(object sender, EventArgs e)
        {
            if (listSavedPlaylists.SelectedIndex == -1) return;

            // Load selected playlist
            currentPlaylist = allPlaylists[listSavedPlaylists.SelectedIndex];
            playlist = currentPlaylist.Tracks;

            // 🎵 Auto-play first track if playlist isn't empty
            if (playlist.Count > 0)
            {
                currentTrackIndex = 0; // Start from first track
                PlayCurrentTrack();   // Play it!
            }

            // Update UI
            listPlaylist.Items.Clear();
            listPlaylist.Items.AddRange(currentPlaylist.Tracks.Select(Path.GetFileName).ToArray());
            tabControl1.SelectedTab = tabCurrent;
            isShuffled = false;
            shuffle.BackColor = Color.FromArgb(254, 184, 195);
            SyncPlaylistToUI();
            UpdateButtonStates();
            UpdateButtonImages();
        }

        // when i right click song and play it
        private void playToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listPlaylist.SelectedIndex != -1)
            {
                // Update current track index
                currentTrackIndex = listPlaylist.SelectedIndex;

                // Play the selected track
                PlayCurrentTrack();

                // Update UI
                UpdateButtonStates();
                UpdateButtonImages();
            }
        }

        // when i double click a song from the playlist
        private void listPlaylist_Click(object sender, EventArgs e)
        {
            if (listPlaylist.SelectedIndex != -1)
            {
                // Update current track index
                currentTrackIndex = listPlaylist.SelectedIndex;

                // Play the selected track
                PlayCurrentTrack();

                // Update UI
                UpdateButtonStates();
            }
        }

        // keep the playlist in sync
        private void SyncPlaylistToUI()
        {
            try
            {
                // Clear the playlist UI
                listPlaylist.Items.Clear();

                // Add all tracks from current playlist
                foreach (string track in currentPlaylist.Tracks)
                {
                    listPlaylist.Items.Add(Path.GetFileName(track));
                }

                // Make sure playlist reference is in sync
                playlist = currentPlaylist.Tracks;

                // Highlight current track if playing
                if (currentTrackIndex >= 0 && currentTrackIndex < listPlaylist.Items.Count)
                {
                    listPlaylist.SelectedIndex = currentTrackIndex;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error syncing playlist: {ex.Message}");
            }
        }

        // adding songs to a playlist via the right click
        private void addToPlaylistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listPlaylist.SelectedIndex == -1) return;

            // Get selected song
            string selectedFile = currentPlaylist.Tracks[listPlaylist.SelectedIndex];

            // Select target playlist
            Playlist target = SelectPlaylistDialog();
            if (target == null) return;

            // Check for existing
            if (!target.Tracks.Contains(selectedFile))
            {
                // Add to existing tracks (this is the fix - we're adding to the existing list)
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

    }
}
