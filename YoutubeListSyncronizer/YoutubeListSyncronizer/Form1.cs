﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Kahia.Common.Extensions.GeneralExtensions;
using Kahia.Common.Extensions.StringExtensions;
using Microsoft.VisualBasic;

namespace YoutubeListSyncronizer
{
    public partial class Form1 : Form
    {
        private readonly SynchronizationContext synchronizationContext;
        public class ParsedVideo
        {
            public String VideoID { get; set; }
            public String Title { get; set; }
            public bool IsSelected { get; set; }
        }

        public class Status
        {
            public int Progress;
            public int Index;
            public ParsedVideo ParsedVideo;
            //public VideoInfo VideoInfo;
            public bool IsSuccessful { get; set; }
            public bool IsSelected { get; set; }
            public bool IsAlreadyExists { get; set; }
            public Exception Exception { get; set; }
            public String ExceptionMessage { get; set; }
            public override string ToString()
            {
                return "Progress: {0}, IsSelected: {4}, IsSuccess: {1}, IsAlreadyExist: {2}, Ex: {3}".FormatString(Progress, IsSuccessful, IsAlreadyExists, Exception.GetExceptionString(), IsSelected);
            }
        }

        public class Args
        {
            public ParsedVideo[] ParsedVideos;
            public int MaxRes;
            public String VideoFolder;
        }

        private String PlaylistUrl;
        private String VideoUrl;
        private String UserName;
        private YTListDownloadWorker ytlistDownloadWorker;
        private YTUserVideoLinksWorker yTUserVideoLinksWorker;
        private ParsedVideo[] ParsedVideos;

        public Form1()
        {
            InitializeComponent();
            synchronizationContext = SynchronizationContext.Current;
            cbmMaxRes.Items.AddRange(MaxResolutions.Cast<object>().ToArray());
            cbmMaxRes.SelectedIndex = 1;
            folderBrowser.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos, Environment.SpecialFolderOption.DoNotVerify);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            const string defaultUrl = "https://www.youtube.com/playlist?list=PLDZMiVQ0iUnCwGbMckmoupzrmTNRIo-Y0";
            String url = null, inputUrl;
            {
                //try to get url from clipboard first
                //var clipboardUrl = Clipboard.GetText().ToStringByDefaultValue();
                //if ((clipboardUrl.Contains("youtube") || clipboardUrl.Contains("tube")) && (clipboardUrl.Contains("http:") || clipboardUrl.Contains("https:")))
                //    url = clipboardUrl;
                //else
                inputUrl = Interaction.InputBox("Enter Youtube video/playlist/user link. Example links:\n\nVideo link: \nhttps://www.youtube.com/watch?v=KxgZR8epXio\nPlaylist link: \nhttps://www.youtube.com/playlist?list=PLmqdK5_Qu7DKBH722l2ab0rQbPxmpqk0k\nUser link:\nhttps://www.youtube.com/user/DarduinMyMenlon", "Link", defaultUrl);
            }

            if (DownloadUrlResolver.TryNormalizeYoutubeUrl(inputUrl, ref url))
            {
                VideoUrl = url;
                btnFetchPlaylist_Click(null, null);
            }
            else if (DownloadUrlResolver.TryNormalizeYoutubePlaylistUrl(inputUrl, ref url))
            {
                PlaylistUrl = url;
                btnFetchPlaylist_Click(null, null);
            }
            else if (DownloadUrlResolver.TryNormalizeYoutubeUserUrl(inputUrl, out UserName))
            {
                btnFetchPlaylist_Click(null, null);
            }
            else
            {
                MessageBox.Show(Resources.WarningInvalidUrl, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.ExitThread();
            }
        }

        private void btnFetchPlaylist_Click(object sender, EventArgs e)
        {
            listView.Items.Clear();
            this.AcceptButton = btnDownload;
            //btnFetchPlaylist.Enabled = false;
            var playlistId = ParsePlaylistId();
            if (playlistId != null)
            {
                ytlistDownloadWorker = new YTListDownloadWorker(playlistId);
                //progressBar.Show();
                ytlistDownloadWorker.ProgressChanged += (o, args) =>
                                                  {
                                                      //progressBar.Value = Math.Min(100, args.ProgressPercentage);
                                                  };
                ytlistDownloadWorker.RunWorkerCompleted += (o, args) =>
                                                 {
                                                     if (args.Error != null)
                                                     {
                                                         MessageBox.Show(args.Error.ConvertExceptionToString());
                                                         Visible = false;
                                                         Logger.Log(args.Error);
                                                         Application.Exit();
                                                         return;
                                                     }
                                                     MessageBox.Show(Resources.TotalVideosInThisList + ytlistDownloadWorker.Model.VideoIDsDictionary.Count);
                                                     btnDownload.Enabled = true;
                                                     listView.Items.Clear();
                                                     var index = 1;

                                                     var orderedDic = ytlistDownloadWorker.Model.VideoIDsDictionary.Reverse();
                                                     foreach (var kvp in orderedDic)
                                                     {
                                                         var item = new ListViewItem(new[] { index.ToString("D4"), kvp.Key, kvp.Value, "" });
                                                         listView.Items.Add(item);
                                                         index++;
                                                     }
                                                     //progressBar.Hide();
                                                     UpdateSelectedVideosArray();
                                                     listView.ToggleChecked();
                                                 };
                ytlistDownloadWorker.RunWorkerAsync();
            }
            else if (UserName != null)
            {
                yTUserVideoLinksWorker = new YTUserVideoLinksWorker(UserName);
                //progressBar.Show();
                yTUserVideoLinksWorker.ProgressChanged += (o, args) =>
                {
                    //progressBar.Value = Math.Min(100, args.ProgressPercentage);
                };
                yTUserVideoLinksWorker.RunWorkerCompleted += (o, args) =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.ConvertExceptionToString());
                        Visible = false;
                        Logger.Log(args.Error);
                        Application.Exit();
                        return;
                    }
                    MessageBox.Show(Resources.TotalVideosInThisList + yTUserVideoLinksWorker.Model.VideoIDsDictionary.Count);
                    btnDownload.Enabled = true;
                    listView.Items.Clear();
                    var index = 1;

                    var orderedDic = yTUserVideoLinksWorker.Model.VideoIDsDictionary.Reverse();
                    foreach (var kvp in orderedDic)
                    {
                        var item = new ListViewItem(new[] { index.ToString("D4"), kvp.Key, kvp.Value, "" });
                        listView.Items.Add(item);
                        index++;
                    }
                    //progressBar.Hide();
                    UpdateSelectedVideosArray();
                    listView.ToggleChecked();
                };
                yTUserVideoLinksWorker.RunWorkerAsync();

            }
            else
            {
                var youtubeVideoID = ParseVideoID();
                if (youtubeVideoID == null)
                {
                    MessageBox.Show(Resources.NotValidUrl, Resources.NotValidUrlTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                var item = new ListViewItem(new[] { 1.ToString("D4"), youtubeVideoID, PlaylistUrl, "" });
                listView.Items.Add(item);
                btnDownload.Enabled = true;
                //btnFetchPlaylist.Enabled = true;
                //progressBar.Hide();
                UpdateSelectedVideosArray();
                MessageBox.Show(Resources.WarningThisUrlIsAVideoLinkInsteadOfAPlaylist, Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Information);
                listView.ToggleChecked();
                btnDownload_Click(null, null);
            }
        }

        private void UpdateSelectedVideosArray()
        {
            var count = listView.Items.Count;
            ParsedVideos = new ParsedVideo[count];
            var items = listView.Items.Cast<ListViewItem>().ToArray();
            for (var i = 0; i < items.Length; i++)
            {
                var item = items[i];
                var videoID = item.SubItems[1].Text;
                var title = item.SubItems[2].Text;
                var isSelected = item.Checked;
                ParsedVideos[i] = new ParsedVideo { VideoID = videoID, Title = title, IsSelected = isSelected };
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

        private string ParseVideoID()
        {
            String normalizedUrl;
            if (VideoUrl.IsNotNullAndEmptyString())
            {
                var qscoll = ParseQueryString(VideoUrl);
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

        private string ParsePlaylistId()
        {
            if (PlaylistUrl.IsNullOrEmptyString())
                return null;
            var qscoll = ParseQueryString(PlaylistUrl);
            return qscoll["list"];
        }

        #endregion

        private void btnDownload_Click(object sender, EventArgs e)
        {
            UpdateSelectedVideosArray();
            if (ParsedVideos.Count(v => v.IsSelected) <= 0)
            {
                MessageBox.Show(Resources.WarningNoVideosSelectedToDownload, Resources.WarningNoVideoSelected, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (folderBrowser.ShowDialog() != DialogResult.OK)
            {
                MessageBox.Show(Resources.WarningNoDirectorySelected, Resources.WarningNoVideoSelected, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var videoFolder = folderBrowser.SelectedPath;
            if (videoFolder.IsNullOrEmptyString())
                return;
            if (ytlistDownloadWorker != null)
                videoFolder = Path.Combine(videoFolder, ytlistDownloadWorker.Model.PlaylistName);
            if (yTUserVideoLinksWorker != null)
                videoFolder = Path.Combine(videoFolder, yTUserVideoLinksWorker.Username);
            if (!Directory.Exists(videoFolder))
                Directory.CreateDirectory(videoFolder);
            if (!HasWritePermissionOnDir(videoFolder))
            {
                MessageBox.Show(Resources.WarningAccessDenied.FormatString(videoFolder), Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (MessageBox.Show(Resources.ConfirmAreYouSure.FormatString(videoFolder), Resources.Confirm, MessageBoxButtons.OKCancel) != DialogResult.OK)
                return;
            listView.BackColor = Color.LightGray;
            //btnDownload.Enabled = btnFetchPlaylist.Enabled = btnCheckAll.Enabled = flowShutdown.Enabled = false;
            btnDownload.Enabled = btnCheckAll.Enabled = false;
            IsListViewReadOnly = true;
            //if downloading a playlist, ytlistDownloadWorker will not be null and use subfolder
            StartDownloading(videoFolder);
        }

        private static readonly int[] MaxResolutions = new[] { 2160, 1080, 720, 480, 360 };

        private void StartDownloading(string videoFolder)
        {
            //progressBar.Value = 0;
            //progressBar.Show();
            var maxResolution = MaxResolutions[cbmMaxRes.SelectedIndex];
            var args = new Args { MaxRes = maxResolution, ParsedVideos = ParsedVideos, VideoFolder = videoFolder };
            var length = args.ParsedVideos.Length;
            var videoFolderInfo = new DirectoryInfo(args.VideoFolder);
            var freshDownloaded = new List<ParsedVideo>();
            for (var i = 0; i < length; i++)
            {
                Helpers.UpdateAndRedrawForm(this);
                var item = listView.Items[i];
                item.SubItems[3].Text = "Loading...";
                item.EnsureVisible();
                listView.Update();
                this.Text = "{0}/{1} Youtube Syncronizer".FormatString(i + 1, length);
                var video = args.ParsedVideos[i];
                var url = "http://www.youtube.com/watch?v=" + video.VideoID;
                //var isSelected = video.IsSelected;
                //var downloader = new YTVideoDownloader(args.VideoFolder, url, video.Title.ToStringByDefaultValue(video.VideoID), i, args.MaxRes, isSelected);
                if (!video.IsSelected)
                    continue;

                if (video.Title == "Deleted video")
                {
                    item.SubItems[3].Text = "Deleted video";
                    continue;
                }

                //check if file exists with a similar name
                var title = video.Title.ToCustomSeoFriendly();
                var hasSimilarFiles = videoFolderInfo.GetFiles().Any(f => f.Name.ToCustomSeoFriendly().Contains(title));
                if (hasSimilarFiles)
                {
                    item.SubItems[3].Text = "Already exists (file exists)";
                    continue;
                }

                var errors = YoutubeDownloadExe
                    .DownloadVideoAndReturnsErrors(i, url, args, async (data, index) =>
                                                                 {
                                                                     await Task.Run(() =>
                                                                     {
                                                                         synchronizationContext.Post(o =>
                                                                         {
                                                                             listView.Items[index].SubItems[3].Text = o+"";
                                                                             txtLogs.Text += o + Environment.NewLine;
                                                                             txtLogs.Update();
                                                                         }, data);
                                                                     });
                                                                    
                                                                 }, async (data, index) =>
                                                                 {
                                                                     await Task.Run(() =>
                                                                     {
                                                                         synchronizationContext.Post(o =>
                                                                         {
                                                                             txtErrors.Text += o + Environment.NewLine;
                                                                             txtErrors.Update();
                                                                         }, data);
                                                                     });
                                                                    
                                                                 });
                if (errors.IsNotNullAndEmptyString())
                {
                    item.SubItems[3].Text = errors.Contains("copyright") ? "[Copyright Error] " + errors : "[Error] " + errors;
                    continue;
                }
                item.SubItems[3].Text = "Downloaded.";
                freshDownloaded.Add(video);
            }
            this.WindowState = FormWindowState.Normal;
            MessageBox.Show("Complete! Downloaded {0} videos:\n{1}".FormatString(freshDownloaded.Count, freshDownloaded.Select(v => v.Title).JoinWith("\n")));
            btnDownload.Enabled = btnCheckAll.Enabled = true;
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
            listView.ToggleChecked();
        }

        //private void timerDownloader_Tick(object sender, EventArgs e)
        //{
        //    var countOfSelectedVideos = ParsedVideos.Count(v => v.IsSelected);
        //    var countOfVideos = listView.Items.Count;
        //    var lastDownloaded = YTVideoDownloader.StatusArr.LastOrDefault(s => s != null && s.Progress == 100);
        //    var lastCompletedDownloadIndex = lastDownloaded == null ? -1 : lastDownloaded.Index;
        //    for (int i = 0; i < countOfVideos; i++)
        //    {
        //        //Do not update already completed elements again and again
        //        var status = YTVideoDownloader.StatusArr[i];
        //        if (status == null)
        //            continue;

        //        var text = "";
        //        if (status.IsSelected)
        //        {
        //            if (status.Progress == 100)
        //            {
        //                if (status.IsSuccessful)
        //                    text = status.IsAlreadyExists ? Resources.StatusAlreadyExists : Resources.StatusCompleted;
        //                else
        //                {
        //                    if (Debugger.IsAttached)
        //                        text = status.Exception.GetExceptionString();
        //                    else
        //                    {
        //                        if (status.Exception is YoutubeBannedException)
        //                            text = status.ExceptionMessage.ToStringByDefaultValue(Resources.ExMessageBannedIP);
        //                        else
        //                            text = status.ExceptionMessage.ToStringByDefaultValue(Resources.ExMessage + status.Exception.GetExceptionString());
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                text = status.Progress + "%";
        //                this.Text = Resources.StatusSyncronizing.FormatString(i + 1, countOfVideos, status.Progress);
        //            }
        //        }
        //        listView.Items[i].SubItems[3].Text = text;
        //    }
        //    listView.Update();
        //    progressBar.Value = Math.Min(100,
        //        Convert.ToInt32(YTVideoDownloader.StatusArr.Where(s => s != null && s.IsSelected).Sum(s => s.Progress) / (countOfSelectedVideos * 1.0)));
        //    Debug.WriteLine("****************************************************************************");
        //    Debug.WriteLine("Total Progress: " + progressBar.Value);
        //    for (int index = 0; index < YTVideoDownloader.StatusArr.Length; index++)
        //        if (YTVideoDownloader.StatusArr[index] != null && YTVideoDownloader.StatusArr[index].IsSelected)
        //            Debug.WriteLine(index + ": " + YTVideoDownloader.StatusArr[index]);
        //    Debug.WriteLine("****************************************************************************");
        //    if (lastCompletedDownloadIndex >= YTVideoDownloader.StatusArr.Length - 1)
        //    {
        //        timerDownloader.Stop();
        //        this.Text = Resources.SyncComplete;
        //        MessageBox.Show(Resources.SyncComplete, Resources.SyncComplete, MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        listView.BackColor = Color.White;
        //        //btnFetchPlaylist.Enabled = btnDownload.Enabled = btnCheckAll.Enabled = true;
        //    }
        //    else
        //    {
        //        listView.Items[Math.Min(lastCompletedDownloadIndex + 2, listView.Items.Count - 1)].EnsureVisible();
        //    }
        //}

        private bool IsListViewReadOnly = false;

        private void listView_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (IsListViewReadOnly)
                e.NewValue = e.CurrentValue;
        }

        private void listView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            e.Item.BackColor = e.Item.Checked ? Color.GreenYellow : Color.White;
        }

        //private void btnOrderFileNames_Click(object sender, EventArgs e)
        //{
        //    if (folderBrowser.ShowDialog() != DialogResult.OK)
        //    {
        //        MessageBox.Show(Resources.WarningNoDirectorySelected, Resources.WarningNoVideoSelected, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }
        //    var videoFolder = folderBrowser.SelectedPath;
        //    if (videoFolder.IsNullOrEmptyString())
        //        return;
        //    var files = new DirectoryInfo(videoFolder).GetFiles("*.mp4", SearchOption.TopDirectoryOnly);
        //    var ordered = files.OrderBy(f => f.Name).ToArray();
        //    var i = 0;
        //    for (; i < ordered.Length; i++)
        //    {
        //        var file = ordered[i];
        //        //var name = file.Name.Split('-').Skip(1).JoinWith();
        //        var startDate = DateTime.Now.AddDays(-5000);
        //        file.CreationTime = startDate.AddDays(+ordered.Length - i);
        //        //file.MoveTo("{0}\\{1:D4}-{2}".FormatString(videoFolder, i, name));
        //    }
        //    MessageBox.Show("Complete! Ordered {0} files.".FormatString(i));
        //}
        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
        }
    }
}
