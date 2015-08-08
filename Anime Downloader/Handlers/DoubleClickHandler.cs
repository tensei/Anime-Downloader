using System.Diagnostics;
using System.Windows.Controls;

namespace Anime_Downloader.Handlers
{
    class DoubleClickHandler
    {
        public bool Open(ListBoxItem item)
        {
            if (item.Tag.Equals("blank")) return false;
            Process.Start(item.Tag.ToString());
            return true;
        }
    }
}
