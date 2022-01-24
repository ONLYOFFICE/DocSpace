/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

using ASC.Notify;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using ASC.Notify.Recipients;

namespace ASC.Core.Notify
{
    public abstract class NotifySource : INotifySource
    {
        private readonly IDictionary<CultureInfo, IActionProvider> actions = new Dictionary<CultureInfo, IActionProvider>();

        private readonly IDictionary<CultureInfo, IPatternProvider> patterns = new Dictionary<CultureInfo, IPatternProvider>();


        protected ISubscriptionProvider SubscriprionProvider;

        protected IRecipientProvider RecipientsProvider;


        protected IActionProvider ActionProvider
        {
            get { return GetActionProvider(); }
        }

        protected IPatternProvider PatternProvider
        {
            get { return GetPatternProvider(); }
        }


        public string ID
        {
            get;
            private set;
        }
        private UserManager UserManager { get; }
        private SubscriptionManager SubscriptionManager { get; }

        protected NotifySource(string id, UserManager userManager, IRecipientProvider recipientsProvider, SubscriptionManager subscriptionManager)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            ID = id;
            UserManager = userManager;
            RecipientsProvider = recipientsProvider;
            SubscriptionManager = subscriptionManager;
        }

        protected NotifySource(Guid id, UserManager userManager, IRecipientProvider recipientsProvider, SubscriptionManager subscriptionManager)
            : this(id.ToString(), userManager, recipientsProvider, subscriptionManager)
        {
        }

        public IActionProvider GetActionProvider()
        {
            lock (actions)
            {
                var culture = Thread.CurrentThread.CurrentCulture;
                if (!actions.ContainsKey(culture))
                {
                    actions[culture] = CreateActionProvider();
                }
                return actions[culture];
            }
        }

        public IPatternProvider GetPatternProvider()
        {
            lock (patterns)
            {
                var culture = Thread.CurrentThread.CurrentCulture;
                if (Thread.CurrentThread.CurrentUICulture != culture)
                {
                    Thread.CurrentThread.CurrentUICulture = culture;
                }
                if (!patterns.ContainsKey(culture))
                {
                    patterns[culture] = CreatePatternsProvider();
                }
                return patterns[culture];
            }
        }

        public IRecipientProvider GetRecipientsProvider()
        {
            return CreateRecipientsProvider();
        }

        public ISubscriptionProvider GetSubscriptionProvider()
        {
            return CreateSubscriptionProvider();
        }


        protected abstract IPatternProvider CreatePatternsProvider();

        protected abstract IActionProvider CreateActionProvider();


        protected virtual ISubscriptionProvider CreateSubscriptionProvider()
        {
            var subscriptionProvider = new DirectSubscriptionProvider(ID, SubscriptionManager, RecipientsProvider);
            return new TopSubscriptionProvider(RecipientsProvider, subscriptionProvider, WorkContext.DefaultClientSenders) ??
                throw new NotifyException("Provider ISubscriprionProvider not instanced.");
        }

        protected virtual IRecipientProvider CreateRecipientsProvider()
        {
            return new RecipientProviderImpl(UserManager) ?? throw new NotifyException("Provider IRecipientsProvider not instanced.");
        }
    }
}