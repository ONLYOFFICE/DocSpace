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

#region Using Statements
using System.Text.RegularExpressions;
#endregion


namespace Textile.States
{
    [FormatterState(SimpleBlockFormatterState.PatternBegin + @"pad[0-9]+" + SimpleBlockFormatterState.PatternEnd)]
    public class PaddingFormatterState : SimpleBlockFormatterState
    {
        public PaddingFormatterState(TextileFormatter formatter)
            : base(formatter)
        {
        }

        public int HeaderLevel { get; private set; } = 0;


        public override void Enter()
        {
            for (var i = 0; i < HeaderLevel; i++)
            {
                Formatter.Output.Write($"<br {FormattedStylesAndAlignment("br")}/>");
            }
        }

        public override void Exit()
        {
        }

        protected override void OnContextAcquired()
        {
            var m = Regex.Match(Tag, @"^pad(?<lvl>[0-9]+)");
            HeaderLevel = int.Parse(m.Groups["lvl"].Value);
        }

        public override void FormatLine(string input)
        {
            Formatter.Output.Write(input);
        }

        public override bool ShouldExit(string intput)
        {
            return true;
        }

        public override bool ShouldNestState(FormatterState other)
        {
            return false;
        }
    }

    /// <summary>
    /// Formatting state for headers and titles.
    /// </summary>
    [FormatterState(SimpleBlockFormatterState.PatternBegin + @"h[0-9]+" + SimpleBlockFormatterState.PatternEnd)]
    public class HeaderFormatterState : SimpleBlockFormatterState
    {
        public int HeaderLevel { get; private set; } = 0;

        public HeaderFormatterState(TextileFormatter f)
            : base(f)
        {
        }

        public override void Enter()
        {
            Formatter.Output.Write("<h" + HeaderLevel + FormattedStylesAndAlignment("h" + HeaderLevel) + ">");
        }

        public override void Exit()
        {
            Formatter.Output.WriteLine("</h" + HeaderLevel + ">");
        }

        protected override void OnContextAcquired()
        {
            var m = Regex.Match(Tag, @"^h(?<lvl>[0-9]+)");
            HeaderLevel = int.Parse(m.Groups["lvl"].Value);
        }

        public override void FormatLine(string input)
        {
            Formatter.Output.Write(input);
        }

        public override bool ShouldExit(string intput)
        {
            return true;
        }

        public override bool ShouldNestState(FormatterState other)
        {
            return false;
        }
    }
}
