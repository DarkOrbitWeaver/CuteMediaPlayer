namespace CuteMediaPlayer
{
    partial class CustomTrackItem
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            pictureAlbumArt = new PictureBox();
            lblTitle = new Label();
            lblArtist = new Label();
            lblDuration = new Label();
            toolTip1 = new ToolTip(components);
            ((System.ComponentModel.ISupportInitialize)pictureAlbumArt).BeginInit();
            SuspendLayout();
            // 
            // pictureAlbumArt
            // 
            pictureAlbumArt.BackColor = Color.FromArgb(254, 230, 235);
            pictureAlbumArt.Location = new Point(10, 9);
            pictureAlbumArt.Name = "pictureAlbumArt";
            pictureAlbumArt.Size = new Size(52, 52);
            pictureAlbumArt.SizeMode = PictureBoxSizeMode.Zoom;
            pictureAlbumArt.TabIndex = 0;
            pictureAlbumArt.TabStop = false;
            // 
            // lblTitle
            // 
            lblTitle.AutoEllipsis = true;
            lblTitle.Font = new Font("MS PGothic", 11F, FontStyle.Bold);
            lblTitle.Location = new Point(77, 9);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(257, 26);
            lblTitle.TabIndex = 1;
            lblTitle.Text = "Title";
            // 
            // lblArtist
            // 
            lblArtist.AutoEllipsis = true;
            lblArtist.Font = new Font("MS PGothic", 9.75F);
            lblArtist.ForeColor = Color.FromArgb(90, 90, 90);
            lblArtist.Location = new Point(77, 28);
            lblArtist.Name = "lblArtist";
            lblArtist.Size = new Size(257, 23);
            lblArtist.TabIndex = 2;
            lblArtist.Text = "Artist";
            // 
            // lblDuration
            // 
            lblDuration.AutoSize = true;
            lblDuration.Font = new Font("MS PGothic", 9.75F);
            lblDuration.ForeColor = Color.FromArgb(90, 90, 90);
            lblDuration.Location = new Point(77, 46);
            lblDuration.Name = "lblDuration";
            lblDuration.Size = new Size(31, 13);
            lblDuration.TabIndex = 3;
            lblDuration.Text = "0:00";
            // 
            // CustomTrackItem
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(254, 218, 224);
            Controls.Add(lblDuration);
            Controls.Add(lblArtist);
            Controls.Add(lblTitle);
            Controls.Add(pictureAlbumArt);
            Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Name = "CustomTrackItem";
            Size = new Size(500, 68);
            ((System.ComponentModel.ISupportInitialize)pictureAlbumArt).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureAlbumArt;
        private Label lblTitle;
        private Label lblArtist;
        private Label lblDuration;
        private ToolTip toolTip1;
    }
}