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
using System.Security.Cryptography;
using System.Text;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Common.Threading;
using ASC.Core;
using ASC.Core.Users;
using ASC.Data.Storage;
//using ASC.Mail.Core.Engine;
using ASC.MessagingSystem;
//using ASC.Web.CRM.Core;
using ASC.Web.Core;
//using ASC.Web.Files.Services.WCFService;
using ASC.Web.Studio.Core.Notify;
//using CrmDaoFactory = ASC.CRM.Core.Dao.DaoFactory;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace ASC.Data.Reassigns
{
    [Transient]
    public class RemoveProgressItem : DistributedTaskProgress
    {
        private readonly IDictionary<string, StringValues> _httpHeaders;

        private int _tenantId;
        private Guid _currentUserId;
        private bool _notify;

        //private readonly IFileStorageService _docService;
        //private readonly MailGarbageEngine _mailEraser;
        public Guid FromUser { get; private set; }
        private IServiceProvider ServiceProvider { get; }
        public UserInfo User { get; private set; }

        public RemoveProgressItem(
            IServiceProvider serviceProvider,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpHeaders = QueueWorker.GetHttpHeaders(httpContextAccessor.HttpContext.Request);
            ServiceProvider = serviceProvider;


            //_docService = Web.Files.Classes.Global.FileStorageService;
            //_mailEraser = new MailGarbageEngine();
        }

        public void Init(int tenantId, UserInfo user, Guid currentUserId, bool notify)
        {
            _tenantId = tenantId;
            User = user;
            FromUser = user.ID;
            _currentUserId = currentUserId;
            _notify = notify;

            Id = QueueWorkerRemove.GetProgressItemId(tenantId, FromUser);
            Status = DistributedTaskStatus.Created;
            Exception = null;
            Percentage = 0;
            IsCompleted = false;
        }

        protected override void DoJob()
        {
            using var scope = ServiceProvider.CreateScope();
            var scopeClass = scope.ServiceProvider.GetService<RemoveProgressItemScope>();
            var (tenantManager, coreBaseSettings, messageService, studioNotifyService, securityContext, userManager, messageTarget, webItemManagerSecurity, storageFactory, userFormatter, options) = scopeClass;
            var logger = options.Get("ASC.Web");
            tenantManager.SetCurrentTenant(_tenantId);
            var userName = userFormatter.GetUserName(User, DisplayUserNameFormat.Default);

            try
            {
                Percentage = 0;
                Status = DistributedTaskStatus.Running;

                securityContext.AuthenticateMeWithoutCookie(_currentUserId);

                long crmSpace;
                GetUsageSpace(webItemManagerSecurity, out var docsSpace, out var mailSpace, out var talkSpace);

                logger.InfoFormat("deleting user data for {0} ", FromUser);

                logger.Info("deleting of data from documents");

                //_docService.DeleteStorage(_userId);
                Percentage = 25;
                PublishChanges();

                if (!coreBaseSettings.CustomMode)
                {
                    logger.Info("deleting of data from crm");


                    //using (var scope = DIHelper.Resolve(_tenantId))
                    //{
                    //    var crmDaoFactory = scope.Resolve<CrmDaoFactory>();
                    crmSpace = 0;// crmDaoFactory.ReportDao.GetFiles(_userId).Sum(file => file.ContentLength);
                                 //    crmDaoFactory.ReportDao.DeleteFiles(_userId);
                                 //}
                    Percentage = 50;
                }
                else
                {
                    crmSpace = 0;
                }

                PublishChanges();

                logger.Info("deleting of data from mail");

                //_mailEraser.ClearUserMail(_userId);
                Percentage = 75;
                PublishChanges();

                logger.Info("deleting of data from talk");
                DeleteTalkStorage(storageFactory);
                Percentage = 99;
                PublishChanges();

                SendSuccessNotify(studioNotifyService, messageService, messageTarget, userName, docsSpace, crmSpace, mailSpace, talkSpace);

                Percentage = 100;
                Status = DistributedTaskStatus.Completed;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Status = DistributedTaskStatus.Failted;
                Exception = ex;
                SendErrorNotify(studioNotifyService, ex.Message, userName);
            }
            finally
            {
                logger.Info("data deletion is complete");
                IsCompleted = true;
                PublishChanges();
            }
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        private void GetUsageSpace(WebItemManagerSecurity webItemManagerSecurity, out long docsSpace, out long mailSpace, out long talkSpace)
        {
            docsSpace = mailSpace = talkSpace = 0;

            var webItems = webItemManagerSecurity.GetItems(Web.Core.WebZones.WebZoneType.All, ItemAvailableState.All);

            foreach (var item in webItems)
            {
                IUserSpaceUsage manager;

                if (item.ID == WebItemManager.DocumentsProductID)
                {
                    manager = item.Context.SpaceUsageStatManager as IUserSpaceUsage;
                    if (manager == null) continue;
                    docsSpace = manager.GetUserSpaceUsageAsync(FromUser).Result;
                }

                if (item.ID == WebItemManager.MailProductID)
                {
                    manager = item.Context.SpaceUsageStatManager as IUserSpaceUsage;
                    if (manager == null) continue;
                    mailSpace = manager.GetUserSpaceUsageAsync(FromUser).Result;
                }

                if (item.ID == WebItemManager.TalkProductID)
                {
                    manager = item.Context.SpaceUsageStatManager as IUserSpaceUsage;
                    if (manager == null) continue;
                    talkSpace = manager.GetUserSpaceUsageAsync(FromUser).Result;
                }
            }
        }

        private void DeleteTalkStorage(StorageFactory storageFactory)
        {
            using var md5 = MD5.Create();
            var data = md5.ComputeHash(Encoding.Default.GetBytes(FromUser.ToString()));

            var sBuilder = new StringBuilder();

            for (int i = 0, n = data.Length; i < n; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            var md5Hash = sBuilder.ToString();

            var storage = storageFactory.GetStorage(_tenantId.ToString(CultureInfo.InvariantCulture), "talk");

            if (storage != null && storage.IsDirectoryAsync(md5Hash).Result)
            {
                storage.DeleteDirectoryAsync(md5Hash).Wait();
            }
        }

        private void SendSuccessNotify(StudioNotifyService studioNotifyService, MessageService messageService, MessageTarget messageTarget, string userName, long docsSpace, long crmSpace, long mailSpace, long talkSpace)
        {
            if (_notify)
                studioNotifyService.SendMsgRemoveUserDataCompleted(_currentUserId, User, userName,
                                                                            docsSpace, crmSpace, mailSpace, talkSpace);

            if (_httpHeaders != null)
                messageService.Send(_httpHeaders, MessageAction.UserDataRemoving, messageTarget.Create(FromUser), new[] { userName });
            else
                messageService.Send(MessageAction.UserDataRemoving, messageTarget.Create(FromUser), userName);
        }

        private void SendErrorNotify(StudioNotifyService studioNotifyService, string errorMessage, string userName)
        {
            if (!_notify) return;

            studioNotifyService.SendMsgRemoveUserDataFailed(_currentUserId, User, userName, errorMessage);
        }
    }

    [Scope]
    public class RemoveProgressItemScope
    {
        private TenantManager TenantManager { get; }
        private CoreBaseSettings CoreBaseSettings { get; }
        private MessageService MessageService { get; }
        private StudioNotifyService StudioNotifyService { get; }
        private SecurityContext SecurityContext { get; }
        private UserManager UserManager { get; }
        private MessageTarget MessageTarget { get; }
        private WebItemManagerSecurity WebItemManagerSecurity { get; }
        private StorageFactory StorageFactory { get; }
        private UserFormatter UserFormatter { get; }
        private IOptionsMonitor<ILog> Options { get; }

        public RemoveProgressItemScope(TenantManager tenantManager,
            CoreBaseSettings coreBaseSettings,
            MessageService messageService,
            StudioNotifyService studioNotifyService,
            SecurityContext securityContext,
            UserManager userManager,
            MessageTarget messageTarget,
            WebItemManagerSecurity webItemManagerSecurity,
            StorageFactory storageFactory,
            UserFormatter userFormatter,
            IOptionsMonitor<ILog> options)
        {
            TenantManager = tenantManager;
            CoreBaseSettings = coreBaseSettings;
            MessageService = messageService;
            StudioNotifyService = studioNotifyService;
            SecurityContext = securityContext;
            UserManager = userManager;
            MessageTarget = messageTarget;
            WebItemManagerSecurity = webItemManagerSecurity;
            StorageFactory = storageFactory;
            UserFormatter = userFormatter;
            Options = options;
        }

        public void Deconstruct(out TenantManager tenantManager,
            out CoreBaseSettings coreBaseSettings,
            out MessageService messageService,
            out StudioNotifyService studioNotifyService,
            out SecurityContext securityContext,
            out UserManager userManager,
            out MessageTarget messageTarget,
            out WebItemManagerSecurity webItemManagerSecurity,
            out StorageFactory storageFactory,
            out UserFormatter userFormatter,
            out IOptionsMonitor<ILog> optionsMonitor)
        {
            tenantManager = TenantManager;
            coreBaseSettings = CoreBaseSettings;
            messageService = MessageService;
            studioNotifyService = StudioNotifyService;
            securityContext = SecurityContext;
            userManager = UserManager;
            messageTarget = MessageTarget;
            webItemManagerSecurity = WebItemManagerSecurity;
            storageFactory = StorageFactory;
            userFormatter = UserFormatter;
            optionsMonitor = Options;
        }
    }

    public static class RemoveProgressItemExtension
    {
        public static void Register(DIHelper services)
        {
            services.TryAdd<RemoveProgressItemScope>();
            services.AddDistributedTaskQueueService<RemoveProgressItem>(1);
        }
    }
}
