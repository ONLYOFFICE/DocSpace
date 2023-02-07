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

public abstract class SimpleBlockFormatterState : FormatterState
{
    internal const string PatternBegin = @"^\s*(?<tag>";
    internal const string PatternEnd = @")" + Globals.AlignPattern + Globals.BlockModifiersPattern + @"\.(?:\s+)?(?<content>.*)$";

    public string Tag { get; private set; }

    public string AlignInfo { get; private set; }

    public string AttInfo { get; private set; }

    protected SimpleBlockFormatterState(TextileFormatter formatter)
        : base(formatter)
    {
    }

    public override string Consume(string input, Match m)
    {
        Tag = m.Groups["tag"].Value;
        AlignInfo = m.Groups["align"].Value;
        AttInfo = m.Groups["atts"].Value;
        input = m.Groups["content"].Value;

        OnContextAcquired();

        this.Formatter.ChangeState(this);

        return input;
    }

    public override bool ShouldNestState(FormatterState other)
    {
        var blockFormatterState = (SimpleBlockFormatterState)other;
        return blockFormatterState.Tag != Tag ||
                blockFormatterState.AlignInfo != AlignInfo ||
                blockFormatterState.AttInfo != AttInfo;
    }

    protected virtual void OnContextAcquired()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected string FormattedAlignment()
    {
        return BlockAttributesParser.ParseBlockAttributes(AlignInfo);
    }

    protected string FormattedStyles(string element)
    {
        return BlockAttributesParser.ParseBlockAttributes(AttInfo, element);
    }

    protected string FormattedStylesAndAlignment(string element)
    {
        return BlockAttributesParser.ParseBlockAttributes(AlignInfo + AttInfo, element);
    }
}