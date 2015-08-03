using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Anime_Downloader.Properties;

namespace Anime_Downloader
{
    internal class uTorrent
    {
        public void open(Dictionary<string, string> list, string title, string filename)
        {
            var sta = new Process();
            var torrenFile = Path.Combine(Settings.Default.PathDownloads, title + ".torrent");
            var downloadsafe = Settings.Default.PathOngoing + @"\" + list[filename];
            var call = string.Format("/DIRECTORY \"{0}\" \"{1}\"", downloadsafe, torrenFile);
            sta.StartInfo.FileName = Settings.Default.PathuTorrent;
            sta.StartInfo.Arguments = call;
            sta.Start();
            sta.Dispose();
        }
    }
}