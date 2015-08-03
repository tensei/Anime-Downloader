using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace Anime_Downloader
{
    class DoubleClickHandler
    {
        private readonly SolidColorBrush ReadColorFg = new SolidColorBrush(Color.FromArgb(255, 140, 140, 140));
        public void Open(ListBoxItem item)
        {
            if (!item.Tag.Equals("blank"))
                Process.Start(item.Tag.ToString());
            item.Foreground = ReadColorFg;
        }
    }
}
