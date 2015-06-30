using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anime_Downloader.Properties;

namespace Anime_Downloader
{
    class uTorrent
    {
        public void open(Dictionary<string,string> list , string title, string filename)
        {
            Process sta = new Process();
            var torrenFile = Path.Combine(Settings.Default.PathDownloads, title+ ".torrent");
            string downloadsafe = Settings.Default.PathOngoing +@"\"+ list[filename];
            var call = string.Format("/DIRECTORY \"{0}\" \"{1}\"", downloadsafe, torrenFile);
            sta.StartInfo.FileName = Settings.Default.PathuTorrent;
            sta.StartInfo.Arguments = call;
            sta.Start();
            sta.Dispose();
        }
    }
}
