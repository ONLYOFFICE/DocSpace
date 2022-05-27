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
using System.Linq;
using System.Security;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Common.Security.Authentication;
using ASC.Common.Security.Authorizing;
using ASC.Common.Threading;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Files.Core;
using ASC.Files.Core.Resources;
using ASC.Files.Core.Security;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ASC.Web.Files.Services.WCFService.FileOperations
{
    public abstract class FileOperation : DistributedTask
    {
        public const string SPLIT_CHAR = ":";
        public const string OWNER = "Owner";
        public const string OPERATION_TYPE = "OperationType";
        public const string SOURCE = "Source";
        public const string PROGRESS = "Progress";
        public const string RESULT = "Result";
        public const string ERROR = "Error";
        public const string PROCESSED = "Processed";
        public const string FINISHED = "Finished";
        public const string HOLD = "Hold";

        protected readonly IPrincipal principal;
        protected readonly string culture;
        public int Total { get; set; }
        public string Source { get; set; }

        protected int processed;
        protected int successProcessed;

        public virtual FileOperationType OperationType { get; }
        public bool HoldResult { get; set; }

        public string Result { get; set; }

        public string Error { get; set; }

        protected DistributedTask TaskInfo { get; set; }

        protected FileOperation(IServiceProvider serviceProvider)
        {
            principal = serviceProvider.GetService<Microsoft.AspNetCore.Http.IHttpContextAccessor>()?.HttpContext?.User ?? Thread.CurrentPrincipal;
            culture = Thread.CurrentThread.CurrentCulture.Name;

            TaskInfo = new DistributedTask();
        }

        public virtual DistributedTask GetDistributedTask()
        {
            FillDistributedTask();
            return TaskInfo;
        }


        protected internal virtual void FillDistributedTask()
        {
            var progress = Total != 0 ? 100 * processed / Total : 0;

            TaskInfo.SetProperty(OPERATION_TYPE, OperationType);
            TaskInfo.SetProperty(OWNER, ((IAccount)(principal ?? Thread.CurrentPrincipal).Identity).ID);
            TaskInfo.SetProperty(PROGRESS, progress < 100 ? progress : 100);
            TaskInfo.SetProperty(RESULT, Result);
            TaskInfo.SetProperty(ERROR, Error);
            TaskInfo.SetProperty(PROCESSED, successProcessed);
            TaskInfo.SetProperty(HOLD, HoldResult);
        }

        public abstract Task RunJobAsync(DistributedTask _, CancellationToken cancellationToken);
        protected abstract Task DoAsync(IServiceScope serviceScope);

    }

    internal class ComposeFileOperation<T1, T2> : FileOperation
        where T1 : FileOperationData<string>
        where T2 : FileOperationData<int>
    {
        public FileOperation<T1, string> ThirdPartyOperation { get; set; }
        public FileOperation<T2, int> DaoOperation { get; set; }

        public ComposeFileOperation(
            IServiceProvider serviceProvider,
            FileOperation<T1, string> thirdPartyOperation,
            FileOperation<T2, int> daoOperation)
            : base(serviceProvider)
        {
            ThirdPartyOperation = thirdPartyOperation;
            DaoOperation = daoOperation;
        }

        public override async Task RunJobAsync(DistributedTask _, CancellationToken cancellationToken)
        {
            ThirdPartyOperation.GetDistributedTask().Publication = PublishChanges;
            await ThirdPartyOperation.RunJobAsync(_, cancellationToken);

            DaoOperation.GetDistributedTask().Publication = PublishChanges;
            await DaoOperation.RunJobAsync(_, cancellationToken);
        }

        protected internal override void FillDistributedTask()
        {
            ThirdPartyOperation.FillDistributedTask();
            DaoOperation.FillDistributedTask();

            HoldResult = ThirdPartyOperation.HoldResult || DaoOperation.HoldResult;
            Total = ThirdPartyOperation.Total + DaoOperation.Total;
            Source = string.Join(SPLIT_CHAR, ThirdPartyOperation.Source, DaoOperation.Source);
            base.FillDistributedTask();
        }

        public virtual void PublishChanges(DistributedTask task)
        {
            var thirdpartyTask = ThirdPartyOperation.GetDistributedTask();
            var daoTask = DaoOperation.GetDistributedTask();

            var error1 = thirdpartyTask.GetProperty<string>(ERROR);
            var error2 = daoTask.GetProperty<string>(ERROR);

            if (!string.IsNullOrEmpty(error1))
            {
                Error = error1;
            }
            else if (!string.IsNullOrEmpty(error2))
            {
                Error = error2;
            }

            var status1 = thirdpartyTask.GetProperty<string>(RESULT);
            var status2 = daoTask.GetProperty<string>(RESULT);

            if (!string.IsNullOrEmpty(status1))
            {
                Result = status1;
            }
            else if (!string.IsNullOrEmpty(status2))
            {
                Result = status2;
            }

            var finished1 = thirdpartyTask.GetProperty<bool?>(FINISHED);
            var finished2 = daoTask.GetProperty<bool?>(FINISHED);

            if (finished1 != null && finished2 != null)
            {
                TaskInfo.SetProperty(FINISHED, finished1);
            }

            successProcessed = thirdpartyTask.GetProperty<int>(PROCESSED) + daoTask.GetProperty<int>(PROCESSED);


            base.FillDistributedTask();

            var progress = 0;

            if (ThirdPartyOperation.Total != 0)
            {
                progress += thirdpartyTask.GetProperty<int>(PROGRESS);
            }

            if (DaoOperation.Total != 0)
            {
                progress += daoTask.GetProperty<int>(PROGRESS);
            }

            if (ThirdPartyOperation.Total != 0 && DaoOperation.Total != 0)
            {
                progress /= 2;
            }

            TaskInfo.SetProperty(PROGRESS, progress < 100 ? progress : 100);
            TaskInfo.PublishChanges();
        }

        protected override Task DoAsync(IServiceScope serviceScope)
        {
            throw new NotImplementedException();
        }
    }

    abstract class FileOperationData<T>
    {
        public List<T> Folders { get; private set; }

        public List<T> Files { get; private set; }

        public Tenant Tenant { get; }

        public bool HoldResult { get; set; }

        protected FileOperationData(IEnumerable<T> folders, IEnumerable<T> files, Tenant tenant, bool holdResult = true)
        {
            Folders = folders?.ToList() ?? new List<T>();
            Files = files?.ToList() ?? new List<T>();
            Tenant = tenant;
            HoldResult = holdResult;
        }
    }

    abstract class FileOperation<T, TId> : FileOperation where T : FileOperationData<TId>
    {
        protected Tenant CurrentTenant { get; private set; }

        protected FileSecurity FilesSecurity { get; private set; }

        protected IFolderDao<TId> FolderDao { get; private set; }

        protected IFileDao<TId> FileDao { get; private set; }

        protected ITagDao<TId> TagDao { get; private set; }

        protected ILinkDao LinkDao { get; private set; }

        protected IProviderDao ProviderDao { get; private set; }

        protected ILog Logger { get; private set; }

        protected CancellationToken CancellationToken { get; private set; }

        protected List<TId> Folders { get; private set; }

        protected List<TId> Files { get; private set; }

        private IServiceProvider ServiceProvider { get; }

        protected FileOperation(IServiceProvider serviceProvider, T fileOperationData) : base(serviceProvider)
        {
            ServiceProvider = serviceProvider;
            Files = fileOperationData.Files;
            Folders = fileOperationData.Folders;
            HoldResult = fileOperationData.HoldResult;
            CurrentTenant = fileOperationData.Tenant;

            using var scope = ServiceProvider.CreateScope();
            var tenantManager = scope.ServiceProvider.GetService<TenantManager>();
            tenantManager.SetCurrentTenant(CurrentTenant);

            var daoFactory = scope.ServiceProvider.GetService<IDaoFactory>();
            FolderDao = daoFactory.GetFolderDao<TId>();

            Total = InitTotalProgressSteps();
            Source = string.Join(SPLIT_CHAR, Folders.Select(f => "folder_" + f).Concat(Files.Select(f => "file_" + f)).ToArray());
        }

        public override async Task RunJobAsync(DistributedTask _, CancellationToken cancellationToken)
        {
            try
            {
                //todo check files> 0 or folders > 0
                CancellationToken = cancellationToken;

                using var scope = ServiceProvider.CreateScope();
                var scopeClass = scope.ServiceProvider.GetService<FileOperationScope>();
                var (tenantManager, daoFactory, fileSecurity, options) = scopeClass;
                tenantManager.SetCurrentTenant(CurrentTenant);


                Thread.CurrentPrincipal = principal;
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(culture);
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(culture);

                FolderDao = daoFactory.GetFolderDao<TId>();
                FileDao = daoFactory.GetFileDao<TId>();
                TagDao = daoFactory.GetTagDao<TId>();
                LinkDao = daoFactory.GetLinkDao();
                ProviderDao = daoFactory.ProviderDao;
                FilesSecurity = fileSecurity;

                Logger = options.CurrentValue;

                await DoAsync(scope);
            }
            catch (AuthorizingException authError)
            {
                Error = FilesCommonResource.ErrorMassage_SecurityException;
                Logger.Error(Error, new SecurityException(Error, authError));
            }
            catch (AggregateException ae)
            {
                ae.Flatten().Handle(e => e is TaskCanceledException || e is OperationCanceledException);
            }
            catch (Exception error)
            {
                Error = error is TaskCanceledException || error is OperationCanceledException
                            ? FilesCommonResource.ErrorMassage_OperationCanceledException
                            : error.Message;
                Logger.Error(error, error);
            }
            finally
            {
                try
                {
                    TaskInfo.SetProperty(FINISHED, true);
                    PublishTaskInfo();
                }
                catch { /* ignore */ }
            }
        }

        public IServiceScope CreateScope()
        {
            var scope = ServiceProvider.CreateScope();
            var tenantManager = scope.ServiceProvider.GetService<TenantManager>();
            tenantManager.SetCurrentTenant(CurrentTenant);
            return scope;
        }

        protected internal override void FillDistributedTask()
        {
            base.FillDistributedTask();

            TaskInfo.SetProperty(SOURCE, Source);
        }

        protected virtual int InitTotalProgressSteps()
        {
            var count = Files.Count;
            Folders.ForEach(f => count += 1 + (FolderDao.CanCalculateSubitems(f) ? FolderDao.GetItemsCountAsync(f).Result : 0));
            return count;
        }

        protected void ProgressStep(TId folderId = default, TId fileId = default)
        {
            if (Equals(folderId, default(TId)) && Equals(fileId, default(TId))
                || !Equals(folderId, default(TId)) && Folders.Contains(folderId)
                || !Equals(fileId, default(TId)) && Files.Contains(fileId))
            {
                processed++;
                PublishTaskInfo();
            }
        }

        protected bool ProcessedFolder(TId folderId)
        {
            successProcessed++;
            if (Folders.Contains(folderId))
            {
                Result += string.Format("folder_{0}{1}", folderId, SPLIT_CHAR);
                return true;
            }
            return false;
        }

        protected bool ProcessedFile(TId fileId)
        {
            successProcessed++;
            if (Files.Contains(fileId))
            {
                Result += string.Format("file_{0}{1}", fileId, SPLIT_CHAR);
                return true;
            }
            return false;
        }

        protected void PublishTaskInfo()
        {
            FillDistributedTask();
            TaskInfo.PublishChanges();
        }
    }

    [Scope]
    public class FileOperationScope
    {
        private TenantManager TenantManager { get; }
        private IDaoFactory DaoFactory { get; }
        private FileSecurity FileSecurity { get; }
        private IOptionsMonitor<ILog> Options { get; }

        public FileOperationScope(TenantManager tenantManager, IDaoFactory daoFactory, FileSecurity fileSecurity, IOptionsMonitor<ILog> options)
        {
            TenantManager = tenantManager;
            DaoFactory = daoFactory;
            FileSecurity = fileSecurity;
            Options = options;
        }

        public void Deconstruct(out TenantManager tenantManager, out IDaoFactory daoFactory, out FileSecurity fileSecurity, out IOptionsMonitor<ILog> optionsMonitor)
        {
            tenantManager = TenantManager;
            daoFactory = DaoFactory;
            fileSecurity = FileSecurity;
            optionsMonitor = Options;
        }
    }
}