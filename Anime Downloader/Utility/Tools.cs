using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Anime_Downloader.Handlers;
using Anime_Downloader.Properties;
using Anime_Downloader.ViewModels;
using Application = System.Windows.Application;

namespace Anime_Downloader.Utility
{
    internal class Tools
    {
        public static readonly NotifyIcon notifyIcon = new NotifyIcon();
        private static readonly ContextMenu contextMenu1 = new ContextMenu();

        private static readonly MenuItem ExitMenuItem = new MenuItem();
        private static readonly MenuItem ShowmenuItem = new MenuItem();
        private static readonly MenuItem ForcemenuItem = new MenuItem();

        public static Dictionary<string, string> GetOnGoing()
        {
            var last = new List<string>();
            var ongoing = new Dictionary<string, string>();
            //Dictionary<string, string> ongoing = new Dictionary<string, string>();
            var dirs = new List<string>(Directory.EnumerateDirectories(Global.OngoingFolder));
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
                        //DataGridAnime.Items.Add(dir.Substring(dir.LastIndexOf("\\") + 1));
                        ongoing[filn] = foldername;
                }
                foreach (var file in diranimefiles)
                {
                    if (file.EndsWith(".mkv") || file.EndsWith(".mp4"))
                        last.Add(file.Split(new[] {"\\"}, StringSplitOptions.None).Last());
                }
            }
            return ongoing;
        }

        public static List<string> FillProcessList()
        {
            var processes = new List<string>();
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
            return processes;
        }

        public static void CreateDataGridItem(string content, string tag, bool isenabled)
        {
            var itmheader = new AnimeViewModel
            {
                Tag = Global.OngoingFolder + @"\" + tag,
                Name = content,
                Added = DateTime.Now
            };
            if (!isenabled)
                itmheader.Status = "Watched";
            //Settings.Default.Listbox += content + "[]" + OngoingFolder + @"\" + tag + "[]" + isenabled + "\n";

            //LstItems.Add(itmheader);
            //Settings.Default.ListboxItems.Insert(0, itmheader);
            //Settings.Default.Save();
            Global.AnimeAdd = itmheader; // (itmheader);
        }

        public static void Savelist()
        {
            var listboxitem = new List<object>();
            foreach (var item in Global.Anime)
            {
                if (!item.Status.ToLower().Equals("watched"))
                {
                    listboxitem.Add(item);
                }
            }
            var items = GetItemInfos.ConverttostringList(listboxitem);
            SaveOnExit.Saveitems(items);
        }

        public static void PopulateListbox()
        {
            var getitems = new Getitems();
            foreach (var item in getitems.get())
            {
                var iteminfo = item.Split(new[] {"[]"}, StringSplitOptions.None);
                var dataGridAnimeItem = new AnimeViewModel();
                if (iteminfo.Length == 2)
                {
                    dataGridAnimeItem = new AnimeViewModel
                    {
                        Name = iteminfo[0],
                        Tag = iteminfo[1],
                        Status = "Not Watched"
                    };
                }
                else
                {
                    dataGridAnimeItem = new AnimeViewModel
                    {
                        Name = iteminfo[0],
                        Tag = iteminfo[1],
                        Added = DateTime.Parse(iteminfo[2]),
                        Status = "Not Watched"
                    };
                }
                Global.AnimeAdd = dataGridAnimeItem;
            }
        }

        public static void SetupNotifyIcon()
        {
            notifyIcon.ContextMenu = contextMenu1;
            notifyIcon.ContextMenu.MenuItems.Add(ExitMenuItem);
            notifyIcon.Icon = Resources.testicon;
            notifyIcon.DoubleClick += ShowMenuItem_Click;
            notifyIcon.Visible = true;
            showBalloon("Anime Downloader", "Starting...");
            AddMenuItems();
        }

        private static void AddMenuItems()
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

        public static void showBalloon(string title, string body)
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

        private static void ExitMenuItemClick(object Sender, EventArgs e)
        {
            // Close the form, which closes the application. 
            Savelist();
            notifyIcon.Dispose();
            Application.Current.MainWindow.Close();
        }

        private static void ShowMenuItem_Click(object Sender, EventArgs e)
        {
            // Close the form, which closes the application. 
            Application.Current.MainWindow.WindowState = WindowState.Normal;
            Application.Current.MainWindow.Topmost = true;
            Application.Current.MainWindow.Topmost = false;
        }

        private static void ForceMenuItem_Click(object Sender, EventArgs e)
        {
            // Close the form, which closes the application. 
            Global.Timer = 1;
        }
    }
}