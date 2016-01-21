using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Anime_Downloader.Handlers
{
    internal class SaveOnExit
    {
        private static readonly string DirName =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Anime Downloader");

        private static readonly string fileName =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Anime Downloader",
                "animeditems.json");

        public static void Checkforfile()
        {
            if (!Directory.Exists(DirName))
            {
                Directory.CreateDirectory(DirName);
            }

            if (!File.Exists(fileName))
            {
                var js = new JObject();
                js["items"] = JToken.FromObject(new List<string>());
                File.WriteAllText(fileName, js.ToString());
            }
        }

        public static void Saveitems(List<string> items)
        {
            Checkforfile();
            var js = JObject.Parse(File.ReadAllText(fileName));
            var newlist = new List<string>();
            foreach (var item in items)
            {
                newlist.Add(item);
            }
            js["items"] = JToken.FromObject(newlist);
            File.WriteAllText(fileName, js.ToString());
        }
    }
}