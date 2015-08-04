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
using Anime_Downloader.Handlers;
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
        private readonly SolidColorBrush ReadColorFg = new SolidColorBrush(Color.FromArgb(255, 140, 140, 140));
        public readonly NotifyIcon notifyIcon = new NotifyIcon();
        private readonly System.Windows.Forms.ContextMenu contextMenu1 = new System.Windows.Forms.ContextMenu();

        private readonly System.Windows.Forms.MenuItem ExitMenuItem = new System.Windows.Forms.MenuItem();
        private readonly System.Windows.Forms.MenuItem ShowmenuItem = new System.Windows.Forms.MenuItem();
        private readonly System.Windows.Forms.MenuItem ForcemenuItem = new System.Windows.Forms.MenuItem();

        private readonly string Filepath = "AnimeDownloader.json";
        private JObject jsonFile = JObject.Parse(File.ReadAllText("AnimeDownloader.json"));
        private int timer = 5;

        //private JObject jsonFile;

        public MainWindow()
        {
            InitializeComponent();

            ThreadStart childref = CheckNow;
            var childThread = new Thread(childref);
            childThread.IsBackground = true;
            childThread.Start();

            FillProcessList();
            notifyIcon.ContextMenu = contextMenu1;
            notifyIcon.ContextMenu.MenuItems.Add(ExitMenuItem);
            notifyIcon.Icon = Properties.Resources.testicon;
            notifyIcon.DoubleClick += ShowMenuItem_Click;
            notifyIcon.Visible = true;
            showBalloon("Anime Downloader", "Starting...");
            AddMenuItems();
            foreach (var item in Settings.Default.Listbox)
            {
                DebugTextbox.Text += item;
            }
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

            OngoingFolder = jsonFile["Ongoing_Folder"].ToString();
            TorrentFiles = jsonFile["Torrent_Files"].ToString();
            TorrentClient = jsonFile["Torrent_Client"].ToString();
            Res = jsonFile["Resolution"].ToString();
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
            notifyIcon.Dispose();
            Close();
        }
        private void ShowMenuItem_Click(object Sender, EventArgs e)
        {
            // Close the form, which closes the application. 
            WindowState = WindowState.Normal;
        }

        private void ForceMenuItem_Click(object Sender, EventArgs e)
        {
            // Close the form, which closes the application. 
            timer = 1;
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
                if (timer >= 1)
                {
                    if (File.Exists(TorrentClient) &&
                        Directory.Exists(OngoingFolder) &&
                        Directory.Exists(TorrentFiles))
                    {
                        Settings.Default.StatusLabel = "Status: Checking in " + timer +
                                                       " seconds.";
                        timer--;
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
                    Settings.Default.StatusLabel = "Status: Checking in " + timer + " seconds.";
                    Torrents = new List<string>(Directory.EnumerateFiles(TorrentFiles));
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
                                                        createListboxItem(title, ongoing[filename] + @"\" + title, true);
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
                                            client.DownloadFile(new Uri(link), TorrentFiles + @"\" + title + @".torrent");
                                            deluge.open(ongoing, title, filename, TorrentFiles, OngoingFolder, TorrentClient);
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
                                            done.Add(title);
                                            Thread.Sleep(300);
                                            break;
                                    }
                                    case "nope":
                                        break;
                                }
                            }
                        }
                        timer++;
                        client.Dispose();
                    }
                    timer = int.Parse(jsonFile["Refresh_Time"].ToString());
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

        private void SaveAllBtn_Click(object sender, RoutedEventArgs e)
        {
            var groups = new List<string>();
            foreach (var s in GroupsTextBox.Text.Split(new[] { ", " }, StringSplitOptions.None))
            {
                if (s.Length == 4)
                {
                    groups.Add(s);
                }
            }
            GroupsTextBox.Text = string.Join(", ", groups);
            jsonFile["Groups"] = JToken.FromObject(groups);
            jsonFile["Resolution"] =((ComboBoxItem)comboBox.SelectedItem).Content.ToString();

            jsonFile["Torrent_Client"] = TorrentClientTextBox.Text;
            jsonFile["Torrent_Files"] = TorrentFilesTextBox.Text;
            jsonFile["Ongoing_Folder"] = OnGoingFolderTextBox.Text;
            jsonFile["Refresh_Time"] = RefreshTimebox.Text;

            File.WriteAllText(Filepath, jsonFile.ToString());

            Res = jsonFile["Resolution"].ToString();
            TorrentClient = TorrentClientTextBox.Text;
            TorrentFiles = TorrentFilesTextBox.Text;
            OngoingFolder = OnGoingFolderTextBox.Text;
            

        }
    }
}