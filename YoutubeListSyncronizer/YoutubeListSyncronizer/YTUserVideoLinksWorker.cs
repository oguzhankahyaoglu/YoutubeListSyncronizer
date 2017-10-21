using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Kahia.Common.Extensions.ConversionExtensions;
using Kahia.Common.Extensions.StringExtensions;

namespace YoutubeListSyncronizer
{
    public class YTUserVideoLinksWorker : BackgroundWorker
    {
        public Dictionary<string, string> VideoIDsDictionary { get; private set; }
        public String PlaylistName { get; private set; }
        public int TotalVideoCount { get; private set; }

        public String Username;

        public YTUserVideoLinksWorker(String username)
        {
            WorkerSupportsCancellation = true;
            WorkerReportsProgress = true;
            VideoIDsDictionary = new Dictionary<string, string>();
            Username = username;
            TotalVideoCount = 1;
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            try
            {
                DownloadPlaylistItems();
                TotalVideoCount = VideoIDsDictionary.Count;
                ReportProgress(100);
            }
            catch (Exception ex)
            {
                throw new Exception("YT listesi çekilirken hata oluştu.", ex);
            }
        }

        private void DownloadPlaylistItems()
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer
            {
                ApplicationName = "YoutubeListSyncronizer",
                ApiKey = "AIzaSyDgUR4esr5twkPl5jRwGlx6yPGR8e6zBPs"
            });

            //fetch playlist name
            {
                var request = youtubeService.Channels.List("contentDetails");
                request.ForUsername = Username;
                var response = request.Execute();
                try
                {
                    PlaylistName = response.Items.ElementAtOrDefault(0)?.ContentDetails?.RelatedPlaylists?.Uploads;
                    if(PlaylistName == null)
                        throw new Exception(Username + " kullanıcısının yüklenen videoları bulunamadı.");
                }
                catch (Exception ex)
                {
                    if (Debugger.IsAttached)
                        throw;
                    PlaylistName = "Youtube";
                }
            }
            ReportProgress(30);

            var nextPageToken = "";
            while (nextPageToken != null)
            {
                var request = youtubeService.PlaylistItems.List("snippet");
                request.PlaylistId = PlaylistName;
                request.MaxResults = 20;
                request.PageToken = nextPageToken;

                var response = request.Execute();

                foreach (var playlistItem in response.Items)
                {
                    var videoId = playlistItem.Snippet.ResourceId.VideoId;
                    if (VideoIDsDictionary.ContainsKey(videoId))
                        continue;
                    var title = playlistItem.Snippet.Title;
                    VideoIDsDictionary.Add(videoId, title);
                }

                nextPageToken = response.NextPageToken;
            }
        }
    }

}
