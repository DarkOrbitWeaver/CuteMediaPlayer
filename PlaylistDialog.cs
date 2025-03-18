using static CuteMediaPlayer.Form1;

namespace CuteMediaPlayer
{
    public partial class PlaylistDialog : Form
    {
        // Replace the ListBox property with a method that returns the selected item
        public string SelectedPlaylist { get; private set; }

        private CustomPlaylistPanel playlistPanel;

        public PlaylistDialog(string promptText = "")
        {
            InitializeComponent();

            // Apply rounded styling
            FormStyling.ApplyRoundedStyle(this);

            // Make specific controls draggable - NOT buttons or the panel
            FormStyling.MakeDraggable(this, this); // The form itself
            FormStyling.MakeDraggable(label1, this); // Just the label

            // Set the prompt text 
            this.label1.Text = promptText;

            // Setup button handlers
            btnOK.Click += BtnOK_Click;
            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;
        }

        // Handle OK button click
        private void BtnOK_Click(object sender, EventArgs e)
        {
            // If nothing selected, show message
            if (SelectedPlaylist == null)
            {
                MessageBox.Show("Please select a playlist first!");
                return;
            }

            DialogResult = DialogResult.OK;
        }

        // Method to populate with playlists
        public void PopulatePlaylists(List<Playlist> playlists)
        {
            // Clear existing items
            playlistPanel.ClearTracks();

            // Add each playlist as a CustomTrackItem with proper track count
            foreach (var playlist in playlists)
            {
                // Format with playlist name and actual track count
                playlistPanel.AddTrack($"PLAYLIST:{playlist.Name}|{playlist.Tracks.Count}");
            }

            // Listen for selection
            playlistPanel.TrackSelected += PlaylistPanel_TrackSelected;
        }

        // Handle playlist selection
        private void PlaylistPanel_TrackSelected(object sender, CustomTrackItem item)
        {
            // Extract the playlist name from the item
            if (item.FilePath.StartsWith("PLAYLIST:"))
            {
                string playlistInfo = item.FilePath.Replace("PLAYLIST:", "").Trim();
                string[] parts = playlistInfo.Split('|');
                SelectedPlaylist = parts[0];  // Get playlist name

                // Highlight the selected item - assuming similar behavior to your CustomPlaylistPanel
                foreach (CustomTrackItem trackItem in playlistPanel.Controls.OfType<CustomTrackItem>())
                {
                    trackItem.SetPlaying(trackItem == item);
                }
            }
        }
    }

}