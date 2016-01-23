using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using Anime_Downloader.Handlers;
using Anime_Downloader.Properties;
using Anime_Downloader.Utility;
using Anime_Downloader.ViewModels;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json.Linq;
using MenuItem = System.Windows.Controls.MenuItem;
using MessageBox = System.Windows.MessageBox;

namespace Anime_Downloader
{
    /// <summary>
    ///     Interaktionslogik für MainWindow.xaml
    /// </summary>
    [Serializable]
    public partial class MainWindow : Window
    {
        private readonly ConfigFileHandler c = new ConfigFileHandler();
        private readonly DoubleClickHandler doubleClick = new DoubleClickHandler();
        private readonly string Filepath = "AnimeDownloader.json";
        private readonly JObject jsonFile;
        public readonly NotifyIcon notifyIcon = new NotifyIcon();
        private readonly string Rssfeed = "http://www.nyaa.se/?page=rss&cats=1_37";
        public readonly DispatcherTimer Timer = new DispatcherTimer();
        public List<object> LstItems = new List<object>();
        public string OngoingFolder;
        public string Res;
        public string TorrentClient;
        public string TorrentFiles;
        //private JObject jsonFile;

        public MainWindow()
        {
            InitializeComponent();
            c.CheckFile();
            jsonFile = JObject.Parse(File.ReadAllText(Filepath));
            foreach (var child in jsonFile["Groups"].Children())
            {
                Settings.Default.Groups.Add(child.ToString());
                Global.GroupAdd = child.ToString();
            }
            //foreach (var Item in Settings.Default.Listbox)
            //{
            //    DebugTextbox.Text += Item;
            //}
            Settings.Default.RSS = jsonFile["RSS"].ToString();

            Global.OngoingFolder = jsonFile["Ongoing_Folder"].ToString();
            Global.TorrentFiles = jsonFile["Torrent_Files"].ToString();
            Global.TorrentClient = jsonFile["Torrent_Client"].ToString();
            Global.Res = jsonFile["Resolution"].ToString();
            Settings.Default.StatusLabel = "Status: Setting things up...";
            //DataGridAnime.ItemsSource = Global.Anime;
            Tools.SetupNotifyIcon();
        }

        //move window func
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == MouseButton.Left)
                    DragMove();
            }
            catch (Exception)
            {
                //
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Tools.Savelist();
            Tools.notifyIcon.Dispose();
            Close();
        }

        private void MiniBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void SettingBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SettingsDialog();
            DialogHost.Show(dialog);
        }

        private void DataGridMangas_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var itemselected = (AnimeViewModel) DataGridAnime.SelectedItem;
                doubleClick.Open(itemselected);
                itemselected.Status = "Watched";
            }
            catch (Exception a)
            {
                var itemselected = (AnimeViewModel) DataGridAnime.SelectedItem;
                MessageBox.Show(itemselected.Tag + " " + a.Message);
            }
        }

        private void MenuItem_click(object sender, RoutedEventArgs e)
        {
            var item = (AnimeViewModel) DataGridAnime.SelectedItem;
            item.Status = "Watched";
        }

        private void DataGridMangas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (DataGridAnime.SelectedIndex.Equals(-1))
            {
                return;
            }
            DataGridAnime.ContextMenu.Items.Clear();

            var item = (AnimeViewModel) DataGridAnime.SelectedItem;
            var name = new MenuItem
            {
                Header = item.Name,
                IsEnabled = false
            };
            DataGridAnime.ContextMenu.Items.Add(name);

            var watchediItem = new MenuItem {Header = "Watched"};
            watchediItem.Click += MenuItem_click;
            DataGridAnime.ContextMenu.Items.Add(watchediItem);
        }

        public async void FillRSSbox()
        {
            if (FilterComboBox.SelectedIndex.Equals(-1)) return;
            var selected = (ComboBoxItem) FilterComboBox.SelectedItem;
            DataGridRss.ItemsSource = await RssUtility.GetFeedTask(selected.Content.ToString());
        }

        private void textBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillRSSbox();
        }

        private async void searchrss()
        {
            DataGridRss.Items.Clear();
            DataGridRss.ItemsSource =
                await RssUtility.SearchGetFeed(Rssfeed + "&term=" + searchbox.Text.Trim().Replace(" ", "+"));
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            if (searchbox.Text.Equals("Search...")) return;
            searchrss();
        }

        private void RSSBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Rssfeedpanel.Visibility == Visibility.Collapsed)
            {
                Rssfeedpanel.Visibility = Visibility.Visible;
                DataGridRss.Visibility = Visibility.Visible;
                CloseStackPanel.Visibility = Visibility.Visible;
                if (FilterComboBox.SelectedIndex.Equals(-1))
                {
                    FilterComboBox.Text = "Show all";
                    FillRSSbox();
                }
                return;
            }
            Rssfeedpanel.Visibility = Visibility.Collapsed;
            DataGridRss.Visibility = Visibility.Collapsed;
            CloseStackPanel.Visibility = Visibility.Collapsed;
        }

        private void DataGridRss_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataGridRss.SelectedIndex.Equals(-1)) return;

            var selectediten = (NyaaseRssViewModel) DataGridRss.SelectedItem;


            var suggestedname =
                Regex.Match(selectediten.Name, ".+](.+)-.+", RegexOptions.IgnoreCase).Groups[1].Value;
            var dialog = new ConfirmDownloadDialog
            {
                TorrentClient = Global.TorrentClient,
                TorrentFiles = Global.TorrentFiles,
                Item = selectediten,
                Filenamelabel =
                {
                    Text = selectediten.Name
                },
                Folderbox =
                {
                    Text = Path.Combine(Global.OngoingFolder, suggestedname.Trim())
                }
            };
            DialogHost.Show(dialog);
        }


        private void Refreshbtn_Click(object sender, RoutedEventArgs e)
        {
            FillRSSbox();
            DataGridRss.ScrollIntoView(DataGridRss.Items[0]);
        }
    }
}