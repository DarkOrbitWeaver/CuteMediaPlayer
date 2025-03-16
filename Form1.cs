using Vlc.DotNet.Forms;
using Vlc.DotNet.Core; // Required for VlcTrackType
using Timer = System.Windows.Forms.Timer;


namespace CuteMediaPlayer
{
    public partial class Form1 : Form
    {

        private Random random = new Random();

        // loop
        private bool loopEnabled = false;
        // 
        private int lastVolumeBarValue = 100;

        // filter files (when opening exploerer to select media)
        string FilesFilter = "Media Files|*.mp4;*.mp3;*.avi;*.mkv;*.wav;*.m4a;*.aac;*.flac;*.ogg;*.wma";

        private Timer uiTimer;

        // dancing girl
        private Timer dancingGirlTimer;
        private List<Image>[] danceFrames = [new List<Image>(), new List<Image>(), new List<Image>(), new List<Image>()];
        private int currentFrame = 0;
        private int currentFrameSet = 0;
        private bool switchFrameSet = false;
        private int lastSwitch = 0; // to prevent fast switch between frameset

        // for the minimzed player 
        // Public properties to access button states
        public bool IsPlayPauseEnabled => btnPlayPause.Enabled;
        public bool IsNextEnabled => btnNext.Enabled;
        public bool IsPrevEnabled => btnPrev.Enabled;

        // Public properties to access button images
        public Image PlayPauseImage => btnPlayPause.BackgroundImage;
        public Image NextImage => btnNext.BackgroundImage;
        public Image PrevImage => btnPrev.BackgroundImage;

        // Event that fires when button states change
        public event EventHandler ButtonStatesChanged;

        public Form1()
        {
            InitializeComponent();
            UpdateButtonStates();
            InitializeMediaPlayer();
            InitializeAudioSampling();

            // set up other ones to handle the change of button size and click stuff kinda like animations
            SetupButtonAppearance(btnPlayPause);
            SetupButtonAppearance(btnStop);
            SetupButtonAppearance(btnPrev);
            SetupButtonAppearance(btnNext);
            SetupButtonAppearance(btnChangeTheme);
            SetupButtonAppearance(btnMute);
            SetupButtonAppearance(loopBtn);
            SetupButtonAppearance(shuffle);
            SetupButtonAppearance(openPlaylist);
            SetupButtonAppearance(btnAddCurrentToPlaylist);

            // dancing girl
            InitializeDancingGirl();

            // new playlist ui
            InitializeCustomPlaylistControls();



            // 🕒 Timer to update UI elements (seek bar/time label)
            uiTimer = new Timer { Interval = 250 }; // Smoother updates at 250ms

            uiTimer.Tick += (s, e) =>
            {
                try
                {
                    // Check if media player exists and has valid media
                    if (mediaPlayer == null || !mediaPlayer.IsPlaying) return;

                    // Check if we have a valid time/length
                    if (mediaPlayer.Length <= 0) return;

                    // Only update seek bar if user isn't dragging it
                    if (!seekBar.Capture)
                    {
                        // Convert media position, seek bar value (0-1000)
                        try
                        {
                            seekBar.Value = (int)(mediaPlayer.Position * seekBar.Maximum);
                        }
                        catch
                        {
                            // Ignore any seek bar update errors
                        }
                    }

                    // 3️⃣ Always update time display
                    UpdateTimeDisplay();
                }
                catch (Exception ex)
                {
                    // 🚨 Show error but don't crash
                    Console.WriteLine($"Timer error: {ex.Message}");
                }
            };

            uiTimer.Start();

            // dancing girl timer
            //dancingGirlTimer = new Timer();
            //dancingGirlTimer.Interval = 80;
            //dancingGirlTimer.Tick += DancingGirlTimer_Tick;
            //dancingGirlTimer.Start();

        }


        // mute 
        private bool isMuted = false;
        private void btnMute_Click(object sender, EventArgs e)
        {
            isMuted = !isMuted;
            mediaPlayer.Audio.IsMute = isMuted;

            HandleMuteState();

            // handle the audio bar to match the last after unmuting 
            if (isMuted)
            {
                // set the volume bar value to be muted
                lastVolumeBarValue = volumeBar.Value;
                volumeBar.Value = 0;
            }
            else
            {
                // restore vloume bar
                volumeBar.Value = lastVolumeBarValue;
            }

        }

        private void HandleMuteState()
        {
            // switch the mute icon
            if (isMuted)
            {
                btnMute.BackgroundImage = Properties.Resources.DisabledMuteIcon;
            }
            else
            {
                btnMute.BackgroundImage = Properties.Resources.MuteIcon;
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenToolStripMenuItemHelper();
        }

        private void OpenToolStripMenuItemHelper()
        {
            using OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = FilesFilter;
            dialog.Multiselect = true;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // Get valid files
                var validFiles = dialog.FileNames.Where(f => File.Exists(f)).ToArray();

                // 🎵 Create NEW temporary playlist
                currentPlaylist = new Playlist { Name = $"Temporary_{DateTime.Now:yyyyMMdd_HHmmss}" };
                foreach (string file in validFiles)
                {
                    currentPlaylist.Tracks.Add(file);
                }

                // 🔄 Sync playlist reference
                playlist = currentPlaylist.Tracks;
                currentTrackIndex = 0; // ALWAYS reset to first track ⭐

                // ⭐ STOP OLD PLAYBACK + START NEW
                if (mediaPlayer.IsPlaying) mediaPlayer.Stop();
                PlayCurrentTrack(); // ⭐ THIS IS THE KEY FIX

                UpdateFileType();
                UpdateWindowTitle();

                // 📋 Update playlist UI
                customPlaylistPanel.ClearTracks();
                currentPlaylist.Tracks.ForEach(t => customPlaylistPanel.AddTrack(t));

                // 🔄 Update buttons
                UpdateButtonStates();
                UpdateButtonImages();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // Properly shut down the app
                if (mediaPlayer != null)
                {
                    mediaPlayer.Stop();
                }

                this.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exiting: {ex.Message}");
                // Force exit if needed
                Environment.Exit(0);
            }
        }

        private void loopBtn_Click(object sender, EventArgs e)
        {
            loopEnabled = !loopEnabled;

            // switch the loop icon
            if (!loopEnabled)
            {
                // if loop is not activated
                loopBtn.BackgroundImage = Properties.Resources.DisabledLoopIcon;
            }
            else
            {
                // when loop is on
                loopBtn.BackgroundImage = Properties.Resources.LoopIcon;
            }
        }

        public void UpdateButtonStates()
        {
            try
            {
                bool mediaLoaded = currentTrackIndex >= 0 && playlist.Count > 0;
                btnPlayPause.Enabled = mediaLoaded;
                UpdateButtonImages();
                btnStop.Enabled = mediaLoaded;
                UpdateButtonImages();
                btnNext.Enabled = mediaLoaded && playlist.Count > 1;
                UpdateButtonImages();
                btnPrev.Enabled = mediaLoaded && playlist.Count > 1;
                UpdateButtonImages();
                seekBar.Enabled = mediaLoaded;
                UpdateButtonImages();
                btnChangeTheme.Enabled = isAudioFile;
                UpdateButtonImages();

                // Notify listeners that button states changed
                OnButtonStatesChanged();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error setting options: {ex.Message}");
                Console.WriteLine($"Error setting options: {ex.Message}");
            }
        }

        private void UpdateWindowTitle()
        {
            // Default title when no media is playing
            string baseTitle = "✨ Cute Media Player ✨";

            // If we have a file loaded in the playlist
            if (currentTrackIndex >= 0 && currentTrackIndex < playlist.Count)
            {
                // Extract the filename with extension, but without the full path
                string fileName = Path.GetFileName(playlist[currentTrackIndex]);
                this.Text = $"{baseTitle} - {fileName}";
            }
            else
            {
                // Reset to default title when no media is playing
                this.Text = baseTitle;
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HelpForm aboutDialogueWindow = new HelpForm();
            aboutDialogueWindow.ShowDialog();
        }

        private void CleanInvalidPaths()
        {
            Directory.CreateDirectory(playlistsFolder);

            // Track original count before cleanup
            int originalCount = currentPlaylist.Tracks.Count;

            // Remove missing files
            currentPlaylist.Tracks = currentPlaylist.Tracks
                .Where(File.Exists)
                .ToList();

            // ✅ Update custom UI
            customPlaylistPanel.SetTracks(currentPlaylist.Tracks);

            // ✅ Check against original count
            if (currentPlaylist.Tracks.Count < originalCount)
            {
                MessageBox.Show("Some files could not be found and were removed");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                // Create playlists folder if missing
                Directory.CreateDirectory(playlistsFolder);

                // Load playlists
                LoadAllPlaylists();

                // Clean invalid paths
                CleanInvalidPaths();

                // Restore volume
                if (Properties.Settings.Default.LastVolume > 0 &&
                    Properties.Settings.Default.LastVolume <= 100)
                {
                    volumeBar.Value = Properties.Settings.Default.LastVolume;
                }
                else
                {
                    volumeBar.Value = 80; // Default value
                }

                // Apply volume to media player
                if (mediaPlayer != null)
                {
                    mediaPlayer.Audio.Volume = volumeBar.Value;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Form_Load: {ex.Message}");
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                SaveAllPlaylists(); // Only saves playlists in allPlaylists
                Properties.Settings.Default.LastVolume = volumeBar.Value;
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving settings: {ex.Message}");
            }
        }

        private void DisposeOfDancingGirlFrames()
        {
            // Stop and dispose timer
            if (dancingGirlTimer != null)
            {
                dancingGirlTimer.Stop();
                dancingGirlTimer.Dispose();
            }

            // Clean up image resources
            foreach (var frame in danceFrames[currentFrameSet])
            {
                if (frame != null)
                    frame.Dispose();
            }
            danceFrames[currentFrameSet].Clear();
        }

        // Load images from folder with transparency support (winform doesnt support it i think?)
        // set timers and the loader to get the frames
        private void InitializeDancingGirl()
        {
            // Set the picture box to have transparent background
            dancingGirlPictureBox.BackColor = Color.Transparent;

            // Load images from folder
            string[] animationAssetsPath = ["hips", "snap", "slide", "balancing"];

            for (int i = 0; i < animationAssetsPath.Length; i++)
            {
                string tempPath = Path.Combine(Application.StartupPath, "Assets", "DancingGirlFiles", animationAssetsPath[i]);
                animationAssetsPath[i] = tempPath;
            }

            for (int i = 0; i < animationAssetsPath.Length; i++)
            {
                // Get all PNG files
                string[] frameFiles = Directory.GetFiles(animationAssetsPath[i], "*.png");

                // Load them in order
                for (int j = 0; j < frameFiles.Length; j++)
                {
                    // Create a bitmap and make black pixels transparent
                    Bitmap bmp = new Bitmap(frameFiles[j]);
                    bmp.MakeTransparent(Color.Black); // This makes black backgrounds transparent

                    danceFrames[i].Add(bmp);
                }

            }






            // Set up timer for animation
            dancingGirlTimer = new Timer();
            dancingGirlTimer.Interval = 100; // Adjust speed as needed (milliseconds)
            dancingGirlTimer.Tick += DancingGirlTimer_Tick;
            dancingGirlTimer.Start();
        }
        // animate girl 
        private void DancingGirlTimer_Tick(object sender, EventArgs e)
        {
            // if loaded frames successfully
            if (danceFrames[currentFrameSet].Count > 0)
            {
                // if not reached the end of the frames
                if (currentFrame < danceFrames[currentFrameSet].Count)
                {

                    dancingGirlPictureBox.Image = danceFrames[currentFrameSet][currentFrame];
                    currentFrame++;
                    dancingGirlTimer.Interval = 100;
                }
                else
                {
                    // change dancing set if allowed
                    if (lastSwitch >= 100)
                    {
                        currentFrameSet = random.Next(danceFrames.Count());
                        switchFrameSet = false;
                        lastSwitch = 0;
                    }

                    // if reached the end restart the animation
                    currentFrame = 0;
                    dancingGirlTimer.Interval = 10;
                }
                //
                lastSwitch++;
            }
        }
        // to open minimized player
        private void openMinimizedPlayer_Click(object sender, EventArgs e)
        {
            // Create the minimized player
            MinimizedPlayer minimizedPlayer = new MinimizedPlayer(this);

            // Minimize the main form
            this.WindowState = FormWindowState.Minimized;

            // Make sure the minimized player appears properly
            minimizedPlayer.WindowState = FormWindowState.Normal;
            minimizedPlayer.StartPosition = FormStartPosition.CenterScreen;
            minimizedPlayer.Show();
            minimizedPlayer.Activate(); // Force focus to the new form
            minimizedPlayer.BringToFront(); // Make sure it's visible on top

            // When minimized player closes, restore the main form
            minimizedPlayer.FormClosed += (s, args) => {
                this.WindowState = FormWindowState.Normal;
                this.Activate(); // Bring the main form to the front
            };
        }

        //  whenever your button states change
        protected void OnButtonStatesChanged()
        {
            ButtonStatesChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}