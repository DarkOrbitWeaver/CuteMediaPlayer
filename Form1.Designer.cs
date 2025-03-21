﻿namespace CuteMediaPlayer
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

                    DisposeOfDancingGirlFrames();

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
            welpToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            videoPanel = new Panel();
            vlcContainer = new Panel();
            dancingGirlPictureBox = new PictureBox();
            panelPlaylist = new Panel();
            tabControl1 = new TabControl();
            tabCurrent = new TabPage();
            currentPlaylistLabelContainer = new Panel();
            lblCurrentPlaylist = new Label();
            currentPlayingButtonContainer = new Panel();
            AddSongsToCurrentPlaylist = new Button();
            openMinimizedPlayer = new Button();
            btnAddToPlaylist = new Button();
            panel3 = new Panel();
            customPlaylistPanel = new CustomPlaylistPanel();
            tabLibrary = new TabPage();
            savedPlaylistsButtonContainer = new Panel();
            NewPlaylist = new Button();
            panel1 = new Panel();
            customPlaylistsPanel = new CustomPlaylistPanel();
            sparkleVisualizer1 = new SparkleVisualizer();
            controlPanel = new Panel();
            btnAddCurrentToPlaylist = new Button();
            shuffle = new Button();
            openPlaylist = new Button();
            loopBtn = new Button();
            lblTime = new Label();
            btnMute = new Button();
            seekBar = new CustomTrackBar();
            btnPlayPause = new Button();
            volumeBar = new CustomTrackBar();
            btnPrev = new Button();
            btnNext = new Button();
            btnStop = new Button();
            btnChangeTheme = new Button();
            menuSavedPlaylists = new ContextMenuStrip(components);
            deleteToolStripMenuItem = new ToolStripMenuItem();
            LoopBtnToolTip = new ToolTip(components);
            ChangeThemeBtnToolTip = new ToolTip(components);
            MuteBtnToolTip = new ToolTip(components);
            PlaylistTooltip = new ToolTip(components);
            AddToPlaylistTooltip = new ToolTip(components);
            ShufflePlaylistTooltip = new ToolTip(components);
            menuStrip1.SuspendLayout();
            videoPanel.SuspendLayout();
            vlcContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dancingGirlPictureBox).BeginInit();
            panelPlaylist.SuspendLayout();
            tabControl1.SuspendLayout();
            tabCurrent.SuspendLayout();
            currentPlaylistLabelContainer.SuspendLayout();
            currentPlayingButtonContainer.SuspendLayout();
            panel3.SuspendLayout();
            tabLibrary.SuspendLayout();
            savedPlaylistsButtonContainer.SuspendLayout();
            panel1.SuspendLayout();
            controlPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)seekBar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)volumeBar).BeginInit();
            menuSavedPlaylists.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.BackColor = Color.FromArgb(254, 184, 195);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, welpToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(7, 4, 0, 4);
            menuStrip1.Size = new Size(982, 27);
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
            // welpToolStripMenuItem
            // 
            welpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aboutToolStripMenuItem });
            welpToolStripMenuItem.Name = "welpToolStripMenuItem";
            welpToolStripMenuItem.Size = new Size(61, 19);
            welpToolStripMenuItem.Text = "Welp :_)";
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(139, 22);
            aboutToolStripMenuItem.Text = "How to use?";
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            // 
            // videoPanel
            // 
            videoPanel.BackColor = Color.Transparent;
            videoPanel.Controls.Add(vlcContainer);
            videoPanel.Controls.Add(controlPanel);
            videoPanel.Dock = DockStyle.Fill;
            videoPanel.Location = new Point(0, 27);
            videoPanel.Margin = new Padding(3, 4, 3, 4);
            videoPanel.Name = "videoPanel";
            videoPanel.Size = new Size(982, 561);
            videoPanel.TabIndex = 1;
            // 
            // vlcContainer
            // 
            vlcContainer.BackColor = Color.Transparent;
            vlcContainer.Controls.Add(dancingGirlPictureBox);
            vlcContainer.Controls.Add(panelPlaylist);
            vlcContainer.Controls.Add(sparkleVisualizer1);
            vlcContainer.Dock = DockStyle.Fill;
            vlcContainer.Location = new Point(0, 0);
            vlcContainer.Margin = new Padding(3, 4, 3, 4);
            vlcContainer.Name = "vlcContainer";
            vlcContainer.Size = new Size(982, 427);
            vlcContainer.TabIndex = 5;
            // 
            // dancingGirlPictureBox
            // 
            dancingGirlPictureBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            dancingGirlPictureBox.BackColor = Color.Transparent;
            dancingGirlPictureBox.Location = new Point(0, 356);
            dancingGirlPictureBox.Name = "dancingGirlPictureBox";
            dancingGirlPictureBox.Size = new Size(62, 71);
            dancingGirlPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            dancingGirlPictureBox.TabIndex = 12;
            dancingGirlPictureBox.TabStop = false;
            dancingGirlPictureBox.Visible = false;
            // 
            // panelPlaylist
            // 
            panelPlaylist.BackColor = Color.FromArgb(255, 192, 192);
            panelPlaylist.Controls.Add(tabControl1);
            panelPlaylist.Dock = DockStyle.Right;
            panelPlaylist.Location = new Point(615, 0);
            panelPlaylist.Margin = new Padding(3, 4, 3, 4);
            panelPlaylist.Name = "panelPlaylist";
            panelPlaylist.Size = new Size(367, 427);
            panelPlaylist.TabIndex = 1;
            panelPlaylist.Visible = false;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabCurrent);
            tabControl1.Controls.Add(tabLibrary);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Font = new Font("Consolas", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            tabControl1.Location = new Point(0, 0);
            tabControl1.Margin = new Padding(3, 4, 3, 4);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(367, 427);
            tabControl1.TabIndex = 0;
            // 
            // tabCurrent
            // 
            tabCurrent.BackColor = Color.FromArgb(254, 184, 195);
            tabCurrent.Controls.Add(currentPlaylistLabelContainer);
            tabCurrent.Controls.Add(currentPlayingButtonContainer);
            tabCurrent.Controls.Add(panel3);
            tabCurrent.Location = new Point(4, 28);
            tabCurrent.Margin = new Padding(3, 4, 3, 4);
            tabCurrent.Name = "tabCurrent";
            tabCurrent.Padding = new Padding(3, 4, 3, 4);
            tabCurrent.Size = new Size(359, 395);
            tabCurrent.TabIndex = 0;
            tabCurrent.Text = "Now Playing";
            // 
            // currentPlaylistLabelContainer
            // 
            currentPlaylistLabelContainer.Controls.Add(lblCurrentPlaylist);
            currentPlaylistLabelContainer.Dock = DockStyle.Top;
            currentPlaylistLabelContainer.Location = new Point(3, 4);
            currentPlaylistLabelContainer.Name = "currentPlaylistLabelContainer";
            currentPlaylistLabelContainer.Size = new Size(353, 22);
            currentPlaylistLabelContainer.TabIndex = 4;
            // 
            // lblCurrentPlaylist
            // 
            lblCurrentPlaylist.AutoSize = true;
            lblCurrentPlaylist.Font = new Font("MS PGothic", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblCurrentPlaylist.Location = new Point(3, 0);
            lblCurrentPlaylist.Name = "lblCurrentPlaylist";
            lblCurrentPlaylist.Size = new Size(0, 16);
            lblCurrentPlaylist.TabIndex = 0;
            // 
            // currentPlayingButtonContainer
            // 
            currentPlayingButtonContainer.Controls.Add(AddSongsToCurrentPlaylist);
            currentPlayingButtonContainer.Controls.Add(openMinimizedPlayer);
            currentPlayingButtonContainer.Controls.Add(btnAddToPlaylist);
            currentPlayingButtonContainer.Dock = DockStyle.Bottom;
            currentPlayingButtonContainer.Location = new Point(3, 348);
            currentPlayingButtonContainer.Name = "currentPlayingButtonContainer";
            currentPlayingButtonContainer.Size = new Size(353, 43);
            currentPlayingButtonContainer.TabIndex = 1;
            // 
            // AddSongsToCurrentPlaylist
            // 
            AddSongsToCurrentPlaylist.BackColor = Color.FromArgb(255, 192, 192);
            AddSongsToCurrentPlaylist.Cursor = Cursors.Hand;
            AddSongsToCurrentPlaylist.Enabled = false;
            AddSongsToCurrentPlaylist.Font = new Font("MS PGothic", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            AddSongsToCurrentPlaylist.Location = new Point(11, 4);
            AddSongsToCurrentPlaylist.Margin = new Padding(3, 4, 3, 4);
            AddSongsToCurrentPlaylist.Name = "AddSongsToCurrentPlaylist";
            AddSongsToCurrentPlaylist.Size = new Size(50, 36);
            AddSongsToCurrentPlaylist.TabIndex = 3;
            AddSongsToCurrentPlaylist.Text = "Add";
            AddSongsToCurrentPlaylist.UseVisualStyleBackColor = false;
            AddSongsToCurrentPlaylist.Click += AddSongsToCurrentPlaylist_Click;
            // 
            // openMinimizedPlayer
            // 
            openMinimizedPlayer.BackColor = Color.FromArgb(255, 192, 192);
            openMinimizedPlayer.Cursor = Cursors.Hand;
            openMinimizedPlayer.Font = new Font("MS PGothic", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            openMinimizedPlayer.Location = new Point(208, 4);
            openMinimizedPlayer.Margin = new Padding(3, 4, 3, 4);
            openMinimizedPlayer.Name = "openMinimizedPlayer";
            openMinimizedPlayer.Size = new Size(134, 36);
            openMinimizedPlayer.TabIndex = 2;
            openMinimizedPlayer.Text = "Minimzed";
            openMinimizedPlayer.UseVisualStyleBackColor = false;
            openMinimizedPlayer.Click += openMinimizedPlayer_Click;
            // 
            // btnAddToPlaylist
            // 
            btnAddToPlaylist.BackColor = Color.FromArgb(255, 192, 192);
            btnAddToPlaylist.Cursor = Cursors.Hand;
            btnAddToPlaylist.Font = new Font("MS PGothic", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnAddToPlaylist.Location = new Point(67, 4);
            btnAddToPlaylist.Margin = new Padding(3, 4, 3, 4);
            btnAddToPlaylist.Name = "btnAddToPlaylist";
            btnAddToPlaylist.Size = new Size(135, 36);
            btnAddToPlaylist.TabIndex = 1;
            btnAddToPlaylist.Text = "Save Songs";
            btnAddToPlaylist.UseVisualStyleBackColor = false;
            btnAddToPlaylist.Click += btnAddToPlaylist_Click;
            // 
            // panel3
            // 
            panel3.Controls.Add(customPlaylistPanel);
            panel3.Dock = DockStyle.Fill;
            panel3.Location = new Point(3, 4);
            panel3.Name = "panel3";
            panel3.Size = new Size(353, 387);
            panel3.TabIndex = 0;
            // 
            // customPlaylistPanel
            // 
            customPlaylistPanel.AutoScroll = true;
            customPlaylistPanel.BackColor = Color.FromArgb(254, 184, 195);
            customPlaylistPanel.Dock = DockStyle.Fill;
            customPlaylistPanel.Location = new Point(0, 0);
            customPlaylistPanel.Name = "customPlaylistPanel";
            customPlaylistPanel.Size = new Size(353, 387);
            customPlaylistPanel.TabIndex = 3;
            // 
            // tabLibrary
            // 
            tabLibrary.BackColor = Color.FromArgb(254, 184, 195);
            tabLibrary.Controls.Add(savedPlaylistsButtonContainer);
            tabLibrary.Controls.Add(panel1);
            tabLibrary.Location = new Point(4, 28);
            tabLibrary.Margin = new Padding(3, 4, 3, 4);
            tabLibrary.Name = "tabLibrary";
            tabLibrary.Padding = new Padding(3, 4, 3, 4);
            tabLibrary.Size = new Size(359, 395);
            tabLibrary.TabIndex = 1;
            tabLibrary.Text = "My Playlists";
            // 
            // savedPlaylistsButtonContainer
            // 
            savedPlaylistsButtonContainer.Controls.Add(NewPlaylist);
            savedPlaylistsButtonContainer.Dock = DockStyle.Bottom;
            savedPlaylistsButtonContainer.Location = new Point(3, 347);
            savedPlaylistsButtonContainer.Name = "savedPlaylistsButtonContainer";
            savedPlaylistsButtonContainer.Size = new Size(353, 44);
            savedPlaylistsButtonContainer.TabIndex = 3;
            // 
            // NewPlaylist
            // 
            NewPlaylist.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            NewPlaylist.BackColor = Color.FromArgb(254, 184, 195);
            NewPlaylist.Cursor = Cursors.Hand;
            NewPlaylist.Font = new Font("MS PGothic", 12F, FontStyle.Bold);
            NewPlaylist.Location = new Point(82, 7);
            NewPlaylist.Margin = new Padding(3, 4, 3, 4);
            NewPlaylist.Name = "NewPlaylist";
            NewPlaylist.Size = new Size(201, 33);
            NewPlaylist.TabIndex = 1;
            NewPlaylist.Text = "New";
            NewPlaylist.UseVisualStyleBackColor = false;
            NewPlaylist.Click += NewPlaylist_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(customPlaylistsPanel);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(3, 4);
            panel1.Name = "panel1";
            panel1.Size = new Size(353, 387);
            panel1.TabIndex = 2;
            // 
            // customPlaylistsPanel
            // 
            customPlaylistsPanel.AutoScroll = true;
            customPlaylistsPanel.BackColor = Color.FromArgb(254, 184, 195);
            customPlaylistsPanel.Dock = DockStyle.Fill;
            customPlaylistsPanel.Location = new Point(0, 0);
            customPlaylistsPanel.Name = "customPlaylistsPanel";
            customPlaylistsPanel.Size = new Size(353, 387);
            customPlaylistsPanel.TabIndex = 2;
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
            sparkleVisualizer1.Size = new Size(982, 427);
            sparkleVisualizer1.TabIndex = 0;
            // 
            // controlPanel
            // 
            controlPanel.BackColor = Color.FromArgb(254, 184, 195);
            controlPanel.Controls.Add(btnAddCurrentToPlaylist);
            controlPanel.Controls.Add(shuffle);
            controlPanel.Controls.Add(openPlaylist);
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
            controlPanel.Location = new Point(0, 427);
            controlPanel.Margin = new Padding(3, 4, 3, 4);
            controlPanel.Name = "controlPanel";
            controlPanel.Size = new Size(982, 134);
            controlPanel.TabIndex = 4;
            // 
            // btnAddCurrentToPlaylist
            // 
            btnAddCurrentToPlaylist.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnAddCurrentToPlaylist.BackColor = Color.Transparent;
            btnAddCurrentToPlaylist.BackgroundImage = Properties.Resources.AddToPlaylistIcon;
            btnAddCurrentToPlaylist.BackgroundImageLayout = ImageLayout.Zoom;
            btnAddCurrentToPlaylist.Cursor = Cursors.Hand;
            btnAddCurrentToPlaylist.FlatAppearance.BorderSize = 0;
            btnAddCurrentToPlaylist.FlatStyle = FlatStyle.Flat;
            btnAddCurrentToPlaylist.Location = new Point(640, 62);
            btnAddCurrentToPlaylist.Margin = new Padding(3, 4, 3, 4);
            btnAddCurrentToPlaylist.Name = "btnAddCurrentToPlaylist";
            btnAddCurrentToPlaylist.Size = new Size(50, 54);
            btnAddCurrentToPlaylist.TabIndex = 11;
            AddToPlaylistTooltip.SetToolTip(btnAddCurrentToPlaylist, "Add To Playlist");
            btnAddCurrentToPlaylist.UseVisualStyleBackColor = true;
            btnAddCurrentToPlaylist.Click += btnAddCurrentToPlaylist_Click;
            // 
            // shuffle
            // 
            shuffle.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            shuffle.BackColor = Color.Transparent;
            shuffle.BackgroundImage = Properties.Resources.DisabledShuffleIcon;
            shuffle.BackgroundImageLayout = ImageLayout.Zoom;
            shuffle.Cursor = Cursors.Hand;
            shuffle.FlatAppearance.BorderSize = 0;
            shuffle.FlatStyle = FlatStyle.Flat;
            shuffle.Location = new Point(914, 70);
            shuffle.Margin = new Padding(3, 4, 3, 4);
            shuffle.Name = "shuffle";
            shuffle.Size = new Size(50, 51);
            shuffle.TabIndex = 10;
            ShufflePlaylistTooltip.SetToolTip(shuffle, "Shuffle");
            shuffle.UseVisualStyleBackColor = false;
            shuffle.Click += shuffle_Click;
            // 
            // openPlaylist
            // 
            openPlaylist.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            openPlaylist.BackColor = Color.Transparent;
            openPlaylist.BackgroundImage = Properties.Resources.PlaylistIcon;
            openPlaylist.BackgroundImageLayout = ImageLayout.Zoom;
            openPlaylist.Cursor = Cursors.Hand;
            openPlaylist.FlatAppearance.BorderSize = 0;
            openPlaylist.FlatStyle = FlatStyle.Flat;
            openPlaylist.Location = new Point(696, 61);
            openPlaylist.Margin = new Padding(3, 4, 3, 4);
            openPlaylist.Name = "openPlaylist";
            openPlaylist.Size = new Size(62, 57);
            openPlaylist.TabIndex = 9;
            PlaylistTooltip.SetToolTip(openPlaylist, "Playlist");
            openPlaylist.UseVisualStyleBackColor = true;
            openPlaylist.Click += openPlaylist_Click;
            // 
            // loopBtn
            // 
            loopBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            loopBtn.BackColor = Color.Transparent;
            loopBtn.BackgroundImage = Properties.Resources.DisabledLoopIcon;
            loopBtn.BackgroundImageLayout = ImageLayout.Zoom;
            loopBtn.Cursor = Cursors.Hand;
            loopBtn.FlatAppearance.BorderSize = 0;
            loopBtn.FlatStyle = FlatStyle.Flat;
            loopBtn.Location = new Point(768, 57);
            loopBtn.Margin = new Padding(3, 4, 3, 4);
            loopBtn.Name = "loopBtn";
            loopBtn.Size = new Size(64, 65);
            loopBtn.TabIndex = 7;
            LoopBtnToolTip.SetToolTip(loopBtn, "Loop");
            loopBtn.UseVisualStyleBackColor = true;
            loopBtn.Click += loopBtn_Click;
            // 
            // lblTime
            // 
            lblTime.AutoSize = true;
            lblTime.Location = new Point(199, 42);
            lblTime.Name = "lblTime";
            lblTime.Size = new Size(95, 16);
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
            btnMute.Location = new Point(329, 75);
            btnMute.Margin = new Padding(3, 4, 3, 4);
            btnMute.Name = "btnMute";
            btnMute.Size = new Size(55, 55);
            btnMute.TabIndex = 5;
            MuteBtnToolTip.SetToolTip(btnMute, "Mute");
            btnMute.UseVisualStyleBackColor = true;
            btnMute.Click += btnMute_Click;
            // 
            // seekBar
            // 
            seekBar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            seekBar.LargeChange = 1;
            seekBar.Location = new Point(183, 11);
            seekBar.Margin = new Padding(3, 4, 3, 4);
            seekBar.Maximum = 1000;
            seekBar.Name = "seekBar";
            seekBar.ProgressColor = Color.DeepPink;
            seekBar.Size = new Size(757, 45);
            seekBar.TabIndex = 4;
            seekBar.Thumb = CustomTrackBar.ThumbShape.Circle;
            seekBar.ThumbColor = Color.HotPink;
            seekBar.ThumbSize = 16;
            seekBar.TickStyle = TickStyle.None;
            seekBar.TrackColor = Color.FromArgb(181, 130, 138);
            seekBar.TrackHeight = 6;
            seekBar.Scroll += seekBar_Scroll;
            // 
            // btnPlayPause
            // 
            btnPlayPause.BackgroundImage = Properties.Resources.PlayIcon;
            btnPlayPause.BackgroundImageLayout = ImageLayout.Zoom;
            btnPlayPause.Cursor = Cursors.Hand;
            btnPlayPause.FlatAppearance.BorderSize = 0;
            btnPlayPause.FlatStyle = FlatStyle.Flat;
            btnPlayPause.Location = new Point(10, 11);
            btnPlayPause.Margin = new Padding(3, 4, 3, 4);
            btnPlayPause.Name = "btnPlayPause";
            btnPlayPause.Size = new Size(90, 82);
            btnPlayPause.TabIndex = 0;
            btnPlayPause.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnPlayPause.UseVisualStyleBackColor = true;
            btnPlayPause.Click += btnPlayPause_Click;
            // 
            // volumeBar
            // 
            volumeBar.AutoSize = false;
            volumeBar.Location = new Point(390, 91);
            volumeBar.Margin = new Padding(3, 4, 3, 4);
            volumeBar.Maximum = 100;
            volumeBar.Name = "volumeBar";
            volumeBar.ProgressColor = Color.DeepPink;
            volumeBar.Size = new Size(112, 23);
            volumeBar.TabIndex = 3;
            volumeBar.Thumb = CustomTrackBar.ThumbShape.Circle;
            volumeBar.ThumbColor = Color.HotPink;
            volumeBar.ThumbSize = 16;
            volumeBar.TickStyle = TickStyle.None;
            volumeBar.TrackColor = Color.FromArgb(181, 130, 138);
            volumeBar.TrackHeight = 6;
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
            btnPrev.Location = new Point(173, 61);
            btnPrev.Margin = new Padding(3, 4, 3, 4);
            btnPrev.Name = "btnPrev";
            btnPrev.Size = new Size(73, 68);
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
            btnNext.Location = new Point(243, 61);
            btnNext.Margin = new Padding(3, 4, 3, 4);
            btnNext.Name = "btnNext";
            btnNext.Size = new Size(72, 68);
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
            btnStop.Location = new Point(106, 33);
            btnStop.Margin = new Padding(3, 4, 3, 4);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(60, 60);
            btnStop.TabIndex = 2;
            btnStop.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // btnChangeTheme
            // 
            btnChangeTheme.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnChangeTheme.BackgroundImage = Properties.Resources.ThemeIcon;
            btnChangeTheme.BackgroundImageLayout = ImageLayout.Zoom;
            btnChangeTheme.Cursor = Cursors.Hand;
            btnChangeTheme.FlatAppearance.BorderSize = 0;
            btnChangeTheme.FlatStyle = FlatStyle.Flat;
            btnChangeTheme.Location = new Point(839, 59);
            btnChangeTheme.Margin = new Padding(3, 4, 3, 4);
            btnChangeTheme.Name = "btnChangeTheme";
            btnChangeTheme.Size = new Size(69, 69);
            btnChangeTheme.TabIndex = 8;
            btnChangeTheme.TextImageRelation = TextImageRelation.ImageBeforeText;
            ChangeThemeBtnToolTip.SetToolTip(btnChangeTheme, "Change Visualizer Theme");
            btnChangeTheme.UseVisualStyleBackColor = true;
            btnChangeTheme.Click += btnChangeTheme_Click;
            // 
            // menuSavedPlaylists
            // 
            menuSavedPlaylists.Items.AddRange(new ToolStripItem[] { deleteToolStripMenuItem });
            menuSavedPlaylists.Name = "menuSavedPlaylists";
            menuSavedPlaylists.Size = new Size(108, 26);
            // 
            // deleteToolStripMenuItem
            // 
            deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            deleteToolStripMenuItem.Size = new Size(107, 22);
            deleteToolStripMenuItem.Text = "Delete";
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
            AutoScaleDimensions = new SizeF(8F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(982, 588);
            Controls.Add(videoPanel);
            Controls.Add(menuStrip1);
            Font = new Font("MS Gothic", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Margin = new Padding(3, 4, 3, 4);
            MinimumSize = new Size(998, 627);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "✨ Cute Media Player ✨";
            FormClosed += Form1_FormClosed;
            Load += Form1_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            videoPanel.ResumeLayout(false);
            vlcContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dancingGirlPictureBox).EndInit();
            panelPlaylist.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            tabCurrent.ResumeLayout(false);
            currentPlaylistLabelContainer.ResumeLayout(false);
            currentPlaylistLabelContainer.PerformLayout();
            currentPlayingButtonContainer.ResumeLayout(false);
            panel3.ResumeLayout(false);
            tabLibrary.ResumeLayout(false);
            savedPlaylistsButtonContainer.ResumeLayout(false);
            panel1.ResumeLayout(false);
            controlPanel.ResumeLayout(false);
            controlPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)seekBar).EndInit();
            ((System.ComponentModel.ISupportInitialize)volumeBar).EndInit();
            menuSavedPlaylists.ResumeLayout(false);
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
        private ToolStripMenuItem aboutToolStripMenuItem;
        private Button openPlaylist;
        private Button shuffle;
        private Panel panelPlaylist;
        private TabControl tabControl1;
        private TabPage tabCurrent;
        private TabPage tabLibrary;
        private Button NewPlaylist;
        private ContextMenuStrip menuSavedPlaylists;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private Button btnAddToPlaylist;
        private Button btnAddCurrentToPlaylist;
        private ToolTip PlaylistTooltip;
        private ToolTip AddToPlaylistTooltip;
        private ToolTip ShufflePlaylistTooltip;
        private PictureBox dancingGirlPictureBox;
        private Button openMinimizedPlayer;
        private CustomPlaylistPanel customPlaylistPanel;
        private CustomPlaylistPanel customPlaylistsPanel;
        private Panel savedPlaylistsButtonContainer;
        private Panel panel1;
        private Panel currentPlayingButtonContainer;
        private Panel panel3;
        private Panel currentPlaylistLabelContainer;
        private Label lblCurrentPlaylist;
        private Button AddSongsToCurrentPlaylist;
    }
}