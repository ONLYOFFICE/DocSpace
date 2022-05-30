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

namespace ASC.Web.Files.Services.DocumentService;

public enum ReportOrigin
{
    CRM,
    Projects,
    ProjectsAuto,
}

public enum ReportStatus
{
    Queued,
    Started,
    Done,
    Failed
}

public class ReportStateData
{
    public string FileName { get; }
    public string TmpFileName { get; }
    public string Script { get; }
    public int ReportType { get; }
    public ReportOrigin Origin { get; }
    public Func<ReportState, string, Task> SaveFileAction { get; }
    public object Obj { get; }
    public int TenantId { get; }
    public Guid UserId { get; }

    public ReportStateData(string fileName, string tmpFileName, string script, int reportType, ReportOrigin origin,
        Func<ReportState, string, Task> saveFileAction, object obj,
        int tenantId, Guid userId)
    {
        FileName = fileName;
        TmpFileName = tmpFileName;
        Script = script;
        ReportType = reportType;
        Origin = origin;
        SaveFileAction = saveFileAction;
        Obj = obj;
        TenantId = tenantId;
        UserId = userId;
    }
}

public class ReportState
{
    public string Id { get; set; }
    public string FileName { get; set; }
    public int FileId { get; set; }
    public int ReportType { get; set; }
    public string Exception { get; set; }
    public ReportStatus Status { get; set; }
    public ReportOrigin Origin { get; set; }
    public object Obj { get; set; }

    internal string BuilderKey { get; set; }
    internal string Script { get; set; }
    internal string TmpFileName { get; set; }
    internal Func<ReportState, string, Task> SaveFileAction { get; set; }
    internal int TenantId { get; set; }
    internal Guid UserId { get; set; }
    internal string ContextUrl { get; set; }

    protected DistributedTask TaskInfo { get; private set; }
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ReportState(IServiceScopeFactory serviceScopeFactory, ReportStateData reportStateData, IHttpContextAccessor httpContextAccessor)
    {
        TaskInfo = new DistributedTask();
        _serviceScopeFactory = serviceScopeFactory;
        FileName = reportStateData.FileName;
        TmpFileName = reportStateData.TmpFileName;
        Script = reportStateData.Script;
        ReportType = reportStateData.ReportType;
        Origin = reportStateData.Origin;
        SaveFileAction = reportStateData.SaveFileAction;
        Obj = reportStateData.Obj;
        TenantId = reportStateData.TenantId;
        UserId = reportStateData.UserId;
        ContextUrl = httpContextAccessor.HttpContext?.Request.GetUrlRewriter().ToString();
    }

    public static ReportState FromTask(
        DistributedTask task,
        IHttpContextAccessor httpContextAccessor,
        int tenantId,
        Guid userId)
    {
        var data = new ReportStateData(
            task["fileName"],
            task["tmpFileName"],
            task["script"],
            task["reportType"],
            (ReportOrigin)task["reportOrigin"],
            null,
            null,
            tenantId,
            userId);

        return new ReportState(null, data, httpContextAccessor)
        {
            Id = task["id"],
            FileId = task["fileId"],
            Status = (ReportStatus)task["status"],
            Exception = task["exception"]
        };

    }

    public async Task GenerateReportAsync(DistributedTask task, CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var scopeClass = scope.ServiceProvider.GetService<ReportStateScope>();
        var (logger, tenantManager, authContext, securityContext, documentServiceConnector) = scopeClass;
        try
        {
            tenantManager.SetCurrentTenant(TenantId);

            Status = ReportStatus.Started;
            PublishTaskInfo(logger);

            //if (HttpContext.Current == null && !WorkContext.IsMono && !string.IsNullOrEmpty(ContextUrl))
            //{
            //    HttpContext.Current = new HttpContext(
            //        new HttpRequest("hack", ContextUrl, string.Empty),
            //        new HttpResponse(new System.IO.StringWriter()));
            //}

            tenantManager.SetCurrentTenant(TenantId);
            securityContext.AuthenticateMeWithoutCookie(UserId);

            (BuilderKey, var urls) = await documentServiceConnector.DocbuilderRequestAsync(null, Script, true);

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException();
                }

                await Task.Delay(1500, cancellationToken);
                (var builderKey, urls) = await documentServiceConnector.DocbuilderRequestAsync(BuilderKey, null, true);
                if (builderKey == null)
                {
                    throw new NullReferenceException();
                }

                if (urls != null && urls.Count == 0)
                {
                    throw new Exception("Empty response");
                }

                if (urls != null && urls.ContainsKey(TmpFileName))
                {
                    break;
                }
            }

            if (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException();
            }

            await SaveFileAction(this, urls[TmpFileName]);

            Status = ReportStatus.Done;
        }
        catch (Exception e)
        {
            logger.ErrorDocbuilderReportsUtility(e);
            Exception = e.Message;
            Status = ReportStatus.Failed;
        }

        PublishTaskInfo(logger);
    }

    public DistributedTask GetDistributedTask()
    {
        FillDistributedTask();

        return TaskInfo;
    }

    protected void PublishTaskInfo(ILogger<ReportStateScope> logger)
    {
        var tries = 3;
        while (tries-- > 0)
        {
            try
            {
                FillDistributedTask();
                TaskInfo.PublishChanges();

                return;
            }
            catch (Exception e)
            {
                logger.ErrorPublishTaskInfo(e);
                if (tries == 0)
                {
                    throw;
                }
            }
        }
    }

    protected void FillDistributedTask()
    {
        TaskInfo["id"] = Id;
        TaskInfo["fileName"] = FileName;
        TaskInfo["tmpFileName"] = TmpFileName;
        TaskInfo["reportType"] = ReportType;
        TaskInfo["fileId"] = FileId;
        TaskInfo["status"] = (int)Status;
        TaskInfo["reportOrigin"] = (int)Origin;
        TaskInfo["exception"] = Exception;
    }
}
[Scope]
public class DocbuilderReportsUtility
{
    private readonly DistributedTaskQueue _tasks;
    private readonly object _locker;

    public static string TmpFileName => $"tmp{DateTime.UtcNow.Ticks}.xlsx";

    public DocbuilderReportsUtility(IDistributedTaskQueueFactory queueFactory)
    {
        _tasks = queueFactory.CreateQueue();
        _locker = new object();
    }

    public void Enqueue(ReportState state)
    {
        lock (_locker)
        {
            _tasks.EnqueueTask(state.GenerateReportAsync, state.GetDistributedTask());
        }
    }

    public void Terminate(ReportOrigin origin, int tenantId, Guid userId)
    {
        lock (_locker)
        {
            var result = _tasks.GetAllTasks().Where(Predicate(origin, tenantId, userId));

            foreach (var t in result)
            {
                _tasks.DequeueTask(t.Id);
            }
        }
    }

    public ReportState Status(ReportOrigin origin, IHttpContextAccessor httpContextAccessor, int tenantId, Guid userId)
    {
        lock (_locker)
        {
            var task = _tasks.GetAllTasks().LastOrDefault(Predicate(origin, tenantId, userId));
            if (task == null)
            {
                return null;
            }

            var result = ReportState.FromTask(task, httpContextAccessor, tenantId, userId);
            var status = (ReportStatus)task["status"];

            if ((int)status > 1)
            {
                _tasks.DequeueTask(task.Id);
            }

            return result;
        }
    }

    private static Func<DistributedTask, bool> Predicate(ReportOrigin origin, int tenantId, Guid userId)
    {
        return t => t["id"] == GetCacheKey(origin, tenantId, userId);
    }

    internal static string GetCacheKey(ReportOrigin origin, int tenantId, Guid userId)
    {
        return $"{tenantId}_{userId}_{(int)origin}";
    }
}

[Scope]
public class DocbuilderReportsUtilityHelper
{
    private readonly DocbuilderReportsUtility _docbuilderReportsUtility;
    private readonly AuthContext _authContext;
    private readonly TenantManager _tenantManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DocbuilderReportsUtilityHelper(
        DocbuilderReportsUtility docbuilderReportsUtility,
        AuthContext authContext,
        TenantManager tenantManager)
    {
        _docbuilderReportsUtility = docbuilderReportsUtility;
        _authContext = authContext;
        _tenantManager = tenantManager;
        TenantId = _tenantManager.GetCurrentTenant().Id;
        UserId = _authContext.CurrentAccount.ID;
    }

    public DocbuilderReportsUtilityHelper(
        DocbuilderReportsUtility docbuilderReportsUtility,
        AuthContext authContext,
        TenantManager tenantManager,
        IHttpContextAccessor httpContextAccessor)
        : this(docbuilderReportsUtility, authContext, tenantManager)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private int TenantId { get; set; }
    private Guid UserId { get; set; }

    public void Enqueue(ReportState state)
    {
        _docbuilderReportsUtility.Enqueue(state);
    }

    public void Terminate(ReportOrigin origin)
    {
        _docbuilderReportsUtility.Terminate(origin, TenantId, UserId);
    }

    public ReportState Status(ReportOrigin origin)
    {
        return _docbuilderReportsUtility.Status(origin, _httpContextAccessor, TenantId, UserId);
    }
}

public class ReportStateScope
{
    private readonly ILogger<ReportStateScope> _options;
    private readonly TenantManager _tenantManager;
    private readonly AuthContext _authContext;
    private readonly SecurityContext _securityContext;
    private readonly DocumentServiceConnector _documentServiceConnector;

    public ReportStateScope(
        ILogger<ReportStateScope> options,
        TenantManager tenantManager,
        AuthContext authContext,
        SecurityContext securityContext,
        DocumentServiceConnector documentServiceConnector)
    {
        _options = options;
        _tenantManager = tenantManager;
        _authContext = authContext;
        _securityContext = securityContext;
        _documentServiceConnector = documentServiceConnector;
    }

    public void Deconstruct(out ILogger<ReportStateScope> optionsMonitor,
        out TenantManager tenantManager,
        out AuthContext authContext,
        out SecurityContext securityContext,
        out DocumentServiceConnector documentServiceConnector)
    {
        optionsMonitor = _options;
        tenantManager = _tenantManager;
        authContext = _authContext;
        securityContext = _securityContext;
        documentServiceConnector = _documentServiceConnector;
    }
}
