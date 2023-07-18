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

namespace ASC.ActiveDirectory.Base;

[Scope]
public class DbHelper
{
    private readonly IDbContextFactory<WebstudioDbContext> _activeDirectoryDbContextFactory;
    private readonly LdapSettings _ldapSettings;

    public DbHelper(
        IDbContextFactory<WebstudioDbContext> activeDirectoryDbContextFactory,
        LdapSettings ldapSettings)
    {
        _activeDirectoryDbContextFactory = activeDirectoryDbContextFactory;
        _ldapSettings = ldapSettings;
    }

    public async Task<List<int>> TenantsAsync()
    {
        var id = _ldapSettings.ID;
        var enableLdapAuthentication = _ldapSettings.EnableLdapAuthentication;

        await using var activeDirectoryDbContext = _activeDirectoryDbContextFactory.CreateDbContext();
        var data = await activeDirectoryDbContext.WebstudioSettings
            .Where(r => r.Id == id)
            .Join(activeDirectoryDbContext.Tenants, r => r.TenantId, r => r.Id, (settings, tenant) => new { settings, tenant })
            .Select(r => DbFunctionsExtension.JsonValue(nameof(r.settings.Data).ToLower(), enableLdapAuthentication.ToString()))
            .Distinct()
            .Select(r => r != null ? Convert.ToInt32(r) : 0)
            .ToListAsync();

        return data;
    }
}
