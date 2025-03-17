using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

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

        private ContextMenuStrip trackMenu;

        //// fire animation
        //private Bitmap spriteSheet;      // Holds the sprite sheet image Small_Iceball_9x24
        //private int currentFrame = 0;    // Tracks the current frame index
        //private int totalFrames = 60;     // Total frames in the sprite sheet
        //private int frameWidth = 9;    // Width of each frame (adjust to your image)
        //private int frameHeight = 24;  // Height of each frame
        //private Timer animationTimer;

        //
        public CustomTrackItem()
        {
            InitializeComponent();
            //LoadAnimationAssets();

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

            // Create a basic menu first - we'll update it in SetTrackData
            trackMenu = new ContextMenuStrip();
            this.ContextMenuStrip = trackMenu;
        }

        // 🚀 Initialize with track data
        public void SetTrackData(string filePath, Image albumArt = null)
        {
            Debug.WriteLine($"### SetTrackData - {filePath} ###");
            this.FilePath = filePath;

            // Now that we have the FilePath, set up the proper context menu
            trackMenu.Items.Clear();

            if (filePath.StartsWith("PLAYLIST:"))
            {
                // 🗂️ Playlist-specific menu
                ToolStripMenuItem deletePlaylistItem = new ToolStripMenuItem("Delete Playlist");
                deletePlaylistItem.Click += DeletePlaylistItem_Click;
                trackMenu.Items.Add(deletePlaylistItem);

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
            else
            {
                // 🎵 Track-specific menu
                ToolStripMenuItem playItem = new ToolStripMenuItem("Play");
                ToolStripMenuItem removeItem = new ToolStripMenuItem("Remove");
                ToolStripMenuItem addToPlaylistItem = new ToolStripMenuItem("Add to Playlist...");

                playItem.Click += (s, e) => OnTrackSelected(EventArgs.Empty);
                removeItem.Click += RemoveItem_Click;
                addToPlaylistItem.Click += AddToPlaylistItem_Click;

                trackMenu.Items.Add(playItem);
                trackMenu.Items.Add(removeItem);
                trackMenu.Items.Add(addToPlaylistItem);
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
            //animationTimer.Enabled = isPlaying; // Start/stop the timer for fire animation

            if (isPlaying)
            {
                this.BackColor = playingBackColor;
                //lblPlayingIndicator.Visible = true;
            }
            else
            {
                this.BackColor = normalBackColor;
                //lblPlayingIndicator.Visible = false;
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


        // 🗑️ Handle remove click
        private void RemoveItem_Click(object sender, EventArgs e)
        {
            // 🚨 Confirm deletion with track name
            DialogResult result = MessageBox.Show(
                $"Delete '{this.Title}' from playlist?",
                "Confirm",
                MessageBoxButtons.YesNo
            );

            if (result == DialogResult.Yes)
            {
                // 🎵 Notify parent to remove this track
                OnTrackRemoved(EventArgs.Empty);
            }
        }

        // 📣 New event for removal
        public event EventHandler TrackRemoved;

        // 🗂️ Playlist deletion logic
        private void DeletePlaylistItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                $"Delete playlist '{this.Title}'?",
                "Confirm",
                MessageBoxButtons.YesNo
            );

            if (result == DialogResult.Yes)
            {
                // 🎯 Notify parent to delete this playlist
                OnPlaylistDeleted(EventArgs.Empty);
            }
        }
        // 📣 New event for playlist deletion
        public event EventHandler PlaylistDeleted;
        protected virtual void OnPlaylistDeleted(EventArgs e)
        {
            PlaylistDeleted?.Invoke(this, e);
        }

        // ➕ Add to playlist logic
        private void AddToPlaylistItem_Click(object sender, EventArgs e)
        {
            // 🎯 Notify parent to add this track to a playlist
            OnAddToPlaylistRequested(EventArgs.Empty);
        }

        public event EventHandler AddToPlaylistRequested;
        protected virtual void OnAddToPlaylistRequested(EventArgs e)
        {
            AddToPlaylistRequested?.Invoke(this, e);
        }
        protected virtual void OnTrackRemoved(EventArgs e)
        {
            TrackRemoved?.Invoke(this, e);
        }
        // small fire (playing indicator Animation)

        //private void LoadAnimationAssets()
        //{
        //    spriteSheet = new Bitmap(Properties.Resources.playingAnimationSprite);
        //    lblPlayingIndicator.Size = new Size(frameWidth, frameHeight);

        //    // Initialize timer
        //    animationTimer = new Timer();
        //    animationTimer.Interval = 100;
        //    animationTimer.Tick += AnimateFireSprite;
        //    animationTimer.Enabled = false; // Start disabled
        //}

        ////private void AnimateFireSprite(object sender, EventArgs e)
        //{
        //    // Update frame index and check bounds
        //    currentFrame = (currentFrame + 1) % totalFrames;
        //    int x = currentFrame * frameWidth;
        //    if (x + frameWidth > spriteSheet.Width)
        //    {
        //        // Adjust if the calculated frame is outside the image bounds
        //        currentFrame = 0;
        //        x = 0;
        //    }
        //    Rectangle srcRect = new Rectangle(x, 0, frameWidth, frameHeight);

        //    // Dispose the previous image if it exists
        //    if (lblPlayingIndicator.BackgroundImage != null)
        //    {
        //        lblPlayingIndicator.BackgroundImage.Dispose();
        //    }

        //    // Clone the current frame from the sprite sheet
        //    Bitmap newFrame = spriteSheet.Clone(srcRect, spriteSheet.PixelFormat);

        //    // Update the label's background image with the new frame
        //    lblPlayingIndicator.BackgroundImage = newFrame;
        //}



    }
}