using System.Collections.Generic;
using System.IO;
using Anime_Downloader.Utility;

namespace Anime_Downloader.Handlers
{
    public class CheckTitleHandler
    {
        public static string CheckTitle(string title, string filename, string TorrentFiles, string TorrentClient,
            string res, List<string> groups)
        {
            var Torrents = new List<string>(Directory.EnumerateFiles(TorrentFiles));
            if (title.Contains(filename) && title.Contains(res) &&
                !title.ToLower().Contains("batch"))
            {
                if (!Torrents.Contains(TorrentFiles + @"\" + title + @".torrent") &&
                    !Global.Done.Contains(title))
                {
                    //download file and add it to torrent downloader
                    if (TorrentClient.ToLower().Contains("utorrent"))
                    {
                        if (Tools.FillProcessList().Contains("utorrent"))
                        {
                            return "utorrent";
                        }
                    }
                    else if (TorrentClient.ToLower().Contains("deluge-console.exe"))
                    {
                        if (Tools.FillProcessList().Contains("deluged"))
                        {
                            return "deluge";
                        }
                    }
                }
            }
            else if (title.Contains(filename) && groups.Contains(title.Substring(0, 4)) &&
                     !title.ToLower().Contains("batch"))
            {
                if (!Torrents.Contains(TorrentFiles + @"\" + title + @".torrent") && !Global.Done.Contains(title))
                {
                    //download file and add it to torrent downloader
                    if (TorrentClient.ToLower().Contains("utorrent"))
                    {
                        if (Tools.FillProcessList().Contains("utorrent"))
                        {
                            return "utorrent";
                        }
                    }
                    else if (TorrentClient.ToLower().Contains("deluge-console.exe"))
                    {
                        if (Tools.FillProcessList().Contains("deluged"))
                        {
                            return "deluge";
                        }
                    }
                }
            }
            return "nope";
        }
    }
}