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

namespace Textile;

public partial class TextileFormatter
{
    private readonly Regex _velocityArguments =
        new Regex("nostyle(?<arg>.*?)/nostyle", RegexOptions.IgnoreCase | RegexOptions.Singleline);

    private string ArgMatchReplace(Match match)
    {
        return match.Result("${arg}");
    }

    #region Formatting Methods

    /// <summary>
    /// Formats the given text.
    /// </summary>
    /// <param name="input">The text to format.</param>
    public void Format(string input)
    {
        Output.Begin();

        // Clean the text...
        var str = PrepareInputForFormatting(input);
        // ...and format each line.
        foreach (var line in str.Split('\n'))
        {
            var tmp = line;

            // Let's see if the current state(s) is(are) finished...
            while (CurrentState != null && CurrentState.ShouldExit(tmp))
            {
                PopState();
            }

            if (!Regex.IsMatch(tmp, @"^\s*$"))
            {
                // Figure out the new state for this text line, if possible.
                if (CurrentState == null || CurrentState.ShouldParseForNewFormatterState(tmp))
                {
                    tmp = HandleFormattingState(tmp);
                }
                // else, the current state doesn't want to be superceded by
                // a new one. We'll leave him be.

                // Modify the line with our block modifiers.
                if (CurrentState == null || CurrentState.ShouldFormatBlocks(tmp))
                {
                    foreach (var blockModifier in _blockModifiers)
                    {
                        //TODO: if not disabled...
                        tmp = blockModifier.ModifyLine(tmp);
                    }

                    for (var i = _blockModifiers.Count - 1; i >= 0; i--)
                    {
                        var blockModifier = _blockModifiers[i];
                        tmp = blockModifier.Conclude(tmp);
                    }
                }

                tmp = _velocityArguments.Replace(tmp, ArgMatchReplace);

                // Format the current line.
                CurrentState.FormatLine(tmp);
            }
        }
        // We're done. There might be a few states still on
        // the stack (for example if the text ends with a nested
        // list), so we must pop them all so that they have
        // their "Exit" method called correctly.
        while (_stackOfStates.Count > 0)
        {
            PopState();
        }

        Output.End();
    }

    #endregion

    #region Preparation Methods

    /// <summary>
    /// Cleans up a text before formatting.
    /// </summary>
    /// <param name="input">The text to clean up.</param>
    /// <returns>The clean text.</returns>
    /// This method cleans stuff like line endings, so that
    /// we don't have to bother with it while formatting.
    private string PrepareInputForFormatting(string input)
    {
        input = CleanWhiteSpace(input);
        return input;
    }

    private string CleanWhiteSpace(string text)
    {
        text = text.Replace("\r\n", "\n");
        text = text.Replace("\t", "");
        text = Regex.Replace(text, @"\n{3,}", "\n\n");
        text = Regex.Replace(text, @"\n *\n", "\n\n");
        text = Regex.Replace(text, "\"$", "\" ");
        return text;
    }

    #endregion
}