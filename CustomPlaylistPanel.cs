using System;
using System.Collections.Generic;
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
            // Create new track item
            CustomTrackItem item = new CustomTrackItem();
            item.SetTrackData(filePath);

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

            // toggle empty label visiblity
            ShowListIsEmptyLabel();
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