using Vlc.DotNet.Forms;
using Timer = System.Windows.Forms.Timer;
using Vlc.DotNet.Core; // Required for VlcTrackType


namespace CuteMediaPlayer
{
    public partial class Form1 : Form
    {

        private Random random = new Random();
        private Timer uiTimer;

        // playlist mangment 
        private List<string> playlist = new List<string>();
        private int currentTrackIndex = -1;

        // to toggle audio visualizer
        private bool isAudioFile = false; 

        // loop
        private bool loopEnabled = false;

        // visualizer theme
        private int lastVolumeBarValue = 100;

        // store user settings
        private List<Playlist> allPlaylists = new List<Playlist>();
        private Playlist currentPlaylist = new Playlist { Name = "Default" };
        private string playlistsFolder = Path.Combine(Application.StartupPath, "Playlists");

        // shuffle 
        private List<string> originalPlaylistOrder = new List<string>();
        private bool isShuffled = false;


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

        // 📻 Media Player Module
        private VlcControl mediaPlayer;

        // initiliaze the media player
        private void InitializeMediaPlayer()
        {
            try
            {
                mediaPlayer = new VlcControl();
                vlcContainer.Controls.Add(mediaPlayer);
                mediaPlayer.Dock = DockStyle.Fill;

                // Get valid VLC path
                var libDirectory = new DirectoryInfo(Path.Combine(Application.StartupPath, "VlcLibs"));

                // Verify directory exists
                if (!libDirectory.Exists)
                {
                    MessageBox.Show($"❌ VLC libraries not found at: {libDirectory.FullName}", "Error");
                    Environment.Exit(1);
                }

                // Initialize VLC with simpler options
                mediaPlayer.BeginInit();
                mediaPlayer.VlcLibDirectory = libDirectory;

                try
                {

                    string[] options = new string[]
                    {
                        "--aout=directsound", // Use DirectSound for audio output
                        "--file-caching=1000", // Add some caching
                        "--audio-resampler=soxr", // High quality resampler
                        "--codec=avcodec" // Force use of FFmpeg/libavcodec
                    };
                    mediaPlayer.VlcMediaplayerOptions = options;

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error setting options: {ex.Message}");
                    Console.WriteLine($"Error setting options: {ex.Message}");
                }



                mediaPlayer.EndInit();  // Complete initialization before using

                // 🔊 Ensure audio is enabled and set volume
                mediaPlayer.Audio.Volume = volumeBar.Value;
                mediaPlayer.Audio.IsMute = false;

                // Set proper control order
                vlcContainer.Controls.SetChildIndex(mediaPlayer, 0); // Back layer
                vlcContainer.Controls.SetChildIndex(sparkleVisualizer1, 1); // Front layer

                // Handle when media ends - using a safer approach
                mediaPlayer.EndReached += (s, e) =>
                {
                    try
                    {
                        // Using BeginInvoke to avoid threading issues
                        this.BeginInvoke(new Action(() =>
                        {
                            try
                            {
                                // Check if loop is enabled
                                if (loopEnabled && playlist.Count > 0)
                                {
                                    // Replay the current file
                                    mediaPlayer.Play(new FileInfo(playlist[currentTrackIndex]));
                                    btnPlayPause.BackgroundImage = Properties.Resources.PauseIcon;
                                }
                                else if (currentTrackIndex < playlist.Count - 1)
                                {
                                    // Play next track if available
                                    currentTrackIndex++;
                                    PlayCurrentTrack();
                                }
                                else
                                {
                                    // We're at the end of the playlist
                                    mediaPlayer.Stop();
                                    mediaPlayer.Position = 0; // Reset position
                                    seekBar.Value = 0;
                                    btnPlayPause.BackgroundImage = Properties.Resources.PlayIcon;

                                    // Show idle visualizer
                                    UpdateVisualizerVisibility();
                                }

                                // Reset audio for next playback
                                mediaPlayer.Audio.Volume = volumeBar.Value;
                                mediaPlayer.Audio.IsMute = false;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error in EndReached action: {ex.Message}");
                            }
                        }));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in EndReached: {ex.Message}");
                    }
                };

                mediaPlayer.Playing += (s, e) =>
                {
                    try
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            Console.WriteLine("VLC Playing event fired");
                            // Force visualizer on when playing audio
                            if (isAudioFile)
                            {
                                sparkleVisualizer1.Visible = true;
                                sparkleVisualizer1.IsIdleMode = false;
                                sparkleVisualizer1.BringToFront();
                                sparkleVisualizer1.RefreshVisuals();
                            }
                        }));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in Playing event: {ex.Message}");
                    }
                };

                mediaPlayer.Paused += (s, e) =>
                {
                    try
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            Console.WriteLine("VLC Paused event fired");
                            // Force visualizer to idle mode but keep visible for audio
                            if (isAudioFile)
                            {
                                sparkleVisualizer1.Visible = true;
                                sparkleVisualizer1.IsIdleMode = true;
                                sparkleVisualizer1.BringToFront();
                                sparkleVisualizer1.RefreshVisuals();
                            }
                        }));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in Paused event: {ex.Message}");
                    }
                };


                // Initial visualizer state
                UpdateVisualizerVisibility();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing VLC: {ex.Message}", "Critical Error");
                Environment.Exit(1); // Gracefully exit
            }



        }

        // provide audio data to the visualizer 

        private Timer audioUpdateTimer;

        // In Form1.cs
        private void InitializeAudioSampling()
        {
            audioUpdateTimer = new Timer { Interval = 16 }; // ~60fps for smoother visuals

            // Beat detection parameters
            float[] energyHistory = new float[20]; // Longer history for better detection
            int historyIndex = 0;
            float beatThreshold = 0.08f;
            float beatDecay = 0.98f;
            float lastBeatTime = 0;
            float currentBeatStrength = 0;

            // Frequency simulation parameters for more realistic response
            float[] bassEnergy = new float[3]; // Low frequency
            float[] midEnergy = new float[3];  // Mid frequency
            float[] highEnergy = new float[3]; // High frequency

            Random beatRandom = new Random();

            audioUpdateTimer.Tick += (s, e) =>
            {
                if (mediaPlayer?.IsPlaying ?? false)
                {
                    // Calculate time-based phase for more realistic beat syncing
                    float time = (float)(DateTime.Now.Ticks / 10000000.0);

                    // Use VLC's time position to better sync with actual audio
                    float mediaTime = mediaPlayer.Time / 1000.0f;

                    // Extract volume as base energy indicator
                    float volume = mediaPlayer.Audio.Volume / 100f;

                    // Create FFT-like simulation based on audio position
                    // This uses media time to ensure sync with actual playing position
                    float bassFactor = 0.6f + (0.4f * (float)Math.Sin(mediaTime * 0.4));
                    float midFactor = 0.5f + (0.5f * (float)Math.Sin(mediaTime * 0.8));
                    float highFactor = 0.3f + (0.7f * (float)Math.Sin(mediaTime * 1.7));

                    // Add subtle randomness for more natural feel
                    bassFactor += (float)(beatRandom.NextDouble() * 0.1 - 0.05);
                    midFactor += (float)(beatRandom.NextDouble() * 0.08 - 0.04);
                    highFactor += (float)(beatRandom.NextDouble() * 0.05 - 0.025);

                    // Shift existing energy values
                    for (int i = bassEnergy.Length - 1; i > 0; i--)
                    {
                        bassEnergy[i] = bassEnergy[i - 1];
                        midEnergy[i] = midEnergy[i - 1];
                        highEnergy[i] = highEnergy[i - 1];
                    }

                    // Calculate new energy values
                    bassEnergy[0] = volume * bassFactor;
                    midEnergy[0] = volume * midFactor;
                    highEnergy[0] = volume * highFactor;

                    // Calculate instantaneous energy with bass-weighted average
                    float instantEnergy = (bassEnergy[0] * 0.7f) + (midEnergy[0] * 0.2f) + (highEnergy[0] * 0.1f);
                    instantEnergy = Math.Min(1.0f, instantEnergy * 1.3f); // Boost for more dynamic response

                    // Update energy history
                    energyHistory[historyIndex] = instantEnergy;
                    historyIndex = (historyIndex + 1) % energyHistory.Length;

                    // Calculate average energy over history window
                    float avgEnergy = 0;
                    for (int i = 0; i < energyHistory.Length; i++)
                        avgEnergy += energyHistory[i];
                    avgEnergy /= energyHistory.Length;

                    // Beat detection - comparing instant energy to average with adaptive threshold
                    float timeSinceLastBeat = time - lastBeatTime;
                    float beatSensitivity = Math.Min(1.0f, timeSinceLastBeat * 4); // Prevents double-beats

                    if (instantEnergy > (avgEnergy + beatThreshold) && instantEnergy > 0.1f && beatSensitivity > 0.2f)
                    {
                        // Beat detected!
                        currentBeatStrength = Math.Min(1.0f, (instantEnergy - avgEnergy) * 2.5f);
                        lastBeatTime = time;
                    }
                    else
                    {
                        // Decay beat strength
                        currentBeatStrength *= beatDecay;
                    }

                    // Prepare data for visualizer (64 bands for more detail)
                    float[] visData = new float[64];

                    // Bass frequencies (first 8 bands, highest amplitude)
                    for (int i = 0; i < 8; i++)
                    {
                        // Create the illusion of bass response - stronger at onset, quick decay
                        visData[i] = bassEnergy[0] * (0.8f + 0.2f * currentBeatStrength);

                        // Add some natural variations
                        visData[i] *= 0.85f + (0.15f * (float)Math.Sin(i * 0.4 + mediaTime * 2));
                    }

                    // Mid frequencies (next 24 bands)
                    for (int i = 8; i < 32; i++)
                    {
                        visData[i] = midEnergy[0] * (0.5f + 0.2f * currentBeatStrength);
                        visData[i] *= 0.9f + (0.1f * (float)Math.Sin(i * 0.2 + mediaTime * 3));
                    }

                    // High frequencies (remaining bands)
                    for (int i = 32; i < visData.Length; i++)
                    {
                        visData[i] = highEnergy[0] * (0.3f + 0.1f * currentBeatStrength);
                        visData[i] *= 0.95f + (0.05f * (float)Math.Sin(i * 0.3 + mediaTime * 5));
                    }

                    // If it's a strong beat, boost certain frequencies for effect
                    if (currentBeatStrength > 0.6f)
                    {
                        for (int i = 0; i < 16; i++)
                        {
                            visData[i] *= 1.0f + (currentBeatStrength * 0.5f);
                        }
                    }

                    // Send the improved data to the visualizer
                    sparkleVisualizer1.UpdateAudioData(visData);
                }
            };

            audioUpdateTimer.Start();
        }

        // custom seek bars



        private void btnPlayPause_Click(object sender, EventArgs e)
        {
            try
            {
                if (mediaPlayer == null) return;

                if (mediaPlayer.IsPlaying)
                {
                    mediaPlayer.Pause();
                    btnPlayPause.BackgroundImage = Properties.Resources.PlayIcon;
                }
                else
                {
                    // If we have a file selected in the playlist
                    if (currentTrackIndex != -1 && currentTrackIndex < playlist.Count)
                    {
                        // Check if file exists
                        if (!File.Exists(playlist[currentTrackIndex]))
                        {
                            MessageBox.Show($"File not found: {Path.GetFileName(playlist[currentTrackIndex])}");
                            return;
                        }

                        // Check if we're at the end by looking at position
                        if (mediaPlayer.Position <= 0.01 || mediaPlayer.Position >= 0.99)
                        {
                            // Reload the current file
                            mediaPlayer.Play(new FileInfo(playlist[currentTrackIndex]));
                        }
                        else
                        {
                            // Just resume if paused in the middle
                            mediaPlayer.Play();
                        }

                        btnPlayPause.BackgroundImage = Properties.Resources.PauseIcon;
                    }
                }

                // Use our consolidated method with force refresh
                UpdateVisualizerVisibility(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error playing media: {ex.Message}", "Playback Error");
            }

            UpdateButtonStates();
            UpdateButtonImages();
        }


        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                // ⏹ Stop playback completely
                mediaPlayer.Stop();

                // Reset UI elements
                btnPlayPause.BackgroundImage = Properties.Resources.PlayIcon;
                seekBar.Value = 0;

                // Reset time label
                lblTime.Text = "0:00 / 0:00";

                // Reset window title to default
                UpdateWindowTitle();

                // Show idle visualizer
                UpdateVisualizerVisibility();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error stopping: {ex.Message}");
            }

            UpdateButtonStates();
            UpdateButtonImages();
        }

        private void volumeBar_Scroll(object sender, EventArgs e)
        {
            // 🔊 Set volume (0-100 scale)
            mediaPlayer.Audio.Volume = volumeBar.Value;
            // update the mute button icon since we have changed value

            if (volumeBar.Value == 0)
            {
                isMuted = true;
            }
            else if (volumeBar.Value >= 1)
            {
                isMuted = false;
            }


            HandleMuteState();

        }

        private void seekBar_Scroll(object sender, EventArgs e)
        {
            // ⏩ Update media position when user drags the seek bar
            if (mediaPlayer.IsPlaying)
            {
                float position = (float)seekBar.Value / seekBar.Maximum;
                mediaPlayer.Position = position;
            }
        }


        private void UpdateTimeDisplay()
        {
            try
            {
                // ⏱️ Format current time and total time
                if (mediaPlayer == null) return;

                TimeSpan currentTime = TimeSpan.FromMilliseconds(mediaPlayer.Time);
                TimeSpan totalTime = TimeSpan.FromMilliseconds(mediaPlayer.Length);
                lblTime.Text = $"{currentTime:mm\\:ss} / {totalTime:mm\\:ss}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating time display: {ex.Message}");
                // Set a default value if there's an error
                lblTime.Text = "0:00 / 0:00";
            }
        }

        // Call this method when opening a file
        private void UpdateVisualizerVisibility(bool forceRefresh = true)
        {
            // Check if we're playing anything
            bool isPlaying = mediaPlayer?.IsPlaying ?? false;
            bool mediaLoaded = currentTrackIndex >= 0 && currentTrackIndex < playlist.Count;
            //sparkleVisualizer1.SetIdleWallpaper(Properties.Resources.bg2);

            // For audio files, always keep the visualizer visible, just change its mode
            if (mediaLoaded && isAudioFile)
            {
                sparkleVisualizer1.Visible = true;
                sparkleVisualizer1.IsIdleMode = !isPlaying; // idle when paused, active when playing
                sparkleVisualizer1.BringToFront();
                //sparkleVisualizer1.UseIdleWallpaper = true;
            }
            // For video files, hide visualizer completely
            else if (mediaLoaded && !isAudioFile)
            {
                sparkleVisualizer1.Visible = false;
                sparkleVisualizer1.UseIdleWallpaper = false;
            }
            // If nothing is loaded, show idle visualizer
            else
            {
                sparkleVisualizer1.Visible = true;
                sparkleVisualizer1.IsIdleMode = true;
                sparkleVisualizer1.BringToFront();
                //sparkleVisualizer1.UseIdleWallpaper = true;
            }

            // Force a redraw if requested
            if (forceRefresh)
            {
                sparkleVisualizer1.RefreshVisuals();

                // Only do a full UI refresh when necessary
                if (isAudioFile)
                {
                    this.Refresh();
                }
            }
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
                btnMute.BackgroundImage = Properties.Resources.DisabledLoopIcon;
            }
            else
            {
                btnMute.BackgroundImage = Properties.Resources.MuteIcon;
            }
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Media Files|*.mp4;*.mp3;*.avi;*.mkv;*.wav;*.m4a;*.aac;*.flac;*.ogg;*.wma";
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

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (playlist.Count == 0) return;
            currentTrackIndex = (currentTrackIndex + 1) % playlist.Count;
            UpdateFileType();
            UpdateWindowTitle();
            mediaPlayer.Play(new FileInfo(playlist[currentTrackIndex]));
            btnPlayPause.BackgroundImage = Properties.Resources.PauseIcon;
            UpdateVisualizerVisibility();
            SyncPlaylistToUI();
            UpdateButtonStates();
            UpdateButtonImages();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (playlist.Count == 0) return;
            currentTrackIndex = (currentTrackIndex - 1 + playlist.Count) % playlist.Count;
            UpdateFileType();
            UpdateWindowTitle();
            mediaPlayer.Play(new FileInfo(playlist[currentTrackIndex]));
            btnPlayPause.BackgroundImage = Properties.Resources.PauseIcon;
            UpdateVisualizerVisibility();
            SyncPlaylistToUI();
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


        private void UpdateFileType()
        {
            if (currentTrackIndex < 0 || currentTrackIndex >= playlist.Count)
                return;

            string ext = Path.GetExtension(playlist[currentTrackIndex]).ToLower();
            isAudioFile = (ext == ".mp3" || ext == ".wav" || ext == ".m4a" || ext == ".aac"
                        || ext == ".flac" || ext == ".ogg" || ext == ".wma");


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

                // Disable/enable playback buttons based on media loaded state
                btnPlayPause.Enabled = mediaLoaded;
                btnStop.Enabled = mediaLoaded;
                btnNext.Enabled = mediaLoaded && playlist.Count > 1;
                btnPrev.Enabled = mediaLoaded && playlist.Count > 1;
                seekBar.Enabled = mediaLoaded;

                // Only keep theme button in audio mode
                btnChangeTheme.Enabled = isAudioFile;

                // Update button images based on enabled state
                UpdateButtonImages();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error setting options: {ex.Message}");
                Console.WriteLine($"Error setting options: {ex.Message}");
            }
        }
        // here i will control the button images scale like when click

        private Dictionary<Button, Size> originalSizes = new Dictionary<Button, Size>();

        private void SetupButtonAppearance(Button button)
        {
            // Store original size
            originalSizes[button] = button.Size;

            // Remove the default white hover effect by setting flat style
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(40, 255, 255, 255);
            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(60, 255, 192, 203);

            // Add mouse event handlers for scaling effect
            button.MouseDown += Button_MouseDown;
            button.MouseUp += Button_MouseUp;
            button.MouseLeave += Button_MouseLeave;
        }

        private void Button_MouseDown(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            if (button.Enabled && e.Button == MouseButtons.Left && originalSizes.ContainsKey(button))
            {
                // Calculate new size (95% of original)
                int newWidth = (int)(originalSizes[button].Width * 0.95);
                int newHeight = (int)(originalSizes[button].Height * 0.95);

                // Calculate new position to keep it centered
                int deltaX = (originalSizes[button].Width - newWidth) / 2;
                int deltaY = (originalSizes[button].Height - newHeight) / 2;

                button.Location = new Point(button.Location.X + deltaX, button.Location.Y + deltaY);
                button.Size = new Size(newWidth, newHeight);
            }
        }

        private void Button_MouseUp(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            if (e.Button == MouseButtons.Left && originalSizes.ContainsKey(button))
            {
                RestoreButtonSize(button);
            }
        }

        private void Button_MouseLeave(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if (originalSizes.ContainsKey(button) && button.Size != originalSizes[button])
            {
                RestoreButtonSize(button);
            }
        }

        private void RestoreButtonSize(Button button)
        {
            // Restore original position
            int deltaX = (originalSizes[button].Width - button.Width) / 2;
            int deltaY = (originalSizes[button].Height - button.Height) / 2;

            button.Location = new Point(button.Location.X - deltaX, button.Location.Y - deltaY);

            // Restore original size
            button.Size = originalSizes[button];
        }






        // New helper method to update button background images based on enabled state
        private void UpdateButtonImages()
        {
            // Play/Pause button
            if (!btnPlayPause.Enabled)
            {
                // Use disabled play icon
                btnPlayPause.BackgroundImage = mediaPlayer?.IsPlaying ?? false
                    ? Properties.Resources.DisabledPauseIcon
                    : Properties.Resources.DisabledPlayIcon;
            }
            else
            {
                // Use normal icon
                btnPlayPause.BackgroundImage = mediaPlayer?.IsPlaying ?? false
                    ? Properties.Resources.PauseIcon
                    : Properties.Resources.PlayIcon;
            }

            // Stop button
            btnStop.BackgroundImage = btnStop.Enabled
                ? Properties.Resources.StopIcon
                : Properties.Resources.DisabledStopIcon;

            // Previous button
            btnPrev.BackgroundImage = btnPrev.Enabled
                ? Properties.Resources.PrevIcon
                : Properties.Resources.DisabledPrevIcon;

            // Next button
            btnNext.BackgroundImage = btnNext.Enabled
                ? Properties.Resources.NextIcon
                : Properties.Resources.DisabledNextIcon;

            // Theme button - if it has disabled state
            if (Properties.Resources.ResourceManager.GetObject("DisabledThemeIcon") != null)
            {
                btnChangeTheme.BackgroundImage = btnChangeTheme.Enabled
                    ? Properties.Resources.ThemeIcon
                    : (Image)Properties.Resources.ResourceManager.GetObject("DisabledThemeIcon");
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


        private void btnChangeTheme_Click(object sender, EventArgs e)
        {
            // Change color scheme
            sparkleVisualizer1.ChangeColorScheme();
            sparkleVisualizer1.Visible = true;

            // Force redraw
            sparkleVisualizer1.RefreshVisuals();

            // Make sure it's in front
            sparkleVisualizer1.BringToFront();
        }


        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HelpForm aboutDialogueWindow = new HelpForm();
            aboutDialogueWindow.ShowDialog();
        }

        //private void LogVisualizerState(string action)
        //{
        //    Console.WriteLine($"Action: {action}");
        //    Console.WriteLine($"  isAudioFile: {isAudioFile}");
        //    Console.WriteLine($"  mediaPlayer.IsPlaying: {mediaPlayer?.IsPlaying}");
        //    Console.WriteLine($"  visualizer.Visible: {sparkleVisualizer1.Visible}");
        //    Console.WriteLine($"  visualizer.IsIdleMode: {sparkleVisualizer1.IsIdleMode}");
        //    Console.WriteLine($"  currentTrackIndex: {currentTrackIndex}");
        //}

        // playlist 
        public class Playlist
        {
            public string Name { get; set; }
            public List<string> Tracks { get; set; } = new List<string>();
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

            // Create directory if it doesn't exist
            Directory.CreateDirectory(playlistsFolder);

            foreach (var file in Directory.GetFiles(playlistsFolder, "*.m3u"))
            {
                var pl = new Playlist
                {
                    Name = Path.GetFileNameWithoutExtension(file),
                    Tracks = File.ReadAllLines(file).Where(File.Exists).ToList()
                };
                allPlaylists.Add(pl);
            }

            listSavedPlaylists.Items.AddRange(allPlaylists.Select(p => p.Name).ToArray());
        }

        private void NewPlaylist_Click(object sender, EventArgs e)
        {
            // 🎀 Use our cute dialog instead of default InputBox
            using (var nameDialog = new NewPlaylistDialog())
            {
                if (nameDialog.ShowDialog() != DialogResult.OK) return;

                string name = nameDialog.PlaylistName.Trim();
                if (string.IsNullOrWhiteSpace(name)) return;

                // Create empty playlist 
                var newPl = new Playlist { Name = name };
                allPlaylists.Add(newPl);
                listSavedPlaylists.Items.Add(name);

                // ❓ Ask to add files (we'll keep MessageBox for now)
                DialogResult result = MessageBox.Show("Add files to this playlist now?", "Add Files", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    using OpenFileDialog fileDialog = new OpenFileDialog();
                    fileDialog.Title = "Select Media Files 🎧"; // Add cute emoji
                    fileDialog.Multiselect = true;
                    fileDialog.Filter = "Media Files|*.mp3;*.mp4;*.avi;*.mkv";

                    if (fileDialog.ShowDialog() == DialogResult.OK)
                    {
                        //  Add files (no duplicates) 
                        foreach (string file in fileDialog.FileNames)
                        {
                            if (!newPl.Tracks.Contains(file))
                            {
                                newPl.Tracks.Add(file);
                            }
                        }
                    }
                }

                // 💾 Save changes 
                SaveAllPlaylists();
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


        private void PlayCurrentTrack()
        {
            try
            {
                // Make sure currentTrackIndex is valid
                if (currentTrackIndex < 0 || currentTrackIndex >= currentPlaylist.Tracks.Count)
                {
                    Console.WriteLine("Invalid track index!");
                    return;
                }

                // Get file to play
                string fileToPlay = currentPlaylist.Tracks[currentTrackIndex];

                // Make sure file exists
                if (!File.Exists(fileToPlay))
                {
                    MessageBox.Show($"File not found: {Path.GetFileName(fileToPlay)}");
                    return;
                }

                // Update UI and play the file
                UpdateFileType();
                mediaPlayer.Play(new FileInfo(fileToPlay));
                btnPlayPause.BackgroundImage = Properties.Resources.PauseIcon;
                UpdateVisualizerVisibility(true);
                UpdateWindowTitle();

                // Highlight current track in playlist
                for (int i = 0; i < listPlaylist.Items.Count; i++)
                {
                    if (i == currentTrackIndex)
                    {
                        listPlaylist.SelectedIndex = i;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PlayCurrentTrack: {ex.Message}");
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
                var pl = allPlaylists[listSavedPlaylists.SelectedIndex];

                // Delete file
                File.Delete(Path.Combine(playlistsFolder, $"{pl.Name}.m3u"));

                // Update UI
                allPlaylists.RemoveAt(listSavedPlaylists.SelectedIndex);
                listSavedPlaylists.Items.RemoveAt(listSavedPlaylists.SelectedIndex);
                SaveAllPlaylists();
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


        // I actually forgot what does this do anyway but its somethings in the playlist buttons lol
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
            //  Check if any track is playing
            if (currentTrackIndex == -1 || playlist.Count == 0)
            {
                MessageBox.Show("No track is playing!");
                return;
            }

            //  Get current track
            string currentFile = playlist[currentTrackIndex];

            //  Select target playlist
            Playlist target = SelectPlaylistDialog();
            if (target == null) return;

            //  Check if already exists
            if (!target.Tracks.Contains(currentFile))
            {
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