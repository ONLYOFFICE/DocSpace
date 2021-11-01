using System.Web;
using JSIStudios.SimpleRESTServices.Core;

namespace JSIStudios.SimpleRESTServices.Server
{
    public class HtmlTagReplacerAndCleaner : ITextCleaner
    {
        public static string Name = "ReplaceCleanHtml";
        
        public string Clean(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return null;
            }

            source = source.Replace("</p>", "</p>\n\n");
            source = source.Replace("<br>", "\n\n");
            source = source.Replace("<br />", "\n\n");
            source = source.Replace("<br/>", "\n\n");
            source = source.Replace("<BR>", "\n\n");
            source = source.Replace("<BR />", "\n\n");
            source = source.Replace("<BR/>", "\n\n");

            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;
            string[,] special = new string[,] { { "&rsquo;", "'" }
                                              , { "&nbsp;", " " }
                                              , { "&quot;", "\"" }
                                              , { "&apos;", "'" }
                                              , { "&lt;", "<" }
                                              , { "&raquo;", ">>" }
                                              , { "&#160;", " "}
                                              , { "&#39;", "'"}
                                              , { "&gt;", ">" }
            };

            //                                              , { "&mdash;", System.Windows.Browser.HttpUtility.HtmlDecode("&mdash;")}
            var sourceLen = source.Length;
            for (int idx = 0; idx < sourceLen; idx++)
            {
                char let = source[idx];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    if (let == '&')
                    {
                        for (int cnt = 0; cnt < special.GetLength(0); cnt++)
                        {
                            var specialLen = special[cnt, 0].Length;
                            if ((idx + specialLen) > sourceLen) 
                            {
                                continue; 
                            }
                            if (source.Substring(idx, specialLen).Equals(special[cnt, 0]))
                            {
                                let = (char)special[cnt, 1][0];
                                idx += specialLen - 1;
                                break;
                            }
                        }
                    }
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            //return new string(array, 0, arrayIndex);
            var s = new string(array, 0, arrayIndex);
            return HttpUtility.HtmlDecode(s.Trim());
        }
    }
}