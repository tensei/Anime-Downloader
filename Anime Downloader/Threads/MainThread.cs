﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using Anime_Downloader.Handlers;
using Anime_Downloader.Properties;
using Anime_Downloader.Torrent_clients;
using Anime_Downloader.Utility;
using Anime_Downloader.ViewModels;

namespace Anime_Downloader.Threads {
    public class MainThread {
        public static void CheckNow() {
            while (true) {
                if (Global.Timer >= 1) {
                    if (File.Exists(Global.TorrentClient) &&
                        Directory.Exists(Global.OngoingFolder) &&
                        Directory.Exists(Global.TorrentFiles)) {
                        Settings.Default.StatusLabel = "Status: Checking in " + Global.Timer +
                                                       " seconds.";
                        Global.Timer--;
                    }
                    else {
                        Settings.Default.StatusLabel = "Status: Please check Setting and mack sure everything is fine";
                    }
                    Thread.Sleep(1000);
                }
                else {
                    var client = new WebClient();
                    Settings.Default.StatusLabel = "Status: Checking in " + Global.Timer + " seconds.";
                    List<NyaaseRssViewModel> rssitems;
                    try {
                        rssitems = Nyaase.GetItems(Global.Rss);
                    }
                    catch (Exception) {
                        rssitems = new List<NyaaseRssViewModel>();
                        //MessageBox.Show(e.Message);
                    }
                    if (rssitems.Count > 1) {
                        var ongoing = Tools.GetOnGoing();
                        foreach (var item in rssitems) {
                            var title = item.Name;
                            var link = item.Link;
                            foreach (var filename in ongoing.Keys) {
                                filename.Replace("Anime Koi", "Anime-Koi").Replace("µ","u");
                                title.Replace("µ", "u");
                                switch (
                                    CheckTitleHandler.CheckTitle(title, filename)) {
                                        case "utorrent": {
                                            //Tools.DownloadFile(link, Global.TorrentFiles + @"\");
                                            uTorrent.open(ongoing, title, filename, Global.TorrentFiles,
                                                Global.OngoingFolder, Global.TorrentClient);
                                            Tools.CreateDataGridItem(title, ongoing[filename] + @"\" + title, true);
                                            Tools.showBalloon("New Anime", "Downloading\n" + title);
                                            Settings.Default.Listbox.Add(title + "[]" + ongoing[filename] + @"\" +
                                                                         title + "[]" + "true" + "\n");
                                            Global.DoneAdd = title;
                                            Thread.Sleep(300);
                                            break;
                                        }
                                        case "deluge": {
                                            //Tools.DownloadFile(link, Global.TorrentFiles + @"\");
                                            Deluge.Open(ongoing[filename], Global.OngoingFolder, Global.TorrentClient, link);
                                            Tools.CreateDataGridItem(title.Replace("'", string.Empty),
                                                ongoing[filename] + @"\" + title.Replace("'", string.Empty), true);
                                            Tools.showBalloon("New Anime", "Downloading\n" + title);
                                            Settings.Default.Listbox.Add(title + "[]" + ongoing[filename] + @"\" +
                                                                         title + "[]" + "true" + "\n");
                                            Global.DoneAdd = title;
                                            Thread.Sleep(300);
                                            break;
                                        }
                                        case "nope": {
                                            break;
                                        }
                                }
                            }
                        }
                        Settings.Default.RefreshCounter++;
                        Global.Timer++;
                        client.Dispose();
                        GC.Collect();
                        Tools.Savelist();
                    }
                    Global.Timer = Global.RefreshTime;
                }
            }
        }
    }
}