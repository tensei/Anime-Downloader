using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Anime_Downloader
{
    internal class nyaase
    {
        public List<string> Get_feed_titles()
        {
            List<string> titleurl = new List<string>();
            string xml;
            string url = "http://www.nyaa.se/?page=rss&cats=1_37";
            using (var webClient = new WebClient())
            {
                xml = Encoding.UTF8.GetString(webClient.DownloadData(url));
            }
            //xml = xml.Replace("pubDate", "datee");
            var bytes = Encoding.ASCII.GetBytes(xml);
            var reader = XmlReader.Create(new MemoryStream(bytes));
            var feed = SyndicationFeed.Load(reader);
            foreach (var mangs in feed.Items)
            {
                titleurl.Add(mangs.Title.Text+ "[]" + mangs.Links[0].Uri.AbsoluteUri);
            }
            return titleurl;
        }
    }
}