namespace CuteMediaPlayer
{
    public partial class PlaylistDialog : Form
    {
        public ListBox PlaylistListBox => listPlaylists;

        public PlaylistDialog()
        {
            InitializeComponent();

            // Apply rounded styling
            FormStyling.ApplyRoundedStyle(this);

            // Make specific controls draggable - NOT buttons or the list
            FormStyling.MakeDraggable(this, this); // The form itself
            FormStyling.MakeDraggable(label1, this); // Just the label

            // Setup button handlers
            btnOK.Click += (s, e) => DialogResult = DialogResult.OK;
            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;
        }
    }
}