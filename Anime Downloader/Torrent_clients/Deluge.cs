using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Debug = Anime_Downloader.Utility.Debug;

namespace Anime_Downloader.Torrent_clients {
    internal class Deluge {
        public static void Open(string downloadsafe, string ongoing, string torrentclient, string link) {
            var sta = new Process();
            //"add -p 'D:/Program Files (x86)/Deluge' 'D:/Development/python34/Anime checker/torrents/[HorribleSubs] Hibike! Euphonium - 13 [720p].mkv.torrent'"
            Debug.Write(ongoing + @"/" + downloadsafe);
            var call = $"\"add -p '{ongoing + @"/"+downloadsafe}' '{link}'\"";
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

        public static void openFeeddownload(string torrentclient, string folder, string link) {
            var sta = new Process();
            var downloadsafe = folder.Trim();
            Debug.Write(downloadsafe);
            //"add -p 'D:/Program Files (x86)/Deluge' 'D:/Development/python34/Anime checker/torrents/[HorribleSubs] Hibike! Euphonium - 13 [720p].mkv.torrent'"
            var call = $"\"add -p '{downloadsafe}' '{link}'\"";
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