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
using System.Linq;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Mail.Core.Dao.Expressions.UserFolder;
using ASC.Mail.Core.Engine.Operations.Base;
using ASC.Mail.Storage;
using ASC.Mail.Enums;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using ASC.Mail.Models;
using ASC.ElasticSearch;
using System.Data;

namespace ASC.Mail.Core.Engine.Operations
{
    public class MailRemoveUserFolderOperation : MailOperation
    {
        private readonly uint _userFolderId;

        public override MailOperationType OperationType
        {
            get { return MailOperationType.RemoveUserFolder; }
        }

        public UserFolderEngine UserFolderEngine { get; }
        public MessageEngine MessageEngine { get; }
        public IndexEngine IndexEngine { get; }
        public FactoryIndexerHelper FactoryIndexerHelper { get; }
        public IServiceProvider ServiceProvider { get; }

        public MailRemoveUserFolderOperation(
            TenantManager tenantManager,
            SecurityContext securityContext,
            DaoFactory daoFactory,
            UserFolderEngine userFolderEngine,
            MessageEngine messageEngine,
            IndexEngine indexEngine,
            CoreSettings coreSettings,
            StorageManager storageManager,
            FactoryIndexerHelper factoryIndexerHelper,
            IServiceProvider serviceProvider,
            IOptionsMonitor<ILog> optionsMonitor,
            uint userFolderId)
            : base(tenantManager, securityContext, daoFactory, coreSettings, storageManager, optionsMonitor)
        {
            UserFolderEngine = userFolderEngine;
            MessageEngine = messageEngine;
            IndexEngine = indexEngine;
            FactoryIndexerHelper = factoryIndexerHelper;
            ServiceProvider = serviceProvider;
            _userFolderId = userFolderId;

            SetSource(userFolderId.ToString());
        }

        protected override void Do()
        {
            try
            {
                SetProgress((int?) MailOperationRemoveUserFolderProgress.Init, "Setup tenant and user");

                TenantManager.SetCurrentTenant(CurrentTenant);

                SecurityContext.AuthenticateMe(CurrentUser);

                SetProgress((int?) MailOperationRemoveUserFolderProgress.DeleteFolders, "Delete folders");

                Delete(_userFolderId);

                SetProgress((int?) MailOperationRemoveUserFolderProgress.Finished);
            }
            catch (Exception e)
            {
                Logger.Error("Mail operation error -> Remove user folder: {0}", e);
                Error = "InternalServerError";
            }
        }

        public void Delete(uint folderId)
        {
            var affectedIds = new List<int>();

            //TODO: Check or increase timeout for DB connection
            //using (var db = new DbManager(Defines.CONNECTION_STRING_NAME, Defines.RecalculateFoldersTimeout))

            var folder = DaoFactory.UserFolderDao.Get(folderId);
            if (folder == null)
                return;

            using (var tx = DaoFactory.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                //Find folder sub-folders
                var expTree = SimpleUserFoldersTreeExp.CreateBuilder()
                    .SetParent(folder.Id)
                    .Build();

                var removeFolderIds = DaoFactory.UserFolderTreeDao.Get(expTree)
                    .ConvertAll(f => f.FolderId);

                if (!removeFolderIds.Contains(folderId))
                    removeFolderIds.Add(folderId);

                //Remove folder with subfolders
                var expFolders = SimpleUserFoldersExp.CreateBuilder(CurrentTenant.TenantId, CurrentUser.ID.ToString())
                    .SetIds(removeFolderIds)
                    .Build();

                DaoFactory.UserFolderDao.Remove(expFolders);

                //Remove folder tree info
                expTree = SimpleUserFoldersTreeExp.CreateBuilder()
                    .SetIds(removeFolderIds)
                    .Build();

                DaoFactory.UserFolderTreeDao.Remove(expTree);

                //Move mails to trash
                foreach (var id in removeFolderIds)
                {
                    var listMailIds = DaoFactory.UserFolderXMailDao.GetMailIds(id);

                    if (!listMailIds.Any()) continue;

                    affectedIds.AddRange(listMailIds);

                    //Move mails to trash
                    MessageEngine.SetFolder(DaoFactory, listMailIds, FolderType.Trash);

                    //Remove listMailIds from 'mail_user_folder_x_mail'
                    DaoFactory.UserFolderXMailDao.Remove(listMailIds);
                }

                tx.Commit();
            }

            DaoFactory.UserFolderDao.RecalculateFoldersCount(folder.ParentId);

            var t = ServiceProvider.GetService<MailWrapper>();
            if (!FactoryIndexerHelper.Support(t) || !affectedIds.Any())
                return;

            var data = new MailWrapper
            {
                Folder = (byte)FolderType.Trash
            };

            IndexEngine.Update(data, s => s.In(m => m.Id, affectedIds.ToArray()), wrapper => wrapper.Unread);
        }
    }
}
