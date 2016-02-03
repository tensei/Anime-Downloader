using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Anime_Downloader.Torrent_clients;
using Anime_Downloader.Utility;
using Anime_Downloader.ViewModels;
using UserControl = System.Windows.Controls.UserControl;

namespace Anime_Downloader {
    /// <summary>
    ///     Interaktionslogik für ConfirmDownloadDialog.xaml
    /// </summary>
    public partial class ConfirmDownloadDialog : UserControl {
        private readonly Deluge deluge = new Deluge();
        public NyaaseRssViewModel Item;
        public string TorrentClient;
        public string TorrentFiles;

        public ConfirmDownloadDialog() {
            InitializeComponent();
        }

        public string OpenFolderDialog() {
            using (var f = new FolderBrowserDialog()) {
                if (f.ShowDialog() == DialogResult.OK) {
                    return f.SelectedPath;
                }
                return "null";
            }
        }

        private void Folderbox_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            var result = OpenFolderDialog();
            if (!result.Equals("null")) {
                Folderbox.Text = result;
            }
        }

        public void Download(string foldertxt, string filename, NyaaseRssViewModel item, string tf, string tc) {
            var selectediten = item;
            if (!Directory.Exists(foldertxt)) {
                Directory.CreateDirectory(foldertxt);
            }
            //var file = Tools.DownloadFile(selectediten.Link, tf + @"\");
            //using (var web = new WebClient())
            //{
            //    web.DownloadFile(new Uri(selectediten.Link),
            //        tf + @"\" + selectediten.Name.Replace("'", string.Empty) + @".torrent");
            //}
            Deluge.openFeeddownload(tc, foldertxt, selectediten.Link);

            var i = new AnimeViewModel {
                Name = selectediten.Name,
                Added = DateTime.Now,
                Tag = Path.Combine(foldertxt, item.Name),
                Status = "Not Watched"
            };
            Global.AnimeAdd = i;

            Tools.showBalloon("Nyaa.se", "Downloading\n" + selectediten.Name);
            GC.Collect();
        }

        private void DownloadBtn_Click_1(object sender, RoutedEventArgs e) {
            var fb = Folderbox.Text.Trim();
            var fn = Filenamelabel.Text.Trim();
            var t =
                new Thread(
                    () => Download(fb, fn, Item, TorrentFiles, TorrentClient)) {
                        IsBackground = true
                    };
            t.Start();
        }
    }
}