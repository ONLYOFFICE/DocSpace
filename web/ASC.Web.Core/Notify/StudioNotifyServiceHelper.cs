// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.Web.Core.Notify;

[Scope]
public class StudioNotifyServiceHelper
{
    private readonly ICacheNotify<NotifyItem> _cache;
    private readonly StudioNotifyHelper _studioNotifyHelper;
    private readonly AuthContext _authContext;
    private readonly TenantManager _tenantManager;
    private readonly CommonLinkUtility _commonLinkUtility;

    public StudioNotifyServiceHelper(
        StudioNotifyHelper studioNotifyHelper,
        AuthContext authContext,
        TenantManager tenantManager,
        CommonLinkUtility commonLinkUtility,
        ICacheNotify<NotifyItem> cache)
    {
        _studioNotifyHelper = studioNotifyHelper;
        _authContext = authContext;
        _tenantManager = tenantManager;
        _commonLinkUtility = commonLinkUtility;
        _cache = cache;
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
        var subscriptionSource = _studioNotifyHelper.NotifySource.GetSubscriptionProvider();
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
            TenantId = _tenantManager.GetCurrentTenant().Id,
            UserId = _authContext.CurrentAccount.ID.ToString(),
            Action = (NotifyAction)action,
            CheckSubsciption = checkSubsciption,
            BaseUrl = _commonLinkUtility.GetFullAbsolutePath("")
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

        _cache.Publish(item, CacheNotifyAction.Any);
    }
}
