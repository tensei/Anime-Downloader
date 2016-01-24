using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Anime_Downloader {
    internal class uTorrent {
        public static void open(Dictionary<string, string> list, string title, string filename, string torrentfiles,
            string ongoing, string torrentclient) {
            var sta = new Process();
            var torrenFile = Path.Combine(torrentfiles, title + ".torrent");
            var downloadsafe = ongoing + @"\" + list[filename];
            var call = string.Format("/DIRECTORY \"{0}\" \"{1}\"", downloadsafe, torrenFile);
            sta.StartInfo.FileName = torrentclient;
            sta.StartInfo.Arguments = call;
            sta.Start();
            sta.Dispose();
        }
    }
}