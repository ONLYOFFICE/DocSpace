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
