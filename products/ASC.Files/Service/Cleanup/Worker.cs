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

using ASC.Core.Common.EF.Context;

using Microsoft.EntityFrameworkCore;

namespace ASC.Files.AutoCleanUp;

[Singletone]
public class Worker
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public Worker(IConfiguration configuration, ILogger<Worker> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task DeleteExpiredFilesInTrash(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        List<TenantUserSettings> activeTenantsUsers;

        await using (var scope = _serviceScopeFactory.CreateAsyncScope())
        {
            using var dbContext = scope.ServiceProvider.GetRequiredService<IDbContextFactory<WebstudioDbContext>>().CreateDbContext();
            activeTenantsUsers = GetTenantsUsers(dbContext);
        }

        if (!activeTenantsUsers.Any())
        {
            return;
        }

        _logger.InfoFoundUsers(activeTenantsUsers.Count);

        await Parallel.ForEachAsync(activeTenantsUsers,
                                    new ParallelOptions { MaxDegreeOfParallelism = 3, CancellationToken = cancellationToken }, //System.Environment.ProcessorCount
                                    DeleteFilesAndFolders);
    }

    private async ValueTask DeleteFilesAndFolders(TenantUserSettings tenantUser, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        try
        {
            await using (var scope = _serviceScopeFactory.CreateAsyncScope())
            {
                var tenantManager = scope.ServiceProvider.GetRequiredService<TenantManager>();
                tenantManager.SetCurrentTenant(tenantUser.TenantId);

                var authManager = scope.ServiceProvider.GetRequiredService<AuthManager>();
                var securityContext = scope.ServiceProvider.GetRequiredService<SecurityContext>();
                var daoFactory = scope.ServiceProvider.GetRequiredService<IDaoFactory>();
                var fileStorageService = scope.ServiceProvider.GetRequiredService<Web.Files.Services.WCFService.FileStorageService<int>>();
                var fileDateTime = scope.ServiceProvider.GetRequiredService<FileDateTime>();

                var userAccount = authManager.GetAccountByID(tenantUser.TenantId, tenantUser.UserId);

                if (userAccount == ASC.Core.Configuration.Constants.Guest)
                {
                    return;
                }

                securityContext.AuthenticateMeWithoutCookie(userAccount);

                var fileDao = daoFactory.GetFileDao<int>();
                var folderDao = daoFactory.GetFolderDao<int>();
                var now = DateTime.UtcNow;

                var filesList = new List<int>();
                var foldersList = new List<int>();

                var trashId = await folderDao.GetFolderIDTrashAsync(false, tenantUser.UserId);

                foldersList.AddRange((await folderDao.GetFoldersAsync(trashId).ToListAsync(cancellationToken: cancellationToken))
                    .Where(x => fileDateTime.GetModifiedOnWithAutoCleanUp(x.ModifiedOn, tenantUser.Setting, true) < now)
                    .Select(f => f.Id));

                filesList.AddRange((await fileDao.GetFilesAsync(trashId, null, default(FilterType), false, Guid.Empty, string.Empty, false).ToListAsync(cancellationToken: cancellationToken))
                    .Where(x => fileDateTime.GetModifiedOnWithAutoCleanUp(x.ModifiedOn, tenantUser.Setting, true) < now)
                    .Select(y => y.Id));

                if (foldersList.Count == 0 && filesList.Count == 0)
                {
                    return;
                }

                _logger.InfoCleanUp(tenantUser.TenantId, trashId);

                fileStorageService.DeleteItems("delete", filesList, foldersList, true, false, true);

                _logger.InfoCleanUpWait(tenantUser.TenantId, trashId);

                while (true)
                {
                    var statuses = fileStorageService.GetTasksStatuses();

                    if (statuses.TrueForAll(r => r.Finished))
                    {
                        break;
                    }

                    await Task.Delay(100, cancellationToken);
                }

                _logger.InfoCleanUpFinish(tenantUser.TenantId, trashId);

            }
        }
        catch (Exception ex)
        {
            _logger.ErrorWithException(ex);
        }
    }

    private List<TenantUserSettings> GetTenantsUsers(WebstudioDbContext dbContext)
    {
        var filesSettingsId = new FilesSettings().ID;
        return dbContext.Tenants
            .Join(dbContext.WebstudioSettings, a => a.Id, b => b.TenantId, (tenants, settings) => new { tenants, settings })
            .Where(x => x.tenants.Status == TenantStatus.Active &&
                        x.settings.Id == filesSettingsId &&
                        Convert.ToBoolean(DbFunctionsExtension.JsonValue(nameof(x.settings.Data).ToLower(), "AutomaticallyCleanUp.IsAutoCleanUp")) == true)
            .Select(r => new TenantUserSettings()
            {
                TenantId = r.tenants.Id,
                UserId = r.settings.UserId,
                Setting = (DateToAutoCleanUp)Convert.ToInt32(DbFunctionsExtension.JsonValue(nameof(r.settings.Data).ToLower(), "AutomaticallyCleanUp.Gap"))
            })
            .ToList();
    }
}