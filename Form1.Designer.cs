namespace CuteMediaPlayer
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>

        //Add proper exception handling and resource disposal
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    // Stop playback and clean up timers
                    if (mediaPlayer != null)
                    {
                        mediaPlayer.Stop();
                        mediaPlayer.Dispose();
                    }

                    if (uiTimer != null)
                    {
                        uiTimer.Stop();
                        uiTimer.Dispose();
                    }

                    if (components != null)
                    {
                        components.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during disposal: {ex.Message}");
                }
            }

            base.Dispose(disposing);
        }


        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            openToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem = new ToolStripMenuItem();
            viewToolStripMenuItem = new ToolStripMenuItem();
            playToolStripMenuItem = new ToolStripMenuItem();
            toolsToolStripMenuItem = new ToolStripMenuItem();
            welpToolStripMenuItem = new ToolStripMenuItem();
            videoPanel = new Panel();
            vlcContainer = new Panel();
            sparkleVisualizer1 = new SparkleVisualizer();
            controlPanel = new Panel();
            loopBtn = new Button();
            lblTime = new Label();
            btnMute = new Button();
            this.seekBar = new CuteMediaPlayer.CustomTrackBar();
            btnPlayPause = new Button();
            this.volumeBar = new CuteMediaPlayer.CustomTrackBar();
            btnPrev = new Button();
            btnNext = new Button();
            btnStop = new Button();
            btnChangeTheme = new Button();
            LoopBtnToolTip = new ToolTip(components);
            ChangeThemeBtnToolTip = new ToolTip(components);
            MuteBtnToolTip = new ToolTip(components);
            menuStrip1.SuspendLayout();
            videoPanel.SuspendLayout();
            vlcContainer.SuspendLayout();
            controlPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)seekBar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)volumeBar).BeginInit();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.BackColor = Color.FromArgb(254, 184, 195);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, viewToolStripMenuItem, playToolStripMenuItem, toolsToolStripMenuItem, welpToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(6, 3, 0, 3);
            menuStrip1.Size = new Size(784, 25);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openToolStripMenuItem, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 19);
            fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new Size(106, 22);
            openToolStripMenuItem.Text = "Open ";
            openToolStripMenuItem.Click += openToolStripMenuItem_Click;
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(106, 22);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // viewToolStripMenuItem
            // 
            viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            viewToolStripMenuItem.Size = new Size(44, 19);
            viewToolStripMenuItem.Text = "View";
            // 
            // playToolStripMenuItem
            // 
            playToolStripMenuItem.Name = "playToolStripMenuItem";
            playToolStripMenuItem.Size = new Size(41, 19);
            playToolStripMenuItem.Text = "Play";
            // 
            // toolsToolStripMenuItem
            // 
            toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            toolsToolStripMenuItem.Size = new Size(46, 19);
            toolsToolStripMenuItem.Text = "Tools";
            // 
            // welpToolStripMenuItem
            // 
            welpToolStripMenuItem.Name = "welpToolStripMenuItem";
            welpToolStripMenuItem.Size = new Size(61, 19);
            welpToolStripMenuItem.Text = "Welp :_)";
            // 
            // videoPanel
            // 
            videoPanel.BackColor = Color.MediumSlateBlue;
            videoPanel.Controls.Add(vlcContainer);
            videoPanel.Controls.Add(controlPanel);
            videoPanel.Dock = DockStyle.Fill;
            videoPanel.Location = new Point(0, 25);
            videoPanel.Name = "videoPanel";
            videoPanel.Size = new Size(784, 526);
            videoPanel.TabIndex = 1;
            // 
            // vlcContainer
            // 
            vlcContainer.BackColor = Color.Black;
            vlcContainer.Controls.Add(sparkleVisualizer1);
            vlcContainer.Dock = DockStyle.Fill;
            vlcContainer.Location = new Point(0, 0);
            vlcContainer.Name = "vlcContainer";
            vlcContainer.Size = new Size(784, 400);
            vlcContainer.TabIndex = 5;
            // 
            // sparkleVisualizer1
            // 
            sparkleVisualizer1.BackColor = Color.Transparent;
            sparkleVisualizer1.Dock = DockStyle.Fill;
            sparkleVisualizer1.Font = new Font("Consolas", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            sparkleVisualizer1.IsIdleMode = true;
            sparkleVisualizer1.Location = new Point(0, 0);
            sparkleVisualizer1.Margin = new Padding(8, 13, 8, 13);
            sparkleVisualizer1.Name = "sparkleVisualizer1";
            sparkleVisualizer1.Size = new Size(784, 400);
            sparkleVisualizer1.TabIndex = 0;
            sparkleVisualizer1.UseIdleWallpaper = true;
            // 
            // controlPanel
            // 
            controlPanel.BackColor = Color.FromArgb(254, 184, 195);
            controlPanel.Controls.Add(loopBtn);
            controlPanel.Controls.Add(lblTime);
            controlPanel.Controls.Add(btnMute);
            controlPanel.Controls.Add(seekBar);
            controlPanel.Controls.Add(btnPlayPause);
            controlPanel.Controls.Add(volumeBar);
            controlPanel.Controls.Add(btnPrev);
            controlPanel.Controls.Add(btnNext);
            controlPanel.Controls.Add(btnStop);
            controlPanel.Controls.Add(btnChangeTheme);
            controlPanel.Dock = DockStyle.Bottom;
            controlPanel.Location = new Point(0, 400);
            controlPanel.Name = "controlPanel";
            controlPanel.Size = new Size(784, 126);
            controlPanel.TabIndex = 4;
            // 
            // loopBtn
            // 
            loopBtn.BackColor = Color.Transparent;
            loopBtn.BackgroundImage = Properties.Resources.DisabledLoopIcon;
            loopBtn.BackgroundImageLayout = ImageLayout.Zoom;
            loopBtn.Cursor = Cursors.Hand;
            loopBtn.FlatAppearance.BorderSize = 0;
            loopBtn.FlatStyle = FlatStyle.Flat;
            loopBtn.Location = new Point(705, 65);
            loopBtn.Name = "loopBtn";
            loopBtn.Size = new Size(53, 51);
            loopBtn.TabIndex = 7;
            LoopBtnToolTip.SetToolTip(loopBtn, "Loop");
            loopBtn.UseVisualStyleBackColor = true;
            loopBtn.Click += loopBtn_Click;
            // 
            // lblTime
            // 
            lblTime.AutoSize = true;
            lblTime.Location = new Point(174, 40);
            lblTime.Name = "lblTime";
            lblTime.Size = new Size(84, 15);
            lblTime.TabIndex = 6;
            lblTime.Text = "0:00 / 0:00";
            // 
            // btnMute
            // 
            btnMute.BackColor = Color.Transparent;
            btnMute.BackgroundImage = Properties.Resources.MuteIcon;
            btnMute.BackgroundImageLayout = ImageLayout.Zoom;
            btnMute.Cursor = Cursors.Hand;
            btnMute.FlatAppearance.BorderSize = 0;
            btnMute.FlatStyle = FlatStyle.Flat;
            btnMute.Location = new Point(334, 71);
            btnMute.Name = "btnMute";
            btnMute.Size = new Size(48, 52);
            btnMute.TabIndex = 5;
            MuteBtnToolTip.SetToolTip(btnMute, "Mute");
            btnMute.UseVisualStyleBackColor = true;
            btnMute.Click += btnMute_Click;
            // 
            // seekBar
            // 
            seekBar.LargeChange = 1;
            seekBar.Location = new Point(160, 10);
            seekBar.Maximum = 1000;
            seekBar.Name = "seekBar";
            seekBar.Size = new Size(618, 45);
            seekBar.TabIndex = 4;
            seekBar.TickStyle = TickStyle.None;
            seekBar.ThumbSize = 16;
            seekBar.Scroll += seekBar_Scroll;
            // 
            // btnPlayPause
            // 
            btnPlayPause.BackgroundImage = Properties.Resources.PlayIcon;
            btnPlayPause.BackgroundImageLayout = ImageLayout.Zoom;
            btnPlayPause.Cursor = Cursors.Hand;
            btnPlayPause.FlatAppearance.BorderSize = 0;
            btnPlayPause.FlatStyle = FlatStyle.Flat;
            btnPlayPause.Location = new Point(12, 10);
            btnPlayPause.Name = "btnPlayPause";
            btnPlayPause.Size = new Size(79, 77);
            btnPlayPause.TabIndex = 0;
            btnPlayPause.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnPlayPause.UseVisualStyleBackColor = true;
            btnPlayPause.Click += btnPlayPause_Click;
            // 
            // volumeBar
            // 
            volumeBar.AutoSize = false;
            volumeBar.Location = new Point(388, 87);
            volumeBar.Maximum = 100;
            volumeBar.Name = "volumeBar";
            volumeBar.Size = new Size(100, 22);
            volumeBar.TabIndex = 3;
            volumeBar.TickStyle = TickStyle.None;
            volumeBar.ThumbSize = 16;
            volumeBar.Value = 100;
            volumeBar.Scroll += volumeBar_Scroll;
            // 
            // btnPrev
            // 
            btnPrev.BackColor = Color.Transparent;
            btnPrev.BackgroundImage = Properties.Resources.PrevIcon;
            btnPrev.BackgroundImageLayout = ImageLayout.Zoom;
            btnPrev.Cursor = Cursors.Hand;
            btnPrev.FlatAppearance.BorderSize = 0;
            btnPrev.FlatStyle = FlatStyle.Flat;
            btnPrev.Location = new Point(167, 58);
            btnPrev.Name = "btnPrev";
            btnPrev.Size = new Size(64, 64);
            btnPrev.TabIndex = 1;
            btnPrev.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnPrev.UseVisualStyleBackColor = false;
            btnPrev.Click += btnPrev_Click;
            // 
            // btnNext
            // 
            btnNext.BackColor = Color.Transparent;
            btnNext.BackgroundImage = Properties.Resources.NextIcon;
            btnNext.BackgroundImageLayout = ImageLayout.Zoom;
            btnNext.Cursor = Cursors.Hand;
            btnNext.FlatAppearance.BorderSize = 0;
            btnNext.FlatStyle = FlatStyle.Flat;
            btnNext.Location = new Point(237, 58);
            btnNext.Name = "btnNext";
            btnNext.Size = new Size(63, 64);
            btnNext.TabIndex = 3;
            btnNext.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnNext.UseVisualStyleBackColor = false;
            btnNext.Click += btnNext_Click;
            // 
            // btnStop
            // 
            btnStop.BackgroundImage = Properties.Resources.StopIcon;
            btnStop.BackgroundImageLayout = ImageLayout.Zoom;
            btnStop.Cursor = Cursors.Hand;
            btnStop.FlatAppearance.BorderSize = 0;
            btnStop.FlatStyle = FlatStyle.Flat;
            btnStop.Location = new Point(87, 31);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(67, 56);
            btnStop.TabIndex = 2;
            btnStop.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // btnChangeTheme
            // 
            btnChangeTheme.BackgroundImage = Properties.Resources.ThemeIcon;
            btnChangeTheme.BackgroundImageLayout = ImageLayout.Zoom;
            btnChangeTheme.Cursor = Cursors.Hand;
            btnChangeTheme.FlatAppearance.BorderSize = 0;
            btnChangeTheme.FlatStyle = FlatStyle.Flat;
            btnChangeTheme.Location = new Point(625, 58);
            btnChangeTheme.Name = "btnChangeTheme";
            btnChangeTheme.Size = new Size(74, 65);
            btnChangeTheme.TabIndex = 8;
            btnChangeTheme.TextImageRelation = TextImageRelation.ImageBeforeText;
            ChangeThemeBtnToolTip.SetToolTip(btnChangeTheme, "Change Visualizer Theme");
            btnChangeTheme.UseVisualStyleBackColor = true;
            btnChangeTheme.Click += btnChangeTheme_Click;
            // 
            // LoopBtnToolTip
            // 
            LoopBtnToolTip.AutoPopDelay = 5000;
            LoopBtnToolTip.InitialDelay = 800;
            LoopBtnToolTip.ReshowDelay = 100;
            // 
            // ChangeThemeBtnToolTip
            // 
            ChangeThemeBtnToolTip.AutoPopDelay = 5000;
            ChangeThemeBtnToolTip.InitialDelay = 800;
            ChangeThemeBtnToolTip.ReshowDelay = 100;
            // 
            // MuteBtnToolTip
            // 
            MuteBtnToolTip.AutoPopDelay = 5000;
            MuteBtnToolTip.InitialDelay = 800;
            MuteBtnToolTip.ReshowDelay = 100;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(255, 240, 250);
            ClientSize = new Size(784, 551);
            Controls.Add(videoPanel);
            Controls.Add(menuStrip1);
            Font = new Font("Consolas", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "✨ Cute Media Player ✨";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            videoPanel.ResumeLayout(false);
            vlcContainer.ResumeLayout(false);
            controlPanel.ResumeLayout(false);
            controlPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)seekBar).EndInit();
            ((System.ComponentModel.ISupportInitialize)volumeBar).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private Panel videoPanel;
        private Panel controlPanel;
        private Button btnPlayPause;
        private Button btnPrev;
        private Button btnNext;
        private Button btnStop;
        private CuteMediaPlayer.CustomTrackBar volumeBar;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripMenuItem playToolStripMenuItem;
        private ToolStripMenuItem toolsToolStripMenuItem;
        private ToolStripMenuItem welpToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private CuteMediaPlayer.CustomTrackBar seekBar;
        private Button btnMute;
        private Label lblTime;
        private Panel vlcContainer;
        private SparkleVisualizer sparkleVisualizer1;
        private Button loopBtn;
        private Button btnChangeTheme;
        private ToolTip LoopBtnToolTip;
        private ToolTip ChangeThemeBtnToolTip;
        private ToolTip MuteBtnToolTip;
    }
}
