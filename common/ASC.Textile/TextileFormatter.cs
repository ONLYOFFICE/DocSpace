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

/// <summary>
/// Class for formatting Textile input into HTML.
/// </summary>
/// This class takes raw Textile text and sends the
/// formatted, ready to display HTML string to the
/// outputter defined in the constructor of the
/// class.
public partial class TextileFormatter
{
    static TextileFormatter()
    {
        RegisterFormatterState(typeof(HeaderFormatterState));
        RegisterFormatterState(typeof(PaddingFormatterState));
        RegisterFormatterState(typeof(BlockQuoteFormatterState));
        RegisterFormatterState(typeof(ParagraphFormatterState));
        RegisterFormatterState(typeof(FootNoteFormatterState));
        RegisterFormatterState(typeof(OrderedListFormatterState));
        RegisterFormatterState(typeof(UnorderedListFormatterState));
        RegisterFormatterState(typeof(TableFormatterState));
        RegisterFormatterState(typeof(TableRowFormatterState));
        RegisterFormatterState(typeof(CodeFormatterState));
        RegisterFormatterState(typeof(PreFormatterState));
        RegisterFormatterState(typeof(PreCodeFormatterState));
        RegisterFormatterState(typeof(NoTextileFormatterState));

        RegisterBlockModifier(new NoTextileBlockModifier());
        RegisterBlockModifier(new CodeBlockModifier());
        RegisterBlockModifier(new PreBlockModifier());
        RegisterBlockModifier(new HyperLinkBlockModifier());
        RegisterBlockModifier(new ImageBlockModifier());
        RegisterBlockModifier(new GlyphBlockModifier());
        RegisterBlockModifier(new EmphasisPhraseBlockModifier());
        RegisterBlockModifier(new StrongPhraseBlockModifier());
        RegisterBlockModifier(new ItalicPhraseBlockModifier());
        RegisterBlockModifier(new BoldPhraseBlockModifier());
        RegisterBlockModifier(new CitePhraseBlockModifier());
        RegisterBlockModifier(new DeletedPhraseBlockModifier());
        RegisterBlockModifier(new InsertedPhraseBlockModifier());
        RegisterBlockModifier(new SuperScriptPhraseBlockModifier());
        RegisterBlockModifier(new SubScriptPhraseBlockModifier());
        RegisterBlockModifier(new SpanPhraseBlockModifier());
        RegisterBlockModifier(new FootNoteReferenceBlockModifier());

        //TODO: capitals block modifier
    }

    /// <summary>
    /// Public constructor, where the formatter is hooked up
    /// to an outputter.
    /// </summary>
    /// <param name="output">The outputter to be used.</param>
    public TextileFormatter(IOutputter output)
    {
        Output = output;
    }

    #region Properties for Output

    /// <summary>
    /// The ouputter to which the formatted text
    /// is sent to.
    /// </summary>
    public IOutputter Output { get; }

    /// <summary>
    /// The offset for the header tags.
    /// </summary>
    /// When the formatted text is inserted into another page
    /// there might be a need to offset all headers (h1 becomes
    /// h4, for instance). The header offset allows this.
    public int HeaderOffset { get; set; }

    #endregion

    #region Properties for Conversion

    public bool FormatImages
    {
        get { return IsBlockModifierEnabled(typeof(ImageBlockModifier)); }
        set { SwitchBlockModifier(typeof(ImageBlockModifier), value); }
    }

    public bool FormatLinks
    {
        get { return IsBlockModifierEnabled(typeof(HyperLinkBlockModifier)); }
        set { SwitchBlockModifier(typeof(HyperLinkBlockModifier), value); }
    }

    public bool FormatLists
    {
        get { return IsBlockModifierEnabled(typeof(OrderedListFormatterState)); }
        set
        {
            SwitchBlockModifier(typeof(OrderedListFormatterState), value);
            SwitchBlockModifier(typeof(UnorderedListFormatterState), value);
        }
    }

    public bool FormatFootNotes
    {
        get { return IsBlockModifierEnabled(typeof(FootNoteReferenceBlockModifier)); }
        set
        {
            SwitchBlockModifier(typeof(FootNoteReferenceBlockModifier), value);
            SwitchFormatterState(typeof(FootNoteFormatterState), value);
        }
    }

    public bool FormatTables
    {
        get { return IsFormatterStateEnabled(typeof(TableFormatterState)); }
        set
        {
            SwitchFormatterState(typeof(TableFormatterState), value);
            SwitchFormatterState(typeof(TableRowFormatterState), value);
        }
    }

    /// <summary>
    /// Attribute to add to all links.
    /// </summary>
    public string Rel { get; set; } = string.Empty;

    #endregion

    #region Utility Methods

    /// <summary>
    /// Utility method for quickly formatting a text without having
    /// to create a TextileFormatter with an IOutputter.
    /// </summary>
    /// <param name="input">The string to format</param>
    /// <returns>The formatted version of the string</returns>
    public static string FormatString(string input)
    {
        var s = new StringBuilderTextileFormatter();
        var f = new TextileFormatter(s);
        f.Format(input);
        return s.GetFormattedText();
    }

    /// <summary>
    /// Utility method for formatting a text with a given outputter.
    /// </summary>
    /// <param name="input">The string to format</param>
    /// <param name="outputter">The IOutputter to use</param>
    public static void FormatString(string input, IOutputter outputter)
    {
        var f = new TextileFormatter(outputter);
        f.Format(input);
    }

    #endregion
}
