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

namespace ASC.Web.Files.Services.WCFService.FileOperations;

public abstract class FileOperation : DistributedTaskProgress
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

    protected readonly IPrincipal _principal;
    protected readonly string _culture;

    protected int _processed;
    public int Total { get; set; }
    protected FileOperation(IServiceProvider serviceProvider)
    {
        _principal = serviceProvider.GetService<IHttpContextAccessor>()?.HttpContext?.User ?? Thread.CurrentPrincipal;
        _culture = Thread.CurrentThread.CurrentCulture.Name;

        this[Owner] = ((IAccount)(_principal ?? Thread.CurrentPrincipal).Identity).ID.ToString();
        this[Src] = _props.ContainsValue(Src) ? this[Src] : "";
        this[Progress] = 0;
        this[Res] = "";
        this[Err] = "";
        this[Process] = 0;
        this[Finish] = false;
    }

    protected void IncrementProgress()
    {
        _processed++;
        var progress = Total != 0 ? 100 * _processed / Total : 0;
        this[Progress] = progress < 100 ? progress : 100;
    }

    protected abstract Task DoJob(IServiceScope serviceScope);
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
        this[Hold] = ThirdPartyOperation[Hold] || DaoOperation[Hold];
    }

    public override async Task RunJob(DistributedTask _, CancellationToken cancellationToken)
    {
        ThirdPartyOperation.Publication = PublishChanges;
        await ThirdPartyOperation.RunJob(_, cancellationToken);

        DaoOperation.Publication = PublishChanges;
        await DaoOperation.RunJob(_, cancellationToken);
    }

    public virtual void PublishChanges(DistributedTask task)
    {
        var thirdpartyTask = ThirdPartyOperation;
        var daoTask = DaoOperation;

        var error1 = thirdpartyTask[Err];
        var error2 = daoTask[Err];

        if (!string.IsNullOrEmpty(error1))
        {
            this[Err] = error1;
        }
        else if (!string.IsNullOrEmpty(error2))
        {
            this[Err] = error2;
        }

        var status1 = thirdpartyTask[Res];
        var status2 = daoTask[Res];

        if (!string.IsNullOrEmpty(status1))
        {
            this[Res] = status1;
        }
        else if (!string.IsNullOrEmpty(status2))
        {
            this[Res] = status2;
        }

        bool finished1 = thirdpartyTask[Finish];
        bool finished2 = daoTask[Finish];

        if (finished1 && finished2)
        {
            this[Finish] = true;
        }

        this[Process] = thirdpartyTask[Process] + daoTask[Process];

        var progress = 0;

        if (ThirdPartyOperation.Total != 0)
        {
            progress += thirdpartyTask[Progress];
        }

        if (DaoOperation.Total != 0)
        {
            progress += daoTask[Progress];
        }

        if (ThirdPartyOperation.Total != 0 && DaoOperation.Total != 0)
        {
            progress /= 2;
        }

        this[Progress] = progress < 100 ? progress : 100;
        PublishChanges();
    }

    protected override Task DoJob(IServiceScope serviceScope)
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
    protected ILogger Logger { get; private set; }
    protected CancellationToken CancellationToken { get; private set; }
    protected internal List<TId> Folders { get; private set; }
    protected internal List<TId> Files { get; private set; }

    protected IServiceProvider _serviceProvider;

    protected FileOperation(IServiceProvider serviceProvider, T fileOperationData) : base(serviceProvider)
    {
        _serviceProvider = serviceProvider;
        Files = fileOperationData.Files;
        Folders = fileOperationData.Folders;
        this[Hold] = fileOperationData.HoldResult;
        CurrentTenant = fileOperationData.Tenant;

        using var scope = _serviceProvider.CreateScope();
        var tenantManager = scope.ServiceProvider.GetService<TenantManager>();
        tenantManager.SetCurrentTenant(CurrentTenant);

        var daoFactory = scope.ServiceProvider.GetService<IDaoFactory>();
        FolderDao = daoFactory.GetFolderDao<TId>();

        Total = InitTotalProgressSteps();
        this[Src] = string.Join(SplitChar, Folders.Select(f => "folder_" + f).Concat(Files.Select(f => "file_" + f)).ToArray());
    }

    public override async Task RunJob(DistributedTask _, CancellationToken cancellationToken)
    {
        try
        {
            //todo check files> 0 or folders > 0
            CancellationToken = cancellationToken;

            await using var scope = _serviceProvider.CreateAsyncScope();
            var scopeClass = scope.ServiceProvider.GetService<FileOperationScope>();
            var (tenantManager, daoFactory, fileSecurity, logger) = scopeClass;
            tenantManager.SetCurrentTenant(CurrentTenant);

            Thread.CurrentPrincipal = _principal;
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(_culture);
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(_culture);

            FolderDao = daoFactory.GetFolderDao<TId>();
            FileDao = daoFactory.GetFileDao<TId>();
            TagDao = daoFactory.GetTagDao<TId>();
            LinkDao = daoFactory.GetLinkDao();
            ProviderDao = daoFactory.ProviderDao;
            FilesSecurity = fileSecurity;

            Logger = logger;

            await DoJob(scope);
        }
        catch (AuthorizingException authError)
        {
            this[Err] = FilesCommonResource.ErrorMassage_SecurityException;
            Logger.ErrorWithException(new SecurityException(this[Err], authError));
        }
        catch (AggregateException ae)
        {
            ae.Flatten().Handle(e => e is TaskCanceledException || e is OperationCanceledException);
        }
        catch (Exception error)
        {
            Logger.ErrorWithException(error);
        }
        finally
        {
            try
            {
                this[Finish] = true;
                PublishTaskInfo();
            }
            catch { /* ignore */ }
        }
    }

    public AsyncServiceScope CreateScopeAsync()
    {
        var scope = _serviceProvider.CreateAsyncScope();
        var tenantManager = scope.ServiceProvider.GetService<TenantManager>();
        tenantManager.SetCurrentTenant(CurrentTenant);

        return scope;
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
            IncrementProgress();
            PublishTaskInfo();
        }
    }

    protected bool ProcessedFolder(TId folderId)
    {
        this[Process]++;
        if (Folders.Contains(folderId))
        {
            this[Res] += $"folder_{folderId}{SplitChar}";

            return true;
        }

        return false;
    }

    protected bool ProcessedFile(TId fileId)
    {
        this[Process]++;
        if (Files.Contains(fileId))
        {
            this[Res] += $"file_{fileId}{SplitChar}";

            return true;
        }

        return false;
    }

    protected void PublishTaskInfo()
    {
        PublishChanges();
    }
}

[Scope]
public class FileOperationScope
{
    private readonly TenantManager _tenantManager;
    private readonly IDaoFactory _daoFactory;
    private readonly FileSecurity _fileSecurity;
    private readonly ILogger _options;

    public FileOperationScope(TenantManager tenantManager, IDaoFactory daoFactory, FileSecurity fileSecurity, ILogger<FileOperationScope> options)
    {
        _tenantManager = tenantManager;
        _daoFactory = daoFactory;
        _fileSecurity = fileSecurity;
        _options = options;
    }

    public void Deconstruct(out TenantManager tenantManager, out IDaoFactory daoFactory, out FileSecurity fileSecurity, out ILogger optionsMonitor)
    {
        tenantManager = _tenantManager;
        daoFactory = _daoFactory;
        fileSecurity = _fileSecurity;
        optionsMonitor = _options;
    }
}
