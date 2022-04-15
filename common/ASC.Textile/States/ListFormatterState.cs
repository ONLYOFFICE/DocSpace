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

namespace Textile.States;

/// <summary>
/// Base formatting state for all lists.
/// </summary>
public abstract class ListFormatterState : FormatterState
{
    internal const string PatternBegin = @"^\s*(?<tag>";
    internal const string PatternEnd = @")" + Globals.BlockModifiersPattern + @"(?:\s+)? (?<content>.*)$";

    private bool _firstItem = true;
    private bool _firstItemLine = true;
    private string _tag;
    private string _attsInfo;
    private string _alignInfo;

    protected int NestingDepth
    {
        get { return _tag.Length; }
    }

    protected ListFormatterState(TextileFormatter formatter)
        : base(formatter)
    {
    }

    public override string Consume(string input, Match m)
    {
        _tag = m.Groups["tag"].Value;
        _alignInfo = m.Groups["align"].Value;
        _attsInfo = m.Groups["atts"].Value;
        input = m.Groups["content"].Value;

        this.Formatter.ChangeState(this);

        return input;
    }

    public sealed override void Enter()
    {
        _firstItem = true;
        _firstItemLine = true;
        WriteIndent();
    }

    public sealed override void Exit()
    {
        Formatter.Output.WriteLine("</li>");
        WriteOutdent();
    }

    public sealed override void FormatLine(string input)
    {
        if (_firstItemLine)
        {
            if (!_firstItem)
            {
                Formatter.Output.WriteLine("</li>");
            }

            Formatter.Output.Write("<li " + FormattedStylesAndAlignment("li") + ">");
            _firstItemLine = false;
        }
        else
        {
            Formatter.Output.WriteLine("<br />");
        }
        Formatter.Output.Write(input);
        _firstItem = false;
    }

    public sealed override bool ShouldNestState(FormatterState other)
    {
        var listState = (ListFormatterState)other;
        return listState.NestingDepth > NestingDepth;
    }

    public sealed override bool ShouldExit(string input)
    {
        // If we have an empty line, we can exit.
        if (string.IsNullOrEmpty(input))
        {
            return true;
        }

        // We exit this list if the next
        // list item is of the same type but less
        // deep as us, or of the other type of
        // list and as deep or less.
        if (NestingDepth > 1)
        {
            if (IsMatchForMe(input, 1, NestingDepth - 1))
            {
                return true;
            }
        }
        if (IsMatchForOthers(input, 1, NestingDepth))
        {
            return true;
        }

        // As it seems we're going to continue taking
        // care of this line, we take the opportunity
        // to check whether it's the same list item as
        // previously (no "**" or "##" tags), or if it's
        // a new list item.
        if (IsMatchForMe(input, NestingDepth, NestingDepth))
        {
            _firstItemLine = true;
        }

        return false;
    }

    public sealed override bool ShouldParseForNewFormatterState(string input)
    {
        // We don't let anyone but ourselves mess with our stuff.
        if (IsMatchForMe(input, 1, 100))
        {
            return true;
        }

        if (IsMatchForOthers(input, 1, 100))
        {
            return true;
        }

        return false;
    }

    protected abstract void WriteIndent();
    protected abstract void WriteOutdent();
    protected abstract bool IsMatchForMe(string input, int minNestingDepth, int maxNestingDepth);
    protected abstract bool IsMatchForOthers(string input, int minNestingDepth, int maxNestingDepth);

    protected string FormattedStylesAndAlignment(string element)
    {
        return BlockAttributesParser.ParseBlockAttributes(_alignInfo + _attsInfo, element);
    }
}
