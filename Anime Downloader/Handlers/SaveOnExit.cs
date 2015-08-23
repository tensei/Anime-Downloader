using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Anime_Downloader.Handlers
{
    class SaveOnExit
    {
        private string DirName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Anime Downloader");
        private string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Anime Downloader", "animeditems.json");

        public void Checkforfile()
        {
            if (!Directory.Exists(DirName))
            {
                Directory.CreateDirectory(DirName);
            }

            if (!File.Exists(fileName))
            {
                JObject js = new JObject();
                js["items"] = JToken.FromObject(new List<string>());
                File.WriteAllText(fileName, js.ToString());
            }
        }

        public void Saveitems(List<string> items)
        {
            Checkforfile();
            JObject js = JObject.Parse(File.ReadAllText(fileName));
            var newlist = new List<string>();
            foreach (var item in items)
            {
                newlist.Add(item);
            }
            js["items"] = JToken.FromObject(newlist);
            File.WriteAllText(fileName ,js.ToString());
        }
    }
}
