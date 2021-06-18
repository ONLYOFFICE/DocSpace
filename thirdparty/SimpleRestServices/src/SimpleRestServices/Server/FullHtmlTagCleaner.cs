using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using JSIStudios.SimpleRESTServices.Core;

namespace JSIStudios.SimpleRESTServices.Server
{
    public class FullHtmlTagCleaner : ITextCleaner
    {
        public static string Name = "FullHtmlTagCleaner";

        private string[] _allowedTags = new string[] { };
        private string[] _illegalTags = new string[] { "style", "script", "embed", "object" };
        private static string[] _illegalCharacters = new[] { "&#0;", "&#x0;", "%0", "&#1;", "&#x1;", "%1", "&#2;", "&#x2;", "%2", "&#3;", "&#x3;", "%3", "&#4;", "&#x4;", "%4", "&#5;", "&#x5;", "%5", "&#6;", "&#x6;", "%6", "&#7;", "&#x7;", "%7", "&#8;", "&#x8;", "%8", "&#9;", "&#x9;", "%9", "&#10;", "&#xa;", "%a", "&#11;", "&#xb;", "%b", "&#12;", "&#xc;", "%c", "&#13;", "&#xd;", "%d", "&#14;", "&#xe;", "%e", "&#15;", "&#xf;", "%f", "&#16;", "&#x10;", "%10", "&#17;", "&#x11;", "%11", "&#18;", "&#x12;", "%12", "&#19;", "&#x13;", "%13", "&#20;", "&#x14;", "%14", "&#21;", "&#x15;", "%15", "&#22;", "&#x16;", "%16", "&#23;", "&#x17;", "%17", "&#24;", "&#x18;", "%18", "&#25;", "&#x19;", "%19", "&#26;", "&#x1a;", "%1a", "&#27;", "&#x1b;", "%1b", "&#28;", "&#x1c;", "%1c", "&#29;", "&#x1d;", "%1d", "&#30;", "&#x1e;", "%1e", "&#31;", "&#x1f;", "%1f", "&#127;", "&#x7f;", "%7f", "&#128;", "&#x80;", "%80", "&#129;", "&#x81;", "%81", "&#130;", "&#x82;", "%82", "&#131;", "&#x83;", "%83", "&#132;", "&#x84;", "%84", "&#133;", "&#x85;", "%85", "&#134;", "&#x86;", "%86", "&#135;", "&#x87;", "%87", "&#136;", "&#x88;", "%88", "&#137;", "&#x89;", "%89", "&#138;", "&#x8a;", "%8a", "&#139;", "&#x8b;", "%8b", "&#140;", "&#x8c;", "%8c", "&#141;", "&#x8d;", "%8d", "&#142;", "&#x8e;", "%8e", "&#143;", "&#x8f;", "%8f", "&#144;", "&#x90;", "%90", "&#145;", "&#x91;", "%91", "&#146;", "&#x92;", "%92", "&#147;", "&#x93;", "%93", "&#148;", "&#x94;", "%94", "&#149;", "&#x95;", "%95", "&#150;", "&#x96;", "%96", "&#151;", "&#x97;", "%97", "&#152;", "&#x98;", "%98", "&#153;", "&#x99;", "%99", "&#154;", "&#x9a;", "%9a", "&#155;", "&#x9b;", "%9b", "&#156;", "&#x9c;", "%9c", "&#157;", "&#x9d;", "%9d", "&#158;", "&#x9e;", "%9e", "&#159;", "&#x9f;", "%9f" };

        public virtual string[] IllegalTags
        {
            get { return _illegalTags; }
            set { _illegalTags = value; }
        }

        public virtual string[] AllowedTags
        {
            get { return _allowedTags; }
            set { _allowedTags = value; }
        }

        public static string[] IllegalCharacters
        {
            get { return _illegalCharacters; }
            set { _illegalCharacters = value; }
        }

        public string Clean(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return null;
            }

            var allowedTags = AllowedTags;
            var illegalTags = IllegalTags;

            source = RemoveIllegalCharacters(source);
            source = illegalTags.Where(illegalTag => illegalTag == "style" || illegalTag == "script").Aggregate(source, (current, illegalTag) => RemoveTagsAndTextBetweenTags("<" + illegalTag + ">", "</" + illegalTag + ">", current));

            var array = new char[source.Length];
            var arrayIndex = 0;

            source = Regex.Replace(source, @"(\r\n)|(\r)|(\n)", " ").Replace("\t", "");
            var sourceLen = source.Length;

            for (int idx = 0; idx < sourceLen; idx++)
            {
                char let = source[idx];
                if (let == '<')
                {
                    var len = 0;
                    var idx2 = idx;
                    char let2;
                    string tag = string.Empty;

                    do
                    {
                        idx2++;
                        if (idx2 >= sourceLen)
                        {
                            tag = string.Empty;
                            break;
                        }

                        len++;
                        let2 = source[idx2];


                        if (string.IsNullOrEmpty(tag) && (let2 == ' ' || (let2 == '/' && len > 1)))
                        {
                            tag = source.Substring(idx + 1, len - 1);
                        }

                    } while (let2 != '>');

                    if (string.IsNullOrEmpty(tag))
                    {
                        int start = idx + 1;
                        if (source[start] == '/')
                        {
                            start++;
                            len--;
                        }

                        tag = source.Substring(start, len - 1);
                    }

                    if (allowedTags.Contains(tag.ToLower()))
                    {
                        for (int i = idx; i <= idx2; i++)
                        {
                            if (i >= sourceLen)
                                break;
                            array[arrayIndex] = source[i];
                            arrayIndex++;
                        }

                    }

                    idx = idx2;

                    if (illegalTags.Contains(tag.ToLower()))
                    {
                        do
                        {
                            idx++;
                            if (idx < (sourceLen - 1))
                            {
                                let = source[idx];

                                if (let == '<' && source[++idx] == '/')
                                {
                                    len = 0;
                                    idx2 = idx;
                                    var endTag = string.Empty;
                                    do
                                    {
                                        idx2++;
                                        len++;
                                        let2 = source[idx2];
                                    } while (let2 != '>' && idx2 < sourceLen);

                                    idx = idx2;
                                    let = let2;
                                }
                            }
                        } while (let != '>' && idx < (sourceLen - 1));
                    }
                }
                else
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            //return new string(array, 0, arrayIndex);
            var s = new string(array, 0, arrayIndex);
            return HttpUtility.HtmlDecode(s.Trim());
            
        }

        private static string RemoveIllegalCharacters(string source)
        {
            var sortedArray = from s in IllegalCharacters
                              orderby s.Length descending
                              select s;
            return sortedArray.Aggregate(source, (current, illegalCharacter) => current.Replace(illegalCharacter, ""));
        }

        private static string RemoveTagsAndTextBetweenTags(string startTag, string endTag, string strSource)
        {
            var temp = strSource;

            // remove the text in start tag if any. eg - <script type='text/javascript'>var _sf_startpt=(new Date()).getTime()</script>
            // replaces the tag with simple tags like <script>var _sf_startpt=(new Date()).getTime()</script>
            while (temp.IndexOf(startTag.Substring(0, startTag.Length - 1), StringComparison.OrdinalIgnoreCase) != -1)
            {
                var result = GetTextAlongWithTag(startTag.Substring(0, startTag.Length - 1), ">", temp, true, true);
                temp = !string.IsNullOrWhiteSpace(result[0]) ? strSource.Replace(result[0], string.Empty) : temp;
                if (result[0] != startTag)
                {
                    // to handle the tags which doesn't have closing tags. eg - <script src="/hive/javascripts/scriptaculous.js"/> 
                    if (result[0].IndexOf("/>") != -1)
                        strSource = !string.IsNullOrWhiteSpace(result[0]) ? strSource.Replace(result[0], startTag + endTag) : strSource;
                    else
                        strSource = !string.IsNullOrWhiteSpace(result[0]) ? strSource.Replace(result[0], startTag) : strSource;
                }
            }

            //declare safety int variable to prevent infinite loop if any use case is not covered.
            var iSafetyCheck = 10000;

            //remove all the tags along with the text in between the tags
            while (strSource.IndexOf(startTag, StringComparison.OrdinalIgnoreCase) != -1)
            {
                if (--iSafetyCheck == 0) return strSource;
                var result = GetTextAlongWithTag(startTag, endTag, strSource, true, true);
                strSource = !string.IsNullOrWhiteSpace(result[0]) ? strSource.Replace(result[0], string.Empty) : strSource;
            }
            return strSource;
        }

        private static string[] GetTextAlongWithTag(string startTag, string endTag, string strSource, bool removeBegin, bool removeEnd)
        {
            string[] result = { string.Empty, string.Empty };
            var iIndexOfBegin = strSource.IndexOf(startTag, StringComparison.OrdinalIgnoreCase);
            if (iIndexOfBegin != -1)
            {
                if (removeBegin)
                    iIndexOfBegin -= startTag.Length;
                strSource = strSource.Substring(iIndexOfBegin
                    + startTag.Length);
                var iEnd = strSource.IndexOf(endTag, StringComparison.OrdinalIgnoreCase);
                if (iEnd != -1)
                {
                    if (removeEnd)
                        iEnd += endTag.Length;
                    result[0] = strSource.Substring(0, iEnd);
                    if (iEnd + endTag.Length < strSource.Length)
                        result[1] = strSource.Substring(iEnd
                            + endTag.Length);
                }
                else
                    result[0] = strSource;
            }
            else
                result[1] = strSource;
            return result;
        }
    }
}
