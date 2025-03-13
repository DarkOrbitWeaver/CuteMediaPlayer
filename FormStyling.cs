using System.Drawing.Drawing2D;

namespace CuteMediaPlayer
{
    public static class FormStyling
    {
        // Track dragging state
        private static bool isDragging = false;
        private static Point startScreenPoint = Point.Empty;
        private static Point startFormLocation = Point.Empty;
        private static Form currentlyDraggingForm = null;

        // Apply rounded corners and styling (unchanged)
        public static void ApplyRoundedStyle(Form form, int cornerRadius = 20, Color? borderColor = null, int borderWidth = 3)
        {
            // Default styling
            Color actualBorderColor = borderColor ?? Color.HotPink;

            // Setup form
            form.FormBorderStyle = FormBorderStyle.None;
            form.BackColor = Color.FromArgb(255, 240, 250); // Your standard pink background

            // Enable double buffering for smooth painting
            typeof(Control).GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance)
                .SetValue(form, true, null);

            // Paint handler for the rounded corners and border
            form.Paint += (sender, e) =>
            {
                using (GraphicsPath path = new GraphicsPath())
                {
                    // Create rounded rectangle path
                    Rectangle bounds = new Rectangle(0, 0, form.Width, form.Height);
                    path.AddArc(bounds.X, bounds.Y, cornerRadius, cornerRadius, 180, 90);
                    path.AddArc(bounds.Right - cornerRadius, bounds.Y, cornerRadius, cornerRadius, 270, 90);
                    path.AddArc(bounds.Right - cornerRadius, bounds.Bottom - cornerRadius, cornerRadius, cornerRadius, 0, 90);
                    path.AddArc(bounds.X, bounds.Bottom - cornerRadius, cornerRadius, cornerRadius, 90, 90);
                    path.CloseFigure();

                    // Apply the region - this defines the form's visible area
                    form.Region = new Region(path);

                    // Draw the border
                    using (Pen pen = new Pen(actualBorderColor, borderWidth))
                    {
                        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            };
        }

        // Make a control draggable
        public static void MakeDraggable(Control control, Form form)
        {
            control.MouseDown += (sender, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    isDragging = true;
                    startScreenPoint = control.PointToScreen(e.Location);
                    startFormLocation = form.Location;
                    currentlyDraggingForm = form;
                }
            };

            control.MouseMove += (sender, e) =>
            {
                if (isDragging && currentlyDraggingForm == form)
                {
                    Point currentScreenPoint = control.PointToScreen(e.Location);
                    int deltaX = currentScreenPoint.X - startScreenPoint.X;
                    int deltaY = currentScreenPoint.Y - startScreenPoint.Y;
                    form.Location = new Point(
                        startFormLocation.X + deltaX,
                        startFormLocation.Y + deltaY
                    );
                }
            };

            control.MouseUp += (sender, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    isDragging = false;
                    currentlyDraggingForm = null;
                }
            };
        }
    }
}