using System;
using Anime_Downloader.Properties;

namespace Anime_Downloader.Utility {
    public class Debug {
        public static void Write(string text) {
            Settings.Default.Debug += $"\n[{DateTime.Now}] {text}";
        }
    }
}