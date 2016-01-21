using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using Anime_Downloader.Properties;
using Newtonsoft.Json.Linq;
using MessageBox = System.Windows.MessageBox;
using UserControl = System.Windows.Controls.UserControl;

namespace Anime_Downloader
{
    /// <summary>
    ///     Interaktionslogik für SettingsDialog.xaml
    /// </summary>
    public partial class SettingsDialog : UserControl
    {
        private readonly string Filepath = "AnimeDownloader.json";
        private readonly JObject jsonFile;

        public SettingsDialog()
        {
            InitializeComponent();

            jsonFile = JObject.Parse(File.ReadAllText(Filepath));
            foreach (var child in jsonFile["Groups"].Children())
            {
                GroupsTextBox.Text += child + ", ";
            }
            comboBox.Text = jsonFile["Resolution"].ToString();
            //groups.AddRange();
            //groups.Remove("");
            TorrentClientTextBox.Text = jsonFile["Torrent_Client"].ToString();
            TorrentFilesTextBox.Text = jsonFile["Torrent_Files"].ToString();
            OnGoingFolderTextBox.Text = jsonFile["Ongoing_Folder"].ToString();
            RefreshTimebox.Text = jsonFile["Refresh_Time"].ToString();
            RSSFeedbox.Text = jsonFile["RSS"].ToString();
        }

        public string OpenFolderDialog()
        {
            using (var f = new FolderBrowserDialog())
            {
                if (f.ShowDialog() == DialogResult.OK)
                {
                    return f.SelectedPath;
                }
                return "null";
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void TorrentClientTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var path = OpenFolderDialog();
            if (Directory.Exists(path))
            {
                var x = new List<string>(Directory.EnumerateFiles(path));
                if (x.Contains(Path.Combine(path, "deluge-console.exe")))
                {
                    TorrentClientTextBox.Text = Path.Combine(path, "deluge-console.exe");
                }
                else if (x.Contains(Path.Combine(path, "utorrent.exe")))
                {
                    TorrentClientTextBox.Text = Path.Combine(path, "utorrent.exe");
                }
                else if (x.Contains(Path.Combine(path, "uTorrent.exe")))
                {
                    TorrentClientTextBox.Text = Path.Combine(path, "uTorrent.exe");
                }
                else
                {
                    MessageBox.Show(
                        $"Couldn't find deluge-console.exe or utorrent.exe.\nIs this the correct Path?\n\n {path}",
                        "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OnGoingFolderTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var result = OpenFolderDialog();
            if (!result.Equals("null"))
            {
                OnGoingFolderTextBox.Text = result;
            }
        }

        private void TorrentFilesTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var result = OpenFolderDialog();
            if (!result.Equals("null"))
            {
                TorrentFilesTextBox.Text = result;
            }
        }

        private void SaveAllBtn_Click(object sender, RoutedEventArgs e)
        {
            var list =
                GroupsTextBox.Text.Split(new[] {", "}, StringSplitOptions.None).Where(s => s.Length == 4).ToList();

            GroupsTextBox.Text = string.Join(", ", list);
            jsonFile["Groups"] = JToken.FromObject(list);
            jsonFile["Resolution"] = ((ComboBoxItem) comboBox.SelectedItem).Content.ToString();

            jsonFile["Torrent_Client"] = TorrentClientTextBox.Text;
            jsonFile["Torrent_Files"] = TorrentFilesTextBox.Text;
            jsonFile["Ongoing_Folder"] = OnGoingFolderTextBox.Text;
            jsonFile["Refresh_Time"] = RefreshTimebox.Text;
            jsonFile["RSS"] = RSSFeedbox.Text;
            File.WriteAllText(Filepath, jsonFile.ToString());

            Settings.Default.RSS = RSSFeedbox.Text;

            Global.TorrentFiles = TorrentFilesTextBox.Text;
            Global.OngoingFolder = OnGoingFolderTextBox.Text;
            Global.TorrentClient = TorrentClientTextBox.Text;
            Global.Res = ((ComboBoxItem) comboBox.SelectedItem).Content.ToString();
            Global.jsonFile = jsonFile;
        }
    }
}