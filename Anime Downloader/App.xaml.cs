using System.Windows;
using Anime_Downloader.Utility;

namespace Anime_Downloader {
    /// <summary>
    ///     Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application {
        private void AppStartup(object sender, StartupEventArgs args) {
            var mainWindow = new MainWindow {
                DataContext = new MainWindowViewModel()
            };
            Tools.StartupInit();
            mainWindow.Show();
        }
    }
}