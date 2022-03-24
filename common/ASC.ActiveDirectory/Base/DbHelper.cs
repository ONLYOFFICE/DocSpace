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


namespace ASC.ActiveDirectory.Base;
[Scope]
public class DbHelper
{
    private readonly Lazy<ActiveDirectoryDbContext> _lazyActiveDirectoryDbContext;
    private readonly LdapSettings _ldapSettings;
    private ActiveDirectoryDbContext ActiveDirectoryDbContext { get => _lazyActiveDirectoryDbContext.Value; }

    public DbHelper(
        DbContextManager<ActiveDirectoryDbContext> activeDirectoryDbContext,
        LdapSettings ldapSettings)
    {
        _ldapSettings = ldapSettings;
        _lazyActiveDirectoryDbContext = new Lazy<ActiveDirectoryDbContext>(() => activeDirectoryDbContext.Value);
    }

    public List<int> GetTenants()
    {
        var id = _ldapSettings.ID;
        var enableLdapAuthentication = _ldapSettings.EnableLdapAuthentication;

        var data = ActiveDirectoryDbContext.WebstudioSettings
            .Where(r => r.Id == id)
            .Join(ActiveDirectoryDbContext.Tenants, r => r.TenantId, r => r.Id, (settings, tenant) => new { settings, tenant })
            .Select(r => JsonExtensions.JsonValue(nameof(r.settings.Data).ToLower(), enableLdapAuthentication.ToString()))
            .Distinct()
            .Select(r => Convert.ToInt32(r))
            .ToList();

        return data;
    }
}
