namespace ASC.Web.Core
{
    [WebZone(WebZoneType.TopNavigationProductList | WebZoneType.StartProductList)]
    public interface IProduct : IWebItem
    {
        Guid ProductID { get; }

        new ProductContext Context { get; }


        void Init();

        void Shutdown();
    }
}
