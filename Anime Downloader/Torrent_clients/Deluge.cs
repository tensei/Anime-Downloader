using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Debug = Anime_Downloader.Utility.Debug;

namespace Anime_Downloader.Torrent_clients {
    internal class Deluge {
        public static void open(Dictionary<string, string> list, string title, string filename, string torrentfiles,
            string ongoing, string torrentclient) {
            var sta = new Process();
            var torrenFile = Path.Combine(torrentfiles, title + ".torrent");
            var downloadsafe = ongoing + @"\" + list[filename];
            //"add -p 'D:/Program Files (x86)/Deluge' 'D:/Development/python34/Anime checker/torrents/[HorribleSubs] Hibike! Euphonium - 13 [720p].mkv.torrent'"
            var call = string.Format("\"add -p '{0}' '{1}'\"", downloadsafe,
                torrenFile.Replace("'", string.Empty).Replace("µ", "u"));
            call = call.Replace(@"\\", "/");
            call = call.Replace(@"\", "/");
            sta.StartInfo.FileName = torrentclient;
            sta.StartInfo.Arguments = call;
            sta.StartInfo.RedirectStandardOutput = true;
            sta.StartInfo.RedirectStandardError = true;
            sta.EnableRaisingEvents = true;
            sta.StartInfo.CreateNoWindow = true;
            // see below for output handler
            sta.ErrorDataReceived += proc_DataReceived;
            sta.OutputDataReceived += proc_DataReceived;
            sta.StartInfo.UseShellExecute = false;
            sta.Start();

            sta.BeginErrorReadLine();
            sta.BeginOutputReadLine();
            sta.WaitForExit();
        }

        public static void openFeeddownload(string torrentfiles, string torrentclient, string filename, string folder) {
            var sta = new Process();
            var torrenFile = Path.Combine(torrentfiles, filename);
            var downloadsafe = folder.Trim();
            //"add -p 'D:/Program Files (x86)/Deluge' 'D:/Development/python34/Anime checker/torrents/[HorribleSubs] Hibike! Euphonium - 13 [720p].mkv.torrent'"
            var call = string.Format("\"add -p '{0}' '{1}'\"", downloadsafe,
                torrenFile.Replace("'", string.Empty).Replace("µ", "u"));
            call = call.Replace(@"\\", "/");
            call = call.Replace(@"\", "/");
            sta.StartInfo.FileName = torrentclient;
            sta.StartInfo.Arguments = call;
            sta.StartInfo.RedirectStandardOutput = true;
            sta.StartInfo.RedirectStandardError = true;
            sta.EnableRaisingEvents = true;
            sta.StartInfo.CreateNoWindow = true;
            // see below for output handler
            sta.ErrorDataReceived += proc_DataReceived;
            sta.OutputDataReceived += proc_DataReceived;

            sta.StartInfo.UseShellExecute = false;
            sta.Start();

            sta.BeginErrorReadLine();
            sta.BeginOutputReadLine();
            sta.WaitForExit();
        }

        private static void proc_DataReceived(object sender, DataReceivedEventArgs e) {
            if (e.Data != null)
                Debug.Write(e.Data);
        }
    }
}