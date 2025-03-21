﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CuteMediaPlayer
{
    public partial class MinimizedPlayer : Form
    {
        private Form1 mainForm;
        private bool positionInitialized = false;
        public MinimizedPlayer(Form1 mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;

            // Apply rounded styling
            FormStyling.ApplyRoundedStyle(this);

            FormStyling.MakeDraggable(this, this);
            FormStyling.MakeDraggable(btnPlayPause, this);
            FormStyling.MakeDraggable(btnNext, this);
            FormStyling.MakeDraggable(btnPrev, this);

            this.TopMost = true; // Ensures the minimized player stays above all windows

            // Connect buttons to main form
            btnPlayPause.Click += (s, e) => mainForm.btnPlayPause_Click(s, e);
            btnNext.Click += (s, e) => mainForm.btnNext_Click(s, e);
            btnPrev.Click += (s, e) => mainForm.btnPrev_Click(s, e);

            // Subscribe to button state changes
            mainForm.ButtonStatesChanged += (s, e) => SyncButtonStates();

            // Initial sync with main form button states
            SyncButtonStates();

            // Unsubscribe when form closes to prevent memory leaks
            this.FormClosed += (s, e) => mainForm.ButtonStatesChanged -= (s2, e2) => SyncButtonStates();
            this.FormClosed += (s, e) => this.Dispose();
        }

        // Update button states and images based on main form
        private void SyncButtonStates()
        {
            // Match enabled states using the public properties
            btnPlayPause.Enabled = mainForm.IsPlayPauseEnabled;
            btnNext.Enabled = mainForm.IsNextEnabled;
            btnPrev.Enabled = mainForm.IsPrevEnabled;

            // Match images using the public properties
            btnPlayPause.BackgroundImage = mainForm.PlayPauseImage;
            btnNext.BackgroundImage = mainForm.NextImage;
            btnPrev.BackgroundImage = mainForm.PrevImage;
        }

        private void closeMinimizerPlayer_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override void OnLoad(EventArgs e)
        {
            // Load saved position
            if (Properties.Settings.Default.MinimizedPlayerLocation != Point.Empty)
            {
                this.Location = Properties.Settings.Default.MinimizedPlayerLocation;
            }
            positionInitialized = true;
            base.OnLoad(e);
        }

        protected override void OnMove(EventArgs e)
        {
            if (positionInitialized && WindowState == FormWindowState.Normal)
            {
                Properties.Settings.Default.MinimizedPlayerLocation = this.Location;
            }
            base.OnMove(e);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Properties.Settings.Default.MinimizedPlayerClosed = true;
            Properties.Settings.Default.Save();
            base.OnFormClosing(e);
        }
    }
}


