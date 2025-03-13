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
            using OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = FilesFilter;
            dialog.Multiselect = true;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // 🗑️ Clear existing playlist and add new files
                currentPlaylist.Tracks.Clear();
                var validFiles = dialog.FileNames.Where(f => File.Exists(f)).ToArray();
                currentPlaylist.Tracks.AddRange(validFiles);

                // 🔄 Sync playlist reference (if still needed elsewhere)
                playlist = currentPlaylist.Tracks;

                // 🎵 Initialize playback
                currentTrackIndex = 0;
                UpdateFileType(); // Detect audio/video type
                UpdateWindowTitle(); // Update title bar

                // 📋 Update playlist UI
                listPlaylist.Items.Clear();
                listPlaylist.Items.AddRange(validFiles.Select(Path.GetFileName).ToArray());

                // 💾 Save to disk
                SaveAllPlaylists();

                // ▶️ Start playback if files were loaded
                if (currentPlaylist.Tracks.Count > 0)
                {
                    mediaPlayer.Play(new FileInfo(currentPlaylist.Tracks[currentTrackIndex]));
                    btnPlayPause.BackgroundImage = Properties.Resources.PauseIcon;

                    // 🔊 Reset audio settings
                    mediaPlayer.Audio.Volume = volumeBar.Value;
                    isMuted = false;
                    btnMute.BackgroundImage = Properties.Resources.MuteIcon;
                }
            }

            // 🔄 Update button states (play/stop etc.)
            UpdateButtonStates();
            UpdateButtonImages();
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

        private void UpdateButtonStates()
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

            // Remove missing files from current playlist
            currentPlaylist.Tracks = currentPlaylist.Tracks
                .Where(File.Exists)
                .ToList();

            listPlaylist.Items.Clear();
            listPlaylist.Items.AddRange(currentPlaylist.Tracks.Select(Path.GetFileName).ToArray());

            // Show warning if many files missing
            if (currentPlaylist.Tracks.Count < listPlaylist.Items.Count)
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
            SaveAllPlaylists();
            Properties.Settings.Default.LastVolume = volumeBar.Value;
            Properties.Settings.Default.Save();
        }

   
    }
}