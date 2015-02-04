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
using Kahia.Common.Extensions.StringExtensions;
using YoutubeListSyncronizer.Library;

namespace YoutubeListSyncronizer
{
    public class YoutubeDownloadBackgroundWorker : BackgroundWorker
    {
        private String DownloadUrl;
        private String DownloadFolder;
        private int Index;
        private int MaxResolution;
        public bool IsSuccessful { get; private set; }
        public Exception Exception { get; private set; }
        public YoutubeDownloadBackgroundWorker(string downloadFolder, string downloadUrl, int index, int maxResolution)
        {
            DownloadUrl = downloadUrl;
            DownloadFolder = downloadFolder;
            Index = index;
            MaxResolution = maxResolution;
            WorkerSupportsCancellation = true;
            WorkerReportsProgress = true;
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            try
            {
                var videoInfos = DownloadUrlResolver.GetDownloadUrls(DownloadUrl, false);
                var video = videoInfos.Where(info => info.VideoType == VideoType.Mp4 && info.Resolution <= MaxResolution)
                    .OrderByDescending(info => info.Resolution).FirstOrDefault();

                if (video == null)
                {
                    IsSuccessful = false;
                    ReportProgress(100, Index);
                }
                if (video.RequiresDecryption)
                    DownloadUrlResolver.DecryptDownloadUrl(video);

                //var downloadFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + "\\Youtube";
                var fileName = "{0:D4} - {1}.{2}".FormatString(Index, RemoveIllegalPathCharacters(video.Title), video.VideoExtension);
                var downloadPath = Path.Combine(DownloadFolder, fileName);
                var videoDownloader = new VideoDownloader(video, downloadPath);
                videoDownloader.DownloadProgressChanged += (sender, args) => ReportProgress(Convert.ToInt32(args.ProgressPercentage), Index);
                videoDownloader.Execute();
                IsSuccessful = true;
                ReportProgress(100, Index);
            }
            catch (Exception ex)
            {
                if(Debugger.IsAttached)
                    throw ex;
                IsSuccessful = false;
                Exception = ex;
                ReportProgress(100, Index);
            }
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
