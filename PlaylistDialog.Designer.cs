namespace CuteMediaPlayer
{
    partial class PlaylistDialog
    {
        private System.ComponentModel.IContainer components = null;
        private Button btnOK;
        private Button btnCancel;
        private Label label1;

        private void InitializeComponent()
        {
            btnOK = new Button();
            btnCancel = new Button();
            label1 = new Label();
            SuspendLayout();

            // Create the CustomPlaylistPanel
            playlistPanel = new CustomPlaylistPanel();
            playlistPanel.Location = new Point(12, 54);
            playlistPanel.Size = new Size(347, 244);
            playlistPanel.AutoScroll = true;
            playlistPanel.Name = "playlistPanel";
            playlistPanel.TabIndex = 0;

            // 
            // btnOK
            // 
            btnOK.Cursor = Cursors.Hand;
            btnOK.Location = new Point(268, 308);
            btnOK.Margin = new Padding(3, 4, 3, 4);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(96, 42);
            btnOK.TabIndex = 1;
            btnOK.Text = "OK";

            // 
            // btnCancel
            // 
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.Location = new Point(168, 308);
            btnCancel.Margin = new Padding(3, 4, 3, 4);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(93, 42);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "Cancel";

            // 
            // label1
            // 
            label1.Font = new Font("MS PGothic", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(13, 9);
            label1.Name = "label1";
            label1.Size = new Size(347, 41);
            label1.TabIndex = 3;
            label1.Text = "Select a playlist";

            // 
            // PlaylistDialog
            // 
            AutoScaleDimensions = new SizeF(10F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(254, 184, 195);
            ClientSize = new Size(376, 363);
            Controls.Add(label1);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            Controls.Add(playlistPanel);
            Font = new Font("MS PGothic", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(4, 2, 4, 2);
            Name = "PlaylistDialog";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Select Playlist";
            ResumeLayout(false);
        }
    }
}