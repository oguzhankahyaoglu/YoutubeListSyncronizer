using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Kahia.Common.Extensions.GeneralExtensions;
using Kahia.Common.Extensions.StringExtensions;
using YoutubeListSyncronizer.Library;

namespace YoutubeListSyncronizer
{
    public class YTVideoDownloader
    {
        #region SubClasses

        public class ParsedVideo
        {
            public String VideoID { get; set; }
            public String VideoURL { get; set; }
            public bool IsSelected { get; set; }
        }

        public class Status
        {
            public int Progress;
            public int Index;
            public bool IsSuccessful { get; set; }
            public bool IsSelected { get; set; }
            public bool IsAlreadyExists { get; set; }
            public Exception Exception { get; set; }
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

        #endregion

        private String DownloadUrl;
        private String DownloadFolder;
        private int Index;
        private int MaxResolution;
        private bool IsSelectedVideo;

        public static Status[] StatusArr;

        public YTVideoDownloader(string downloadFolder, string downloadUrl, int index, int maxResolution, bool isSelected)
        {
            DownloadUrl = downloadUrl;
            DownloadFolder = downloadFolder;
            Index = index;
            MaxResolution = maxResolution;
            IsSelectedVideo = isSelected;
            StatusArr[Index] = new Status { IsSelected = isSelected, Index = index};
        }

        public void Start()
        {
            StatusArr[Index].IsAlreadyExists = false;
            if (!IsSelectedVideo)
            {
                StatusArr[Index].IsSuccessful = true;
                StatusArr[Index].Progress = 100;
                return;
            }
            try
            {
                var video = FindVideoInfo();
                if (video == null)
                {
                    StatusArr[Index].IsSuccessful = false;
                    StatusArr[Index].Progress = 100;
                    return;
                }
                if (video.RequiresDecryption)
                    DownloadUrlResolver.DecryptDownloadUrl(video);

                //var fileName = "{0:D4} - {1}_{3}p{2}".FormatString(Index, RemoveIllegalPathCharacters(video.Title).Left(100), video.VideoExtension, video.Resolution);
                //var fileName = "{0} - {1}_{3}p{2}".FormatString(video.VideoID, RemoveIllegalPathCharacters(video.Title).Left(100), video.VideoExtension, video.Resolution);
                var fileName = "{1}_{3}p{2}".FormatString(video.VideoID, RemoveIllegalPathCharacters(video.Title).Left(100), video.VideoExtension, video.Resolution);
                var downloadPath = Path.Combine(DownloadFolder, fileName);
                if (!File.Exists(downloadPath))
                {
                    var videoDownloader = new VideoDownloader(video, downloadPath);
                    videoDownloader.DownloadProgressChanged += (sender, args) => StatusArr[Index].Progress = Convert.ToInt32(args.ProgressPercentage);
                    videoDownloader.Execute();
                }
                else
                    StatusArr[Index].IsAlreadyExists = true;
                StatusArr[Index].IsSuccessful = true;
                StatusArr[Index].Progress = 100;
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                    throw ex;
                StatusArr[Index].IsSuccessful = false;
                StatusArr[Index].Exception = ex;
                StatusArr[Index].Progress = 100;
            }
        }

        private VideoInfo FindVideoInfo()
        {
            String videoID;
            var videoInfos = DownloadUrlResolver.GetDownloadUrls(DownloadUrl, out videoID, false);
            var videosWithAudio = videoInfos.Where(info => info.AudioType != AudioType.Unknown).OrderByDescending(info => info.Resolution);
            var mp4Videos = videosWithAudio.Where(info => info.VideoType == VideoType.Mp4);
            var resolutionLimitedVideos = mp4Videos.Where(info => info.Resolution <= MaxResolution);
            var video = resolutionLimitedVideos.FirstOrDefault()
                ?? mp4Videos.FirstOrDefault();
            if (video != null)
                video.VideoID = videoID;
            Debug.WriteLine(video);
            return video;
        }

        //private static void DownloadAudio(IEnumerable<VideoInfo> videoInfos)
        //{
        //    /*
        //     * We want the first extractable video with the highest audio quality.
        //     */
        //    VideoInfo video = videoInfos
        //        .Where(info => info.CanExtractAudio)
        //        .OrderByDescending(info => info.AudioBitrate)
        //        .First();

        //    /*
        //     * If the video has a decrypted signature, decipher it
        //     */
        //    if (video.RequiresDecryption)
        //    {
        //        DownloadUrlResolver.DecryptDownloadUrl(video);
        //    }

        //    /*
        //     * Create the audio downloader.
        //     * The first argument is the video where the audio should be extracted from.
        //     * The second argument is the path to save the audio file.
        //     */

        //    var audioDownloader = new AudioDownloader(video,
        //        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        //        RemoveIllegalPathCharacters(video.Title) + video.AudioExtension));

        //    // Register the progress events. We treat the download progress as 85% of the progress
        //    // and the extraction progress only as 15% of the progress, because the download will
        //    // take much longer than the audio extraction.
        //    audioDownloader.DownloadProgressChanged += (sender, args) => Console.WriteLine(args.ProgressPercentage * 0.85);
        //    audioDownloader.AudioExtractionProgressChanged += (sender, args) => Console.WriteLine(85 + args.ProgressPercentage * 0.15);

        //    /*
        //     * Execute the audio downloader.
        //     * For GUI applications note, that this method runs synchronously.
        //     */
        //    audioDownloader.Execute();
        //}

        #region Helpers

        private static string RemoveIllegalPathCharacters(string path)
        {
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            var r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            return r.Replace(path, "");
        }

        #endregion
    }
}
