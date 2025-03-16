namespace CuteMediaPlayer
{
    partial class CustomPlaylistPanel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            listEmptyLabel = new Label();
            btnAddSongs = new Button();
            SuspendLayout();
            // 
            // listEmptyLabel
            // 
            listEmptyLabel.Dock = DockStyle.Top;
            listEmptyLabel.Font = new Font("MS PGothic", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            listEmptyLabel.Location = new Point(0, 0);
            listEmptyLabel.Name = "listEmptyLabel";
            listEmptyLabel.Size = new Size(332, 38);
            listEmptyLabel.TabIndex = 0;
            listEmptyLabel.Text = "this list is empty, try adding something ^_~";
            listEmptyLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnAddSongs
            // 
            btnAddSongs.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnAddSongs.BackColor = Color.FromArgb(255, 192, 192);
            btnAddSongs.Cursor = Cursors.Hand;
            btnAddSongs.Font = new Font("MS PGothic", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnAddSongs.Location = new Point(91, 56);
            btnAddSongs.Margin = new Padding(3, 4, 3, 4);
            btnAddSongs.Name = "btnAddSongs";
            btnAddSongs.Size = new Size(126, 36);
            btnAddSongs.TabIndex = 1;
            btnAddSongs.Text = "Open Files";
            btnAddSongs.UseVisualStyleBackColor = false;
            btnAddSongs.Click += btnAddSongs_Click;
            // 
            // CustomPlaylistPanel
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(254, 184, 195);
            Controls.Add(btnAddSongs);
            Controls.Add(listEmptyLabel);
            Name = "CustomPlaylistPanel";
            Size = new Size(332, 327);
            ResumeLayout(false);
        }

        #endregion

        private Label listEmptyLabel;
        private Button btnAddSongs;
    }
}
