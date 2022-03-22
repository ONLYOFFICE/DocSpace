/*
 *
 * (c) Copyright Ascensio System Limited 2010-2021
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/


using ASC.Core.Common.EF.Model;

namespace ASC.ActiveDirectory.Base;
[Scope]
public class DbHelper
{
    private readonly Guid id = new Guid("{197149b3-fbc9-44c2-b42a-232f7e729c16}");
    private readonly bool enableLdapAuthentication = false;
    private readonly Lazy<WebstudioDbContext> _lazyWebstudioDbContext;
    private readonly Lazy<TenantDbContext> _lazyTenantDbContext;
    private WebstudioDbContext WebstudioDbContext { get => _lazyWebstudioDbContext.Value; }
    private TenantDbContext TenantDbContext { get => _lazyTenantDbContext.Value; }

    public DbHelper(
        DbContextManager<WebstudioDbContext> webstudioDbContext,
        DbContextManager<TenantDbContext> tenantDbContext)
    {
        _lazyWebstudioDbContext = new Lazy<WebstudioDbContext>(() => webstudioDbContext.Value);
        _lazyTenantDbContext = new Lazy<TenantDbContext>(() => tenantDbContext.Value);
    }

    public List<int> GetTenants()
    {

        var q = WebstudioDbContext.WebstudioSettings
            .Where(r => r.Id == id).ToList();

        var data = q
            .Join(TenantDbContext.Tenants, r => r.TenantId, r => r.Id, (settings, tenant) => new { settings, tenant })
            .Select(r => JsonExtensions.JsonValue(nameof(r.settings.Data).ToLower(), enableLdapAuthentication.ToString()))
            //.Where()
            .Distinct()
            .Select(r => Convert.ToInt32(r[0]))
            .ToList();

        return data;
    }

}
