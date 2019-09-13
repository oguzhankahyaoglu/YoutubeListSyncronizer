using System;
using System.Collections.Generic;
using System.ComponentModel;
using Swagger.YTAPI.Model;

namespace YoutubeListSyncronizer
{
    public class YTListDownloadWorker : BackgroundWorker
    {
        public YoutubePlaylist Model { get; set; }
        private String PlaylistID;

        public YTListDownloadWorker(String playlistID)
        {
            WorkerSupportsCancellation = true;
            WorkerReportsProgress = true;
            PlaylistID = playlistID;
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            try
            {
                ReportProgress(10);
                DownloadPlaylistItems(PlaylistID);
                ReportProgress(100);
            }
            catch (Exception ex)
            {
                throw new Exception("YT listesi çekilirken hata oluştu.", ex);
            }
        }

        private void DownloadPlaylistItems(string playlistId)
        {
            var api = new Swagger.YTAPI.Api.PlaylistApi();
            Model = api.Get(playlistId);
//            var youtubeService = new YouTubeService(new BaseClientService.Initializer
//            {
//                ApplicationName = "YoutubeListSyncronizer",
//                ApiKey = "AIzaSyDgUR4esr5twkPl5jRwGlx6yPGR8e6zBPs"
//            });
//
//            //fetch playlist name
//            {
//                var request = youtubeService.Playlists.List("snippet");
//                request.Id = PlaylistID;
//                var response = request.Execute();
//                try
//                {
//                    PlaylistName = response.Items[0].Snippet.Title;
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
//                request.PlaylistId = playlistId;
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
