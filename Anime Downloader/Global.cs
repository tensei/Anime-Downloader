using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using Anime_Downloader.ViewModels;
using Newtonsoft.Json.Linq;

namespace Anime_Downloader
{
    public class Global : INotifyPropertyChanged
    {
        private const string Filepath = "AnimeDownloader.json";
        private static string _ongoingFolder = "";
        private static string _torrentFiles = "";
        private static string _torrentClient = "";
        private static string _resolution = "";
        public static List<string> Done = new List<string>();
        public static List<string> Groups = new List<string>();

        private static JObject _jsonFile;

        public static int _timerInternal = 5;

        public static ObservableCollection<AnimeViewModel> Anime = MainWindowViewModel._animeInternal;

        public static string GroupAdd
        {
            set
            {
                if (Groups.Contains(value)) return;
                Groups.Insert(0, value);
            }
        }

        public static string DoneAdd
        {
            set
            {
                if (Done.Contains(value)) return;
                Done.Add(value);
            }
        }

        public static AnimeViewModel AnimeAdd
        {
            set
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(delegate
                {
                    var a = MainWindowViewModel._animeInternal;
                    if (a.Contains(value)) return;
                    a.Insert(0, value);
                }));
            }
        }

        public static int Timer
        {
            get { return _timerInternal; }
            set
            {
                if (_timerInternal.Equals(value)) return;
                _timerInternal = value;
            }
        }

        public static string OngoingFolder
        {
            get { return _ongoingFolder; }
            set
            {
                if (_ongoingFolder.Equals(value)) return;
                _ongoingFolder = value;
            }
        }

        public static string TorrentFiles
        {
            get { return _torrentFiles; }
            set
            {
                if (_torrentFiles.Equals(value)) return;
                _torrentFiles = value;
            }
        }

        public static string TorrentClient
        {
            get { return _torrentClient; }
            set
            {
                if (_torrentClient.Equals(value)) return;
                _torrentClient = value;
            }
        }

        public static string Res
        {
            get { return _resolution; }
            set
            {
                if (_resolution.Equals(value)) return;
                _resolution = value;
            }
        }

        public static JObject jsonFile
        {
            get
            {
                _jsonFile = JObject.Parse(File.ReadAllText(Filepath));
                return _jsonFile;
            }
            set
            {
                if (_jsonFile.Equals(value)) return;
                _jsonFile = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}