/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
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
using System.Data;
using System.Linq;
using System.Security;
using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Mail.Core.Entities;
using ASC.Mail.Models;
using ASC.Mail.Server.Core.Entities;
using ASC.Mail.Utils;
using ASC.Web.Core;
using Microsoft.Extensions.Options;
using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.Mail.Core.Engine
{
    public class ServerMailgroupEngine
    {
        public int Tenant
        {
            get
            {
                return TenantManager.GetCurrentTenant().TenantId;
            }
        }

        public string User
        {
            get
            {
                return SecurityContext.CurrentAccount.ID.ToString();
            }
        }

        private bool IsAdmin
        {
            get
            {
                return WebItemSecurity.IsProductAdministrator(WebItemManager.MailProductID, SecurityContext.CurrentAccount.ID);
            }
        }

        public SecurityContext SecurityContext { get; }
        public TenantManager TenantManager { get; }
        public DaoFactory DaoFactory { get; }
        public CacheEngine CacheEngine { get; }
        public CoreBaseSettings CoreBaseSettings { get; }
        public WebItemSecurity WebItemSecurity { get; }
        public IServiceProvider ServiceProvider { get; }

        public ILog Log { get; private set; }

        public ServerMailgroupEngine(
            SecurityContext securityContext,
            TenantManager tenantManager,
            DaoFactory daoFactory,
            CacheEngine cacheEngine,
            CoreBaseSettings coreBaseSettings,
            WebItemSecurity webItemSecurity,
            IServiceProvider serviceProvider,
            IOptionsMonitor<ILog> option)
        {
            SecurityContext = securityContext;
            TenantManager = tenantManager;

            DaoFactory = daoFactory;
            CacheEngine = cacheEngine;
            CoreBaseSettings = coreBaseSettings;
            WebItemSecurity = webItemSecurity;
            ServiceProvider = serviceProvider;

            Log = option.Get("ASC.Mail.ServerMailgroupEngine");
        }

        public List<ServerDomainGroupData> GetMailGroups()
        {
            if (!IsAdmin)
                throw new SecurityException("Need admin privileges.");

            var domains = DaoFactory.ServerDomainDao.GetDomains();

            var groups = DaoFactory.ServerGroupDao.GetList();

            var list = from serverGroup in groups
                       let address = DaoFactory.ServerAddressDao.Get(serverGroup.AddressId)
                       let domain = domains.FirstOrDefault(d => d.Id == address.DomainId)
                       where domain != null
                       let serverGroupAddress = ServerMailboxEngine.ToServerDomainAddressData(address, domain)
                       let serverGroupAddresses =
                           DaoFactory.ServerAddressDao.GetGroupAddresses(serverGroup.Id)
                               .ConvertAll(a => ServerMailboxEngine.ToServerDomainAddressData(a, domain))
                       select ToServerDomainGroupData(serverGroup.Id, serverGroupAddress, serverGroupAddresses);


            return list.ToList();
        }

        public ServerDomainGroupData CreateMailGroup(string name, int domainId, List<int> addressIds)
        {
            if (!IsAdmin)
                throw new SecurityException("Need admin privileges.");

            if (string.IsNullOrEmpty(name))
                throw new ArgumentException(@"Invalid mailgroup name.", "name");

            if (domainId < 0)
                throw new ArgumentException(@"Invalid domain id.", "domainId");

            if (name.Length > 64)
                throw new ArgumentException(@"Local part of mailgroup exceed limitation of 64 characters.", "name");

            if (!Parser.IsEmailLocalPartValid(name))
                throw new ArgumentException(@"Incorrect group name.", "name");

            if (!addressIds.Any())
                throw new ArgumentException(@"Empty collection of address_ids.", "addressIds");

            var mailgroupName = name.ToLowerInvariant();

            var serverDomain = DaoFactory.ServerDomainDao.GetDomain(domainId);

            if (serverDomain.Tenant == Defines.SHARED_TENANT_ID)
                throw new InvalidOperationException("Creating mail group is not allowed for shared domain.");

            if (DaoFactory.ServerAddressDao.IsAddressAlreadyRegistered(mailgroupName, serverDomain.Name))
            {
                throw new DuplicateNameException("You want to create a group with already existing address.");
            }

            var utcNow = DateTime.UtcNow;

            var address = new ServerAddress
            {
                Id = 0,
                Tenant = Tenant,
                MailboxId = -1,
                DomainId = serverDomain.Id,
                AddressName = mailgroupName,
                IsAlias = false,
                IsMailGroup = true,
                DateCreated = utcNow
            };

            var groupEmail = string.Format("{0}@{1}", mailgroupName, serverDomain.Name);

            var groupAddressData = ServerMailboxEngine.ToServerDomainAddressData(address, groupEmail);

            var newGroupMembers = DaoFactory.ServerAddressDao.GetList(addressIds);

            var newGroupMemberIds = newGroupMembers.ConvertAll(m => m.Id);

            var newGroupMemberDataList =
                newGroupMembers.ConvertAll(m =>
                    ServerMailboxEngine.ToServerDomainAddressData(m,
                        string.Format("{0}@{1}", m.AddressName, serverDomain.Name)));

            var goTo = string.Join(",",
                newGroupMembers.Select(m => string.Format("{0}@{1}", m.AddressName, serverDomain.Name)));

            var server = DaoFactory.ServerDao.Get(Tenant);

            var engine = new Server.Core.ServerEngine(server.Id, server.ConnectionString);

            var group = new ServerGroup
            {
                Id = 0,
                Tenant = Tenant,
                Address = groupEmail,
                AddressId = 0,
                DateCreated = utcNow
            };

            using (var tx = DaoFactory.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                address.Id = DaoFactory.ServerAddressDao.Save(address);

                group.AddressId = address.Id;

                group.Id = DaoFactory.ServerGroupDao.Save(group);

                DaoFactory.ServerAddressDao.AddAddressesToMailGroup(group.Id, newGroupMemberIds);

                var serverAddress = new Alias
                {
                    Name = "",
                    Address = groupEmail,
                    Goto = goTo,
                    Domain = serverDomain.Name,
                    Active = true,
                    Islist = true,
                    Modified = utcNow,
                    Created = utcNow
                };

                engine.SaveAlias(serverAddress);

                tx.Commit();
            }

            CacheEngine.ClearAll();

            return ToServerDomainGroupData(group.Id, groupAddressData, newGroupMemberDataList);
        }

        public ServerDomainGroupData AddMailGroupMember(int mailgroupId, int addressId)
        {
            if (!IsAdmin)
                throw new SecurityException("Need admin privileges.");

            if (addressId < 0)
                throw new ArgumentException(@"Invalid address id.", "addressId");

            if (mailgroupId < 0)
                throw new ArgumentException(@"Invalid mailgroup id.", "mailgroupId");

            var group = DaoFactory.ServerGroupDao.Get(mailgroupId);

            if (group == null)
                throw new Exception("Group not found");

            var groupMembers = DaoFactory.ServerAddressDao.GetGroupAddresses(mailgroupId);

            if (groupMembers.Exists(a => a.Id == addressId))
                throw new DuplicateNameException("Member already exists");

            var newMemberAddress = DaoFactory.ServerAddressDao.Get(addressId);

            if (newMemberAddress == null)
                throw new Exception("Member not found");

            var server = DaoFactory.ServerDao.Get(Tenant);

            var engine = new Server.Core.ServerEngine(server.Id, server.ConnectionString);

            var utcNow = DateTime.UtcNow;

            ServerAddress groupAddress;
            string groupEmail;
            List<ServerDomainAddressData> newGroupMemberDataList;

            using (var tx = DaoFactory.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                DaoFactory.ServerAddressDao.AddAddressesToMailGroup(mailgroupId, new List<int> { addressId });

                groupMembers.Add(newMemberAddress);

                groupAddress = DaoFactory.ServerAddressDao.Get(group.AddressId);

                var serverDomain = DaoFactory.ServerDomainDao.GetDomain(groupAddress.DomainId);

                var goTo = string.Join(",",
                    groupMembers.Select(m => string.Format("{0}@{1}", m.AddressName, serverDomain.Name)));

                groupEmail = string.Format("{0}@{1}", groupAddress.AddressName, serverDomain.Name);

                newGroupMemberDataList =
                    groupMembers.ConvertAll(m =>
                        ServerMailboxEngine.ToServerDomainAddressData(m,
                            string.Format("{0}@{1}", m.AddressName, serverDomain.Name)));

                var serverAddress = new Alias
                {
                    Name = "",
                    Address = groupEmail,
                    Goto = goTo,
                    Domain = serverDomain.Name,
                    Active = true,
                    Islist = true,
                    Modified = utcNow,
                    Created = utcNow
                };

                engine.SaveAlias(serverAddress);

                tx.Commit();
            }

            var groupAddressData = ServerMailboxEngine.ToServerDomainAddressData(groupAddress, groupEmail);

            CacheEngine.ClearAll();

            return ToServerDomainGroupData(group.Id, groupAddressData, newGroupMemberDataList);
        }

        public void RemoveMailGroupMember(int mailgroupId, int addressId)
        {
            if (!IsAdmin)
                throw new SecurityException("Need admin privileges.");

            if (addressId < 0)
                throw new ArgumentException(@"Invalid address id.", "addressId");

            if (mailgroupId < 0)
                throw new ArgumentException(@"Invalid mailgroup id.", "mailgroupId");

            var group = DaoFactory.ServerGroupDao.Get(mailgroupId);

            if (group == null)
                throw new Exception("Group not found");

            var groupMembers = DaoFactory.ServerAddressDao.GetGroupAddresses(mailgroupId);

            var removeMember = groupMembers.FirstOrDefault(a => a.Id == addressId);

            if (removeMember == null)
                throw new ArgumentException("Member not found");

            groupMembers.Remove(removeMember);

            if (groupMembers.Count == 0)
                throw new Exception("Can't remove last member; Remove group.");

            var server = DaoFactory.ServerDao.Get(Tenant);

            var groupAddress = DaoFactory.ServerAddressDao.Get(group.AddressId);

            var serverDomain = DaoFactory.ServerDomainDao.GetDomain(groupAddress.DomainId);

            var engine = new Server.Core.ServerEngine(server.Id, server.ConnectionString);

            var utcNow = DateTime.UtcNow;

            using (var tx = DaoFactory.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                DaoFactory.ServerAddressDao.DeleteAddressFromMailGroup(mailgroupId, addressId);

                var goTo = string.Join(",",
                    groupMembers.Select(m => string.Format("{0}@{1}", m.AddressName, serverDomain.Name)));

                var groupEmail = string.Format("{0}@{1}", groupAddress.AddressName, serverDomain.Name);

                var serverAddress = new Alias
                {
                    Name = "",
                    Address = groupEmail,
                    Goto = goTo,
                    Domain = serverDomain.Name,
                    Active = true,
                    Islist = true,
                    Modified = utcNow,
                    Created = group.DateCreated
                };

                engine.SaveAlias(serverAddress);

                tx.Commit();
            }

            CacheEngine.ClearAll();
        }

        public void RemoveMailGroup(int id)
        {
            if (!IsAdmin)
                throw new SecurityException("Need admin privileges.");

            if (id < 0)
                throw new ArgumentException(@"Invalid mailgroup id.", "id");

            var group = DaoFactory.ServerGroupDao.Get(id);

            if (group == null)
                throw new Exception("Group not found");

            var server = DaoFactory.ServerDao.Get(Tenant);

            var engine = new Server.Core.ServerEngine(server.Id, server.ConnectionString);

            using (var tx = DaoFactory.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                DaoFactory.ServerGroupDao.Delete(id);

                DaoFactory.ServerAddressDao.DeleteAddressesFromMailGroup(id);

                DaoFactory.ServerAddressDao.Delete(group.AddressId);

                engine.RemoveAlias(group.Address);

                tx.Commit();
            }

            CacheEngine.ClearAll();
        }

        public static ServerDomainGroupData ToServerDomainGroupData(int groupId, ServerDomainAddressData address, List<ServerDomainAddressData> addresses)
        {
            var group = new ServerDomainGroupData
            {
                Id = groupId,
                Address = address,
                Addresses = addresses
            };

            return group;
        }
    }

    public static class ServerMailgroupEngineExtension
    {
        public static DIHelper AddServerMailgroupEngineService(this DIHelper services)
        {
            services.TryAddScoped<ServerMailgroupEngine>();

            services.AddSecurityContextService()
                .AddTenantManagerService()
                .AddDaoFactoryService()
                .AddCacheEngineService()
                .AddCoreBaseSettingsService()
                .AddWebItemSecurity();

            return services;
        }
    }
}
