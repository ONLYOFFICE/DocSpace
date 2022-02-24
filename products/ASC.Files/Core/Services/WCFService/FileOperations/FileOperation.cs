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

namespace ASC.Web.Files.Services.WCFService.FileOperations
{
    public abstract class FileOperation : DistributedTask
    {
        public const string SplitChar = ":";
        public const string Owner = "Owner";
        public const string OpType = "OperationType";
        public const string Src = "Source";
        public const string Progress = "Progress";
        public const string Res = "Result";
        public const string Err = "Error";
        public const string Process = "Processed";
        public const string Finish = "Finished";
        public const string Hold = "Hold";

        protected readonly IPrincipal Principal;
        protected readonly string Culture;
        public int Total { get; set; }
        public string Source { get; set; }

        protected int Processed;
        protected int SuccessProcessed;

        public virtual FileOperationType OperationType { get; }
        public bool HoldResult { get; set; }
        public string Result { get; set; }
        public string Error { get; set; }

        protected DistributedTask TaskInfo;

        protected FileOperation(IServiceProvider serviceProvider)
        {
            Principal = serviceProvider.GetService<Microsoft.AspNetCore.Http.IHttpContextAccessor>()?.HttpContext?.User ?? Thread.CurrentPrincipal;
            Culture = Thread.CurrentThread.CurrentCulture.Name;

            TaskInfo = new DistributedTask();
        }

        public virtual DistributedTask GetDistributedTask()
        {
            FillDistributedTask();

            return TaskInfo;
        }


        protected internal virtual void FillDistributedTask()
        {
            var progress = Total != 0 ? 100 * Processed / Total : 0;

            TaskInfo.SetProperty(OpType, OperationType);
            TaskInfo.SetProperty(Owner, ((IAccount)(Principal ?? Thread.CurrentPrincipal).Identity).ID);
            TaskInfo.SetProperty(Progress, progress < 100 ? progress : 100);
            TaskInfo.SetProperty(Res, Result);
            TaskInfo.SetProperty(Err, Error);
            TaskInfo.SetProperty(Process, SuccessProcessed);
            TaskInfo.SetProperty(Hold, HoldResult);
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
            Source = string.Join(SplitChar, ThirdPartyOperation.Source, DaoOperation.Source);
            base.FillDistributedTask();
        }

        public virtual void PublishChanges(DistributedTask task)
        {
            var thirdpartyTask = ThirdPartyOperation.GetDistributedTask();
            var daoTask = DaoOperation.GetDistributedTask();

            var error1 = thirdpartyTask.GetProperty<string>(Err);
            var error2 = daoTask.GetProperty<string>(Err);

            if (!string.IsNullOrEmpty(error1))
            {
                Error = error1;
            }
            else if (!string.IsNullOrEmpty(error2))
            {
                Error = error2;
            }

            var status1 = thirdpartyTask.GetProperty<string>(Res);
            var status2 = daoTask.GetProperty<string>(Res);

            if (!string.IsNullOrEmpty(status1))
            {
                Result = status1;
            }
            else if (!string.IsNullOrEmpty(status2))
            {
                Result = status2;
            }

            var finished1 = thirdpartyTask.GetProperty<bool?>(Finish);
            var finished2 = daoTask.GetProperty<bool?>(Finish);

            if (finished1 != null && finished2 != null)
            {
                TaskInfo.SetProperty(Finish, finished1);
            }

            SuccessProcessed = thirdpartyTask.GetProperty<int>(Process) + daoTask.GetProperty<int>(Process);


            base.FillDistributedTask();

            var progress = 0;

            if (ThirdPartyOperation.Total != 0)
            {
                progress += thirdpartyTask.GetProperty<int>(Progress);
            }

            if (DaoOperation.Total != 0)
            {
                progress += daoTask.GetProperty<int>(Progress);
            }

            if (ThirdPartyOperation.Total != 0 && DaoOperation.Total != 0)
            {
                progress /= 2;
            }

            TaskInfo.SetProperty(Progress, progress < 100 ? progress : 100);
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

        private readonly IServiceProvider _serviceProvider;

        protected FileOperation(IServiceProvider serviceProvider, T fileOperationData) : base(serviceProvider)
        {
            _serviceProvider = serviceProvider;
            Files = fileOperationData.Files;
            Folders = fileOperationData.Folders;
            HoldResult = fileOperationData.HoldResult;
            CurrentTenant = fileOperationData.Tenant;

            using var scope = _serviceProvider.CreateScope();
            var tenantManager = scope.ServiceProvider.GetService<TenantManager>();
            tenantManager.SetCurrentTenant(CurrentTenant);

            var daoFactory = scope.ServiceProvider.GetService<IDaoFactory>();
            FolderDao = daoFactory.GetFolderDao<TId>();

            Total = InitTotalProgressSteps();
            Source = string.Join(SplitChar, Folders.Select(f => "folder_" + f).Concat(Files.Select(f => "file_" + f)).ToArray());
        }

        public override async Task RunJobAsync(DistributedTask _, CancellationToken cancellationToken)
        {
            try
            {
                //todo check files> 0 or folders > 0
                CancellationToken = cancellationToken;

                using var scope = _serviceProvider.CreateScope();
                var scopeClass = scope.ServiceProvider.GetService<FileOperationScope>();
                var (tenantManager, daoFactory, fileSecurity, options) = scopeClass;
                tenantManager.SetCurrentTenant(CurrentTenant);


                Thread.CurrentPrincipal = Principal;
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(Culture);
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(Culture);

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
                    TaskInfo.SetProperty(Finish, true);
                    PublishTaskInfo();
                }
                catch { /* ignore */ }
            }
        }

        public IServiceScope CreateScope()
        {
            var scope = _serviceProvider.CreateScope();
            var tenantManager = scope.ServiceProvider.GetService<TenantManager>();
            tenantManager.SetCurrentTenant(CurrentTenant);

            return scope;
        }

        protected internal override void FillDistributedTask()
        {
            base.FillDistributedTask();

            TaskInfo.SetProperty(Src, Source);
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
                Processed++;
                PublishTaskInfo();
            }
        }

        protected bool ProcessedFolder(TId folderId)
        {
            SuccessProcessed++;
            if (Folders.Contains(folderId))
            {
                Result += $"folder_{folderId}{SplitChar}";

                return true;
            }

            return false;
        }

        protected bool ProcessedFile(TId fileId)
        {
            SuccessProcessed++;
            if (Files.Contains(fileId))
            {
                Result += $"file_{fileId}{SplitChar}";

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
        private readonly TenantManager _tenantManager;
        private readonly IDaoFactory _daoFactory;
        private readonly FileSecurity _fileSecurity;
        private readonly IOptionsMonitor<ILog> _options;

        public FileOperationScope(TenantManager tenantManager, IDaoFactory daoFactory, FileSecurity fileSecurity, IOptionsMonitor<ILog> options)
        {
            _tenantManager = tenantManager;
            _daoFactory = daoFactory;
            _fileSecurity = fileSecurity;
            _options = options;
        }

        public void Deconstruct(out TenantManager tenantManager, out IDaoFactory daoFactory, out FileSecurity fileSecurity, out IOptionsMonitor<ILog> optionsMonitor)
        {
            tenantManager = _tenantManager;
            daoFactory = _daoFactory;
            fileSecurity = _fileSecurity;
            optionsMonitor = _options;
        }
    }
}