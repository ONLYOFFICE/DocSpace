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
    #region State Registration

    private static readonly List<Type> s_registeredStates = new List<Type>();
    private static readonly List<FormatterStateAttribute> s_registeredStatesAttributes = new List<FormatterStateAttribute>();

    public static void RegisterFormatterState(Type formatterStateType)
    {
        if (!formatterStateType.IsSubclassOf(typeof(FormatterState)))
            throw new ArgumentException("The formatter state must be a sub-public class of FormatterStateBase.");

        if (formatterStateType.GetConstructor(new Type[] { typeof(TextileFormatter) }) == null)
            throw new ArgumentException("The formatter state must have a constructor that takes a TextileFormatter reference.");

        var att = FormatterStateAttribute.Get(formatterStateType);
        if (att == null)
            throw new ArgumentException("The formatter state must have the FormatterStateAttribute.");

        s_registeredStates.Add(formatterStateType);
        s_registeredStatesAttributes.Add(att);
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
            _disabledFormatterStates.Remove(type);
        else if (!_disabledFormatterStates.Contains(type))
            _disabledFormatterStates.Add(type);
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
                return _stackOfStates.Peek();
            else
                return null;
        }
    }

    internal void ChangeState(FormatterState formatterState)
    {
        if (CurrentState != null && CurrentState.GetType() == formatterState.GetType())
        {
            if (!CurrentState.ShouldNestState(formatterState))
                return;
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
        for (var i = 0; i < s_registeredStates.Count; i++)
        {
            var type = s_registeredStates[i];
            if (IsFormatterStateEnabled(type))
            {
                var att = s_registeredStatesAttributes[i];
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
            ChangeState(new States.ParagraphFormatterState(this));
        }
        return input;
    }

    #endregion
}
