using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        private const String URI = "http://log.okahyaoglu.net/?";
        //private const String URI = "http://localhost/LogMaster/?";
        public static void Log(Exception ex)
        {
            var exStr = ConvertExceptionToString(ex);
            var values = new NameValueCollection
            {
                { "app", "YoutubeListSyncronizer" },
                { "key", "20562056" },
                { "title", ex.Message },
                { "log", exStr },
            };

            try
            {
                using (var wc = new WebClient())
                {
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    var result = wc.UploadValues(URI, values);
                    var resultStr = Encoding.UTF8.GetString(result);
                    Debug.WriteLine("[Logger Result] " + resultStr);
                }
            }
            catch (WebException)
            {
                //loglayamazsa hatayı
            }
            catch (Exception)
            {
                if (Debugger.IsAttached)
                    throw;
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
