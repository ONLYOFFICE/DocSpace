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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Web.Core.WebZones;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ASC.Web.Core
{
    [Flags]
    public enum ItemAvailableState
    {
        Normal = 1,
        Disabled = 2,
        All = Normal | Disabled
    }

    [Singletone]
    public class WebItemManager
    {
        private readonly ILog log;

        private ConcurrentDictionary<Guid, IWebItem> items;
        private ConcurrentDictionary<Guid, IWebItem> Items
        {
            get
            {
                if (lazyItems.IsValueCreated)
                {
                    return items;
                }

                return items = lazyItems.Value;
            }
        }
        private readonly Lazy<ConcurrentDictionary<Guid, IWebItem>> lazyItems;
        private readonly List<string> disableItem;

        public static Guid CommunityProductID
        {
            get { return new Guid("{EA942538-E68E-4907-9394-035336EE0BA8}"); }
        }

        public static Guid ProjectsProductID
        {
            get { return new Guid("{1e044602-43b5-4d79-82f3-fd6208a11960}"); }
        }

        public static Guid CRMProductID
        {
            get { return new Guid("{6743007C-6F95-4d20-8C88-A8601CE5E76D}"); }
        }

        public static Guid DocumentsProductID
        {
            get { return new Guid("{E67BE73D-F9AE-4ce1-8FEC-1880CB518CB4}"); }
        }

        public static Guid PeopleProductID
        {
            get { return new Guid("{F4D98AFD-D336-4332-8778-3C6945C81EA0}"); }
        }

        public static Guid MailProductID
        {
            get { return new Guid("{2A923037-8B2D-487b-9A22-5AC0918ACF3F}"); }
        }

        public static Guid CalendarProductID
        {
            get { return new Guid("{32D24CB5-7ECE-4606-9C94-19216BA42086}"); }
        }

        public static Guid BirthdaysProductID
        {
            get { return new Guid("{37620AE5-C40B-45ce-855A-39DD7D76A1FA}"); }
        }

        public static Guid TalkProductID
        {
            get { return new Guid("{BF88953E-3C43-4850-A3FB-B1E43AD53A3E}"); }
        }

        public static Guid VoipModuleID
        {
            get { return new Guid("{46CFA73A-F320-46CF-8D5B-CD82E1D67F26}"); }
        }

        public IServiceProvider ServiceProvider { get; }
        private IConfiguration Configuration { get; }


        public IWebItem this[Guid id]
        {
            get
            {
                Items.TryGetValue(id, out var i);
                return i;
            }
        }

        public WebItemManager(IServiceProvider serviceProvider, IConfiguration configuration, IOptionsMonitor<ILog> options)
        {
            ServiceProvider = serviceProvider;
            Configuration = configuration;
            log = options.Get("ASC.Web");
            disableItem = (Configuration["web:disabled-items"] ?? "").Split(",").ToList();
            lazyItems = new Lazy<ConcurrentDictionary<Guid, IWebItem>>(LoadItems, System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);
        }

        private ConcurrentDictionary<Guid, IWebItem> LoadItems()
        {
            var result = new ConcurrentDictionary<Guid, IWebItem>();

            foreach (var webitem in ServiceProvider.GetService<IEnumerable<IWebItem>>())
            {
                var file = webitem.ID.ToString();
                try
                {
                    if (DisabledWebItem(file)) continue;

                    RegistryItem(result, webitem);
                }
                catch (Exception exc)
                {
                    log.Error(string.Format("Couldn't load web item {0}", file), exc);
                }
            }

            return result;
        }

        private void RegistryItem(ConcurrentDictionary<Guid, IWebItem> result, IWebItem webitem)
        {
            if (webitem != null && !result.TryGetValue(webitem.ID, out _))
            {
                if (webitem is IAddon addon)
                {
                    addon.Init();
                }
                if (webitem is IProduct product)
                {
                    product.Init();
                }

                if (webitem is IModule module)
                {
                    if (module.Context != null && module.Context.SearchHandler != null)
                    {
                        //TODO
                        //SearchHandlerManager.Registry(module.Context.SearchHandler);
                    }
                }

                result.TryAdd(webitem.ID, webitem);
                log.DebugFormat("Web item {0} loaded", webitem.Name);
            }
        }

        public Guid GetParentItemID(Guid itemID)
        {
            return this[itemID] is IModule m ? m.ProjectId : Guid.Empty;
        }

        public int GetSortOrder(IWebItem item)
        {
            return item != null && item.Context != null ? item.Context.DefaultSortOrder : 0;
        }

        public List<IWebItem> GetItemsAll()
        {
            var list = Items.Values.ToList();
            list.Sort((x, y) => GetSortOrder(x).CompareTo(GetSortOrder(y)));
            return list;
        }

        public List<T> GetItemsAll<T>() where T : IWebItem
        {
            return GetItemsAll().OfType<T>().ToList();
        }

        private bool DisabledWebItem(string name)
        {
            return disableItem.Contains(name);
        }
    }

    [Scope]
    public class WebItemManagerSecurity
    {
        private WebItemSecurity WebItemSecurity { get; }
        private AuthContext AuthContext { get; }
        private WebItemManager WebItemManager { get; }

        public WebItemManagerSecurity(WebItemSecurity webItemSecurity, AuthContext authContext, WebItemManager webItemManager)
        {
            WebItemSecurity = webItemSecurity;
            AuthContext = authContext;
            WebItemManager = webItemManager;
        }

        public List<IWebItem> GetItems(WebZoneType webZone)
        {
            return GetItems(webZone, ItemAvailableState.Normal);
        }

        public List<IWebItem> GetItems(WebZoneType webZone, ItemAvailableState avaliableState)
        {
            var copy = WebItemManager.GetItemsAll().ToList();
            var list = copy.Where(item =>
                {
                    if ((avaliableState & ItemAvailableState.Disabled) != ItemAvailableState.Disabled && item.IsDisabled(WebItemSecurity, AuthContext))
                    {
                        return false;
                    }
                    var attribute = (WebZoneAttribute)Attribute.GetCustomAttribute(item.GetType(), typeof(WebZoneAttribute), true);
                    return attribute != null && (attribute.Type & webZone) != 0;
                }).ToList();

            list.Sort((x, y) => WebItemManager.GetSortOrder(x).CompareTo(WebItemManager.GetSortOrder(y)));
            return list;
        }

        public List<IWebItem> GetSubItems(Guid parentItemID)
        {
            return GetSubItems(parentItemID, ItemAvailableState.Normal);
        }

        public List<IWebItem> GetSubItems(Guid parentItemID, ItemAvailableState avaliableState)
        {
            return GetItems(WebZoneType.All, avaliableState).OfType<IModule>()
                                                            .Where(p => p.ProjectId == parentItemID)
                                                            .Cast<IWebItem>()
                                                            .ToList();
        }
    }
}