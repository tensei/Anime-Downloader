using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Anime_Downloader.Handlers;
using Anime_Downloader.Properties;
using Anime_Downloader.Utility;
using Anime_Downloader.ViewModels;
using MaterialDesignThemes.Wpf;

namespace Anime_Downloader {
    /// <summary>
    ///     Interaktionslogik für MainWindow.xaml
    /// </summary>
    [Serializable]
    public partial class MainWindow : Window {
        private readonly ConfigFileHandler c = new ConfigFileHandler();
        private readonly DoubleClickHandler doubleClick = new DoubleClickHandler();
        public readonly DispatcherTimer Timer = new DispatcherTimer();
        //private JObject jsonFile;

        public MainWindow() {
            InitializeComponent();
            c.CheckFile();
            Settings.Default.StatusLabel = "Status: Setting things up...";
            Tools.SetupNotifyIcon();
        }

        //move window func
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e) {
            try {
                if (e.ChangedButton == MouseButton.Left)
                    DragMove();
            }
            catch (Exception) {
                //
            }
        }

        private void button_Click(object sender, RoutedEventArgs e) {
            Tools.Savelist();
            Tools.notifyIcon.Dispose();
            Close();
        }

        private void MiniBtn_Click(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Minimized;
        }

        private void SettingBtn_Click(object sender, RoutedEventArgs e) {
            var dialog = new SettingsDialog();
            DialogHost.Show(dialog);
        }

        private void DataGridMangas_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            try {
                var itemselected = (AnimeViewModel) DataGridAnime.SelectedItem;
                doubleClick.Open(itemselected);
                itemselected.Status = "Watched";
            }
            catch (Exception a) {
                var itemselected = (AnimeViewModel) DataGridAnime.SelectedItem;
                MessageBox.Show(itemselected.Tag + " " + a.Message);
            }
        }

        private void MenuItemWatched_click(object sender, RoutedEventArgs e) {
            var item = (AnimeViewModel) DataGridAnime.SelectedItem;
            item.Status = "Watched";
        }

        private void MenuItemNotWatched_click(object sender, RoutedEventArgs e) {
            var item = (AnimeViewModel) DataGridAnime.SelectedItem;
            item.Status = "Not Watched";
        }

        // ReSharper disable once InconsistentNaming
        private void FillRSSbox() {
            if(FilterComboBox.SelectedIndex.Equals(-1)) return;
            var selected = (ComboBoxItem) FilterComboBox.SelectedItem;
            MainWindowViewModel._animeRssInternal.Clear();
            RssUtility.GetFeedTask(selected.Content.ToString());
        }

        private void textBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            FillRSSbox();
        }

        private void Searchrss() {
            MainWindowViewModel._animeRssInternal.Clear();
            RssUtility.SearchGetFeed(Global.Rss + "&term=" + searchbox.Text.Trim().Replace(" ", "+"));
        }

        private void button_Click_1(object sender, RoutedEventArgs e) {
            if (searchbox.Text.Equals("Search...")) return;
            Searchrss();
        }

        private void RSSBtn_Click(object sender, RoutedEventArgs e) {
            if(!Rssfeedpanel.Visibility.Equals(Visibility.Visible)) {
                Rssfeedpanel.Visibility = Visibility.Visible;
                DataGridRss.Visibility = Visibility.Visible;
                CloseStackPanel.Visibility = Visibility.Visible;
                FilterComboBox.SelectedItem = FilterComboBox.Items[0];
                FillRSSbox();
                return;
            }
            Rssfeedpanel.Visibility = Visibility.Collapsed;
            DataGridRss.Visibility = Visibility.Collapsed;
            CloseStackPanel.Visibility = Visibility.Collapsed;
        }

        private void DataGridRss_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            if (DataGridRss.SelectedIndex.Equals(-1)) return;

            var selectediten = (NyaaseRssViewModel) DataGridRss.SelectedItem;


            var suggestedname =
                Regex.Match(selectediten.Name, ".+](.+)-.+", RegexOptions.IgnoreCase).Groups[1].Value;
            var dialog = new ConfirmDownloadDialog {
                TorrentClient = Global.TorrentClient,
                TorrentFiles = Global.TorrentFiles,
                Item = selectediten,
                Filenamelabel = {
                    Text = selectediten.Name
                },
                Folderbox = {
                    Text = Path.Combine(Global.OngoingFolder, suggestedname.Trim())
                }
            };
            DialogHost.Show(dialog);
        }


        private void Refreshbtn_Click(object sender, RoutedEventArgs e) {
            FillRSSbox();
        }

        private void DebugBtn_Click(object sender, RoutedEventArgs e) {
            if (DebugTextBox.Visibility.Equals(Visibility.Collapsed)) {
                DebugTextBox.Visibility = Visibility.Visible;
                return;
            }
            DebugTextBox.Visibility = Visibility.Collapsed;
        }

        private void DataGridAnime_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (DataGridAnime.SelectedIndex.Equals(-1)) {
                return;
            }
            //DataGridAnime.ContextMenu.Items.Clear();

            var item = (AnimeViewModel) DataGridAnime.SelectedItem;
            MenuItemHeader.Header = item.Name;
        }
    }
}