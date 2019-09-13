using System;
using System.Collections.Generic;
using System.ComponentModel;
using Swagger.YTAPI.Model;

namespace YoutubeListSyncronizer
{
    public class YTUserVideoLinksWorker : BackgroundWorker
    {
       public YoutubeUserVideos Model { get; set; }

        public String Username;

        public YTUserVideoLinksWorker(String username)
        {
            WorkerSupportsCancellation = true;
            WorkerReportsProgress = true;
            Username = username;
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            try
            {
                DownloadPlaylistItems();
                ReportProgress(100);
            }
            catch (Exception ex)
            {
                throw new Exception("YT listesi çekilirken hata oluştu.", ex);
            }
        }

        private void DownloadPlaylistItems()
        {
            var api = new Swagger.YTAPI.Api.UserVideosApi();
            Model = api.Get(Username);
//            var youtubeService = new YouTubeService(new BaseClientService.Initializer
//            {
//                ApplicationName = "YoutubeListSyncronizer",
//                ApiKey = "AIzaSyDgUR4esr5twkPl5jRwGlx6yPGR8e6zBPs"
//            });
//
//            //fetch playlist name
//            {
//                var request = youtubeService.Channels.List("contentDetails");
//                request.ForUsername = Username;
//                var response = request.Execute();
//                try
//                {
//                    PlaylistName = response.Items.ElementAtOrDefault(0)?.ContentDetails?.RelatedPlaylists?.Uploads;
//                    if(PlaylistName == null)
//                        throw new Exception(Username + " kullanıcısının yüklenen videoları bulunamadı.");
//                }
//                catch (Exception ex)
//                {
//                    if (Debugger.IsAttached)
//                        throw;
//                    PlaylistName = "Youtube";
//                }
//            }
//            ReportProgress(30);
//
//            var nextPageToken = "";
//            while (nextPageToken != null)
//            {
//                var request = youtubeService.PlaylistItems.List("snippet");
//                request.PlaylistId = PlaylistName;
//                request.MaxResults = 20;
//                request.PageToken = nextPageToken;
//
//                var response = request.Execute();
//
//                foreach (var playlistItem in response.Items)
//                {
//                    var videoId = playlistItem.Snippet.ResourceId.VideoId;
//                    if (VideoIDsDictionary.ContainsKey(videoId))
//                        continue;
//                    var title = playlistItem.Snippet.Title;
//                    VideoIDsDictionary.Add(videoId, title);
//                }
//
//                nextPageToken = response.NextPageToken;
//            }
        }
    }

}
