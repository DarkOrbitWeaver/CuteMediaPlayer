namespace CuteMediaPlayer
{
    partial class NewPlaylistDialog : Form
    {
        public string PlaylistName => txtInput.Text;

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