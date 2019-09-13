using System;
using System.Net;

namespace YoutubeListSyncronizer
{
    public class WebClientExtended : WebClient
    {
        protected override WebRequest GetWebRequest(Uri uri)
        {
            var w = base.GetWebRequest(uri);
            w.Timeout = 20 * 60 * 1000;
            return w;
        }
    }
}
