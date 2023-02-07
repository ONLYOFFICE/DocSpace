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

namespace Textile.Blocks;

public abstract class PhraseBlockModifier : BlockModifier
{
    protected PhraseBlockModifier()
    {
    }

    protected string PhraseModifierFormat(string input, string modifier, string tag)
    {
        // All phrase modifiers are one character, or a double character. Sometimes,
        // there's an additional escape character for the regex ('\').
        var compressedModifier = modifier;
        if (modifier.Length == 4)
        {
            compressedModifier = modifier.Substring(0, 2);
        }
        else if (modifier.Length == 2)
        {
            if (modifier[0] != '\\')
            {
                compressedModifier = modifier[0].ToString();
            }
            //else: compressedModifier = modifier;
        }
        //else: compressedModifier = modifier;

        // We try to remove the Textile tag used for the formatting from
        // the punctuation pattern, so that we match the end of the formatted
        // zone correctly.
        var punctuationPattern = Globals.PunctuationPattern.Replace(compressedModifier, "");

        // Now we can do the replacement.
        var pmme = new PhraseModifierMatchEvaluator(tag);
        var res = Regex.Replace(input,
                                        @"(?<=\s|" + punctuationPattern + @"|[{\(\[]|^)" +
                                        modifier +
                                        Globals.BlockModifiersPattern +
                                        @"(:(?<cite>(\S+)))?" +
                                        @"(?<content>[^" + compressedModifier + "]*)" +
                                        @"(?<end>" + punctuationPattern + @"*)" +
                                        modifier +
                                        @"(?=[\]\)}]|" + punctuationPattern + @"+|\s|$)",
                                    new MatchEvaluator(pmme.MatchEvaluator)
                                    );
        return res;
    }

    private sealed class PhraseModifierMatchEvaluator
    {
        private readonly string _tag;

        public PhraseModifierMatchEvaluator(string tag)
        {
            _tag = tag;
        }

        public string MatchEvaluator(Match m)
        {
            if (m.Groups["content"].Length == 0)
            {
                // It's possible that the "atts" match groups eats the contents
                // when the user didn't want to give block attributes, but the content
                // happens to match the syntax. For example: "*(blah)*".
                if (m.Groups["atts"].Length == 0)
                {
                    return m.ToString();
                }

                return "<" + _tag + ">" + m.Groups["atts"].Value + m.Groups["end"].Value + "</" + _tag + ">";
            }

            var atts = BlockAttributesParser.ParseBlockAttributes(m.Groups["atts"].Value, _tag);
            if (m.Groups["cite"].Length > 0)
            {
                atts += " cite=\"" + m.Groups["cite"] + "\"";
            }

            var res = "<" + _tag + atts + ">" +
                            m.Groups["content"].Value + m.Groups["end"].Value +
                            "</" + _tag + ">";
            return res;
        }
    }
}
