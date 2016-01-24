using System.Collections.Generic;
using Anime_Downloader.ViewModels;

namespace Anime_Downloader.Utility {
    public class RssUtility {
        public static List<NyaaseRssViewModel> GetFeedTask(string item) {
            switch (item) {
                case "Show all": {
                    var feed = Nyaase.Get_feed_titles("http://www.nyaa.se/?page=rss&cats=1_37");
                    return feed;
                }
                case "Trusted only": {
                    var feed = Nyaase.Get_feed_titles("http://www.nyaa.se/?page=rss&cats=1_37&filter=2");
                    return feed;
                }
                case "Filter remakes": {
                    var feed = Nyaase.Get_feed_titles("http://www.nyaa.se/?page=rss&cats=1_37&filter=1");
                    return feed;
                }
                case "A+ only": {
                    var feed = Nyaase.Get_feed_titles("http://www.nyaa.se/?page=rss&cats=1_37&filter=3");
                    return feed;
                }
                default: {
                    break;
                }
            }
            return null;
        }

        public static List<NyaaseRssViewModel> SearchGetFeed(string url) {
            var feed = Nyaase.Get_feed_titles(url);
            return feed;
        }
    }
}