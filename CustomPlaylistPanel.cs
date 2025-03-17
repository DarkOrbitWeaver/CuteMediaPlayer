using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;

namespace CuteMediaPlayer
{
    public partial class CustomPlaylistPanel : UserControl
    {
        // 📦 Store track items for access
        private List<CustomTrackItem> trackItems = new List<CustomTrackItem>();

        // 📣 Events for track selection
        public event EventHandler<CustomTrackItem> TrackSelected;
        // one for adding new songs via explorer
        public event EventHandler AddSongsClicked;

        public delegate void PlaylistItemEventHandler(object sender, CustomTrackItem item);
        public event PlaylistItemEventHandler PlaylistDeleted;

        public event EventHandler<CustomTrackItem> AddToPlaylistRequested;
        public event EventHandler<string> TrackRemovedFromPlaylist;

        public CustomPlaylistPanel()
        {
            InitializeComponent();
            this.AutoScroll = true; // Enable scrolling
        }

        // 🔄 Set tracks from a list of file paths
        public void SetTracks(List<string> filePaths)
        {
            // Clear existing tracks
            ClearTracks();

            // Add new tracks
            if (filePaths != null)
            {
                foreach (string path in filePaths)
                {
                    AddTrack(path);
                }
            }

            // toggle empty label visiblity
            ShowListIsEmptyLabel();
        }

        // ➕ Add a single track
        public void AddTrack(string filePath)
        {
            CustomTrackItem item = new CustomTrackItem();
            item.SetTrackData(filePath);

            // 🎧 Listen for removal events
            item.TrackRemoved += (s, e) =>
            {
                this.Controls.Remove(item);
                trackItems.Remove(item);
                ReorderTrackPositions();
                ShowListIsEmptyLabel();

                // 🚨 Fire the new event (sends track path to parent)
                TrackRemovedFromPlaylist?.Invoke(this, item.FilePath);
            };

            item.AddToPlaylistRequested += (s, e) =>
            {
                // Forward the event to the parent control with the track item
                AddToPlaylistRequested?.Invoke(this, item);
            };

            // Set position (stack from top)
            item.Width = this.ClientSize.Width;
            item.Top = trackItems.Count * item.Height;
            item.Left = 0;
            item.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // Add to control
            this.Controls.Add(item);
            trackItems.Add(item);

            // Handle track selection
            item.TrackSelected += (s, e) =>
            {
                // Raise event to parent
                TrackSelected?.Invoke(this, item);
            };

            // Handle playlist events if needed
            if (filePath.StartsWith("PLAYLIST:"))
            {
                item.PlaylistDeleted += (s, e) =>
                {
                    // Pass the event up to parent
                Debug.WriteLine($"Playlist '{item.Title}' deletion event received.");
                PlaylistDeleted?.Invoke(this, item);
                };
            }

            // toggle empty label visiblity
            ShowListIsEmptyLabel();
        }


        // 🔄 Reset track positions after removal
        private void ReorderTrackPositions()
        {
            for (int i = 0; i < trackItems.Count; i++)
            {
                trackItems[i].Top = i * trackItems[i].Height;
            }
        }

        // 🗑️ Clear all tracks
        public void ClearTracks()
        {
            foreach (var item in trackItems)
            {
                this.Controls.Remove(item);
                item.Dispose();
            }
            trackItems.Clear();
            // toggle empty label visiblity
            ShowListIsEmptyLabel();
        }

        // 🔍 Find and select a track
        public void SelectTrack(int index)
        {
            // Reset all tracks
            foreach (var item in trackItems)
            {
                item.SetPlaying(false);
            }

            // Select the new track
            if (index >= 0 && index < trackItems.Count)
            {
                trackItems[index].SetPlaying(true);

                // Ensure the track is visible in the viewport
                this.ScrollControlIntoView(trackItems[index]);
            }

            // toggle empty label visiblity
            ShowListIsEmptyLabel();
        }

        private void ShowListIsEmptyLabel()
        {
            //
            if (trackItems.Count <= 0)
            {
                listEmptyLabel.Visible = true;
                btnAddSongs.Visible = true;
            }
            else
            {
                listEmptyLabel.Visible = false;
                btnAddSongs.Visible = false;
            }
        }

        private void btnAddSongs_Click(object sender, EventArgs e)
        {
            AddSongsClicked?.Invoke(this, EventArgs.Empty); 
        }
    }
}