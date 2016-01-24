using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using Anime_Downloader.Utility;
using MessageBox = System.Windows.MessageBox;
using UserControl = System.Windows.Controls.UserControl;

namespace Anime_Downloader {
    /// <summary>
    ///     Interaktionslogik für SettingsDialog.xaml
    /// </summary>
    public partial class SettingsDialog : UserControl {
        public SettingsDialog() {
            InitializeComponent();
            GroupsComboBox.Items.Clear();
            foreach (var group in Global.Groups) {
                GroupsComboBox.Items.Add(group);
            }
            if (Global.Groups.Count > 0)
                GroupsComboBox.Text = Global.Groups[0];
            comboBox.Text = Global.Resolution;
            TorrentClientTextBox.Text = Global.TorrentClient;
            TorrentFilesTextBox.Text = Global.TorrentFiles;
            OnGoingFolderTextBox.Text = Global.OngoingFolder;
            RefreshTimebox.Text = Global.RefreshTime.ToString();
            RSSFeedbox.Text = Global.Rss;
            GroupsComboBox.Text = Global.Groups[0];
        }

        public string OpenFolderDialog() {
            using (var f = new FolderBrowserDialog()) {
                if (f.ShowDialog() == DialogResult.OK) {
                    return f.SelectedPath;
                }
                return "null";
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e) {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void TorrentClientTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            var path = OpenFolderDialog();
            if (Directory.Exists(path)) {
                var x = new List<string>(Directory.EnumerateFiles(path));
                if (x.Contains(Path.Combine(path, "deluge-console.exe"))) {
                    TorrentClientTextBox.Text = Path.Combine(path, "deluge-console.exe");
                }
                else if (x.Contains(Path.Combine(path, "utorrent.exe"))) {
                    TorrentClientTextBox.Text = Path.Combine(path, "utorrent.exe");
                }
                else if (x.Contains(Path.Combine(path, "uTorrent.exe"))) {
                    TorrentClientTextBox.Text = Path.Combine(path, "uTorrent.exe");
                }
                else {
                    MessageBox.Show(
                        $"Couldn't find deluge-console.exe or utorrent.exe.\nIs this the correct Path?\n\n {path}",
                        "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OnGoingFolderTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            var result = OpenFolderDialog();
            if (!result.Equals("null")) {
                OnGoingFolderTextBox.Text = result;
            }
        }

        private void TorrentFilesTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            var result = OpenFolderDialog();
            if (!result.Equals("null")) {
                TorrentFilesTextBox.Text = result;
            }
        }

        private void SaveAllBtn_Click(object sender, RoutedEventArgs e) {
            Tools.SaveSettings();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Tools.StartupInit(); // reset everything
        }

        private void TorrentClientTextBox_TextChanged(object sender, TextChangedEventArgs e) {
            Global.TorrentClient = TorrentClientTextBox.Text;
        }

        private void TorrentFilesTextBox_TextChanged(object sender, TextChangedEventArgs e) {
            Global.TorrentFiles = TorrentFilesTextBox.Text;
        }

        private void OnGoingFolderTextBox_TextChanged(object sender, TextChangedEventArgs e) {
            Global.OngoingFolder = OnGoingFolderTextBox.Text;
        }

        private void RefreshTimebox_TextChanged(object sender, TextChangedEventArgs e) {
            Global.RefreshTime = int.Parse(RefreshTimebox.Text);
        }

        private void AddGrpButton_Click(object sender, RoutedEventArgs e) {
            var gn = GroupsComboBox.Text;
            if (gn.Length.Equals(4) && !Global.Groups.Contains(gn) && gn.StartsWith("[")) {
                Global.Groups.Add(gn);
            }
        }

        private void DeleteGrpButton_Click(object sender, RoutedEventArgs e) {
            var gn = GroupsComboBox.Text;
            if (Global.Groups.Contains(gn)) {
                Global.Groups.Remove(gn);
                GroupsComboBox.Items.Clear();
                foreach (var group in Global.Groups) {
                    GroupsComboBox.Items.Add(group);
                }
                if (Global.Groups.Count > 0)
                    GroupsComboBox.Text = Global.Groups[0];
            }
        }

        private void GroupsComboBox_DropDownOpened(object sender, EventArgs e) {
            GroupsComboBox.Items.Clear();
            foreach (var group in Global.Groups) {
                GroupsComboBox.Items.Add(group);
            }
            if (Global.Groups.Count > 0)
                GroupsComboBox.Text = Global.Groups[0];
        }
    }
}