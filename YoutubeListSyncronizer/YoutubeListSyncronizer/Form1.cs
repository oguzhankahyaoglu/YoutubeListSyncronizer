using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
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
            cbmMaxRes.Items.AddRange(MaxResolutions.Cast<object>().ToArray());
            cbmMaxRes.SelectedIndex = 2;
        }

        private YoutubeListDownloadWorker listWorker;
        private void btnFetchPlaylist_Click(object sender, EventArgs e)
        {
            btnFetchPlaylist.Enabled = false;
            var playlistId = ParsePlaylistId(txtPlaylist.Text);
            listWorker = new YoutubeListDownloadWorker(playlistId);
            progressBar.Show();
            listWorker.ProgressChanged += (o, args) =>
                                              {
                                                  progressBar.Value = Math.Min(100, args.ProgressPercentage);
                                              };
            listWorker.RunWorkerCompleted += (o, args) =>
                                             {
                                                 MessageBox.Show("Total videos in this list:" + listWorker.TotalVideoCount);
                                                 btnDownload.Enabled = true;
                                                 listView.Items.Clear();
                                                 var index = 1;

                                                 var orderedDic = listWorker.VideoIDsDictionary.Reverse();
                                                 foreach (var kvp in orderedDic)
                                                 {
                                                     var item = new ListViewItem(new[] { index.ToString("D4"), kvp.Key, kvp.Value, "" });
                                                     listView.Items.Add(item);
                                                     index++;
                                                 }
                                                 progressBar.Hide();
                                             };
            listWorker.RunWorkerAsync();
        }

        #region Helpers

        public static NameValueCollection ParseQueryString(string s)
        {
            var nvc = new NameValueCollection();

            // remove anything other than query string from url
            if (s.Contains("?"))
            {
                s = s.Substring(s.IndexOf('?') + 1);
            }

            foreach (string vp in Regex.Split(s, "&"))
            {
                string[] singlePair = Regex.Split(vp, "=");
                if (singlePair.Length == 2)
                {
                    nvc.Add(singlePair[0], singlePair[1]);
                }
                else
                {
                    // only one key with no value specified in query string
                    nvc.Add(singlePair[0], string.Empty);
                }
            }

            return nvc;
        }

        #endregion

        private string ParsePlaylistId(string link)
        {
            var qscoll = ParseQueryString(link);
            return qscoll["list"];
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            folderBrowser.ShowDialog();
            var videoFolder = folderBrowser.SelectedPath;
            if (videoFolder.IsNullOrEmptyString())
                return;
            btnDownload.Enabled = btnFetchPlaylist.Enabled = false;
            StartDownloading(videoFolder);
        }

        private static readonly int[] MaxResolutions = new[] { 1080, 720, 480, 360 };

        private int[] ProgressArr;
        private int CountOfVideos;
        private void StartDownloading(string videoFolder)
        {
            var index = 0;
            var completed = 0;
            var videoIDsDictionary = listWorker.VideoIDsDictionary.Reverse();
            CountOfVideos = videoIDsDictionary.Count();
            progressBar.Value = 0;
            progressBar.Show();
            ProgressArr = new int[CountOfVideos];
            foreach (var kvp in videoIDsDictionary)
            {
                var videoID = kvp.Key;
                var url = "http://www.youtube.com/watch?v=" + videoID;
                var innerWorker = new YoutubeDownloadBackgroundWorker(videoFolder, url, index, MaxResolutions[cbmMaxRes.SelectedIndex]);
                innerWorker.ProgressChanged += (o, args) =>
                                                   {
                                                       var itemIndex = args.UserState.ToInt();
                                                       var text = "%" + args.ProgressPercentage;
                                                       ProgressArr[itemIndex] = args.ProgressPercentage;
                                                       if (args.ProgressPercentage == 100)
                                                       {
                                                           if (innerWorker.IsSuccessful)
                                                               text = innerWorker.IsAlreadyExists ? "Already exists." : "Completed!";
                                                           else
                                                               text = "Failed to find resolution: " + (innerWorker.Exception != null ? innerWorker.Exception.Message : "");
                                                       }
                                                       listView.Items[itemIndex].SubItems[3].Text = text;
                                                       progressBar.Value = Math.Min(100, Convert.ToInt32(ProgressArr.Sum() / (CountOfVideos * 1.0)));
                                                   };
                innerWorker.RunWorkerCompleted += (o, args) =>
                                                      {
                                                          completed++;
                                                          var _index = args.UserState.ToNullableInt();
                                                          if (_index != null)
                                                              listView.Items[_index.Value].SubItems[3].Text = "Complete!";
                                                          if (completed >= CountOfVideos)
                                                              MessageBox.Show("Completed Syncronization!", "Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                      };
                innerWorker.RunWorkerAsync();
                index++;
            }
        }


        ToolTip mTooltip = new ToolTip();
        Point mLastPos = new Point(-1, -1);
        private void listView_MouseMove(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo info = listView.HitTest(e.X, e.Y);
            if (mLastPos != e.Location)
            {
                if (info.Item != null && info.SubItem != null)
                {
                    mTooltip.ToolTipTitle = info.Item.Text;
                    mTooltip.Show(info.SubItem.Text, info.Item.ListView, e.X, e.Y, 20000);
                }
                else
                {
                    mTooltip.SetToolTip(listView, string.Empty);
                }
            }
            mLastPos = e.Location;
        }
    }
}
