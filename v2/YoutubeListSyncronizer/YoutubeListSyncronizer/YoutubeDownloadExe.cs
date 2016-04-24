using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kahia.Common.Extensions.StringExtensions;

namespace YoutubeListSyncronizer
{
    public static class YoutubeDownloadExe
    {
        #region Youtube-DL Exe

        public static string DownloadVideoAndReturnsErrors(int index, string url, Form1.Args args, Action<String,int> onConsoleDataReceived)
        {
            //"D:\Dropbox\Youtube Downloader\youtube-dl.exe" https://www.youtube.com/playlist?list=PLDZMiVQ0iUnCwGbMckmoupzrmTNRIo-Y0 -o "D:\_Videos\YT Favourites\%(autonumber)s-%(title)s.%(ext)s" --no-continue -w --playlist-reverse -i -f "mp4[height<=?720]"
            string[] processArgs =
            {
                url,
                "--no-continue -w -i",
                "-o \"{0}\\{1:D4}-%(title)s.%(ext)s\"".FormatString(args.VideoFolder,index+1),
                "-v ",
                //"--encoding cp857",
                //"--console-title",
                //"--write-pages",
                //"--print-json",
                //debug
                "-f \"mp4[height<=?720]\"",
            };
            var ytExe = FindYtDownloaderExe();
            var process = new Process();
            process.StartInfo.FileName = ytExe.FullName;
            process.StartInfo.Arguments = processArgs.JoinWith(" ");
            //process.StartInfo.UseShellExecute = false;
            //process.StartInfo.CreateNoWindow = false;
            //process.StartInfo.RedirectStandardOutput = false;
            //process.StartInfo.RedirectStandardError = true;
            //process.OutputDataReceived += (sender, eventArgs) => onConsoleDataReceived.Invoke(eventArgs.Data, index);
            process.Start();
            //var result = process.StandardOutput.ReadToEnd();
            //var errors = process.StandardError.ReadToEnd();
            process.WaitForExit();
            //return errors;
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
