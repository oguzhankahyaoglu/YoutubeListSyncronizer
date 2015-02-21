using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Kahia.Common.Extensions.ConversionExtensions;
using Kahia.Common.Extensions.StringExtensions;

namespace YoutubeListSyncronizer
{
    public class YTListDownloadWorker : BackgroundWorker
    {
        public Dictionary<string, string> VideoIDsDictionary { get; private set; }
        public int TotalVideoCount { get; private set; }

        private String PlaylistID;
        public YTListDownloadWorker(String playlistID)
        {
            WorkerSupportsCancellation = true;
            WorkerReportsProgress = true;
            VideoIDsDictionary = new Dictionary<string, string>();
            PlaylistID = playlistID;
            TotalVideoCount = 1;
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            var startIndex = 1;
            var pageSize = 0;
            while (TotalVideoCount > VideoIDsDictionary.Count)
            {
                var xmlStr = DownloadPlaylistXml(startIndex);
                TotalVideoCount = ParsePlaylistXml(xmlStr, out pageSize);
                if (pageSize == 0)
                    break;
                startIndex += pageSize;
                ReportProgress(Convert.ToInt32((startIndex * 100.0 / TotalVideoCount)));
            }
            ReportProgress(100);
        }

        #region Helpers

        private int ParsePlaylistXml(string xmlStr, out int pageSize)
        {
            //var xDoc = XDocument.Parse(xmlStr, LoadOptions.PreserveWhitespace);
            XDocument xDoc;
            using (var xmlStream = new StringReader(xmlStr))
            using (var xmlReader = new XmlTextReader(xmlStream))
            {
                xDoc = XDocument.Load(xmlReader, LoadOptions.PreserveWhitespace);
            }
            var rootElem = xDoc.Elements().First();
            var entries = rootElem.Elements(XName.Get("entry", "http://www.w3.org/2005/Atom"));
            foreach (var entry in entries)
            {
                var mediaGroupElem = entry.Element(XName.Get("group", "http://search.yahoo.com/mrss/"));
                var videoIdElem = mediaGroupElem.Element(XName.Get("videoid", "http://gdata.youtube.com/schemas/2007"));
                var videoId = videoIdElem.Value;

                var mediaTitleElem = mediaGroupElem.Element(XName.Get("title", "http://search.yahoo.com/mrss/"));
                var title = mediaTitleElem.Value;
                if (!VideoIDsDictionary.ContainsKey(videoId))
                    VideoIDsDictionary.Add(videoId, title);
            }
            var totalVideoCount = rootElem.Element(XName.Get("totalResults", "http://a9.com/-/spec/opensearch/1.1/")).Value.ToNullableInt(0);
            pageSize = entries.Count();
            return totalVideoCount;
        }

        private string DownloadPlaylistXml(int startIndex)
        {
            const string urlFormat = @"https://gdata.youtube.com/feeds/api/playlists/{0}?v=2&max-results=20&start-index={1}";
            string xmlStr;
            using (var client = new WebClientExtended())
            {
                client.Encoding = Encoding.UTF8;
                xmlStr = client.DownloadString(urlFormat.FormatString(PlaylistID, startIndex));
            }
            return xmlStr;
        }

        #endregion
    }
}
