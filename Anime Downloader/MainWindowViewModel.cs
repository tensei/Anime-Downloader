using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;
using Anime_Downloader.Threads;
using Anime_Downloader.Utility;
using Anime_Downloader.ViewModels;
using Newtonsoft.Json.Linq;

namespace Anime_Downloader
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly ObservableCollection<AnimeViewModel> _animessInternal =
            new ObservableCollection<AnimeViewModel>();

        private readonly string Filepath = "AnimeDownloader.json";


        //private string _currentSite;
        //private string _threadStatus;

        private readonly ThreadStart Childref;
        private readonly Thread ChildThread;

        public List<string> done = new List<string>();
        public List<string> groups = new List<string>();
        private JObject jsonFile;
        public List<string> last = new List<string>();


        public Dictionary<string, string> ongoing = new Dictionary<string, string>();
        public string Res;

        public List<string> Torrents = new List<string>();

        public MainWindowViewModel()
        {
            Animes = new ReadOnlyObservableCollection<AnimeViewModel>(_animessInternal);
            Tools.PopulateListbox();
            RefreshCommand = new ActionCommand(changeitem);

            jsonFile = JObject.Parse(File.ReadAllText(Filepath));
            Childref = MainThread.CheckNow;
            ChildThread = new Thread(Childref) {IsBackground = true};
            ChildThread.Start();
            //ThreadStatus = "Running.";
            //CheckNow();
        }

        public ICommand RefreshCommand { get; }
        public ReadOnlyObservableCollection<AnimeViewModel> Animes { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void changeitem()
        {
            foreach (var x in _animessInternal)
            {
                MessageBox.Show(x.Name);
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}