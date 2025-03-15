using CuteMediaPlayer;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Xml.Linq;
using Font = System.Drawing.Font;

namespace CuteMediaPlayer
{
    partial class MinimizedPlayer
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnNext = new Button();
            btnPrev = new Button();
            btnPlayPause = new Button();
            closeMinimizerPlayer = new Button();
            SuspendLayout();
            // 
            // btnNext
            // 
            btnNext.BackColor = Color.Transparent;
            btnNext.BackgroundImage = Properties.Resources.NextIcon;
            btnNext.BackgroundImageLayout = ImageLayout.Zoom;
            btnNext.Cursor = Cursors.Hand;
            btnNext.FlatAppearance.BorderSize = 0;
            btnNext.FlatStyle = FlatStyle.Flat;
            btnNext.Location = new Point(219, 45);
            btnNext.Name = "btnNext";
            btnNext.Size = new Size(85, 80);
            btnNext.TabIndex = 1;
            btnNext.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnNext.UseVisualStyleBackColor = false;
            // 
            // btnPrev
            // 
            btnPrev.BackColor = Color.Transparent;
            btnPrev.BackgroundImage = Properties.Resources.PrevIcon;
            btnPrev.BackgroundImageLayout = ImageLayout.Zoom;
            btnPrev.Cursor = Cursors.Hand;
            btnPrev.FlatAppearance.BorderSize = 0;
            btnPrev.FlatStyle = FlatStyle.Flat;
            btnPrev.Location = new Point(12, 40);
            btnPrev.Name = "btnPrev";
            btnPrev.Size = new Size(86, 90);
            btnPrev.TabIndex = 2;
            btnPrev.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnPrev.UseVisualStyleBackColor = false;
            // 
            // btnPlayPause
            // 
            btnPlayPause.BackgroundImage = Properties.Resources.PlayIcon;
            btnPlayPause.BackgroundImageLayout = ImageLayout.Zoom;
            btnPlayPause.Cursor = Cursors.Hand;
            btnPlayPause.FlatAppearance.BorderSize = 0;
            btnPlayPause.FlatStyle = FlatStyle.Flat;
            btnPlayPause.Location = new Point(104, 34);
            btnPlayPause.Name = "btnPlayPause";
            btnPlayPause.Size = new Size(109, 93);
            btnPlayPause.TabIndex = 3;
            btnPlayPause.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnPlayPause.UseVisualStyleBackColor = true;
            // 
            // closeMinimizerPlayer
            // 
            closeMinimizerPlayer.BackColor = Color.Transparent;
            closeMinimizerPlayer.Cursor = Cursors.Hand;
            closeMinimizerPlayer.Location = new Point(248, 4);
            closeMinimizerPlayer.Name = "closeMinimizerPlayer";
            closeMinimizerPlayer.Size = new Size(67, 35);
            closeMinimizerPlayer.TabIndex = 4;
            closeMinimizerPlayer.Text = "Close";
            closeMinimizerPlayer.UseVisualStyleBackColor = false;
            closeMinimizerPlayer.Click += closeMinimizerPlayer_Click;
            // 
            // MinimizedPlayer
            // 
            AutoScaleDimensions = new SizeF(9F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(254, 184, 195);
            ClientSize = new Size(317, 137);
            Controls.Add(closeMinimizerPlayer);
            Controls.Add(btnPlayPause);
            Controls.Add(btnPrev);
            Controls.Add(btnNext);
            Font = new Font("MS PGothic", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(4, 2, 4, 2);
            Name = "MinimizedPlayer";
            StartPosition = FormStartPosition.CenterParent;
            Text = "HelpForm";
            ResumeLayout(false);
        }

        #endregion
        private Button btnNext;
        private Button btnPrev;
        private Button btnPlayPause;
        private Button closeMinimizerPlayer;
    }
}
