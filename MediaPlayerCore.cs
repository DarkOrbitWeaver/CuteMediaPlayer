using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vlc.DotNet.Forms;

namespace CuteMediaPlayer
{
    public partial class Form1
    {
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

                                    // switch the position of dancing girl
                                    if (random.Next(3) == 2)
                                    {
                                        dancingGirlPictureBox.Location = new Point(551, 356);
                                        dancingGirlPictureBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
                                    }else
                                    {
                                        dancingGirlPictureBox.Location = new Point(0, 356);
                                        dancingGirlPictureBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                                    }

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
                                dancingGirlPictureBox.Visible = true;
                                sparkleVisualizer1.IsIdleMode = false;
                                sparkleVisualizer1.BringToFront();
                                sparkleVisualizer1.RefreshVisuals();
                                dancingGirlPictureBox.BringToFront();
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
                                dancingGirlPictureBox.Visible = false;
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
                customPlaylistPanel.SelectTrack(currentTrackIndex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PlayCurrentTrack: {ex.Message}");
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

        private void UpdateFileType()
        {
            if (currentTrackIndex < 0 || currentTrackIndex >= playlist.Count)
                return;

            string ext = Path.GetExtension(playlist[currentTrackIndex]).ToLower();
            isAudioFile = (ext == ".mp3" || ext == ".wav" || ext == ".m4a" || ext == ".aac"
                        || ext == ".flac" || ext == ".ogg" || ext == ".wma");


        }

        public void btnPlayPause_Click(object sender, EventArgs e)
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

        public void btnStop_Click(object sender, EventArgs e)
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

        public void btnNext_Click(object sender, EventArgs e)
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

        public void btnPrev_Click(object sender, EventArgs e)
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



    }
}
