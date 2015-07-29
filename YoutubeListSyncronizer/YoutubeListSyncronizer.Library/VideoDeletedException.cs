using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeListSyncronizer.Library
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
