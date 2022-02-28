#region License Statement
// Copyright (c) L.A.B.Soft.  All rights reserved.
//
// The use and distribution terms for this software are covered by the 
// Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file CPL.TXT at the root of this distribution.
// By using this software in any fashion, you are agreeing to be bound by 
// the terms of this license.
//
// You must not remove this notice, or any other, from this software.
#endregion


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
                PopState();

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
            PopState();

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