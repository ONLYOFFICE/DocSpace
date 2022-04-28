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

namespace ASC.Web.Studio.Core.Quota;

public class QuotaSync
{
    public const string TenantIdKey = "tenantID";
    protected DistributedTask TaskInfo { get; private set; }
    private readonly int _tenantId;
    private readonly IServiceProvider _serviceProvider;

    public QuotaSync(int tenantId, IServiceProvider serviceProvider)
    {
        _tenantId = tenantId;
        TaskInfo = new DistributedTask();
        _serviceProvider = serviceProvider;
    }

    public void RunJob()//DistributedTask distributedTask, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var scopeClass = scope.ServiceProvider.GetService<QuotaSyncJob>();
        scopeClass.RunJob(_tenantId);
    }

    public virtual DistributedTask GetDistributedTask()
    {
        TaskInfo[TenantIdKey] = _tenantId;
        return TaskInfo;
    }
}

class QuotaSyncJob
{
    private readonly TenantManager _tenantManager;
    private readonly StorageFactoryConfig _storageFactoryConfig;
    private readonly StorageFactory _storageFactory;

    public QuotaSyncJob(TenantManager tenantManager, StorageFactoryConfig storageFactoryConfig, StorageFactory storageFactory)
    {
        _tenantManager = tenantManager;
        _storageFactoryConfig = storageFactoryConfig;
        _storageFactory = storageFactory;
    }

    public void RunJob(int tenantId)
    {
        _tenantManager.SetCurrentTenant(tenantId);

        var storageModules = _storageFactoryConfig.GetModuleList(string.Empty);

        foreach (var module in storageModules)
        {
            var storage = _storageFactory.GetStorage(tenantId.ToString(), module);
            storage.ResetQuotaAsync("").Wait();

            var domains = _storageFactoryConfig.GetDomainList(string.Empty, module);
            foreach (var domain in domains)
            {
                storage.ResetQuotaAsync(domain).Wait();
            }

        }
    }
}
