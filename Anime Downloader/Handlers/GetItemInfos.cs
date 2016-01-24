using System.Collections.Generic;
using Anime_Downloader.ViewModels;

namespace Anime_Downloader.Handlers {
    public class GetItemInfos {
        public static List<string> ConverttostringList(List<object> items) {
            var itemlist = new List<string>();

            foreach (AnimeViewModel animeViewModel in items) {
                var item = animeViewModel.Name + "[]" + animeViewModel.Tag + "[]" + animeViewModel.Added;
                if (animeViewModel.Status != "Watched") {
                    itemlist.Add(item);
                }
            }
            return itemlist;
        }
    }
}