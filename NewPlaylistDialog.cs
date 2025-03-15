namespace CuteMediaPlayer
{
    partial class NewPlaylistDialog : Form
    {
        // Allow setting the textbox value
        public string PlaylistName
        {
            get => txtInput.Text;
            set => txtInput.Text = value; // New setter!
        }

        public NewPlaylistDialog()
        {
            InitializeComponent();

            // Apply cute styling 🌸
            FormStyling.ApplyRoundedStyle(this);
            FormStyling.MakeDraggable(lblPrompt, this);
            FormStyling.MakeDraggable(this, this);

            // Button handlers
            btnOK.Click += (s, e) => DialogResult = DialogResult.OK;
            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;
        }
    }
}

