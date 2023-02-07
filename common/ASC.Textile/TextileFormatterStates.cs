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
    #region State Registration

    private static readonly List<Type> _registeredStates = new List<Type>();
    private static readonly List<FormatterStateAttribute> _registeredStatesAttributes = new List<FormatterStateAttribute>();

    public static void RegisterFormatterState(Type formatterStateType)
    {
        if (!formatterStateType.IsSubclassOf(typeof(FormatterState)))
        {
            throw new ArgumentException("The formatter state must be a sub-public class of FormatterStateBase.");
        }

        if (formatterStateType.GetConstructor(new Type[] { typeof(TextileFormatter) }) == null)
        {
            throw new ArgumentException("The formatter state must have a constructor that takes a TextileFormatter reference.");
        }

        var att = FormatterStateAttribute.Get(formatterStateType);
        if (att == null)
        {
            throw new ArgumentException("The formatter state must have the FormatterStateAttribute.");
        }

        _registeredStates.Add(formatterStateType);
        _registeredStatesAttributes.Add(att);
    }

    #endregion

    #region State Management

    private readonly List<Type> _disabledFormatterStates = new List<Type>();
    private readonly Stack<FormatterState> _stackOfStates = new Stack<FormatterState>();

    private bool IsFormatterStateEnabled(Type type)
    {
        return !_disabledFormatterStates.Contains(type);
    }

    private void SwitchFormatterState(Type type, bool onOff)
    {
        if (onOff)
        {
            _disabledFormatterStates.Remove(type);
        }
        else if (!_disabledFormatterStates.Contains(type))
        {
            _disabledFormatterStates.Add(type);
        }
    }

    /// <summary>
    /// Pushes a new state on the stack.
    /// </summary>
    /// <param name="s">The state to push.</param>
    /// The state will be entered automatically.
    private void PushState(FormatterState s)
    {
        _stackOfStates.Push(s);
        s.Enter();
    }

    /// <summary>
    /// Removes the last state from the stack.
    /// </summary>
    /// The state will be exited automatically.
    private void PopState()
    {
        _stackOfStates.Peek().Exit();
        _stackOfStates.Pop();
    }

    /// <summary>
    /// The current state, if any.
    /// </summary>
    internal FormatterState CurrentState
    {
        get
        {
            if (_stackOfStates.Count > 0)
            {
                return _stackOfStates.Peek();
            }
            else
            {
                return null;
            }
        }
    }

    internal void ChangeState(FormatterState formatterState)
    {
        if (CurrentState != null && CurrentState.GetType() == formatterState.GetType())
        {
            if (!CurrentState.ShouldNestState(formatterState))
            {
                return;
            }
        }
        PushState(formatterState);
    }

    #endregion

    #region State Handling

    /// <summary>
    /// Parses the string and updates the state accordingly.
    /// </summary>
    /// <param name="input">The text to process.</param>
    /// <returns>The text, ready for formatting.</returns>
    /// This method modifies the text because it removes some
    /// syntax stuff. Maybe the states themselves should handle
    /// their own syntax and remove it?
    private string HandleFormattingState(string input)
    {
        for (var i = 0; i < _registeredStates.Count; i++)
        {
            var type = _registeredStates[i];
            if (IsFormatterStateEnabled(type))
            {
                var att = _registeredStatesAttributes[i];
                var m = Regex.Match(input, att.Pattern);
                if (m.Success)
                {
                    var formatterState = (FormatterState)Activator.CreateInstance(type, this);
                    return formatterState.Consume(input, m);
                }
            }
        }

        // Default, when no block is specified, we ask the current state, or
        // use the paragraph state.
        if (CurrentState != null)
        {
            if (CurrentState.FallbackFormattingState != null)
            {
                var formatterState = (FormatterState)Activator.CreateInstance(CurrentState.FallbackFormattingState, this);
                ChangeState(formatterState);
            }
            // else, the current state doesn't want to be superceded by
            // a new one. We'll leave him be.
        }
        else
        {
            ChangeState(new ParagraphFormatterState(this));
        }
        return input;
    }

    #endregion
}
