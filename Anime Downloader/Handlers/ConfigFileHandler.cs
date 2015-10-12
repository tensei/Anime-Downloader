using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Anime_Downloader
{
    internal class ConfigFileHandler
    {
        private readonly string path = "AnimeDownloader.json";

        public void CheckFile()
        {
            if (!File.Exists(path))
            {
                CreateFile();

            }
        }

        private void CreateFile()
        {
            //if (!Directory.Exists("Settings"))
            //{
            //    Directory.CreateDirectory("Settings");
            //}
            var file = new JObject();
            file["RSS"] = "http://www.nyaa.se/?page=rss&cats=1_37";
            file["Groups"] = JToken.FromObject(new List<string> {"[FFF", "[Ani", "[Viv", "[Ase", "[JnM", "[Com"});
            file["Exceptions"] = JToken.FromObject(new List<string>());
            file["Resolution"] = "720p";

            file["Torrent_Client"] = "";
            file["Torrent_Files"] = "";
            file["Ongoing_Folder"] = "";
            file["Refresh_Time"] = "300";

            File.WriteAllText(path, file.ToString());
        }
    }
}