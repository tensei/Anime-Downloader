using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Anime_Downloader.ViewModels;

namespace Anime_Downloader {
    public class Global : INotifyPropertyChanged {
        private const string Filepath = "AnimeDownloader.json";
        private static string _ongoingFolder = "";
        private static string _torrentFiles = "";
        private static string _torrentClient = "";
        private static string _resolution = "";
        private static string _rssFeed = "";
        public static List<string> Done = new List<string>();
        public static List<string> Groups = new List<string>();

        public static int _timerInternal = 5;

        public static ObservableCollection<AnimeViewModel> Anime = MainWindowViewModel._animeInternal;
        //public static ObservableCollection<NyaaseRssViewModel> AnimeRss = MainWindowViewModel._animeRssInternal;
        private static int _refreshTime { get; set; } = 300;

        public static string DoneAdd {
            set {
                if (Done.Contains(value)) return;
                Done.Add(value);
            }
        }

        public static AnimeViewModel AnimeAdd {
            set {
                Application.Current.Dispatcher.BeginInvoke(new Action(delegate {
                    var a = MainWindowViewModel._animeInternal;
                    if (a.Contains(value)) return;
                    a.Insert(0, value);
                }));
            }
        }
        public static NyaaseRssViewModel AnimeRssAdd {
            set {
                Application.Current.Dispatcher.BeginInvoke(new Action(delegate {
                    var a = MainWindowViewModel._animeRssInternal;
                    if (a.Contains(value)) return;
                    a.Add(value);
                }));
            }
        }

        public static int Timer {
            get { return _timerInternal; }
            set {
                if (_timerInternal.Equals(value)) return;
                _timerInternal = value;
            }
        }

        public static string OngoingFolder {
            get { return _ongoingFolder; }
            set {
                if (_ongoingFolder.Equals(value)) return;
                _ongoingFolder = value;
            }
        }

        public static string TorrentFiles {
            get { return _torrentFiles; }
            set {
                if (_torrentFiles.Equals(value)) return;
                _torrentFiles = value;
            }
        }

        public static string TorrentClient {
            get { return _torrentClient; }
            set {
                if (_torrentClient.Equals(value)) return;
                _torrentClient = value;
            }
        }

        public static string Resolution {
            get { return _resolution; }
            set {
                if (_resolution.Equals(value)) return;
                _resolution = value;
            }
        }

        public static string Rss {
            get { return _rssFeed; }
            set {
                if (_rssFeed.Equals(value)) return;
                _rssFeed = value;
            }
        }

        public static int RefreshTime {
            get { return _refreshTime; }
            set {
                if (_refreshTime.Equals(value)) return;
                _refreshTime = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}