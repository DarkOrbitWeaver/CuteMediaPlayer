// ✨ Sparkle Visualizer Module
using SkiaSharp;
using System.Drawing.Drawing2D;
using Timer = System.Windows.Forms.Timer;


namespace CuteMediaPlayer
{

    public partial class SparkleVisualizer : UserControl
    {
        private readonly Random random = new Random();
        private readonly List<Point> sparkles = new List<Point>();

        public bool IsIdleMode { get; set; } = true;
        private float rotationAngle = 0;


        // Add these at the top of SparkleVisualizer.cs
        private float[] audioValues = new float[10]; // Store audio levels
        private List<ColorScheme> colorSchemes = new List<ColorScheme>();
        private int currentColorScheme = 5; // white color as default

        // background
        private Image idleWallpaper;
        private bool useIdleWallpaper = true;

        public bool UseIdleWallpaper
        {
            get { return useIdleWallpaper; }
            set
            {
                useIdleWallpaper = value;
                Invalidate(); // Redraw when changed
            }
        }


        public SparkleVisualizer()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

            // Allow transparent background
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = Color.Transparent;

            // Add color schemes
            colorSchemes.Add(new ColorScheme(Color.HotPink, Color.DeepSkyBlue, Color.Gold)); // Default
            colorSchemes.Add(new ColorScheme(Color.Purple, Color.Violet, Color.LightPink)); // Purple theme
            colorSchemes.Add(new ColorScheme(Color.Blue, Color.LightBlue, Color.DodgerBlue)); // Blue theme
            colorSchemes.Add(new ColorScheme(Color.Green, Color.LightGreen, Color.YellowGreen)); // Green theme
            colorSchemes.Add(new ColorScheme(Color.Red, Color.Crimson, Color.IndianRed)); // Red theme
            colorSchemes.Add(new ColorScheme(
                 Color.FromArgb(255, 214, 249),  // Baby pink 
                 Color.FromArgb(255, 255, 255),  // Pure white
                 Color.FromArgb(255, 163, 232)   // Bubblegum pink
            ));

            // Update visuals every 50ms
            Timer animationTimer = new Timer { Interval = 50 };
            animationTimer.Tick += (s, e) => Invalidate();
            animationTimer.Start();
        }

        // i disabled this for now
        //public void SetIdleWallpaper(Image wallpaper)
        //{
        //    idleWallpaper = wallpaper;
        //    Invalidate(); // Redraw with new wallpaper
        //}



        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Transparent); // Ensure transparency
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // 🎨 Idle animation (rotating star)
            if (IsIdleMode)
            {
                // Draw wallpaper if available and enabled
                if (idleWallpaper != null && useIdleWallpaper)
                {
                    // Draw the wallpaper to fill the control
                    e.Graphics.DrawImage(idleWallpaper, ClientRectangle);
                }
                //else
                //{
                //    // Original background drawing code (black)
                //    e.Graphics.Clear(Color.Black);
                //}

                // Draw a rotating pink star
                using (var starBrush = new SolidBrush(Color.FromArgb(0xFE, 0xB8, 0xC3))) //this color #feb8c3
                {
                    rotationAngle += 2; // Rotation speed
                    e.Graphics.TranslateTransform(Width / 2, Height / 2);
                    e.Graphics.RotateTransform(rotationAngle % 360);

                    var points = new PointF[]
                    {
                new PointF(0, -40), new PointF(15, -15),
                new PointF(40, 0), new PointF(15, 15),
                new PointF(0, 40), new PointF(-15, 15),
                new PointF(-40, 0), new PointF(-15, -15)
                    };
                    e.Graphics.FillPolygon(starBrush, points);
                }
            }
            else
            {
                // Only draw visualizer if visible
                if (this.Visible)
                {
                    ColorScheme scheme = colorSchemes[currentColorScheme];

                    // Get current audio values - use maximum as a "beat" strength
                    float maxAudio = 0;
                    for (int i = 0; i < audioValues.Length; i++)
                    {
                        maxAudio = Math.Max(maxAudio, audioValues[i]);
                    }

                    // Draw the wave circle visualization - now this is our only visualization mode
                    DrawWaveCircleVisualization(e.Graphics, scheme, maxAudio);
                }
            }
        }



        // Call this from Form1 to update audio levels
        public void UpdateAudioData(float[] newValues)
        {
            // Copy values into our buffer
            for (int i = 0; i < Math.Min(newValues.Length, audioValues.Length); i++)
            {
                audioValues[i] = newValues[i];
            }

            // Update visualizer
            Invalidate();
        }

        // change the color scheme
        public void ChangeColorScheme()
        {
            currentColorScheme = (currentColorScheme + 1) % colorSchemes.Count;
        }


        public void RefreshVisuals()
        {
            // Force the visualizer to refresh with the current settings
            Invalidate();
        }

        private void DrawWaveCircleVisualization(Graphics g, ColorScheme scheme, float maxAudio)
        {
            int centerX = Width / 2;
            int centerY = Height / 2;

            // Get current time for animation
            double time = DateTime.Now.TimeOfDay.TotalMilliseconds / 1000.0;

            // Calculate beat response for smoother transitions
            float beatPulse = 0;
            float bassEnergy = 0;

            // Get bass energy (first few bands) for better bass response
            for (int i = 0; i < Math.Min(4, audioValues.Length); i++)
            {
                bassEnergy += audioValues[i];
            }
            bassEnergy /= 4; // Average

            // Enhanced beat pulse calculation that feels more responsive
            beatPulse = Math.Max(maxAudio, bassEnergy * 1.2f);

            // Core size and parameters
            int minCoreSize = Math.Min(Width, Height) / 6;
            int maxCoreExpansion = Math.Min(Width, Height) / 3;

            // Core size now directly tied to bass energy for better response
            int coreSize = minCoreSize + (int)(beatPulse * maxCoreExpansion);

            // Add subtle "breathing" effect even when no audio
            coreSize += (int)(3 * Math.Sin(time * 1.5));

            // Draw glowing core with enhanced gradient
            using (var corePath = new GraphicsPath())
            {
                corePath.AddEllipse(
                    centerX - coreSize / 2,
                    centerY - coreSize / 2,
                    coreSize,
                    coreSize
                );

                // Create gradient for core
                using (PathGradientBrush coreBrush = new PathGradientBrush(corePath))
                {
                    // Color intensity based on beat
                    int colorIntensity = 50 + (int)(beatPulse * 50);

                    // Brighten center color based on beat intensity
                    Color centerColor = Color.FromArgb(
                        Math.Min(255, scheme.Primary.R + colorIntensity),
                        Math.Min(255, scheme.Primary.G + colorIntensity),
                        Math.Min(255, scheme.Primary.B + colorIntensity)
                    );

                    coreBrush.CenterColor = centerColor;
                    coreBrush.SurroundColors = new Color[] { scheme.Primary };

                    g.FillPath(coreBrush, corePath);
                }
            }

            // Number of waves depends on audio intensity
            int baseWaveCount = 3;
            int dynamicWaveCount = (int)(maxAudio * 5); // More waves on louder parts
            int numberOfWaves = baseWaveCount + dynamicWaveCount;

            // Improved wave parameters for better visual response
            float waveAmplitudeMultiplier = 2.0f + (beatPulse * 3.0f); // Waves expand more on beats

            // Draw waves in reverse order (outer to inner) for better layering
            for (int i = numberOfWaves - 1; i >= 0; i--)
            {
                // Each wave responds to different audio frequencies
                int bandIndex = (audioValues.Length * i) / numberOfWaves;
                bandIndex = Math.Min(bandIndex, audioValues.Length - 1);

                // Get specific band energy for this wave
                float bandEnergy = bandIndex < audioValues.Length ? audioValues[bandIndex] : 0;

                // Calculate wave parameters
                int baseRadius = coreSize / 2 + (i * 25); // Base wave spacing

                // Wave intensity based on band energy and distance
                float waveIntensity = bandEnergy * (1.0f - (i * 0.1f));

                // Wave distortion based on specific frequency response 
                float waveDistortion = bandEnergy * 30 * (1.0f - (i * 0.08f));

                // Create dynamic wave shape
                int segments = 72; // Higher segments for smoother waves
                PointF[] wavePoints = new PointF[segments];

                for (int j = 0; j < segments; j++)
                {
                    double angle = j * Math.PI * 2 / segments;

                    // Complex waveform that combines multiple frequencies
                    float distortion = waveDistortion * (
                        (float)Math.Sin(angle * 2 + time * 1.5) * 0.5f +
                        (float)Math.Sin(angle * 3 + time * 2.5 + i) * 0.3f +
                        (float)Math.Sin(angle * 5 + time * 0.8) * 0.2f
                    );

                    // More responsive radius calculation
                    float radius = baseRadius + distortion;

                    // Add specific band energy influence
                    radius += (int)(bandEnergy * 50 * waveAmplitudeMultiplier);

                    // Add subtle time-based motion
                    radius += (float)(5 * Math.Sin(angle * 2 + time + i * 0.7));

                    wavePoints[j] = new PointF(
                        centerX + (float)(radius * Math.Cos(angle)),
                        centerY + (float)(radius * Math.Sin(angle))
                    );
                }

                // Calculate alpha based on energy and wave index
                int baseAlpha = 120 - (i * 8);
                int energyAlpha = (int)(100 * waveIntensity);
                int alpha = Math.Min(200, Math.Max(40, baseAlpha + energyAlpha));

                // Choose wave color based on frequency band
                Color waveColor;
                if (bandIndex < audioValues.Length / 3) // Bass frequencies
                    waveColor = scheme.Primary;
                else if (bandIndex < 2 * audioValues.Length / 3) // Mid frequencies
                    waveColor = scheme.Accent;
                else // High frequencies
                    waveColor = scheme.Secondary;

                // Apply alpha to color
                Color alphaColor = Color.FromArgb(alpha, waveColor);

                // Wave thickness based on energy
                float thickness = 1.5f + (3 * waveIntensity);
                thickness = Math.Max(1, thickness); // Ensure minimum thickness

                using (var wavePen = new Pen(alphaColor, thickness))
                {
                    // Add glow effect on strong beats
                    if (beatPulse > 0.7f && i < 3)
                    {
                        wavePen.Width += beatPulse * 2;
                    }

                    // Draw the wave with tension adjusted for smoother curves
                    g.DrawClosedCurve(wavePen, wavePoints, 0.3f, FillMode.Winding);
                }
            }

            // Add sparkle effects on beat
            if (beatPulse > 0.65f)
            {
                int sparkleCount = (int)(beatPulse * 20); // More sparkles on stronger beats
                using (var sparkleBrush = new SolidBrush(Color.FromArgb(200, scheme.Accent)))
                {
                    for (int i = 0; i < sparkleCount; i++)
                    {
                        double angle = random.NextDouble() * Math.PI * 2;
                        float distance = coreSize / 2 + (float)(random.NextDouble() * coreSize * 1.5);
                        int sparkleSize = 2 + (int)(random.NextDouble() * 5 * beatPulse);

                        g.FillEllipse(sparkleBrush,
                            centerX + (float)(distance * Math.Cos(angle)) - sparkleSize / 2,
                            centerY + (float)(distance * Math.Sin(angle)) - sparkleSize / 2,
                            sparkleSize,
                            sparkleSize
                        );
                    }
                }
            }
        }






    }
    // Simple class to store color schemes
    internal class ColorScheme
    {
        public Color Primary { get; set; }
        public Color Secondary { get; set; }
        public Color Accent { get; set; }

        public ColorScheme(Color primary, Color secondary, Color accent)
        {
            Primary = primary;
            Secondary = secondary;
            Accent = accent;
        }
    }

}

