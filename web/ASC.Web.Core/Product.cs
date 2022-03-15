//using ASC.Web.Core.Client.HttpHandlers;

namespace ASC.Web.Core
{
    [WebZone(WebZoneType.TopNavigationProductList | WebZoneType.StartProductList)]
    public abstract class Product : IProduct
    {
        public abstract Guid ProductID { get; }

        public abstract string Name { get; }

        public abstract string Description { get; }

        public abstract string StartURL { get; }

        public abstract string HelpURL { get; }

        public abstract string ProductClassName { get; }

        public abstract bool Visible { get; }

        public abstract void Init();

        public abstract ProductContext Context { get; }

        public virtual void Shutdown() { }

        public virtual string ExtendedDescription { get { return Description; } }

        WebItemContext IWebItem.Context { get { return ((IProduct)this).Context; } }

        Guid IWebItem.ID { get { return ProductID; } }

        public virtual bool IsPrimary { get => false; }

        public abstract string ApiURL { get; }
    }
}
