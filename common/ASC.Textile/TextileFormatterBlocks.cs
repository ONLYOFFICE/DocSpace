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
    #region Block Modifiers Registration

    private static readonly List<BlockModifier> _blockModifiers = new List<BlockModifier>();
    private static readonly List<Type> _blockModifiersTypes = new List<Type>();

    public static void RegisterBlockModifier(BlockModifier blockModifer)
    {
        _blockModifiers.Add(blockModifer);
        _blockModifiersTypes.Add(blockModifer.GetType());
    }

    #endregion

    #region Block Modifiers Management

    private readonly List<Type> _disabledBlockModifiers = new List<Type>();

    public bool IsBlockModifierEnabled(Type type)
    {
        return !_disabledBlockModifiers.Contains(type);
    }

    public void SwitchBlockModifier(Type type, bool onOff)
    {
        if (onOff)
            _disabledBlockModifiers.Remove(type);
        else if (!_disabledBlockModifiers.Contains(type))
            _disabledBlockModifiers.Add(type);
    }

    #endregion
}
