using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using Anime_Downloader.ViewModels;

namespace Anime_Downloader {
    internal class Nyaase {
        public static void Get_feed_titles(string url) {
            var t = new Thread(new ThreadStart(delegate {
                foreach (var nyaaseRssViewModel in GetItems(url)) {
                    if (!MainWindowViewModel._animeRssInternal.Contains(nyaaseRssViewModel))
                        Global.AnimeRssAdd = nyaaseRssViewModel;
                }
            })) {IsBackground = true};
            t.Start();
        }

        public static string GetStatus(string summary) {
            if (summary.ToLower().Contains("remake")) {
                return "remake";
            }
            if (summary.ToLower().Contains("a+ - trusted")) {
                return "a+ - trusted";
            }
            if (summary.ToLower().Contains("trusted")) {
                return "trusted";
            }
            return "normal";
        }

        public static List<NyaaseRssViewModel> GetItems(string url) {
            var items = new List<NyaaseRssViewModel>();
            string xml;
            using (var webClient = new WebClient()) {
                xml = Encoding.UTF8.GetString(webClient.DownloadData(url));
            }
            //xml = xml.Replace("pubDate", "datee");
            var bytes = Encoding.ASCII.GetBytes(xml);
            var reader = XmlReader.Create(new MemoryStream(bytes));
            var feed = SyndicationFeed.Load(reader);
            // "0 seeder(s), 1 leecher(s), 17 download(s) - 281.3 MiB - Remake"
            foreach (var mangs in feed.Items) {
                var summary = Regex.Match(mangs.Summary.Text,
                    @"(\d+) seeder\(s\), (\d+) leecher\(s\), (\d+) download\(s\) - (.+\s[a-z]iB)",
                    RegexOptions.IgnoreCase);
                var item = new NyaaseRssViewModel {
                    Name = mangs.Title.Text,
                    Link = mangs.Links[0].Uri.AbsoluteUri,
                    Seeder = summary.Groups[1].Value,
                    Leecher = summary.Groups[2].Value,
                    Downloads = summary.Groups[3].Value,
                    Size = summary.Groups[4].Value,
                    Color = GetStatus(mangs.Summary.Text)
                };
                if (!items.Contains(item)) {
                    items.Add(item);
                }
            }
            return items;
        }
    }
}    