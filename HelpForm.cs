using System.Drawing.Drawing2D;

namespace CuteMediaPlayer
{
    public partial class HelpForm : Form
    {
        public HelpForm()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
    
                // Draw rounded rectangle
                int cornerRadius = 20;
                GraphicsPath path = new GraphicsPath();
                path.AddArc(0, 0, cornerRadius, cornerRadius, 180, 90); // Top-left
                path.AddArc(Width - cornerRadius, 0, cornerRadius, cornerRadius, 270, 90); // Top-right
                path.AddArc(Width - cornerRadius, Height - cornerRadius, cornerRadius, cornerRadius, 0, 90); // Bottom-right
                path.AddArc(0, Height - cornerRadius, cornerRadius, cornerRadius, 90, 90); // Bottom-left
                path.CloseFigure();
    
                // Apply the rounded region to the form
                this.Region = new Region(path);
    
                // Optional: Draw a border
                using (Pen pen = new Pen(Color.HotPink, 3))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            }


        // drage stuff
        private bool dragging;
        private Point startPoint;

        private void HelpForm_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            startPoint = new Point(e.X, e.Y);
        }

        private void HelpForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point newPoint = PointToScreen(new Point(e.X, e.Y));
                Location = new Point(newPoint.X - startPoint.X, newPoint.Y - startPoint.Y);
            }
        }

        private void HelpForm_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
