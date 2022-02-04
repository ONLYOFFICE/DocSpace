using System.Linq;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Core;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using ASC.Notify.Recipients;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Core.Notify
{
    [Scope]
    public class StudioNotifyServiceHelper
    {
        private ICacheNotify<NotifyItem> Cache { get; }
        private StudioNotifyHelper StudioNotifyHelper { get; }
        private AuthContext AuthContext { get; }
        private TenantManager TenantManager { get; }
        public CommonLinkUtility CommonLinkUtility { get; }

        public StudioNotifyServiceHelper(
            StudioNotifyHelper studioNotifyHelper,
            AuthContext authContext,
            TenantManager tenantManager,
            CommonLinkUtility commonLinkUtility,
            ICacheNotify<NotifyItem> cache)
        {
            StudioNotifyHelper = studioNotifyHelper;
            AuthContext = authContext;
            TenantManager = tenantManager;
            CommonLinkUtility = commonLinkUtility;
            Cache = cache;
        }

        public void SendNoticeToAsync(INotifyAction action, IRecipient[] recipients, string[] senderNames, params ITagValue[] args)
        {
            SendNoticeToAsync(action, null, recipients, senderNames, false, args);
        }

        public void SendNoticeToAsync(INotifyAction action, string objectID, IRecipient[] recipients, string[] senderNames, params ITagValue[] args)
        {
            SendNoticeToAsync(action, objectID, recipients, senderNames, false, args);
        }

        public void SendNoticeToAsync(INotifyAction action, string objectID, IRecipient[] recipients, params ITagValue[] args)
        {
            SendNoticeToAsync(action, objectID, recipients, null, false, args);
        }

        public void SendNoticeToAsync(INotifyAction action, string objectID, IRecipient[] recipients, bool checkSubscription, params ITagValue[] args)
        {
            SendNoticeToAsync(action, objectID, recipients, null, checkSubscription, args);
        }

        public void SendNoticeAsync(INotifyAction action, string objectID, IRecipient recipient, params ITagValue[] args)
        {
            SendNoticeToAsync(action, objectID, new[] { recipient }, null, false, args);
        }

        public void SendNoticeAsync(INotifyAction action, string objectID, params ITagValue[] args)
        {
            var subscriptionSource = StudioNotifyHelper.NotifySource.GetSubscriptionProvider();
            var recipients = subscriptionSource.GetRecipients(action, objectID);

            SendNoticeToAsync(action, objectID, recipients, null, false, args);
        }

        public void SendNoticeAsync(INotifyAction action, string objectID, IRecipient recipient, bool checkSubscription, params ITagValue[] args)
        {
            SendNoticeToAsync(action, objectID, new[] { recipient }, null, checkSubscription, args);
        }

        public void SendNoticeToAsync(INotifyAction action, string objectID, IRecipient[] recipients, string[] senderNames, bool checkSubsciption, params ITagValue[] args)
        {
            var item = new NotifyItem
            {
                TenantId = TenantManager.GetCurrentTenant().TenantId,
                UserId = AuthContext.CurrentAccount.ID.ToString(),
                Action = (NotifyAction)action,
                CheckSubsciption = checkSubsciption,
                BaseUrl = CommonLinkUtility.GetFullAbsolutePath("")
            };

            if (objectID != null)
            {
                item.ObjectId = objectID;
            }

            if (recipients != null)
            {
                foreach (var r in recipients)
                {
                    var recipient = new Recipient { Id = r.ID, Name = r.Name };
                    if (r is IDirectRecipient d)
                    {
                        recipient.Addresses.AddRange(d.Addresses);
                        recipient.CheckActivation = d.CheckActivation;
                    }

                    if (r is IRecipientsGroup g)
                    {
                        recipient.IsGroup = true;
                    }

                    item.Recipients.Add(recipient);
                }
            }

            if (senderNames != null)
            {
                item.SenderNames.AddRange(senderNames);
            }

            if (args != null)
            {
                item.Tags.AddRange(args.Select(r => new Tag { Tag_ = r.Tag, Value = r.Value.ToString() }));
            }

            Cache.Publish(item, CacheNotifyAction.Any);
        }
    }
}