using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using Color = System.Drawing.Color;

namespace Anime_Downloader.Handlers
{
    class GetItemInfos
    {
        public List<string> ConverttostringList(List<object> items)
        {
            List<string> itemlist = new List<string>();

            foreach (var listBoxItem in items)
            {
                var item = ((ListBoxItem) listBoxItem).Content + "[]" + ((ListBoxItem) listBoxItem).Tag;
                if (((ListBoxItem) listBoxItem).Foreground !=
                    new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 140, 140, 140)))
                {
                    itemlist.Add(item);
                }
            }
            return itemlist;
        }
    }
}
