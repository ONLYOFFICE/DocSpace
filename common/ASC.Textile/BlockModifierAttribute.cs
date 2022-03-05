namespace Textile;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class BlockModifierAttribute : Attribute
{
    public BlockModifierAttribute()
    {
    }
}
