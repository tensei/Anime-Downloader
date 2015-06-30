﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Anime_Downloader;
using Anime_Downloader.Properties;

namespace Anime_Downloader
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<string> last = new List<string>();
        public Dictionary<string, string> ongoing = new Dictionary<string, string>();
        public List<string> Torrents = new List<string>();
        nyaase neko = new nyaase();
        public List<string> done = new List<string>();
        public string PathOngoing = Settings.Default.PathOngoing;//@"D:\dummy\Ongoing";
        public string PathuTorrent = Settings.Default.PathuTorrent;//@"D:\dummy\uTorrent.exe";
        public string PathDownloads = Settings.Default.PathDownloads;// @"D:\Dummy\torrents";
        public List<string> groups = new List<string> { "[FFF", "[Ani", "[Viv", "[Ase", "[JnM", "[Com" };
        private WebClient client = new WebClient();
        uTorrent uTorrent = new uTorrent();
        
        public MainWindow()
        {
            InitializeComponent();
            if (Settings.Default.PathOngoing == "")
                Settings.Default.PathOngoing = @"D:\dummy\Ongoing";
                Settings.Default.Save();
            if (Settings.Default.PathDownloads == "")
                Settings.Default.PathDownloads = @"D:\dummy\torrents";
                Settings.Default.Save();
            if (Settings.Default.PathuTorrent == "")
                Settings.Default.PathuTorrent = @"D:\dummy\uTorrent.exe";
                Settings.Default.Save();
            //= 
            //PathDownloads = ;
            //PathuTorrent = ;
            uTorrentTextBox.Text = Settings.Default.PathuTorrent;
            TorrentsFolder.Text = Settings.Default.PathDownloads;
            OnGoingTextBox.Text = Settings.Default.PathOngoing;
            RefreshTimebox.Text = Settings.Default.RefreshWaitTime.ToString();
            Settings.Default.RefreshTimer = Settings.Default.RefreshWaitTime;
            //neko.Get_feed_titles();
            //GetOnGoing();
            ThreadStart childref = CheckNow;
            var childThread = new Thread(childref);
            childThread.IsBackground = true;
            childThread.Start();
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
                        Settings.Default.StatusLabel = "Status: Checking in " + Settings.Default.RefreshTimer +" seconds.";
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
                        MessageBox.Show(e.Message);
                    }
                    if (rssitems.Count > 1)
                    {
                        GetOnGoing();
                        foreach (var item in rssitems)
                        {
                            var itemSplit = item.Split(new[] { "[]" }, StringSplitOptions.None);
                            var title = itemSplit[0];
                            var link = itemSplit[1];
                            foreach (var filename in ongoing.Keys)
                            {
                                filename.Replace("Anime Koi", "Anime-Koi");
                                if (title.Contains(filename) && title.Contains("720") && !last.Contains(title) &&
                                    !title.ToLower().Contains("batch"))
                                {
                                    if (!Torrents.Contains(PathDownloads + @"\" + title + @".torrent") && !done.Contains(title))
                                    {
                                        Dispatcher.BeginInvoke(new Action(delegate()
                                        {
                                            createListboxItem(title, ongoing[filename]+@"\"+ title , true); }));
                                        //download file and add it to torrent downloader
                                        client.DownloadFile(new Uri(link), PathDownloads + @"\" + title + @".torrent");
                                        uTorrent.open(ongoing, title, filename);
                                        done.Add(title);
                                        Thread.Sleep(200);
                                    }
                                }
                                else if (title.Contains(filename) && groups.Contains(title.Substring(0, 4)) && !last.Contains(title) &&
                                         !title.ToLower().Contains("batch"))
                                {
                                    if (!Torrents.Contains(title + ".torrent") && !done.Contains(title))
                                    {
                                        Dispatcher.BeginInvoke(new Action(delegate ()
                                        {
                                            createListboxItem(title, ongoing[filename] + @"\" + title, true);
                                        }));
                                        //download file and add it to torrent downloader
                                        client.DownloadFile(new Uri(link), PathDownloads + @"\" + title + @".torrent");
                                        uTorrent.open(ongoing, title, filename);
                                        done.Add(title);
                                        Thread.Sleep(200);
                                    }
                                }
                            }
                        }
                        Settings.Default.RefreshCounter++;
                        Settings.Default.Save();
                    }
                    Settings.Default.RefreshTimer = Settings.Default.RefreshWaitTime;
                }
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.Save();
            Close();
        }

        private void MiniBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        public void createListboxItem(string content, string tag, bool isenabled)
        {
            ListBoxItem itmheader = new ListBoxItem();
            itmheader.Tag = PathOngoing+@"\"+tag;
            itmheader.Content = content;
            itmheader.IsEnabled = isenabled;
            listBox.Items.Add(itmheader);
        }
        private void GetOnGoing()
        {
            last.Clear();
            ongoing.Clear();
            //Dictionary<string, string> ongoing = new Dictionary<string, string>();
            List<string> dirs = new List<string>(Directory.EnumerateDirectories(PathOngoing));
            //List<string> last = new List<string>();
            foreach (var dir in dirs)
            {
                var foldername = dir.Substring(dir.LastIndexOf("\\") + 1);

                List<string> diranimefiles = new List<string>(Directory.EnumerateFiles(dir));
                var nameFile = diranimefiles[0].Split(new[] { "\\" }, StringSplitOptions.None);
                if(!nameFile.Contains(".sync"))
                    //listBox.Items.Add(dir.Substring(dir.LastIndexOf("\\") + 1));
                    ongoing[nameFile.Last().Split(new[] { "-" }, StringSplitOptions.None)[0]] = foldername;
                foreach (var file in diranimefiles)
                {
                    if(file.Contains(".mkv"))
                        last.Add(file.Split(new[] { "\\" }, StringSplitOptions.None).Last());
                }
            }
        }

        private void SettingBtn_Click(object sender, RoutedEventArgs e)
        {
            if (listBox.Visibility == Visibility.Collapsed)
            {
                listBox.Visibility = Visibility.Visible;
            }
            else
            {
                listBox.Visibility = Visibility.Collapsed;
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
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void listBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var itemselected = ((ListBoxItem)listBox.SelectedItem);
                Process.Start(itemselected.Tag.ToString());
            }
            catch (Exception a)
            {
                MessageBox.Show(a.Message);
            }
        }
    }
}