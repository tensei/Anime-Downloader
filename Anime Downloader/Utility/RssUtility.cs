using System.Collections.Generic;
using Anime_Downloader.ViewModels;

namespace Anime_Downloader.Utility {
    public class RssUtility {
        public static void GetFeedTask(string item) {
            switch (item) {
                case "Show all": {
                    Nyaase.Get_feed_titles("http://www.nyaa.se/?page=rss&cats=1_37");
                    return;
                }
                case "Trusted only": {
                    Nyaase.Get_feed_titles("http://www.nyaa.se/?page=rss&cats=1_37&filter=2");
                    return ;
                }
                case "Filter remakes": {
                    Nyaase.Get_feed_titles("http://www.nyaa.se/?page=rss&cats=1_37&filter=1");
                    return;
                }
                case "A+ only": {
                    Nyaase.Get_feed_titles("http://www.nyaa.se/?page=rss&cats=1_37&filter=3");
                    return;
                }
                default: {
                    break;
                }
            }
        }

        public static void SearchGetFeed(string url) {
            Nyaase.Get_feed_titles(url);
        }
    }
}