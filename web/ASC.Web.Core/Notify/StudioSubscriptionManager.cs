namespace ASC.Web.Studio.Core.Notify
{
    internal class StudioSubscriptionManager : ISubscriptionManager
    {
        public StudioSubscriptionManager(StudioNotifyHelper studioNotifyHelper)
        {
            StudioNotifyHelper = studioNotifyHelper;
        }

        #region ISubscriptionManager Members

        public List<SubscriptionObject> GetSubscriptionObjects(Guid subItem)
        {
            return new List<SubscriptionObject>();
        }

        public List<SubscriptionType> GetSubscriptionTypes()
        {
            var types = new List<SubscriptionType>
            {
                new SubscriptionType()
                {
                    ID = new Guid("{148B5E30-C81A-4ff8-B749-C46BAE340093}"),
                    Name = Resource.WhatsNewSubscriptionName,
                    NotifyAction = Actions.SendWhatsNew,
                    Single = true
                }
            };

            var astype = new SubscriptionType()
            {
                ID = new Guid("{A4FFC01F-BDB5-450e-88C4-03FED17D67C5}"),
                Name = Resource.AdministratorNotifySenderTypeName,
                NotifyAction = Actions.SendWhatsNew,
                Single = false
            };

            types.Add(astype);

            return types;
        }

        public ISubscriptionProvider SubscriptionProvider
        {
            get { return StudioNotifyHelper.SubscriptionProvider; }
        }

        private StudioNotifyHelper StudioNotifyHelper { get; }

        #endregion
    }
}
