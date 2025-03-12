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
        private bool isAudioFile = false; // Add this class-level variable

        // loop
        private bool loopEnabled = false;

        // visualizer theme
        private int lastVolumeBarValue = 100;

        public Form1()
        {
            InitializeComponent();
            UpdateButtonStates();
            InitializeMediaPlayer();
            InitializeAudioSampling();

            // Set up event handlers for disabled state changes
            btnPlayPause.EnabledChanged += (s, e) => UpdateButtonImages();
            btnStop.EnabledChanged += (s, e) => UpdateButtonImages();
            btnPrev.EnabledChanged += (s, e) => UpdateButtonImages();
            btnNext.EnabledChanged += (s, e) => UpdateButtonImages();
            btnChangeTheme.EnabledChanged += (s, e) => UpdateButtonImages();

            // set up other ones to handle the change of button size and click stuff kinda like animations
            // Add this to the InitializeMediaPlayer method after creating the buttons
            SetupButtonAppearance(btnPlayPause);
            SetupButtonAppearance(btnStop);
            SetupButtonAppearance(btnPrev);
            SetupButtonAppearance(btnNext);
            SetupButtonAppearance(btnChangeTheme);
            SetupButtonAppearance(btnMute);
            SetupButtonAppearance(loopBtn);


            // 🕒 Timer to update UI elements (seek bar/time label)
            uiTimer = new Timer { Interval = 250 }; // Smoother updates at 250ms

            uiTimer.Tick += (s, e) =>
            {
                try
                {
                    // Check if media player exists and has valid media
                    if (mediaPlayer == null || !mediaPlayer.IsPlaying) return;

                    // Only update seek bar if user isn't dragging it
                    if (!seekBar.Capture)
                    {
                        // Convert media position, seek bar value (0-1000)
                        seekBar.Value = (int)(mediaPlayer.Position * seekBar.Maximum);
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
                            // Check if loop is enabled
                            if (loopEnabled && playlist.Count > 0)
                            {
                                // Replay the current file
                                mediaPlayer.Play(new FileInfo(playlist[currentTrackIndex]));
                                btnPlayPause.BackgroundImage = Properties.Resources.PauseIcon;
                            }
                            else
                            {
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
                        }));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in EndReached: {ex.Message}");
                    }
                };


                // Add this in InitializeMediaPlayer() after other event handlers
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
                if (mediaPlayer.IsPlaying)
                {
                    mediaPlayer.Pause();
                    //btnPlayPause.Text = "Play";
                    btnPlayPause.BackgroundImage = Properties.Resources.PlayIcon;
                }
                else
                {
                    // If we have a file selected in the playlist
                    if (currentTrackIndex != -1)
                    {
                        // Check if we're at the end by looking at position
                        if (mediaPlayer.Position <= 0.01 || mediaPlayer.Position >= 0.99)
                        {
                            if (playlist.Count > 0 && currentTrackIndex < playlist.Count)
                            {
                                // Reload the current file
                                mediaPlayer.Play(new FileInfo(playlist[currentTrackIndex]));
                            }
                        }
                        else
                        {
                            // Just resume if paused in the middle
                            mediaPlayer.Play();
                        }
                    }
                    btnPlayPause.BackgroundImage = Properties.Resources.PauseIcon;

                    //btnPlayPause.Text = "Pause";
                }

                // Use our consolidated method with force refresh
                UpdateVisualizerVisibility(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error playing media: {ex.Message}", "Playback Error");
            }

            UpdateButtonStates();
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
            // ⏱️ Format current time and total time
            TimeSpan currentTime = TimeSpan.FromMilliseconds(mediaPlayer.Time);
            TimeSpan totalTime = TimeSpan.FromMilliseconds(mediaPlayer.Length);
            lblTime.Text = $"{currentTime:mm\\:ss} / {totalTime:mm\\:ss}";
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
            dialog.Filter = "Media Files|*.mp4;*.mp3;*.avi;*.mkv;*.wav;*.m4a;*.aac;*.flac;*.ogg;*.wma";
            dialog.Multiselect = true;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                playlist = dialog.FileNames.ToList();
                currentTrackIndex = 0;

                // Update file type
                UpdateFileType();

                // Update window title
                UpdateWindowTitle();

                // Set visualizer state
                UpdateVisualizerVisibility(false); // Don't refresh yet

                // Play the file
                mediaPlayer.Play(new FileInfo(playlist[currentTrackIndex]));
                btnPlayPause.BackgroundImage = Properties.Resources.PauseIcon;

                // Set audio settings
                mediaPlayer.Audio.Volume = volumeBar.Value;
                isMuted = false;
                btnMute.BackgroundImage = Properties.Resources.MuteIcon;
            }

            UpdateButtonStates();
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

        private void LogVisualizerState(string action)
        {
            Console.WriteLine($"Action: {action}");
            Console.WriteLine($"  isAudioFile: {isAudioFile}");
            Console.WriteLine($"  mediaPlayer.IsPlaying: {mediaPlayer?.IsPlaying}");
            Console.WriteLine($"  visualizer.Visible: {sparkleVisualizer1.Visible}");
            Console.WriteLine($"  visualizer.IsIdleMode: {sparkleVisualizer1.IsIdleMode}");
            Console.WriteLine($"  currentTrackIndex: {currentTrackIndex}");
        }


    }
}
