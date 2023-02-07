// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.Notify.Patterns;

public abstract class PatternFormatter : IPatternFormatter
{
    private readonly bool _doformat;
    private readonly string _tagSearchPattern;

    protected Regex RegEx { get; private set; }

    protected PatternFormatter() { }

    protected PatternFormatter(string tagSearchRegExp)
        : this(tagSearchRegExp, false)
    {
    }

    protected PatternFormatter(string tagSearchRegExp, bool formatMessage)
    {
        if (string.IsNullOrEmpty(tagSearchRegExp))
        {
            throw new ArgumentException(nameof(tagSearchRegExp));
        }

        _tagSearchPattern = tagSearchRegExp;
        RegEx = new Regex(_tagSearchPattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
        _doformat = formatMessage;
    }

    public string[] GetTags(IPattern pattern)
    {
        ArgumentNullException.ThrowIfNull(pattern);

        var findedTags = new List<string>(SearchTags(pattern.Body));
        Array.ForEach(SearchTags(pattern.Subject), tag => { if (!findedTags.Contains(tag)) { findedTags.Add(tag); } });
        return findedTags.ToArray();
    }

    public void FormatMessage(INoticeMessage message, ITagValue[] tagsValues)
    {
        ArgumentNullException.ThrowIfNull(message);
        ArgumentNullException.ThrowIfNull(message.Pattern);
        ArgumentNullException.ThrowIfNull(tagsValues);

        BeforeFormat(message, tagsValues);

        message.Subject = FormatText(_doformat ? message.Subject : message.Pattern.Subject, tagsValues);
        message.Body = FormatText(_doformat ? message.Body : message.Pattern.Body, tagsValues);

        AfterFormat(message);
    }

    protected abstract string FormatText(string text, ITagValue[] tagsValues);

    protected virtual void BeforeFormat(INoticeMessage message, ITagValue[] tagsValues) { }

    protected virtual void AfterFormat(INoticeMessage message) { }

    protected virtual string[] SearchTags(string text)
    {
        if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(_tagSearchPattern))
        {
            return Array.Empty<string>();
        }

        var maches = RegEx.Matches(text);
        var findedTags = new List<string>(maches.Count);
        foreach (Match mach in maches)
        {
            var tag = mach.Groups["tagName"].Value;
            if (!findedTags.Contains(tag))
            {
                findedTags.Add(tag);
            }
        }

        return findedTags.ToArray();
    }
}
