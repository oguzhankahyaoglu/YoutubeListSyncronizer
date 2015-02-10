using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Kahia.Common.Extensions.ConversionExtensions;
using Kahia.Common.Extensions.GeneralExtensions;
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
            cbmMaxRes.SelectedIndex = 1;
        }

        private YoutubeListDownloadWorker listWorker;
        private void btnFetchPlaylist_Click(object sender, EventArgs e)
        {
            btnFetchPlaylist.Enabled = false;
            var playlistId = ParsePlaylistId(txtPlaylist.Text);
            if (playlistId != null)
            {
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
            }else
            {
                var youtubeVideoID = ParseVideoID(txtPlaylist.Text);
                if(youtubeVideoID == null)
                {
                    MessageBox.Show("'{0}' is neither a Youtube Playlist nor a Youtube Video Link! Try links in other formats or report if you think this is a bug.","Url Wrong Format", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                var item = new ListViewItem(new[] { 1.ToString("D4"), youtubeVideoID, txtPlaylist.Text, "" });
                listView.Items.Add(item);
                btnDownload.Enabled = true;
                btnFetchPlaylist.Enabled = true;
                progressBar.Hide();
                MessageBox.Show("The url is a youtube video link, instead of a playlist. Single video will be downloaded.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #region Helpers

        public static bool HasWritePermissionOnDir(string path)
        {
            var writeAllow = false;
            var writeDeny = false;
            var accessControlList = Directory.GetAccessControl(path);
            if (accessControlList == null)
                return false;
            var accessRules = accessControlList.GetAccessRules(true, true,
                                        typeof(System.Security.Principal.SecurityIdentifier));
            if (accessRules == null)
                return false;

            foreach (FileSystemAccessRule rule in accessRules)
            {
                if ((FileSystemRights.Write & rule.FileSystemRights) != FileSystemRights.Write)
                    continue;

                if (rule.AccessControlType == AccessControlType.Allow)
                    writeAllow = true;
                else if (rule.AccessControlType == AccessControlType.Deny)
                    writeDeny = true;
            }

            return writeAllow && !writeDeny;
        }

        private string ParseVideoID(string url)
        {
            String normalizedUrl;
            if(DownloadUrlResolver.TryNormalizeYoutubeUrl(url, out normalizedUrl))
            {
                var qscoll = ParseQueryString(normalizedUrl);
                var ytid = qscoll["v"];
                return ytid;
            }
            return null;
        }

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

        private string ParsePlaylistId(string link)
        {
            var qscoll = ParseQueryString(link);
            return qscoll["list"];
        }

        #endregion

        private void btnDownload_Click(object sender, EventArgs e)
        {
            folderBrowser.ShowDialog();
            var videoFolder = folderBrowser.SelectedPath;
            if (videoFolder.IsNullOrEmptyString())
                return;
            if (!HasWritePermissionOnDir(videoFolder))
            {
                MessageBox.Show("The path '{0}' could not be accessedç Try to run this application as administrator, or select another path.".FormatString(videoFolder), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            btnDownload.Enabled = btnFetchPlaylist.Enabled = listView.Enabled = false;
            VideoDownloader.MaxActiveRequestCount = numericUpDown.Value.ToInt();
            Debug.WriteLine("VideoDownloader.MaxActiveRequestCount : " + VideoDownloader.MaxActiveRequestCount);
            StartDownloading(videoFolder);
        }

        private static readonly int[] MaxResolutions = new[] { 1080, 720, 480, 360 };

        private int[] ProgressArr;
        private int CountOfVideos;
        private void StartDownloading(string videoFolder)
        {
            var index = 0;
            var completed = 0;
            IEnumerable<KeyValuePair<string, string>> selectedDictionary;
            var videoIDsDictionary = PrepareVideoIDsDictionary(out selectedDictionary);
            CountOfVideos = selectedDictionary.Count();
            progressBar.Value = 0;
            progressBar.Show();
            ProgressArr = new int[videoIDsDictionary.Count()];
            foreach (var kvp in videoIDsDictionary)
            {
                if (!listView.Items[index].Checked)
                {
                    index++;
                    continue;
                }

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
                                                               text = "Failed to find resolution: " + innerWorker.Exception.GetExceptionString();
                                                       }
                                                       listView.Items[itemIndex].SubItems[3].Text = text;
                                                       listView.Items[itemIndex].EnsureVisible();
                                                       progressBar.Value = Math.Min(100, Convert.ToInt32(ProgressArr.Sum() / (CountOfVideos * 1.0)));
                                                   };
                innerWorker.RunWorkerCompleted += (o, args) =>
                                                      {
                                                          completed++;
                                                          var _index = args.UserState.ToNullableInt();
                                                          if (_index != null)
                                                          {
                                                              listView.Items[_index.Value].SubItems[3].Text = "Complete!";
                                                              listView.Items[_index.Value].EnsureVisible();
                                                          }
                                                          if (completed >= CountOfVideos)
                                                          {
                                                              MessageBox.Show("Completed Syncronization!", "Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                              btnFetchPlaylist.Enabled = true;
                                                              listView.Enabled = true;
                                                          }
                                                      };
                innerWorker.RunWorkerAsync();
                index++;
            }
        }

        private IEnumerable<KeyValuePair<string, string>> PrepareVideoIDsDictionary(out IEnumerable<KeyValuePair<string, string>> selectedKVPs)
        {
            var dictionary = listWorker != null ? listWorker.VideoIDsDictionary : new Dictionary<string, string> { { listView.Items[0].SubItems[1].Text, listView.Items[0].SubItems[2].Text } };
            selectedKVPs = dictionary.Where((kvp, i) => listView.Items[i].Checked);
            return dictionary;
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

        private void btnCheckAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listView.Items.Count; i++)
            {
                listView.Items[i].Checked = !listView.Items[i].Checked;
            }
        }
    }
}
