using System.Diagnostics;
using System.Windows.Controls;
using Anime_Downloader.ViewModels;

namespace Anime_Downloader.Handlers
{
    class DoubleClickHandler
    {
        public bool Open(AnimeViewModel item)
        {
            if (item.Tag.Equals("blank")) return false;
            Process.Start(item.Tag);
            return true;
        }
    }
}
