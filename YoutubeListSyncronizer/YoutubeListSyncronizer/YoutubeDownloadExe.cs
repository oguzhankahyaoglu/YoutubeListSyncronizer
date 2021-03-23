using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using Kahia.Common.Extensions.ConversionExtensions;
using Kahia.Common.Extensions.StringExtensions;

namespace YoutubeListSyncronizer
{
    public static class YoutubeDownloadExe
    {

        #region Youtube-DL Exe

        public static string DownloadVideoAndReturnsErrors(int index, string url, Form1.Args args, 
            Action<String, int> onConsoleDataReceived,
            Action<String, int> onErrorDataRecieved
            )
        {
            var now = DateTime.Now;
            //"D:\Dropbox\Youtube Downloader\youtube-dl.exe" https://www.youtube.com/playlist?list=PLDZMiVQ0iUnCwGbMckmoupzrmTNRIo-Y0 -o "D:\_Videos\YT Favourites\%(autonumber)s-%(title)s.%(ext)s" --no-continue -w --playlist-reverse -i -f "mp4[height<=?720]"
            string[] processArgs =
            {
                url,
                "--no-continue -w -i",
                $"-o \"{args.VideoFolder}\\{now:yyyyMM}-{(index+1):D4}-%(title)s.%(ext)s\"",
                "-v ",
                //"--encoding cp857",
                //"--console-title",
                //"--write-pages",
                //"--print-json",
                //debug
                //"-f \"mp4[height<=?{0}]\"".FormatString(args.MaxRes)
                "-f \"mp4+best\""
            };
            var ytExe = FindYtDownloaderExe();
            var process = new Process
            {
                StartInfo =
                {
                    FileName = ytExe.FullName,
                    Arguments = processArgs.JoinWith(" "),
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };
            process.OutputDataReceived += (sender, eventArgs) => onConsoleDataReceived.Invoke(eventArgs.Data, index);
            process.ErrorDataReceived += (sender, eventArgs) => onErrorDataRecieved.Invoke(eventArgs.Data, index);
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            return null;
        }

        private static FileInfo YtDownloaderExe;
        public static FileInfo FindYtDownloaderExe()
        {
            if (YtDownloaderExe != null)
                return YtDownloaderExe;
            var youtubeDlFolder = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.GetDirectories("youtube-dl")[0];
            YtDownloaderExe = youtubeDlFolder.GetFiles("youtube-dl.exe")[0];
            return YtDownloaderExe;
        }

        #endregion

    }
}
