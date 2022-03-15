namespace ASC.Web.Core.WebZones
{
    [Flags]
    public enum WebZoneType
    {
        Nowhere = 1,
        StartProductList = 2,
        TopNavigationProductList = 4,
        CustomProductList = 8,

        All = Nowhere | StartProductList | TopNavigationProductList | CustomProductList
    }

    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class WebZoneAttribute : Attribute
    {
        public WebZoneType Type { get; private set; }

        public WebZoneAttribute(WebZoneType type)
        {
            Type = type;
        }
    }

    public interface IRenderWebItem
    {
    }
}