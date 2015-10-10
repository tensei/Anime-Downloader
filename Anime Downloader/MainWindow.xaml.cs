using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using Anime_Downloader.Handlers;
using Anime_Downloader.Properties;
using Newtonsoft.Json.Linq;
using Color = System.Windows.Media.Color;
using MenuItem = System.Windows.Controls.MenuItem;
using MessageBox = System.Windows.MessageBox;
using SystemColors = System.Windows.SystemColors;

namespace Anime_Downloader
{
    /// <summary>
    ///     Interaktionslogik für MainWindow.xaml
    /// </summary>
    [Serializable]
    public partial class MainWindow : Window
    {
        private string Rssfeed = "http://www.nyaa.se/?page=rss&cats=1_37";
        public List<string> done = new List<string>();
        public List<string> groups = new List<string>();
        public List<string> last = new List<string>();
        public Dictionary<string, string> ongoing = new Dictionary<string, string>();
        public string TorrentFiles;
        public string OngoingFolder;
        public string TorrentClient;
        public string Res;
        public List<string> Torrents = new List<string>();
        private readonly Deluge deluge = new Deluge();
        private readonly nyaase neko = new nyaase();
        private readonly  DoubleClickHandler doubleClick = new DoubleClickHandler();
        public CheckTitleHandler CheckTitle = new CheckTitleHandler();
        private readonly List<string> processes = new List<string>();
        private readonly uTorrent uTorrent = new uTorrent();
        public List<object> LstItems = new List<object>();
        private readonly SolidColorBrush borderBrush = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
        private readonly SolidColorBrush ReadColorFg = new SolidColorBrush(Color.FromArgb(255, 140, 140, 140));
        public readonly NotifyIcon notifyIcon = new NotifyIcon();
        private readonly System.Windows.Forms.ContextMenu contextMenu1 = new System.Windows.Forms.ContextMenu();

        private readonly System.Windows.Forms.MenuItem ExitMenuItem = new System.Windows.Forms.MenuItem();
        private readonly System.Windows.Forms.MenuItem ShowmenuItem = new System.Windows.Forms.MenuItem();
        private readonly System.Windows.Forms.MenuItem ForcemenuItem = new System.Windows.Forms.MenuItem();

        private readonly ConfigFileHandler c = new ConfigFileHandler(); 
        private readonly string Filepath = "AnimeDownloader.json";
        private JObject jsonFile;
        private int _timer = 5;
        
        //private JObject jsonFile;

        public MainWindow()
        {
            InitializeComponent();
            c.CheckFile();
            jsonFile = JObject.Parse(File.ReadAllText(Filepath));

            ThreadStart childref = CheckNow;
            var childThread = new Thread(childref) {IsBackground = true};
            childThread.Start();

            FillProcessList();
            notifyIcon.ContextMenu = contextMenu1;
            notifyIcon.ContextMenu.MenuItems.Add(ExitMenuItem);
            notifyIcon.Icon = Properties.Resources.testicon;
            notifyIcon.DoubleClick += ShowMenuItem_Click;
            notifyIcon.Visible = true;
            showBalloon("Anime Downloader", "Starting...");
            AddMenuItems();
            //foreach (var item in Settings.Default.Listbox)
            //{
            //    DebugTextbox.Text += item;
            //}
            foreach (var child in jsonFile["Groups"].Children())
            {
                GroupsTextBox.Text += child + ", ";
                groups.Add(child.ToString());
            }
            comboBox.Text = jsonFile["Resolution"].ToString();
            //groups.AddRange();
            //groups.Remove("");
            TorrentClientTextBox.Text = jsonFile["Torrent_Client"].ToString();
            TorrentFilesTextBox.Text = jsonFile["Torrent_Files"].ToString();
            OnGoingFolderTextBox.Text = jsonFile["Ongoing_Folder"].ToString();
            RefreshTimebox.Text = jsonFile["Refresh_Time"].ToString();
            RSSFeedbox.Text = jsonFile["RSS"].ToString();
            Settings.Default.RSS = jsonFile["RSS"].ToString();

            OngoingFolder = jsonFile["Ongoing_Folder"].ToString();
            TorrentFiles = jsonFile["Torrent_Files"].ToString();
            TorrentClient = jsonFile["Torrent_Client"].ToString();
            Res = jsonFile["Resolution"].ToString();
            Settings.Default.StatusLabel = "Status: Setting things up...";
            PopulateListbox();
        }

        private void PopulateListbox()
        {
            Getitems getitems = new Getitems();
            foreach (var item in getitems.get())
            {
                var iteminfo = item.Split(new[] { "[]" }, StringSplitOptions.None);
                ListBoxItem listBoxItem = new ListBoxItem();
                listBoxItem.Content = iteminfo[0];
                listBoxItem.Tag = iteminfo[1];
                listBox.Items.Insert(0, listBoxItem);
            }
        }

        private void AddMenuItems()
        {
            ExitMenuItem.Index = 0;
            ExitMenuItem.Text = "Exit";
            ExitMenuItem.Click += ExitMenuItemClick;

            ShowmenuItem.Index = 1;
            ShowmenuItem.Text = "Show";
            ShowmenuItem.Click += ShowMenuItem_Click;

            ForcemenuItem.Index = 2;
            ForcemenuItem.Text = "Force";
            ForcemenuItem.Click += ForceMenuItem_Click;

            notifyIcon.ContextMenu.MenuItems.Add(ForcemenuItem);
            notifyIcon.ContextMenu.MenuItems.Add(ShowmenuItem);
            notifyIcon.ContextMenu.MenuItems.Add(ExitMenuItem);

        }
        private void ExitMenuItemClick(object Sender, EventArgs e)
        {
            // Close the form, which closes the application. 
            GetItemInfos getItemInfos = new GetItemInfos();
            SaveOnExit saveOnExit = new SaveOnExit();
            var listboxitem = new List<object>();
            foreach (var item in listBox.Items)
            {
                var it = ((ListBoxItem)item);
                if (!it.Foreground.Equals(ReadColorFg))
                {
                    listboxitem.Add(item);
                }
            }
            var items = getItemInfos.ConverttostringList(listboxitem);
            saveOnExit.Saveitems(items);
            notifyIcon.Dispose();
            Close();
        }
        private void ShowMenuItem_Click(object Sender, EventArgs e)
        {
            // Close the form, which closes the application. 

            WindowState = WindowState.Normal;
            Topmost = true;
            Topmost = false;

        }

        private void ForceMenuItem_Click(object Sender, EventArgs e)
        {
            // Close the form, which closes the application. 
            _timer = 1;
        }

        private void FillProcessList()
        {
            processes.Clear();
            var processlist = Process.GetProcesses();

            foreach (var theprocess in processlist)
            {
                var pname = theprocess.ProcessName;
                processes.Add(pname);
            }
            //if (TorrentClientTextBox.Text.ToLower().Contains("utorrent") && !processes.Contains("uTorrent"))
            //{
            //    Process.Start(TorrentClient);
            //}
            //else if(TorrentClientTextBox.Text.ToLower().Contains("deluge") && !processes.Contains("deluge"))
            //{
            //    Process.Start(TorrentClient.Replace("-console", "d"));
            //}

        }

        private void showBalloon(string title, string body)
        {
            if (title != null)
            {
                notifyIcon.BalloonTipTitle = title;
            }

            if (body != null)
            {
                notifyIcon.BalloonTipText = body;
            }
            notifyIcon.ShowBalloonTip(2000);
        }

        private void CheckNow()
        {
            while (true)
            {
                if (_timer >= 1)
                {
                    if (File.Exists(TorrentClient) &&
                        Directory.Exists(OngoingFolder) &&
                        Directory.Exists(TorrentFiles))
                    {
                        Settings.Default.StatusLabel = "Status: Checking in " + _timer +
                                                       " seconds.";
                        _timer--;
                    }
                    else
                    {
                        Settings.Default.StatusLabel = "Status: Please check Setting and mack sure everything is fine";
                    }
                    Thread.Sleep(1000);
                }
                else
                {
                    var client = new WebClient();
                    Settings.Default.StatusLabel = "Status: Checking in " + _timer + " seconds.";
                    Torrents = new List<string>(Directory.EnumerateFiles(TorrentFiles));
                    GetOnGoing();
                    List<string> rssitems;
                    try
                    {
                        rssitems = neko.Get_feed_titles(Settings.Default.RSS);
                    }
                    catch (Exception)
                    {
                        rssitems = new List<string>();
                        //MessageBox.Show(e.Message);
                    }
                    if (rssitems.Count > 1)
                    {
                        FillProcessList();
                        GetOnGoing();
                        foreach (var item in rssitems)
                        {
                            var itemSplit = item.Split(new[] {"[]"}, StringSplitOptions.None);
                            var title = itemSplit[0];
                            var link = itemSplit[1];
                            foreach (var filename in ongoing.Keys)
                            {
                                filename.Replace("Anime Koi", "Anime-Koi");
                                switch (CheckTitle.CheckTitle(title, filename, TorrentFiles, TorrentClient, Res, processes, groups, last))
                                {
                                    case "uTorrent":
                                    {
                                            client.DownloadFile(new Uri(link), TorrentFiles + @"\" + title + @".torrent");
                                            uTorrent.open(ongoing, title, filename, TorrentFiles, OngoingFolder, TorrentClient);
                                            Dispatcher.BeginInvoke(
                                                new Action(
                                                    delegate
                                                    {
                                                        CreateListboxItem(title, ongoing[filename] + @"\" + title, true);
                                                        showBalloon("New Anime", "Downloading\n" + title);
                                                        Settings.Default.Listbox.Add(title + "[]" + ongoing[filename] + @"\" +
                                                            title + "[]" + "true" + "\n");
                                                        Settings.Default.Save();
                                                    }));
                                            done.Add(title);
                                            Thread.Sleep(300);
                                            break;
                                    }
                                    case "deluge":
                                    {
                                            client.DownloadFile(new Uri(link), TorrentFiles + @"\" + title.Replace("'", string.Empty) + @".torrent");
                                            deluge.open(ongoing, title, filename, TorrentFiles, OngoingFolder, TorrentClient);
                                            Dispatcher.BeginInvoke(
                                                new Action(
                                                    delegate
                                                    {
                                                        CreateListboxItem(title.Replace("'", string.Empty), ongoing[filename] + @"\" + title.Replace("'", string.Empty), true);
                                                        showBalloon("New Anime", "Downloading\n" + title);
                                                        Settings.Default.Listbox.Add(title + "[]" + ongoing[filename] + @"\" +
                                                            title + "[]" + "true" + "\n");
                                                        Settings.Default.Save();
                                                    }));
                                            done.Add(title);
                                            Thread.Sleep(300);
                                            break;
                                    }
                                    case "nope":
                                        break;
                                }
                            }
                        }
                        _timer++;
                        client.Dispose();
                    }
                    _timer = int.Parse(jsonFile["Refresh_Time"].ToString());
                }
            }
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
            GetItemInfos getItemInfos = new GetItemInfos();
            SaveOnExit saveOnExit = new SaveOnExit();
            var listboxitem = new List<object>();
            foreach (var item in listBox.Items)
            {
                var it = ((ListBoxItem) item);
                if (!it.Foreground.Equals(ReadColorFg))
                {
                    listboxitem.Add(item);
                }
            }
            var items = getItemInfos.ConverttostringList(listboxitem);
            saveOnExit.Saveitems(items);
            notifyIcon.Dispose();
            Close();
        }

        private void MiniBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        public void CreateListboxItem(string content, string tag, bool isenabled)
        {
            var itmheader = new ListBoxItem();
            itmheader.Tag = OngoingFolder + @"\" + tag;
            itmheader.Content = content;
            if (!isenabled)
                itmheader.Foreground = ReadColorFg;
            //Settings.Default.Listbox += content + "[]" + OngoingFolder + @"\" + tag + "[]" + isenabled + "\n";

            //LstItems.Add(itmheader);
            //Settings.Default.ListboxItems.Insert(0, itmheader);
            //Settings.Default.Save();
            listBox.Items.Insert(0, itmheader);// (itmheader);
        }

        private void GetOnGoing()
        {
            last.Clear();
            ongoing.Clear();
            //Dictionary<string, string> ongoing = new Dictionary<string, string>();
            var dirs = new List<string>(Directory.EnumerateDirectories(OngoingFolder));
            //List<string> last = new List<string>();
            foreach (var dir in dirs)
            {
                var foldername = dir.Substring(dir.LastIndexOf("\\") + 1);

                var diranimefiles = new List<string>(Directory.EnumerateFiles(dir));
                foreach (var f in diranimefiles)
                {
                    var nameFile = f.Split(new[] {"\\"}, StringSplitOptions.None).Last();
                    var filn = nameFile.Split(new[] {"-"}, StringSplitOptions.None)[0];
                    if (!f.Contains(".sync") && !ongoing.Keys.Contains(filn) && filn.StartsWith("["))
                        //listBox.Items.Add(dir.Substring(dir.LastIndexOf("\\") + 1));
                        ongoing[filn] = foldername;
                }
                foreach (var file in diranimefiles)
                {
                    if (file.EndsWith(".mkv") || file.EndsWith(".mp4"))
                        last.Add(file.Split(new[] {"\\"}, StringSplitOptions.None).Last());
                }
            }
        }

        private void SettingBtn_Click(object sender, RoutedEventArgs e)
        {
            if (listBox.Visibility == Visibility.Collapsed)
            {
                listBox.Visibility = Visibility.Visible;
                listBox1.Visibility = Visibility.Visible;
            }
            else
            {
                listBox.Visibility = Visibility.Collapsed;
                listBox1.Visibility = Visibility.Collapsed;
                Rssfeedpanel.Visibility = Visibility.Collapsed;
                FeeditemBox.Visibility = Visibility.Collapsed;
                CloseStackPanel.Visibility = Visibility.Collapsed;
            }
        }



        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void listBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var itemselected = ((ListBoxItem) listBox.SelectedItem);
                doubleClick.Open(itemselected);
                itemselected.Foreground = ReadColorFg;
            }
            catch (Exception a)
            {
                var itemselected = ((ListBoxItem)listBox.SelectedItem);
                MessageBox.Show(itemselected.Tag+" " +a.Message);
            }
        }

        private void SaveAllBtn_Click(object sender, RoutedEventArgs e)
        {
            var groups = GroupsTextBox.Text.Split(new[] {", "}, StringSplitOptions.None).Where(s => s.Length == 4).ToList();
            
            GroupsTextBox.Text = string.Join(", ", groups);
            jsonFile["Groups"] = JToken.FromObject(groups);
            jsonFile["Resolution"] =((ComboBoxItem)comboBox.SelectedItem).Content.ToString();

            jsonFile["Torrent_Client"] = TorrentClientTextBox.Text;
            jsonFile["Torrent_Files"] = TorrentFilesTextBox.Text;
            jsonFile["Ongoing_Folder"] = OnGoingFolderTextBox.Text;
            jsonFile["Refresh_Time"] = RefreshTimebox.Text;
            jsonFile["RSS"] = RSSFeedbox.Text;
            File.WriteAllText(Filepath, jsonFile.ToString());

            Settings.Default.RSS = RSSFeedbox.Text;
            Res = jsonFile["Resolution"].ToString();
            TorrentClient = TorrentClientTextBox.Text;
            TorrentFiles = TorrentFilesTextBox.Text;
            OngoingFolder = OnGoingFolderTextBox.Text;
            

        }

        private void MenuItem_click(object sender, RoutedEventArgs e)
        {
            var item = ((ListBoxItem) listBox.SelectedItem);
            item.Foreground = ReadColorFg;
        }

        private void listBox_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (listBox.SelectedIndex.Equals(-1))
            {
                return;
            }
            listBox.ContextMenu.Items.Clear();

            var item = ((ListBoxItem) listBox.SelectedItem);
            MenuItem name = new MenuItem();
            name.Header = item.Content;
            name.Margin = new Thickness(+15, 0, -40, 0);
            name.IsEnabled = false;
            listBox.ContextMenu.Items.Add(name);
            
            MenuItem watchediItem = new MenuItem();
            watchediItem.Header = "Watched";
            watchediItem.Margin = new Thickness(+15, 0, -40, 0);
            watchediItem.Click += MenuItem_click;
            listBox.ContextMenu.Items.Add(watchediItem);
        }

        public SolidColorBrush GetStatus(string summary)
        {
            SolidColorBrush Trusted = new SolidColorBrush(Color.FromArgb(255, 152, 217, 168));
            SolidColorBrush Aplus = new SolidColorBrush(Color.FromArgb(255, 96, 176, 240));
            SolidColorBrush Remake = new SolidColorBrush(Color.FromArgb(255, 240, 176, 128));
            SolidColorBrush Normal = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            if (summary.ToLower().Contains("remake"))
            {
                return Remake;
            }
            else if (summary.ToLower().Contains("a+ - trusted"))
            {
                return Aplus;
            }
            else if (summary.ToLower().Contains("trusted"))
            {
                return Trusted;
            }
            else
            {
                return Normal;
            }

        }

        public void FillRSSbox()
        {
            var selected = ((ComboBoxItem)FilterComboBox.SelectedItem);
            switch (selected.Content.ToString())
            {
                case "Show all":
                    {
                        FeeditemBox.Items.Clear();
                        var feed = neko.Get_feed_titles("http://www.nyaa.se/?page=rss&cats=1_37");
                        Rssfeed = "http://www.nyaa.se/?page=rss&cats=1_37";
                        foreach (var item in feed)
                        {
                            var isplit = item.Split(new[] { "[]" }, StringSplitOptions.None);
                            ListBoxItem i = new ListBoxItem();
                            i.Content = isplit[0];
                            i.Tag = isplit[1];
                            i.ToolTip = isplit[2];
                            i.Background = GetStatus(isplit[2]);
                            i.BorderBrush = borderBrush;
                            i.BorderThickness = new Thickness(1, 1, 1, 0);
                            FeeditemBox.Items.Add(i);
                        }
                        FeeditemBox.ScrollIntoView(FeeditemBox.Items[0]);
                        break;
                    }
                case "Trusted only":
                    {
                        FeeditemBox.Items.Clear();
                        var feed = neko.Get_feed_titles("http://www.nyaa.se/?page=rss&cats=1_37&filter=2");
                        Rssfeed = "http://www.nyaa.se/?page=rss&cats=1_37&filter=2";
                        foreach (var item in feed)
                        {
                            var isplit = item.Split(new[] { "[]" }, StringSplitOptions.None);
                            ListBoxItem i = new ListBoxItem();
                            i.Content = isplit[0];
                            i.Tag = isplit[1];
                            i.ToolTip = isplit[2];
                            i.Background = GetStatus(isplit[2]);
                            i.BorderBrush = borderBrush;
                            i.BorderThickness = new Thickness(1, 1, 1, 0);
                            FeeditemBox.Items.Add(i);
                        }
                        FeeditemBox.ScrollIntoView(FeeditemBox.Items[0]);
                        break;
                    }
                case "Filter remakes":
                    {
                        FeeditemBox.Items.Clear();
                        var feed = neko.Get_feed_titles("http://www.nyaa.se/?page=rss&cats=1_37&filter=1");
                        Rssfeed = "http://www.nyaa.se/?page=rss&cats=1_37&filter=1";
                        foreach (var item in feed)
                        {
                            var isplit = item.Split(new[] { "[]" }, StringSplitOptions.None);
                            ListBoxItem i = new ListBoxItem();
                            i.Content = isplit[0];
                            i.Tag = isplit[1];
                            i.ToolTip = isplit[2];
                            i.Background = GetStatus(isplit[2]);
                            i.BorderBrush = borderBrush;
                            i.BorderThickness = new Thickness(1, 1, 1, 0);
                            FeeditemBox.Items.Add(i);
                        }
                        FeeditemBox.ScrollIntoView(FeeditemBox.Items[0]);
                        break;
                    }
                case "A+ only":
                    {
                        FeeditemBox.Items.Clear();
                        var feed = neko.Get_feed_titles("http://www.nyaa.se/?page=rss&cats=1_37&filter=3");
                        Rssfeed = "http://www.nyaa.se/?page=rss&cats=1_37&filter=3";
                        foreach (var item in feed)
                        {
                            var isplit = item.Split(new[] { "[]" }, StringSplitOptions.None);
                            ListBoxItem i = new ListBoxItem();
                            i.Content = isplit[0];
                            i.Tag = isplit[1];
                            i.ToolTip = isplit[2];
                            i.Background = GetStatus(isplit[2]);
                            i.BorderBrush = borderBrush;
                            i.BorderThickness = new Thickness(1, 1, 1, 0);
                            FeeditemBox.Items.Add(i);
                        }
                        FeeditemBox.ScrollIntoView(FeeditemBox.Items[0]);
                        break;
                    }
            }
        }

        private void textBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillRSSbox();
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            FeeditemBox.Items.Clear();
            var feed = neko.Get_feed_titles(Rssfeed + "&term=" + searchbox.Text.Trim().Replace(" ", "+"));
            foreach (var item in feed)
            {
                var isplit = item.Split(new[] { "[]" }, StringSplitOptions.None);
                ListBoxItem i = new ListBoxItem();
                i.Content = isplit[0];
                i.Tag = isplit[1];
                i.ToolTip = isplit[2];
                i.Background = GetStatus(isplit[2]);
                i.BorderBrush = borderBrush;
                i.BorderThickness = new Thickness(1,1,1,0);
                FeeditemBox.Items.Add(i);
            }
        }

        private void RSSBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Rssfeedpanel.Visibility == Visibility.Collapsed)
            {
                Rssfeedpanel.Visibility = Visibility.Visible;
                FeeditemBox.Visibility = Visibility.Visible;
                CloseStackPanel.Visibility = Visibility.Visible;
                if (FilterComboBox.SelectedIndex.Equals(-1))
                {
                    FilterComboBox.Text = "Show all";
                    FillRSSbox();
                }
                return;
            }
            Rssfeedpanel.Visibility = Visibility.Collapsed;
            FeeditemBox.Visibility = Visibility.Collapsed;
            CloseStackPanel.Visibility = Visibility.Collapsed;

        }

        private void FeeditemBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectediten = ((ListBoxItem) FeeditemBox.SelectedItem);
            Savepanel.Visibility = Visibility.Visible;
            Filenamelabel.Text = selectediten.Content.ToString();
            var suggestedname =
                Regex.Match(selectediten.Content.ToString(), ".+](.+)-.+", RegexOptions.IgnoreCase).Groups[1].Value;
            Folderbox.Text = Path.Combine(OngoingFolder, suggestedname.Trim());
        }

        private void ClosefeedpanelBtn_Copy_Click(object sender, RoutedEventArgs e)
        {
            Savepanel.Visibility = Visibility.Collapsed;
            Folderbox.Text = String.Empty;
            Filenamelabel.Text = "";
        }

        private void DownloadBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectediten = ((ListBoxItem)FeeditemBox.SelectedItem);
                if (!Directory.Exists(Folderbox.Text.Trim()))
                {
                    Directory.CreateDirectory(Folderbox.Text.Trim());
                }
                WebClient web = new WebClient();
                web.DownloadFile(new Uri(selectediten.Tag.ToString()), TorrentFiles + @"\" + selectediten.Content.ToString().Replace("'", string.Empty) + @".torrent");
                web.Dispose();
                deluge.openFeeddownload(TorrentFiles, TorrentClient, Filenamelabel.Text.Trim(), Folderbox.Text.Trim());
                
                ListBoxItem i = new ListBoxItem();
                i.Content = selectediten.Content;
                i.Tag = Path.Combine(Folderbox.Text.Trim(), Filenamelabel.Text.Trim());
                listBox.Items.Insert(0, i);
                Savepanel.Visibility = Visibility.Collapsed;
            }
            catch (Exception se)
            {
                MessageBox.Show(se.Message);
            }
        }

        private void Refreshbtn_Click(object sender, RoutedEventArgs e)
        {
            FillRSSbox();
            FeeditemBox.ScrollIntoView(FeeditemBox.Items[0]);
        }

        private void searchbox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (searchbox.Text == "Search...")
            {
                searchbox.Text = string.Empty;
            }
        }

        private void searchbox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (searchbox.Text == "")
            {
                searchbox.Text = "Search...";
            }
        }
    }
}