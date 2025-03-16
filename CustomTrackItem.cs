using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace CuteMediaPlayer
{
    public partial class CustomTrackItem : UserControl
    {
        // 🎵 Track info properties
        public string FilePath { get; private set; }
        public string Title { get; private set; }
        public string Artist { get; private set; }
        public bool IsPlaying { get; private set; } = false;

        // 🎨 Colors for styling
        private Color normalBackColor = Color.FromArgb(254, 218, 224);
        private Color hoverBackColor = Color.FromArgb(255, 200, 210);
        private Color selectedBackColor = Color.FromArgb(254, 184, 195);
        private Color playingBackColor = Color.FromArgb(255, 160, 180);

        // 📦 Default constructor
        public CustomTrackItem()
        {
            InitializeComponent();

            // 🖌️ Set up appearance
            this.BackColor = normalBackColor;
            this.Cursor = Cursors.Hand;

            // 🖱️ Handle all mouse events at the parent level
            this.MouseEnter += CustomTrackItem_MouseEnter;
            this.MouseLeave += CustomTrackItem_MouseLeave;
            this.Click += CustomTrackItem_Click;

            // Disable mouse events for child controls
            pictureAlbumArt.MouseEnter += (s, e) => this.OnMouseEnter(e);
            pictureAlbumArt.MouseLeave += (s, e) => this.OnMouseLeave(e);
            pictureAlbumArt.Click += (s, e) => this.OnClick(e);
            
            lblTitle.MouseEnter += (s, e) => this.OnMouseEnter(e);
            lblTitle.MouseLeave += (s, e) => this.OnMouseLeave(e);
            lblTitle.Click += (s, e) => this.OnClick(e);
            
            lblArtist.MouseEnter += (s, e) => this.OnMouseEnter(e);
            lblArtist.MouseLeave += (s, e) => this.OnMouseLeave(e);
            lblArtist.Click += (s, e) => this.OnClick(e);
        }

        // 🚀 Initialize with track data
        public void SetTrackData(string filePath, Image albumArt = null)
        {
            Debug.WriteLine($"### SetTrackData - {filePath} ###");
            this.FilePath = filePath;

            // Check if it's a playlist item
            if (filePath.StartsWith("PLAYLIST:"))
            {
                // Extract clean playlist name
                this.Title = filePath.Replace("PLAYLIST:", "");
                this.Artist = "Playlist";
                Debug.WriteLine($"Setting playlist item: {this.Title}");

                // Update UI
                lblTitle.Text = this.Title;
                lblArtist.Text = this.Artist;
                lblDuration.Text = "";// Hide duration for playlists

                // Make sure we have the playlist icon resource
                if (Properties.Resources.PlaylistIcon != null)
                {
                    pictureAlbumArt.Image = Properties.Resources.SavedPlaylistIcon;
                }
                else
                {
                    pictureAlbumArt.Image = Properties.Resources.MusicPlaceholderIcon;
                }
                return;
            }

            // Handle regular media files
            try
            {
                // Use path as fallback title
                this.Title = Path.GetFileNameWithoutExtension(filePath);
                this.Artist = "Unknown Artist";

                // Try to get metadata if file exists
                if (File.Exists(filePath))
                {
                    Debug.WriteLine("Getting metadata for file");

                    try
                    {
                        var metadata = MediaHelper.GetMetadata(filePath);

                        //  duration
                        lblDuration.Text = FormatDuration(metadata.duration); 

                        // Only update if we got valid data
                        if (!string.IsNullOrEmpty(metadata.title))
                        {
                            this.Title = metadata.title;
                        }

                        if (!string.IsNullOrEmpty(metadata.artist))
                        {
                            this.Artist = metadata.artist;
                        }



                        // Set album art if available
                        if (metadata.albumArt != null)
                        {
                            pictureAlbumArt.Image = metadata.albumArt;
                        }
                        else
                        {
                            // Set default icon based on file type
                            SetDefaultIcon(filePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error getting metadata: {ex.Message}");
                        SetDefaultIcon(filePath);
                        lblDuration.Text = "0:00";
                    }
                }
                else
                {
                    Debug.WriteLine("File not found - using defaults");
                    pictureAlbumArt.Image = Properties.Resources.MusicPlaceholderIcon;
                }

                // Update UI labels
                lblTitle.Text = this.Title;
                lblArtist.Text = this.Artist;
                Debug.WriteLine($"Set track data - Title: {this.Title}, Artist: {this.Artist}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error setting track data: {ex.Message}");

                // Set defaults for errors
                this.Title = Path.GetFileNameWithoutExtension(filePath);
                this.Artist = "Unknown Artist";
                lblTitle.Text = this.Title;
                lblArtist.Text = this.Artist;
                pictureAlbumArt.Image = Properties.Resources.MusicPlaceholderIcon;
            }
        }

        //time or duration helper
        // 🕒 Convert time to "3:45" format
        private string FormatDuration(TimeSpan duration)
        {
            if (duration.TotalSeconds < 1) return "0:00";

            int totalMinutes = (int)duration.TotalMinutes;
            int seconds = duration.Seconds;

            return $"{totalMinutes}:{seconds:D2}";
        }
        private void SetDefaultIcon(string filePath)
        {
            try
            {
                string ext = Path.GetExtension(filePath).ToLower();
                if (ext == ".mp3" || ext == ".wav" || ext == ".m4a" || ext == ".aac"
                    || ext == ".flac" || ext == ".ogg" || ext == ".wma")
                {
                    pictureAlbumArt.Image = Properties.Resources.MusicPlaceholderIcon;
                }
                else
                {
                    pictureAlbumArt.Image = Properties.Resources.VideoPlaceholderIcon;
                }
            }
            catch
            {
                // Fallback to music icon if we can't determine file type
                pictureAlbumArt.Image = Properties.Resources.MusicPlaceholderIcon;
            }
        }

        // 🎯 Set if this track is currently playing
        public void SetPlaying(bool isPlaying)
        {
            this.IsPlaying = isPlaying;

            if (isPlaying)
            {
                this.BackColor = playingBackColor;
                lblPlayingIndicator.Visible = true;
            }
            else
            {
                this.BackColor = normalBackColor;
                lblPlayingIndicator.Visible = false;
            }
        }

        // 🖱️ Mouse enter - hover effect
        private void CustomTrackItem_MouseEnter(object sender, EventArgs e)
        {
            if (!IsPlaying)
            {
                this.BackColor = hoverBackColor;
            }
        }

        // 🖱️ Mouse leave - restore normal color
        private void CustomTrackItem_MouseLeave(object sender, EventArgs e)
        {
            if (!IsPlaying)
            {
                this.BackColor = normalBackColor;
            }
        }

        // 🖱️ Handle click event
        private void CustomTrackItem_Click(object sender, EventArgs e)
        {
            // Trigger the event for outside subscribers
            OnTrackSelected(EventArgs.Empty);
        }

        // 📣 Events for outside control
        public event EventHandler TrackSelected;

        protected virtual void OnTrackSelected(EventArgs e)
        {
            TrackSelected?.Invoke(this, e);
        }

    }
}