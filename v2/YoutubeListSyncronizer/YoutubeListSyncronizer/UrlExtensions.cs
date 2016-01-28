using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kahia.Common.Extensions.StringExtensions;

namespace YoutubeListSyncronizer
{
    /// <summary>
    /// 
    /// </summary>
    public static class UrlExtensions
    {
        public static bool EqualsIgnoreCaseInvariant(this String parameter, string target)
        {
            var result = parameter.Equals(target, StringComparison.InvariantCultureIgnoreCase);
            return result;
        }

        public static string ToCustomSeoFriendly(this string _text)
        {
            var text = _text.ToStringByDefaultValue().Trim().ToLower();
            var url = new StringBuilder();
            char lastCh = '\0';
            for (int index = 0; index < text.Length; index++)
            {
                char ch = text[index];
                //lastCh = ch;
                switch (ch)
                {
                    case 'ş': AddCharToStringBuilder(ref lastCh, url, 's'); break;
                    case 'ç': AddCharToStringBuilder(ref lastCh, url, 'c'); break;
                    case 'ö': AddCharToStringBuilder(ref lastCh, url, 'o'); break;
                    case 'ü': AddCharToStringBuilder(ref lastCh, url, 'u'); break;
                    case 'ğ': AddCharToStringBuilder(ref lastCh, url, 'g'); break;
                    case 'ı': AddCharToStringBuilder(ref lastCh, url, 'i'); break;
                    case ' ':
                    case '-':
                    case '.':
                    case ',':
                    case '+':
                        if (lastCh != '-')
                            AddCharToStringBuilder(ref lastCh, url, '-');
                        break;
                    case '\'': break;
                    default:
                        if ((ch >= '0' && ch <= '9') || (ch >= 'a' && ch <= 'z'))
                            AddCharToStringBuilder(ref lastCh, url, ch);
                        break;
                }
            }

            return url.ToStringByDefaultValue("_").Replace("-",""); //burda - da olmaması lazım tamamen metin bazlı, full-text gibi bir kıyaslama yapmalı
        }

        private static void AddCharToStringBuilder(ref char lastChar, StringBuilder builder, char charToBeAdded)
        {
            builder.Append(charToBeAdded);
            lastChar = charToBeAdded;
        }

        public static String GetFullUrlWithPort(this Uri uri)
        {
            const string format = "{0}://{1}{2}";
            var host = uri.Port != 80 ? uri.Host + ":" + uri.Port : uri.Host;
            var result = format.FormatString(uri.Scheme, host, uri.PathAndQuery);
            return result;
        }
    }
}
