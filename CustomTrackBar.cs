using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;


// i removed the heart shape since its broken bruh

namespace CuteMediaPlayer
{
    public class CustomTrackBar : TrackBar
    {
        // Customizable colors
        public Color TrackColor { get; set; } = Color.Pink;
        public Color ThumbColor { get; set; } = Color.HotPink;
        public Color ProgressColor { get; set; } = Color.DeepPink; // Color for the "filled" part

        // Customizable sizes
        public int TrackHeight { get; set; } = 6;
        public int ThumbSize { get; set; } = 14; // Larger thumb for easier interaction

        // Shape options
        public enum ThumbShape { Circle, Heart }
        public ThumbShape Thumb { get; set; } = ThumbShape.Circle;

        // Track if mouse is pressed
        private bool isDragging = false;

        public CustomTrackBar()
        {
            // Enable custom painting and other needed styles
            SetStyle(ControlStyles.UserPaint |
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw, true);

            // Make sure there's a visible focus rectangle when the control has focus
            SetStyle(ControlStyles.Selectable, true);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                UpdateValueFromMousePosition(e.X);
                this.Capture = true;
                this.Focus();
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (isDragging)
            {
                UpdateValueFromMousePosition(e.X);
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            isDragging = false;
            this.Capture = false;
            base.OnMouseUp(e);
        }

        private void UpdateValueFromMousePosition(int x)
        {
            // Calculate padding for left and right bounds
            int padding = ThumbSize / 2;
            int usableWidth = ClientRectangle.Width - (padding * 2);

            // Limit x to the valid range
            x = Math.Max(padding, Math.Min(x, Width - padding));

            // Calculate the new value based on mouse position
            float fraction = (float)(x - padding) / usableWidth;
            int newValue = Minimum + (int)Math.Round(fraction * (Maximum - Minimum));

            // Update value with bounds check
            Value = Math.Max(Minimum, Math.Min(newValue, Maximum));

            // Trigger the Scroll event
            OnScroll(new EventArgs());
        }

        // This will be called when Value is changed programmatically
        protected override void OnValueChanged(EventArgs e)
        {
            // Invalidate to trigger repaint
            Invalidate();
            base.OnValueChanged(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = ClientRectangle;
            int padding = ThumbSize / 2;
            int trackY = rect.Height / 2;

            // Draw the background track
            using (Pen trackPen = new Pen(TrackColor, TrackHeight))
            {
                g.DrawLine(trackPen, padding, trackY, rect.Width - padding, trackY);
            }

            // Calculate the position for the thumb based on current Value
            float fraction = (Value - Minimum) / (float)(Maximum - Minimum);
            int thumbX = padding + (int)(fraction * (rect.Width - (padding * 2)));

            // Draw the filled/progress portion of the track
            using (Pen progressPen = new Pen(ProgressColor, TrackHeight))
            {
                g.DrawLine(progressPen, padding, trackY, thumbX, trackY);
            }

            // Draw the thumb based on selected shape
            using (SolidBrush thumbBrush = new SolidBrush(ThumbColor))
            {
                if (Thumb == ThumbShape.Circle)
                {
                    // Draw circle thumb
                    g.FillEllipse(thumbBrush, thumbX - (ThumbSize / 2), trackY - (ThumbSize / 2), ThumbSize, ThumbSize);

                    // Add a subtle highlight for 3D effect
                    using (SolidBrush highlightBrush = new SolidBrush(Color.FromArgb(80, Color.White)))
                    {
                        g.FillEllipse(highlightBrush, thumbX - (ThumbSize / 2) + 2, trackY - (ThumbSize / 2) + 2, ThumbSize / 2, ThumbSize / 2);
                    }
                }
                //else if (Thumb == ThumbShape.Heart)
                //{
                //    // Draw heart-shaped thumb
                //    DrawHeart(g, thumbBrush, thumbX, trackY, ThumbSize);
                //}
            }

            // Removed focus rectangle
        }

        //// Method to draw a heart shape
        //private void DrawHeart(Graphics g, Brush brush, int centerX, int centerY, int size)
        //{
        //    // Create a path for the heart shape
        //    using (GraphicsPath path = new GraphicsPath())
        //    {
        //        // Scale factor - makes the heart fit better in the given size
        //        float scale = size / 20f;

        //        // Offset to center the heart
        //        int offsetX = centerX - (int)(10 * scale);
        //        int offsetY = centerY - (int)(10 * scale);

        //        // Create the heart shape
        //        path.AddBezier(
        //            offsetX + 10 * scale, offsetY + 6 * scale,  // Start point
        //            offsetX + 7 * scale, offsetY + 0 * scale,   // Control point 1
        //            offsetX + 0 * scale, offsetY + 5 * scale,   // Control point 2
        //            offsetX + 5 * scale, offsetY + 15 * scale   // End point
        //        );

        //        path.AddBezier(
        //            offsetX + 5 * scale, offsetY + 15 * scale,  // Start point
        //            offsetX + 15 * scale, offsetY + 5 * scale,  // Control point 1
        //            offsetX + 13 * scale, offsetY + 0 * scale,  // Control point 2
        //            offsetX + 10 * scale, offsetY + 6 * scale   // End point
        //        );

        //        // Fill the heart
        //        g.FillPath(brush, path);

        //        // Add a subtle highlight for 3D effect
        //        using (Brush highlightBrush = new SolidBrush(Color.FromArgb(80, Color.White)))
        //        {
        //            g.FillEllipse(
        //                highlightBrush,
        //                offsetX + 3 * scale,
        //                offsetY + 5 * scale,
        //                6 * scale,
        //                6 * scale
        //            );
        //        }
        //    }
        //}

        // Handle keyboard navigation
        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                case Keys.Down:
                    Value = Math.Max(Minimum, Value - SmallChange);
                    e.Handled = true;
                    break;

                case Keys.Right:
                case Keys.Up:
                    Value = Math.Min(Maximum, Value + SmallChange);
                    e.Handled = true;
                    break;

                case Keys.Home:
                    Value = Minimum;
                    e.Handled = true;
                    break;

                case Keys.End:
                    Value = Maximum;
                    e.Handled = true;
                    break;

                case Keys.PageDown:
                    Value = Math.Max(Minimum, Value - LargeChange);
                    e.Handled = true;
                    break;

                case Keys.PageUp:
                    Value = Math.Min(Maximum, Value + LargeChange);
                    e.Handled = true;
                    break;
            }

            if (e.Handled)
            {
                OnScroll(new EventArgs());
            }
            else
            {
                base.OnKeyDown(e);
            }
        }
    }
}


// our custom track bar.
// here is the code to set it 
/*
 
// In your Form1.cs constructor or InitializeComponent method
seekBar.Thumb = CustomTrackBar.ThumbShape.Heart;
volumeBar.Thumb = CustomTrackBar.ThumbShape.Heart;

// Adjust thumb size if needed (hearts may look better slightly larger)
seekBar.ThumbSize = 16;
volumeBar.ThumbSize = 16;
 
 */