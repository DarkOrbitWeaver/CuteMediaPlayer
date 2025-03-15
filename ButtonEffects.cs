using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuteMediaPlayer
{
    public partial class Form1
    {
        // here i will control the button images scale like when click

        private Dictionary<Button, Size> originalSizes = new Dictionary<Button, Size>();

        private void SetupButtonAppearance(Button button)
        {
            // Store original size
            originalSizes[button] = button.Size;

            // Remove the default white hover effect by setting flat style
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(40, 255, 255, 255);
            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(60, 255, 192, 203);

            // Add mouse event handlers for scaling effect
            button.MouseDown += Button_MouseDown;
            button.MouseUp += Button_MouseUp;
            button.MouseLeave += Button_MouseLeave;
        }

        private void Button_MouseDown(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            if (button.Enabled && e.Button == MouseButtons.Left && originalSizes.ContainsKey(button))
            {
                // Calculate new size (95% of original)
                int newWidth = (int)(originalSizes[button].Width * 0.95);
                int newHeight = (int)(originalSizes[button].Height * 0.95);

                // Calculate new position to keep it centered
                int deltaX = (originalSizes[button].Width - newWidth) / 2;
                int deltaY = (originalSizes[button].Height - newHeight) / 2;

                button.Location = new Point(button.Location.X + deltaX, button.Location.Y + deltaY);
                button.Size = new Size(newWidth, newHeight);
            }
        }

        private void Button_MouseUp(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            if (e.Button == MouseButtons.Left && originalSizes.ContainsKey(button))
            {
                RestoreButtonSize(button);
            }
        }

        private void Button_MouseLeave(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if (originalSizes.ContainsKey(button) && button.Size != originalSizes[button])
            {
                RestoreButtonSize(button);
            }
        }

        private void RestoreButtonSize(Button button)
        {
            // Restore original position
            int deltaX = (originalSizes[button].Width - button.Width) / 2;
            int deltaY = (originalSizes[button].Height - button.Height) / 2;

            button.Location = new Point(button.Location.X - deltaX, button.Location.Y - deltaY);

            // Restore original size
            button.Size = originalSizes[button];
        }

        // New helper method to update button background images based on enabled state
        public void UpdateButtonImages()
        {
            // Play/Pause button
            if (!btnPlayPause.Enabled)
            {
                // Use disabled play icon
                btnPlayPause.BackgroundImage = mediaPlayer?.IsPlaying ?? false
                    ? Properties.Resources.DisabledPauseIcon
                    : Properties.Resources.DisabledPlayIcon;
            }
            else
            {
                // Use normal icon
                btnPlayPause.BackgroundImage = mediaPlayer?.IsPlaying ?? false
                    ? Properties.Resources.PauseIcon
                    : Properties.Resources.PlayIcon;
            }

            // Stop button
            btnStop.BackgroundImage = btnStop.Enabled
                ? Properties.Resources.StopIcon
                : Properties.Resources.DisabledStopIcon;

            // Previous button
            btnPrev.BackgroundImage = btnPrev.Enabled
                ? Properties.Resources.PrevIcon
                : Properties.Resources.DisabledPrevIcon;

            // Next button
            btnNext.BackgroundImage = btnNext.Enabled
                ? Properties.Resources.NextIcon
                : Properties.Resources.DisabledNextIcon;

            // Theme button

            btnChangeTheme.BackgroundImage = btnChangeTheme.Enabled
                ? Properties.Resources.ThemeIcon
                : Properties.Resources.DisabledThemeIcon;

            // Notify listeners that button states changed
            OnButtonStatesChanged();

        }


    }
}
