using System.Collections.Generic;
using System.Diagnostics;
using Anime_Downloader.Properties;
using System.IO;

namespace Anime_Downloader
{
    internal class Deluge
    {
        public void open(Dictionary<string, string> list, string title, string filename)
        {
            var sta = new Process();
            var torrenFile = Path.Combine(Settings.Default.PathDownloads, title + ".torrent");
            var downloadsafe = Settings.Default.PathOngoing + @"\" + list[filename];
            //"add -p 'D:/Program Files (x86)/Deluge' 'D:/Development/python34/Anime checker/torrents/[HorribleSubs] Hibike! Euphonium - 13 [720p].mkv.torrent'"
            var call = string.Format("\"add -p '{0}' '{1}'\"", downloadsafe, torrenFile);
            call = call.Replace(@"\\", "/");
            call = call.Replace(@"\", "/");
            sta.StartInfo.FileName = Settings.Default.PathuTorrent;
            sta.StartInfo.Arguments = call;
            sta.StartInfo.CreateNoWindow = true;
            sta.Start();
            sta.Dispose();
        }
    }
}