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

namespace ASC.Web.Core.RemovePortal;

[Transient]
public class RemovePortalOperation : DistributedTaskProgress
{
    private readonly StorageFactory _storageFactory;
    private readonly ITenantService _tenantService;
    private readonly StorageFactoryConfig _storageFactoryConfig;
    private readonly TenantManager _tenantManager;
    private readonly UserManager _userManager;
    private readonly ILogger<RemovePortalOperation> _logger;
    public int TenantId { get; set; }


    public RemovePortalOperation(StorageFactory storageFactory,
        StorageFactoryConfig storageFactoryConfig,
        ITenantService tenantService,
        TenantManager tenantManager,
        UserManager userManager,
        ILogger<RemovePortalOperation> logger)
    {
        _storageFactory = storageFactory;
        _storageFactoryConfig = storageFactoryConfig;
        _tenantService = tenantService;
        _tenantManager = tenantManager;
        _userManager = userManager;
        _logger = logger;
    }

    public void Init(int tenantId)
    {
        TenantId = tenantId;
    }

    protected override async Task DoJob()
    {
        try
        {
            CustomSynchronizationContext.CreateContext();

            _logger.DebugStartRemoveTenant(TenantId);

            var tenant = _tenantManager.GetTenant(TenantId);
            _tenantManager.SetCurrentTenant(tenant);

            var modules = _storageFactoryConfig.GetModuleList();
            foreach (var module in modules)
            {
                Percentage += 100 / (modules.Count() + 1);
                PublishChanges();
                _logger.DebugRemoveModule(module);
                var storage = await _storageFactory.GetStorageAsync(TenantId, module);
                foreach (var domain in _storageFactoryConfig.GetDomainList(module))
                {
                    await storage.DeleteDirectoryAsync(domain, "");
                }
                await storage.DeleteDirectoryAsync("");
            }
            var owner = _userManager.GetUsers(tenant.OwnerId);

            _logger.DebugRemoveTenantFromDb();
            await _tenantService.PermanentlyRemoveTenantAsync(TenantId);

            _logger.DebugEndRemoveTenant(TenantId);
            Percentage = 100;
        }
        catch (Exception e)
        {
            Exception = e;
            _logger.ErrorRemoveTenant(TenantId, e);
        }
        finally
        {
            IsCompleted = true;
            PublishChanges();
        }
    }
}