using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Kahia.Common.Extensions.StringExtensions;

namespace YoutubeListSyncronizer
{
    public static class Logger
    {
        private const String URI = "http://www.myurl.com/post.php";
        public static void Log(Exception ex)
        {
            var exStr = ConvertExceptionToString(ex);
            var parameters = "app=YoutubeListSyncronizer&key=20562056&log="+ exStr;
            using (var wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                var result = wc.UploadString(URI, parameters);
                Debug.WriteLine("[Logger Result] " + result);
            }
        }


        private static String ConvertExceptionToString(Exception exception)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<log>");
            string[] stackTrace = exception.StackTrace.ToStringByDefaultValue().Split(new[] { " at " }, StringSplitOptions.None);
            sb.AppendFormat("<message>{0}</message>\n", exception.Message);
            sb.AppendFormat("<source>{0}</source>\n", exception.Source);
            sb.AppendFormat("<stack>{0}</stack>\n", String.Join("\nat ", stackTrace));
            if (exception.InnerException != null)
                sb.AppendFormat("<innerException>{0}</innerException>", ConvertExceptionToString(exception.InnerException));
            sb.AppendLine("</log>");
            return sb.ToString();
        }

    }
}
