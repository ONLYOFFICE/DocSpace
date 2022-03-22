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

public class BaseDbContextManager<T> : OptionsManager<T>, IDisposable where T : class, IDisposable, IAsyncDisposable, new()
{
    private readonly Dictionary<string, T> _pairs;
    private readonly MigrationHistory _migrationHistory;
    private readonly List<T> _asyncList;
    private readonly IOptionsFactory<T> _factory;
    private readonly IConfiguration _configuration;

    public BaseDbContextManager(IOptionsFactory<T> factory, IConfiguration configuration,
        MigrationHistory migrationHistory) : base(factory)
    {
        _pairs = new Dictionary<string, T>();
        _asyncList = new List<T>();
        _factory = factory;
        _configuration = configuration;
        _migrationHistory = migrationHistory;
    }

    public override T Get(string name)
    {
        if (!_pairs.ContainsKey(name))
        {
            var t = base.Get(name);
            _pairs.Add(name, t);

            if (t is BaseDbContext dbContext)
            {
                if (_configuration["migration:enabled"] == "true"
                    && _migrationHistory.TryAddMigratedContext(t.GetType()))
                {
                    dbContext.Migrate();
                }
            }
        }

        return _pairs[name];
    }

    public T GetNew(string name = "default")
    {
        var result = _factory.Create(name);

        _asyncList.Add(result);

        return result;
    }

    public void Dispose()
    {
        foreach (var v in _pairs)
        {
            v.Value.Dispose();
        }

        foreach (var v in _asyncList)
        {
            v.Dispose();
        }
    }
}

[Scope(typeof(ConfigureDbContext<>))]
public class DbContextManager<T> : BaseDbContextManager<T> where T : BaseDbContext, new()
{
    public DbContextManager(IOptionsFactory<T> factory, IConfiguration configuration,
        MigrationHistory migrationHistory) : base(factory, configuration, migrationHistory)
    {
    }
}

public class MultiRegionalDbContextManager<T> : BaseDbContextManager<MultiRegionalDbContext<T>> where T : BaseDbContext, new()
{
    public MultiRegionalDbContextManager(IOptionsFactory<MultiRegionalDbContext<T>> factory, IConfiguration configuration,
        MigrationHistory migrationHistory) : base(factory, configuration, migrationHistory)
    {
    }
}

public static class DbContextManagerExtension
{
    public static DIHelper AddDbContextManagerService<T>(this DIHelper services) where T : BaseDbContext, new()
    {
        //TODO
        //services.TryAddScoped<MultiRegionalDbContextManager<T>>();
        //services.TryAddScoped<IConfigureOptions<MultiRegionalDbContext<T>>, ConfigureMultiRegionalDbContext<T>>();
        return services;
    }
}
