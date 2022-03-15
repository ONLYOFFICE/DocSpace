namespace ASC.Web.Core
{
    public class ProductContext : WebItemContext
    {
        private IProductSubscriptionManager _sunscriptionManager;

        public new IProductSubscriptionManager SubscriptionManager
        {
            get { return _sunscriptionManager; }
            set
            {
                _sunscriptionManager = value;
                base.SubscriptionManager = value;
            }
        }
    }
}