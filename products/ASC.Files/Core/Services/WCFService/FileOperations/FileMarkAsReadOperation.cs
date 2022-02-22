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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Security.Authentication;
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;
using ASC.Files.Core;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Utils;
using ASC.Web.Studio.Core;

using Microsoft.Extensions.DependencyInjection;

namespace ASC.Web.Files.Services.WCFService.FileOperations
{
    class FileMarkAsReadOperationData<T> : FileOperationData<T>
    {
        public FileMarkAsReadOperationData(IEnumerable<object> folders, IEnumerable<object> files, Tenant tenant, bool holdResult = true)
            : this(folders.OfType<T>(), files.OfType<T>(), tenant, holdResult)
        {
        }
        public FileMarkAsReadOperationData(IEnumerable<T> folders, IEnumerable<T> files, Tenant tenant, bool holdResult = true) : base(folders, files, tenant, holdResult)
        {
        }
    }

    [Transient]
    class FileMarkAsReadOperation : ComposeFileOperation<FileMarkAsReadOperationData<string>, FileMarkAsReadOperationData<int>>
    {
        public FileMarkAsReadOperation(IServiceProvider serviceProvider, FileOperation<FileMarkAsReadOperationData<string>, string> f1, FileOperation<FileMarkAsReadOperationData<int>, int> f2)
            : base(serviceProvider, f1, f2)
        {
        }

        public override FileOperationType OperationType
        {
            get { return FileOperationType.MarkAsRead; }
        }
    }

    class FileMarkAsReadOperation<T> : FileOperation<FileMarkAsReadOperationData<T>, T>
    {
        public override FileOperationType OperationType
        {
            get { return FileOperationType.MarkAsRead; }
        }


        public FileMarkAsReadOperation(IServiceProvider serviceProvider, FileMarkAsReadOperationData<T> fileOperationData)
            : base(serviceProvider, fileOperationData)
        {
        }


        protected override int InitTotalProgressSteps()
        {
            return Files.Count + Folders.Count;
        }

        protected override async Task DoAsync(IServiceScope scope)
        {
            var scopeClass = scope.ServiceProvider.GetService<FileMarkAsReadOperationScope>();
            var (fileMarker, globalFolder, daoFactory, settingsManager) = scopeClass;
            var entries = AsyncEnumerable.Empty<FileEntry<T>>();
            if (Folders.Count > 0)
            {
                entries.Concat(FolderDao.GetFoldersAsync(Folders));
            }
            if (Files.Count > 0)
            {
                entries.Concat(FileDao.GetFilesAsync(Files));
            }

            await entries.ForEachAwaitAsync(async x =>
            {
                CancellationToken.ThrowIfCancellationRequested();

                await fileMarker.RemoveMarkAsNewAsync(x, ((IAccount)Thread.CurrentPrincipal.Identity).ID);

                if (x.FileEntryType == FileEntryType.File)
                {
                    ProcessedFile(((File<T>)x).ID);
                }
                else
                {
                    ProcessedFolder(((Folder<T>)x).ID);
                }
                ProgressStep();
            });

            var rootIds = new List<int>
                {
                    globalFolder.GetFolderMy(fileMarker, daoFactory),
                    await globalFolder.GetFolderCommonAsync(fileMarker, daoFactory),
                    await globalFolder.GetFolderShareAsync(daoFactory),
                    await globalFolder.GetFolderProjectsAsync(daoFactory),
                };

            if (PrivacyRoomSettings.GetEnabled(settingsManager))
            {
                rootIds.Add(await globalFolder.GetFolderPrivacyAsync(daoFactory));
            }

            var newrootfolder = new List<string>();

            foreach (var r in rootIds)
            {
                var item = new KeyValuePair<int, int>(r, await fileMarker.GetRootFoldersIdMarkedAsNewAsync(r));
                newrootfolder.Add($"new_{{\"key\"? \"{item.Key}\", \"value\"? \"{item.Value}\"}}");
            }

            Result += string.Join(SPLIT_CHAR, newrootfolder.ToArray());
        }
    }

    [Scope]
    public class FileMarkAsReadOperationScope
    {
        private FileMarker FileMarker { get; }
        private GlobalFolder GlobalFolder { get; }
        private IDaoFactory DaoFactory { get; }
        private SettingsManager SettingsManager { get; }

        public FileMarkAsReadOperationScope(
            FileMarker fileMarker,
            GlobalFolder globalFolder,
            IDaoFactory daoFactory,
            SettingsManager settingsManager)
        {
            FileMarker = fileMarker;
            GlobalFolder = globalFolder;
            DaoFactory = daoFactory;
            SettingsManager = settingsManager;
        }

        public void Deconstruct(
            out FileMarker fileMarker,
            out GlobalFolder globalFolder,
            out IDaoFactory daoFactory,
            out SettingsManager settingsManager)
        {
            fileMarker = FileMarker;
            globalFolder = GlobalFolder;
            daoFactory = DaoFactory;
            settingsManager = SettingsManager;
        }
    }
}