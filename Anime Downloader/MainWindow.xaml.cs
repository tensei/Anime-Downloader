using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using Anime_Downloader.Properties;
using Newtonsoft.Json.Linq;
using Color = System.Windows.Media.Color;
using MessageBox = System.Windows.MessageBox;

namespace Anime_Downloader
{
    /// <summary>
    ///     Interaktionslogik für MainWindow.xaml
    /// </summary>
    [Serializable]
    public partial class MainWindow : Window
    {
        public List<string> done = new List<string>();
        public List<string> groups = new List<string> {"[FFF", "[Ani", "[Viv", "[Ase", "[JnM", "[Com"};
        public List<string> last = new List<string>();
        public Dictionary<string, string> ongoing = new Dictionary<string, string>();
        public string PathDownloads = Settings.Default.PathDownloads; // @"D:\Dummy\torrents";
        public string PathOngoing = Settings.Default.PathOngoing; //@"D:\dummy\Ongoing";
        public string PathuTorrent = Settings.Default.PathuTorrent; //@"D:\dummy\uTorrent.exe";
        public List<string> Torrents = new List<string>();
        private readonly Deluge deluge = new Deluge();
        private readonly nyaase neko = new nyaase();
        private readonly  DoubleClickHandler doubleClick = new DoubleClickHandler();
        private readonly List<string> processes = new List<string>();
        private readonly uTorrent uTorrent = new uTorrent();
        public List<object> LstItems = new List<object>();
        private readonly SolidColorBrush ReadColorFg = new SolidColorBrush(Color.FromArgb(255, 140, 140, 140));
        public readonly NotifyIcon notifyIcon = new NotifyIcon();
        private readonly System.Windows.Forms.ContextMenu contextMenu1 = new System.Windows.Forms.ContextMenu();
        private readonly System.Windows.Forms.MenuItem menuItem1 = new System.Windows.Forms.MenuItem();
        private readonly string Filepath = "AnimeDownloader.json";

        private JObject jsonFile;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            if (Settings.Default.PathOngoing == "")
                Settings.Default.PathOngoing = @"D:\dummy\Ongoing";
            Settings.Default.Save();
            if (Settings.Default.PathDownloads == "")
                Settings.Default.PathDownloads = @"D:\dummy\torrents";
            Settings.Default.Save();
            if (Settings.Default.PathuTorrent == "")
                Settings.Default.PathuTorrent = @"D:\dummy\uTorrent.exe";
            Settings.Default.Save();

            uTorrentTextBox.Text = Settings.Default.PathuTorrent;
            TorrentsFolder.Text = Settings.Default.PathDownloads;
            OnGoingTextBox.Text = Settings.Default.PathOngoing;
            RefreshTimebox.Text = Settings.Default.RefreshWaitTime.ToString();

            Settings.Default.RefreshTimer = 5;

            ThreadStart childref = CheckNow;
            var childThread = new Thread(childref);
            childThread.IsBackground = true;
            childThread.Start();

            FillProcessList();
            menuItem1.Index = 0;
            menuItem1.Text = "Exit";
            menuItem1.Click += menuItem1_Click;
            notifyIcon.ContextMenu = contextMenu1;
            notifyIcon.ContextMenu.MenuItems.Add(menuItem1);
            notifyIcon.Icon = Properties.Resources.testicon;
            notifyIcon.Visible = true;
            showBalloon("Anime Downloader", "Starting...");

            foreach (var item in Settings.Default.Listbox)
            {
                DebugTextbox.Text += item;
            }
            jsonFile = JObject.Parse(File.ReadAllText(Filepath));
            foreach (var child in jsonFile["Groups"].Children())
            {
                GroupsTextBox.Text += child + ", ";
            }
            comboBox.Text = jsonFile["Resolution"].ToString();
        }
        private void menuItem1_Click(object Sender, EventArgs e)
        {
            // Close the form, which closes the application. 
            notifyIcon.Dispose();
            Close();
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
                if (Settings.Default.RefreshTimer >= 1)
                {
                    if (File.Exists(PathuTorrent) &&
                        Directory.Exists(PathOngoing) &&
                        Directory.Exists(PathDownloads))
                    {
                        Settings.Default.StatusLabel = "Status: Checking in " + Settings.Default.RefreshTimer +
                                                       " seconds.";
                        Settings.Default.RefreshTimer--;
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
                    Settings.Default.StatusLabel = "Status: Checking in " + Settings.Default.RefreshTimer + " seconds.";
                    Torrents = new List<string>(Directory.EnumerateFiles(PathDownloads));
                    GetOnGoing();
                    List<string> rssitems;
                    try
                    {
                        rssitems = neko.Get_feed_titles();
                    }
                    catch (WebException e)
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
                                if (title.Contains(filename) && title.Contains("720") && !last.Contains(title) &&
                                    !title.ToLower().Contains("batch"))
                                {
                                    if (!Torrents.Contains(PathDownloads + @"\" + title + @".torrent") &&
                                        !done.Contains(title))
                                    {
                                        Dispatcher.BeginInvoke(
                                            new Action(
                                                delegate
                                                {
                                                    createListboxItem(title, ongoing[filename] + @"\" + title, true);
                                                    showBalloon("New Anime", "Downloading\n"+ title);
                                                    Settings.Default.Listbox.Add(title + "[]" + ongoing[filename] + @"\" +
                                                        title + "[]" + "true" + "\n");
                                                    Settings.Default.Save();
                                                }));
                                        //download file and add it to torrent downloader
                                        client.DownloadFile(new Uri(link), PathDownloads + @"\" + title + @".torrent");
                                        if (Settings.Default.PathuTorrent.ToLower().Contains("utorrent") &&
                                            processes.Contains("uTorrent"))
                                        {
                                            if (processes.Contains("uTorrent"))
                                            {
                                                uTorrent.open(ongoing, title, filename);
                                            }
                                            else
                                            {
                                                Process.Start(Settings.Default.PathuTorrent);
                                                Thread.Sleep(1500);
                                                uTorrent.open(ongoing, title, filename);
                                            }
                                        }
                                        else if (Settings.Default.PathuTorrent.ToLower().Contains("deluge-console.exe"))
                                        {
                                            if (processes.Contains("deluged"))
                                            {
                                                deluge.open(ongoing, title, filename);
                                            }
                                            else
                                            {
                                                Process.Start(Settings.Default.PathuTorrent.Replace("-console", "d"));
                                                Thread.Sleep(1500);
                                                deluge.open(ongoing, title, filename);
                                            }
                                        }
                                        done.Add(title);
                                        Thread.Sleep(200);
                                    }
                                }
                                else if (title.Contains(filename) && groups.Contains(title.Substring(0, 4)) &&
                                         !last.Contains(title) &&
                                         !title.ToLower().Contains("batch"))
                                {
                                    if (!Torrents.Contains(title + ".torrent") && !done.Contains(title))
                                    {
                                        Dispatcher.BeginInvoke(
                                            new Action(
                                                delegate
                                                {
                                                    createListboxItem(title, ongoing[filename] + @"\" + title, true);
                                                    showBalloon("New Anime", "Downloading\n" + title);
                                                    Settings.Default.Listbox.Add(title + "[]" + ongoing[filename] + @"\" +
                                                        title + "[]" + "true" + "\n");
                                                    Settings.Default.Save();
                                                }));
                                        //download file and add it to torrent downloader
                                        client.DownloadFile(new Uri(link),
                                            PathDownloads + @"\" + title + @".torrent");
                                        if (Settings.Default.PathuTorrent.ToLower().Contains("utorrent") &&
                                            processes.Contains("uTorrent"))
                                        {
                                            if (processes.Contains("uTorrent"))
                                            {
                                                uTorrent.open(ongoing, title, filename);
                                            }
                                            else
                                            {
                                                Process.Start(Settings.Default.PathuTorrent);
                                                Thread.Sleep(1500);
                                                uTorrent.open(ongoing, title, filename);
                                            }
                                        }
                                        else if (Settings.Default.PathuTorrent.ToLower().Contains("deluge-console.exe"))
                                        {
                                            if (processes.Contains("deluged"))
                                            {
                                                deluge.open(ongoing, title, filename);
                                            }
                                            else
                                            {
                                                Process.Start(Settings.Default.PathuTorrent.Replace("-console", "d"));
                                                Thread.Sleep(1500);
                                                deluge.open(ongoing, title, filename);
                                            }
                                        }
                                        done.Add(title);
                                        Thread.Sleep(200);
                                    }
                                }
                                else if (title.Contains("Dragon Ball Super") && title.Contains("[DragonTeam]"))
                                {
                                    if (!Torrents.Contains(title + ".torrent") && !done.Contains(title) &&
                                         !last.Contains(title))
                                    {
                                        Dispatcher.BeginInvoke(
                                           new Action(
                                               delegate
                                               {
                                                   createListboxItem(title, ongoing[filename] + @"\" + title, true);
                                                   showBalloon("New Anime", "Downloading\n" + title);
                                                   Settings.Default.Listbox.Add(title + "[]" + ongoing[filename] + @"\" +
                                                       title + "[]" + "true" + "\n");
                                                   Settings.Default.Save();
                                               }));
                                        //download file and add it to torrent downloader
                                        client.DownloadFile(new Uri(link),
                                            PathDownloads + @"\" + title + @".torrent");
                                        if (Settings.Default.PathuTorrent.ToLower().Contains("utorrent") &&
                                            processes.Contains("uTorrent"))
                                        {
                                            if (processes.Contains("uTorrent"))
                                            {
                                                uTorrent.open(ongoing, title, filename);
                                            }
                                            else
                                            {
                                                Process.Start(Settings.Default.PathuTorrent);
                                                Thread.Sleep(1500);
                                                uTorrent.open(ongoing, title, filename);
                                            }
                                        }
                                        else if (Settings.Default.PathuTorrent.ToLower().Contains("deluge-console.exe"))
                                        {
                                            if (processes.Contains("deluged"))
                                            {
                                                deluge.open(ongoing, title, filename);
                                            }
                                            else
                                            {
                                                Process.Start(Settings.Default.PathuTorrent.Replace("-console", "d"));
                                                Thread.Sleep(1500);
                                                deluge.open(ongoing, title, filename);
                                            }
                                        }
                                        done.Add(title);
                                        Thread.Sleep(200);
                                    }
                                }
                            }
                        }
                        Settings.Default.RefreshCounter++;
                        Settings.Default.Save();
                        client.Dispose();
                    }
                    Settings.Default.RefreshTimer = Settings.Default.RefreshWaitTime;
                }
            }
        }

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
            notifyIcon.Dispose();
            Close();
        }

        private void MiniBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        public void createListboxItem(string content, string tag, bool isenabled)
        {
            var itmheader = new ListBoxItem();
            itmheader.Tag = PathOngoing + @"\" + tag;
            itmheader.Content = content;
            if (!isenabled)
                itmheader.Foreground = ReadColorFg;
            //Settings.Default.Listbox += content + "[]" + PathOngoing + @"\" + tag + "[]" + isenabled + "\n";

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
            var dirs = new List<string>(Directory.EnumerateDirectories(PathOngoing));
            //List<string> last = new List<string>();
            foreach (var dir in dirs)
            {
                var foldername = dir.Substring(dir.LastIndexOf("\\") + 1);

                var diranimefiles = new List<string>(Directory.EnumerateFiles(dir));
                foreach (var f in diranimefiles)
                {
                    var nameFile = f.Split(new[] {"\\"}, StringSplitOptions.None).Last();
                    var filn = nameFile.Split(new[] {"-"}, StringSplitOptions.None)[0];
                    if (!f.Contains(".sync") && !ongoing.Keys.Contains(filn))
                        //listBox.Items.Add(dir.Substring(dir.LastIndexOf("\\") + 1));
                        ongoing[filn] = foldername;
                }
                foreach (var file in diranimefiles)
                {
                    if (file.Contains(".mkv"))
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
            }
        }

        private void uTorrentTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Settings.Default.PathuTorrent = uTorrentTextBox.Text;
            Settings.Default.Save();
            PathuTorrent = uTorrentTextBox.Text;
        }

        private void OnGoingTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Settings.Default.PathOngoing = OnGoingTextBox.Text;
            Settings.Default.Save();
            PathOngoing = OnGoingTextBox.Text;
        }

        private void TorrentsFolder_TextChanged(object sender, TextChangedEventArgs e)
        {
            Settings.Default.PathDownloads = TorrentsFolder.Text;
            Settings.Default.Save();
            PathDownloads = TorrentsFolder.Text;
        }

        private void RefreshTimebox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (RefreshTimebox.Text != "" && int.Parse(RefreshTimebox.Text) > 9)
            {
                Settings.Default.RefreshWaitTime = int.Parse(RefreshTimebox.Text);
                Settings.Default.Save();
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
            }
            catch (Exception a)
            {
                var itemselected = ((ListBoxItem)listBox.SelectedItem);
                MessageBox.Show(itemselected.Tag+" " +a.Message);
            }
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            jsonFile["Resolution"] =((ComboBoxItem)comboBox.SelectedItem).Content.ToString();
            File.WriteAllText(Filepath, jsonFile.ToString());
        }

        private void SaveGroupsBtn_Click(object sender, RoutedEventArgs e)
        {
            var groups = new List<string>();
            groups.AddRange(GroupsTextBox.Text.Split(new[] {", "}, StringSplitOptions.None));
            jsonFile["Groups"] = JToken.FromObject(groups);
            File.WriteAllText(Filepath, jsonFile.ToString());
        }
    }
}