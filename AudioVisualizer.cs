using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timer = System.Windows.Forms.Timer;

namespace CuteMediaPlayer
{
    public partial class Form1
    {
        // to toggle audio visualizer
        private bool isAudioFile = false;
        private Timer audioUpdateTimer;

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
                        // for the dancing girl to change dance
                        if (random.Next(3) == 2)
                        {
                            switchFrameSet = true;
                        }
                    }

                    // Send the improved data to the visualizer
                    sparkleVisualizer1.UpdateAudioData(visData);
                }
            };

            audioUpdateTimer.Start();
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
                dancingGirlPictureBox.BringToFront();
                dancingGirlPictureBox.Visible = isPlaying;
                //sparkleVisualizer1.UseIdleWallpaper = true;
            }
            // For video files, hide visualizer completely
            else if (mediaLoaded && !isAudioFile)
            {
                sparkleVisualizer1.Visible = false;
                //sparkleVisualizer1.UseIdleWallpaper = false;
                dancingGirlPictureBox.Visible = false;
            }
            // If nothing is loaded, show idle visualizer
            else
            {
                sparkleVisualizer1.Visible = true;
                sparkleVisualizer1.IsIdleMode = true;
                dancingGirlPictureBox.Visible = false;
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


        private void btnChangeTheme_Click(object sender, EventArgs e)
        {
            // Change color scheme
            sparkleVisualizer1.ChangeColorScheme();
            sparkleVisualizer1.Visible = true;
            dancingGirlPictureBox.Visible = true;
            // Force redraw
            sparkleVisualizer1.RefreshVisuals();

            // Make sure it's in front
            sparkleVisualizer1.BringToFront();
            dancingGirlPictureBox.BringToFront();
            UpdateVisualizerVisibility();
        }


    }
}
