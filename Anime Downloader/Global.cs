using System.Collections.Generic;
using System.IO;
using Anime_Downloader.ViewModels;
using Newtonsoft.Json.Linq;

namespace Anime_Downloader
{
    public class Global
    {
        private const string Filepath = "AnimeDownloader.json";
        private static string _ongoingFolder = "";
        private static string _torrentFiles = "";
        private static string _torrentClient = "";
        private static string _resolution = "";

        private static readonly List<AnimeViewModel> _animeInternal = new List<AnimeViewModel>();
        private static JObject _jsonFile;

        public static int _timerInternal = 5;

        public static List<AnimeViewModel> Anime = new List<AnimeViewModel>(_animeInternal);

        public static AnimeViewModel AnimeAdd
        {
            set
            {
                if (Anime.Contains(value)) return;
                Anime.Insert(0, value);
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
    }
}