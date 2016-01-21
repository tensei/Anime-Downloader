using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Anime_Downloader.Handlers
{
    internal class Getitems
    {
        private readonly string fileName =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Anime Downloader",
                "animeditems.json");

        public List<string> get()
        {
            SaveOnExit.Checkforfile();
            var jObject = JObject.Parse(File.ReadAllText(fileName));
            var items = new List<string>();

            foreach (var child in jObject["items"].Children())
            {
                items.Add(child.ToString());
            }
            return items;
        }
    }
}