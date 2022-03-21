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

namespace ASC.Core.Common.EF;

[Scope]
public class ConfigureDbContext<T> : IConfigureNamedOptions<T> where T : BaseDbContext, new()
{
    public const string BaseName = "default";
    private readonly EFLoggerFactory _loggerFactory;
    private readonly ConfigurationExtension _configuration;
    private readonly string _migrateAssembly;

    public ConfigureDbContext(EFLoggerFactory loggerFactory, ConfigurationExtension configuration)
    {
        _loggerFactory = loggerFactory;
        _configuration = configuration;
        _migrateAssembly = _configuration["testAssembly"];
    }

    public void Configure(string name, T context)
    {
        context.LoggerFactory = _loggerFactory;
        context.ConnectionStringSettings = _configuration.GetConnectionStrings(name) ?? _configuration.GetConnectionStrings(BaseName);
        context.MigrateAssembly = _migrateAssembly;
    }

    public void Configure(T context)
    {
        Configure(BaseName, context);
    }
}

public class ConfigureMultiRegionalDbContext<T> : IConfigureNamedOptions<MultiRegionalDbContext<T>> where T : BaseDbContext, new()
{
    private readonly string _baseName = "default";
    private readonly ConfigurationExtension _configuration;
    private readonly DbContextManager<T> _dbContext;

    public ConfigureMultiRegionalDbContext(ConfigurationExtension configuration, DbContextManager<T> dbContext)
    {
        _configuration = configuration;
        _dbContext = dbContext;
    }

    public void Configure(string name, MultiRegionalDbContext<T> context)
    {
        context.Context = new List<T>();

        const StringComparison cmp = StringComparison.InvariantCultureIgnoreCase;

        foreach (var c in _configuration.GetConnectionStrings().Where(r =>
        r.Name.Equals(name, cmp) || r.Name.StartsWith(name + ".", cmp) ||
        r.Name.Equals(_baseName, cmp) || r.Name.StartsWith(_baseName + ".", cmp)
        ))
        {
            context.Context.Add(_dbContext.Get(c.Name));
        }
    }

    public void Configure(MultiRegionalDbContext<T> context)
    {
        Configure(_baseName, context);
    }
}
