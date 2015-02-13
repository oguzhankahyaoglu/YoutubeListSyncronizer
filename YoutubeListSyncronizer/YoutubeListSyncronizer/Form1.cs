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
            cbmMaxRes.SelectedIndex = 0;
            folderBrowser.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos, Environment.SpecialFolderOption.DoNotVerify);
        }

        private YTVideoDownloader.ParsedVideo[] ParsedVideos;
        private void btnFetchPlaylist_Click(object sender, EventArgs e)
        {
            btnFetchPlaylist.Enabled = false;
            var playlistId = ParsePlaylistId(txtPlaylist.Text);
            if (playlistId != null)
            {
                var listWorker = new YTListDownloadWorker(playlistId);
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
                                                     UpdateSelectedVideosArray();
                                                 };
                listWorker.RunWorkerAsync();
            }
            else
            {
                var youtubeVideoID = ParseVideoID(txtPlaylist.Text);
                if (youtubeVideoID == null)
                {
                    MessageBox.Show("'{0}' is neither a Youtube Playlist nor a Youtube Video Link! Try links in other formats or report if you think this is a bug.", "Url Wrong Format", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                var item = new ListViewItem(new[] { 1.ToString("D4"), youtubeVideoID, txtPlaylist.Text, "" });
                listView.Items.Add(item);
                btnDownload.Enabled = true;
                btnFetchPlaylist.Enabled = true;
                progressBar.Hide();
                UpdateSelectedVideosArray();
                MessageBox.Show("The url is a youtube video link, instead of a playlist. Single video will be downloaded.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void UpdateSelectedVideosArray()
        {
            var count = listView.Items.Count;
            ParsedVideos = new YTVideoDownloader.ParsedVideo[count];
            var items = listView.Items.Cast<ListViewItem>().ToArray();
            for (var i = 0; i < items.Length; i++)
            {
                var item = items[i];
                var videoID = item.SubItems[1].Text;
                var videoURL = item.SubItems[2].Text;
                var isSelected = item.Checked;
                ParsedVideos[i] = new YTVideoDownloader.ParsedVideo
                                        {
                                            VideoID = videoID,
                                            VideoURL = videoURL,
                                            IsSelected = isSelected
                                        };
            }
        }

        #region Helpers

        public static bool HasWritePermissionOnDir(string path)
        {
            var writeAllow = false;
            var writeDeny = false;
            var accessControlList = Directory.GetAccessControl(path);
            var accessRules = accessControlList.GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));

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
            if (DownloadUrlResolver.TryNormalizeYoutubeUrl(url, out normalizedUrl))
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
            UpdateSelectedVideosArray();
            if (ParsedVideos.Count(v => v.IsSelected) <= 0)
            {
                MessageBox.Show("No videos selected to download. Select at least one video to download!", "No Video Selected!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
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
            StartDownloading(videoFolder);
        }

        private static readonly int[] MaxResolutions = new[] { 1080, 720, 480, 360 };

        private int LastCompletedDownloadIndex;
        private void StartDownloading(string videoFolder)
        {
            progressBar.Value = 0;
            progressBar.Show();
            LastCompletedDownloadIndex = -1;
            var maxResolution = MaxResolutions[cbmMaxRes.SelectedIndex];
            var args = new YTVideoDownloader.Args
            {
                MaxRes = maxResolution,
                ParsedVideos = ParsedVideos,
                VideoFolder = videoFolder
            };
            StartDownloadThread(args);
        }

        private void StartDownloadThread(YTVideoDownloader.Args x)
        {
            var thread = new Thread(o =>
            {
                var args = (YTVideoDownloader.Args)o;
                var length = args.ParsedVideos.Length;
                YTVideoDownloader.StatusArr = new YTVideoDownloader.Status[length];
                for (int i = 0; i < length; i++)
                {
                    var video = args.ParsedVideos[i];
                    var url = "http://www.youtube.com/watch?v=" + video.VideoID;
                    var isSelected = video.IsSelected;
                    var downloader = new YTVideoDownloader(args.VideoFolder, url, i, args.MaxRes, isSelected);
                    downloader.Start();
                }
            });
            thread.Start(x);
            timerDownloader.Start();
        }

        #region Tooltip

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

        #endregion

        private void btnCheckAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listView.Items.Count; i++)
            {
                listView.Items[i].Checked = !listView.Items[i].Checked;
            }
        }

        private void timerDownloader_Tick(object sender, EventArgs e)
        {
            var countOfSelectedVideos = ParsedVideos.Count(v => v.IsSelected);
            for (int i = 0; i < listView.Items.Count; i++)
            {
                //Do not update already completed elements again and again
                var status = YTVideoDownloader.StatusArr[i];
                if (status == null || i <= LastCompletedDownloadIndex)
                    continue;

                var text = "";
                if (status.IsSelected)
                {
                    if (status.Progress == 100)
                    {
                        if (status.IsSuccessful)
                            text = status.IsAlreadyExists ? "Already exists." : "Completed!";
                        else
                            text = "Failed to find resolution: " + status.Exception.GetExceptionString();
                        LastCompletedDownloadIndex = i;
                    }
                    else
                        text = status.Progress.ToString() + "%";
                }
                listView.Items[i].SubItems[3].Text = text;
            }
            listView.Update();
            progressBar.Value = Math.Min(100, 
                Convert.ToInt32(YTVideoDownloader.StatusArr.Where(s=> s != null && s.IsSelected).Sum(s => s.Progress) / (countOfSelectedVideos * 1.0)));
            if (Debugger.IsAttached)
            {
                Debug.WriteLine("****************************************************************************");
                Debug.WriteLine("Total Progress: " + progressBar.Value);
                for (int index = 0; index < YTVideoDownloader.StatusArr.Length; index++)
                    if (YTVideoDownloader.StatusArr[index] != null && YTVideoDownloader.StatusArr[index].IsSelected)
                        Debug.WriteLine(index + ": " + YTVideoDownloader.StatusArr[index]);
                Debug.WriteLine("****************************************************************************");
            }
            var lastSelectedIndex = YTVideoDownloader.StatusArr.Last(s => s != null && s.IsSelected).Index;
            if (LastCompletedDownloadIndex >= lastSelectedIndex)
            {
                timerDownloader.Stop();
                MessageBox.Show("Completed Syncronization!", "Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnFetchPlaylist.Enabled = btnDownload.Enabled = true;
                listView.Enabled = true;
            }
            else
            {
                var downloadActiveIndex = YTVideoDownloader.StatusArr.Last(s => s != null && s.Progress < 100).Index;
                listView.Items[downloadActiveIndex].EnsureVisible();
            }
        }
    }
}
