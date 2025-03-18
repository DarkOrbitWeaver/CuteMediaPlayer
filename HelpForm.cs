// HelpForm.cs
namespace CuteMediaPlayer
{
    public partial class HelpForm : Form
    {
        public HelpForm()
        {
            InitializeComponent();

            // Apply rounded styling
            FormStyling.ApplyRoundedStyle(this, borderColor: Color.FromArgb(254, 184, 195), backgroundColor: Color.FromArgb(255, 240, 250));

            // Make specific controls draggable - NOT the button
            FormStyling.MakeDraggable(this, this); // The form itself
            FormStyling.MakeDraggable(label1, this); // The label
            // Note: button1 is NOT draggable
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}