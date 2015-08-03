using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Anime_Downloader.Properties;

namespace Anime_Downloader.Handlers
{
    public class CheckTitleHandler
    {
        public List<string> done = new List<string>();

        public string CheckTitle(string title, string filename, string TorrentFiles, string TorrentClient, string res,
            List<string> processes, List<string> groups, List<string> last )
        {
            var Torrents = new List<string>(Directory.EnumerateFiles(TorrentFiles));
            if (title.Contains(filename) && title.Contains(res) && !last.Contains(title) &&
                !title.ToLower().Contains("batch"))
            {
                if (!Torrents.Contains(TorrentFiles + @"\" + title + @".torrent") &&
                    !done.Contains(title))
                {
                    //download file and add it to torrent downloader
                    if (TorrentClient.ToLower().Contains("utorrent") &&
                        processes.Contains("uTorrent"))
                    {
                        if (processes.Contains("uTorrent"))
                        {
                            return "utTorrent";
                        }

                    }
                    else if (TorrentClient.ToLower().Contains("deluge-console.exe"))
                    {
                        if (processes.Contains("deluged"))
                        {
                            return "deluge";
                        }

                    }
                }
            }
            else if (title.Contains(filename) && groups.Contains(title.Substring(0, 4)) &&
                     !last.Contains(title) &&
                     !title.ToLower().Contains("batch"))
            {
                if (!Torrents.Contains(title + ".torrent") && !done.Contains(title))
                {
                    //download file and add it to torrent downloader
                    if (TorrentClient.ToLower().Contains("utorrent") &&
                        processes.Contains("uTorrent"))
                    {
                        if (processes.Contains("uTorrent"))
                        {
                            return "uTorrent";
                        }

                    }
                    else if (TorrentClient.ToLower().Contains("deluge-console.exe"))
                    {
                        if (processes.Contains("deluged"))
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
