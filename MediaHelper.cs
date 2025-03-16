using System;
using TagLib;

namespace CuteMediaPlayer
{
    public static class MediaHelper
    {
        // 🎵 Get basic metadata from media file
        public static (string title, string artist, Image albumArt, TimeSpan duration) GetMetadata(string filePath)
        {
            try
            {
                // Open file with TagLib
                using var file = TagLib.File.Create(filePath);
                
                // 🎤 Get title (use filename if empty)
                string title = file.Tag.Title ?? Path.GetFileNameWithoutExtension(filePath);
                
                // 🎤 Get artist (use "Unknown" if empty)
                string artist = file.Tag.FirstPerformer ?? "Unknown Artist";

                // get media duration
                TimeSpan duration = file.Properties?.Duration ?? TimeSpan.Zero; 

                // 🖼️ Get album art (use placeholder if none)
                Image albumArt = file.Tag.Pictures.Length > 0 
                    ? Image.FromStream(new MemoryStream(file.Tag.Pictures[0].Data.Data))
                    : null;

                return (title, artist, albumArt, duration);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading metadata: {ex.Message}");
                // Return fallback values
                return (Path.GetFileNameWithoutExtension(filePath), "Unknown Artist", null, TimeSpan.Zero); // Add zero duration
            }
        }
    }
}
