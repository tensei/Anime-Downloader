using System.Collections.Generic;
using System.IO;
using Anime_Downloader.Utility;

namespace Anime_Downloader.Handlers {
    public class CheckTitleHandler {
        public static string CheckTitle(string title, string filename) {
            var Torrents = new List<string>(Directory.EnumerateFiles(Global.TorrentFiles));
            if (title.Contains(filename) && title.Contains(Global.Resolution) &&
                !title.ToLower().Contains("batch")) {
                if (!Global.AllFiles.Contains(title.ToLower()) &&
                    !Global.Done.Contains(title)) {
                    //download file and add it to torrent downloader
                    if (Global.TorrentClient.ToLower().Contains("utorrent")) {
                        if (Tools.FillProcessList().Contains("utorrent")) {
                            return "utorrent";
                        }
                    }
                    else if (Global.TorrentClient.ToLower().Contains("deluge-console.exe")) {
                        if (Tools.FillProcessList().Contains("deluged")) {
                            return "deluge";
                        }
                    }
                }
            }
            else if (title.Contains(filename) && Global.Groups.Contains(title.Substring(0, 4)) &&
                     !title.ToLower().Contains("batch")) {
                if (!Global.AllFiles.Contains(title.ToLower()) && !Global.Done.Contains(title)) {
                    //download file and add it to torrent downloader
                    if (Global.TorrentClient.ToLower().Contains("utorrent")) {
                        if (Tools.FillProcessList().Contains("utorrent")) {
                            return "utorrent";
                        }
                    }
                    else if (Global.TorrentClient.ToLower().Contains("deluge-console.exe")) {
                        if (Tools.FillProcessList().Contains("deluged")) {
                            return "deluge";
                        }
                    }
                }
            }
            return "nope";
        }
    }
}