using System;

namespace YoutubeListSyncronizer.Library.Exceptions
{
    public class VideoDeletedException : Exception
    {
        public String VideoUrl;

        public VideoDeletedException(String url)
        {
            VideoUrl = url;
        }
    }
}
