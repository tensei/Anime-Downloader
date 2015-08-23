using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Anime_Downloader.Handlers
{
    class Getitems
    {
        private string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Anime Downloader", "animeditems.json");
        SaveOnExit saveOnExit = new SaveOnExit();

        public List<string> get()
        {
            saveOnExit.Checkforfile();
            JObject jObject = JObject.Parse(File.ReadAllText(fileName));
            List<string> items = new List<string>();

            foreach (var child in jObject["items"].Children())
            {
                items.Add(child.ToString());
            }
            return items;
        } 
    }
}
