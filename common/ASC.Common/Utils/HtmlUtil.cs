namespace ASC.Common.Utils;

public static class HtmlUtil
{
    private static readonly Regex _tagReplacer
        = new Regex("<[^>]*>", RegexOptions.Multiline | RegexOptions.Compiled);

    private static readonly Regex _commentsReplacer
        = new Regex("<!--(?s).*?-->", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex _xssReplacer
        = new Regex(@"<\s*(style|script)[^>]*>(.*?)<\s*/\s*(style|script)>", RegexOptions.IgnoreCase
            | RegexOptions.CultureInvariant | RegexOptions.Compiled | RegexOptions.Singleline);

    private static readonly Regex _worder =
        new Regex(@"\S+", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    public static string GetText(string html, int maxLength = 0, string endBlockTemplate = "...")
    {
        var unformatedText = string.Empty;

        if (!string.IsNullOrEmpty(html))
        {
            html = _xssReplacer.Replace(html, string.Empty); //Clean malicious tags. <script> <style>

            if (string.IsNullOrEmpty(html))
            {
                return html;
            }

            unformatedText = _tagReplacer.Replace(html, string.Empty);

            if (!string.IsNullOrEmpty(unformatedText))
            {
                // kill comments
                unformatedText = _commentsReplacer.Replace(unformatedText, string.Empty);
                unformatedText = unformatedText.Trim();

                if (!string.IsNullOrEmpty(unformatedText))
                {
                    if (maxLength == 0 || unformatedText.Length < maxLength)
                    {
                        return HttpUtility.HtmlDecode(unformatedText);
                    }

                    //Set maximum length with end block
                    maxLength = Math.Max(0, maxLength - endBlockTemplate.Length);
                    var startIndex = Math.Max(0, Math.Min(unformatedText.Length - 1, maxLength));
                    var countToScan = Math.Max(0, startIndex - 1);

                    var lastSpaceIndex = unformatedText.LastIndexOf(' ', startIndex, countToScan);

                    unformatedText = lastSpaceIndex > 0 && lastSpaceIndex < unformatedText.Length
                                         ? unformatedText.Remove(lastSpaceIndex)
                                         : unformatedText.Substring(0, maxLength);

                    if (!string.IsNullOrEmpty(endBlockTemplate))
                    {
                        unformatedText += endBlockTemplate;
                    }
                }
            }
        }

        return HttpUtility.HtmlDecode(unformatedText);//TODO:!!!
    }

    public static string ToPlainText(string html)
    {
        return GetText(html);
    }

    /// <summary>
    /// The function highlight all words in htmlText by searchText.
    /// </summary>
    /// <param name="searchText">the space separated string</param>
    /// <param name="htmlText">html for highlight</param>
    /// <param name="withoutLink"></param>
    /// <returns>highlighted html</returns>
    public static string SearchTextHighlight(string searchText, string htmlText, bool withoutLink = false)
    {
        if (string.IsNullOrEmpty(searchText) || string.IsNullOrEmpty(htmlText))
        {
            return htmlText;
        }

            var regexpstr = _worder.Matches(searchText).Select(m => m.Value).Distinct().Aggregate((r, n) => r + "|" + n);
        var wordsFinder = new Regex(Regex.Escape(regexpstr), RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline);
            return wordsFinder.Replace(htmlText, m => "<span class='searchTextHighlight" + (withoutLink ? " bold" : string.Empty) + "'>" + m.Value + "</span>");
    }
}