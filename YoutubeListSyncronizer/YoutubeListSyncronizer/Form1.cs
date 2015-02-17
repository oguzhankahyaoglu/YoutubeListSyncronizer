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
                                                     ToggleCheckedVideos();
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
                var title = item.SubItems[2].Text;
                var isSelected = item.Checked;
                ParsedVideos[i] = new YTVideoDownloader.ParsedVideo
                                        {
                                            VideoID = videoID,
                                            Title= title,
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
            listView.BackColor = Color.LightGray;
            btnDownload.Enabled = btnFetchPlaylist.Enabled = btnCheckAll.Enabled = false;
            IsListViewReadOnly = true;
            StartDownloading(videoFolder);
        }

        private static readonly int[] MaxResolutions = new[] { 2160, 1080, 720, 480, 360 };

        private void StartDownloading(string videoFolder)
        {
            progressBar.Value = 0;
            progressBar.Show();
            var maxResolution = MaxResolutions[cbmMaxRes.SelectedIndex];
            var args = new YTVideoDownloader.Args
            {
                MaxRes = maxResolution,
                ParsedVideos = ParsedVideos,
                VideoFolder = videoFolder
            };
            StartDownloadThread(args);
        }

        private Thread YTDownloadThread;
        private void StartDownloadThread(YTVideoDownloader.Args x)
        {
            YTDownloadThread = new Thread(o =>
            {
                var args = (YTVideoDownloader.Args)o;
                var length = args.ParsedVideos.Length;
                YTVideoDownloader.StatusArr = new YTVideoDownloader.Status[length];
                for (int i = 0; i < length; i++)
                {
                    var video = args.ParsedVideos[i];
                    var url = "http://www.youtube.com/watch?v=" + video.VideoID;
                    
                    var isSelected = video.IsSelected;
                    var downloader = new YTVideoDownloader(args.VideoFolder, url, video.Title, i, args.MaxRes, isSelected);
                    downloader.Start();
                }
            });
            YTDownloadThread.Start(x);
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

        private void ToggleCheckedVideos()
        {
            for (int i = 0; i < listView.Items.Count; i++)
            {
                listView.Items[i].Checked = !listView.Items[i].Checked;
            }
        }

        private void btnCheckAll_Click(object sender, EventArgs e)
        {
            ToggleCheckedVideos();
        }

        private void timerDownloader_Tick(object sender, EventArgs e)
        {
            var countOfSelectedVideos = ParsedVideos.Count(v => v.IsSelected);
            var countOfVideos = listView.Items.Count;
            var lastDownloaded = YTVideoDownloader.StatusArr.LastOrDefault(s => s != null && s.Progress == 100);
            var lastCompletedDownloadIndex = lastDownloaded == null ? -1 : lastDownloaded.Index;
            for (int i = 0; i < countOfVideos; i++)
            {
                //Do not update already completed elements again and again
                var status = YTVideoDownloader.StatusArr[i];
                if (status == null)
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
                    }
                    else
                    {
                        text = status.Progress.ToString() + "%";
                        this.Text = "{0} / {1} : {2}% - Syncronizing...".FormatString(i+1, countOfVideos, status.Progress);
                    }
                }
                listView.Items[i].SubItems[3].Text = text;
            }
            listView.Update();
            progressBar.Value = Math.Min(100,
                Convert.ToInt32(YTVideoDownloader.StatusArr.Where(s => s != null && s.IsSelected).Sum(s => s.Progress) / (countOfSelectedVideos * 1.0)));
            Debug.WriteLine("****************************************************************************");
            Debug.WriteLine("Total Progress: " + progressBar.Value);
            for (int index = 0; index < YTVideoDownloader.StatusArr.Length; index++)
                if (YTVideoDownloader.StatusArr[index] != null && YTVideoDownloader.StatusArr[index].IsSelected)
                    Debug.WriteLine(index + ": " + YTVideoDownloader.StatusArr[index]);
            Debug.WriteLine("****************************************************************************");
            if (lastCompletedDownloadIndex >= YTVideoDownloader.StatusArr.Length - 1)
            {
                timerDownloader.Stop();
                this.Text = "Syncronization Complete!";
                MessageBox.Show("Completed Syncronization!", "Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                listView.BackColor = Color.White;
                btnFetchPlaylist.Enabled = btnDownload.Enabled = btnCheckAll.Enabled = true;
                IsListViewReadOnly = false;
            }
            else
            {
                //var downloadActiveIndex = YTVideoDownloader.StatusArr.Last(s => s != null && s.Progress < 100).Index;
                listView.Items[Math.Min(lastCompletedDownloadIndex + 2, listView.Items.Count - 1)].EnsureVisible();
            }
        }

        private bool IsListViewReadOnly = false;
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                YTDownloadThread.Abort();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.GetExceptionString());
            }
        }

        private void listView_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (IsListViewReadOnly)
                e.NewValue = e.CurrentValue;
        }

        private void listView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            e.Item.BackColor = e.Item.Checked ? Color.Wheat : Color.White;
        }


    }
}
