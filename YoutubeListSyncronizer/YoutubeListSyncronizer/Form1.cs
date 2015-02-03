using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Kahia.Common.Extensions.ConversionExtensions;
using Kahia.Common.Extensions.StringExtensions;
using YoutubeListSyncronizer.Library;

namespace YoutubeListSyncronizer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Dictionary<string, string> VideoIDsDictionary;
        private void btnFetchPlaylist_Click(object sender, EventArgs e)
        {
            String xmlStr = DownloadPlaylistXml();
            VideoIDsDictionary = new Dictionary<string, string>();
            var videoCount = PrepareVideoIDs(xmlStr, VideoIDsDictionary);
            MessageBox.Show("Total videos in this list:" + videoCount);
            btnDownload.Enabled = true;
            listView.Items.Clear();
            var index = 1;
            foreach (var kvp in VideoIDsDictionary)
            {
                var item = new ListViewItem(new[] { index.ToString(), kvp.Key, kvp.Value, "" });
                listView.Items.Add(item);
                index++;
            }
        }

        #region Helpers

        private int PrepareVideoIDs(string xmlStr, Dictionary<string, string> videoIDs)
        {
            var xDoc = XDocument.Parse(xmlStr);
            var rootElem = xDoc.Elements().First();
            var entries = rootElem.Elements(XName.Get("entry", "http://www.w3.org/2005/Atom"));
            foreach (var entry in entries)
            {
                var mediaGroupElem = entry.Element(XName.Get("group", "http://search.yahoo.com/mrss/"));
                var videoIdElem = mediaGroupElem.Element(XName.Get("videoid", "http://gdata.youtube.com/schemas/2007"));
                var videoId = videoIdElem.Value;

                var mediaTitleElem = mediaGroupElem.Element(XName.Get("title", "http://search.yahoo.com/mrss/"));
                var title = mediaTitleElem.Value;
                videoIDs.Add(videoId, title);
            }
            var totalVideoCount = rootElem.Element(XName.Get("totalResults", "http://a9.com/-/spec/opensearch/1.1/")).Value.ToNullableInt(0);
            return totalVideoCount;
        }

        private string DownloadPlaylistXml()
        {
            const string urlFormat = @"https://gdata.youtube.com/feeds/api/playlists/{0}?v=2&max-results=50&start-index={1}";
            string xmlStr;
            using (var client = new WebClient())
            {
                xmlStr = client.DownloadString(urlFormat.FormatString(txtPlaylist.Text, txtPageStart.Value));
            }
            return xmlStr;
        }

        #endregion

        private void btnDownload_Click(object sender, EventArgs e)
        {
            btnDownload.Enabled = btnFetchPlaylist.Enabled = false;
            folderBrowser.ShowDialog();
            var videoFolder = folderBrowser.SelectedPath;
            StartDownloading(videoFolder);
        }

        private void StartDownloading(string videoFolder)
        {
            var index = 0;
            var completed = 0;
            foreach (var kvp in VideoIDsDictionary)
            {
                var videoID = kvp.Key;
                var url = "http://www.youtube.com/watch?v=" + videoID;
                var innerWorker = new YoutubeDownloadBackgroundWorker(videoFolder, url, index);
                innerWorker.ProgressChanged += (o, args) =>
                                                   {
                                                       listView.Items[args.UserState.ToInt()].SubItems[3].Text = "%" + args.ProgressPercentage;
                                                   };
                innerWorker.RunWorkerCompleted += (o, args) =>
                                                      {
                                                          completed++;
                                                          var _index = args.UserState.ToNullableInt();
                                                          if(_index!=null)
                                                            listView.Items[_index.Value].SubItems[3].Text = "Complete!";
                                                      };
                innerWorker.RunWorkerAsync();
                index++;
                listView.Invalidate();
                listView.Update();
                listView.Refresh();
                Application.DoEvents();
            }
        }
    }
}
