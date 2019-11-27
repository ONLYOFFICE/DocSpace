/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Dialects;
using ASC.Common.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ASC.Common.Data
{
    public class DbProviderFactoryCustom
    {
        public string Name { get; set; }
        public string Invariant { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
    }

    public class DbRegistry
    {
        private const string DEFAULT = "DEFAULT";
        private static readonly object syncRoot = new object();
        private readonly IDictionary<string, DbProviderFactory> providers = new Dictionary<string, DbProviderFactory>(StringComparer.InvariantCultureIgnoreCase);
        private readonly IDictionary<string, string> connnectionStrings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        private readonly IDictionary<string, ISqlDialect> dialects = new Dictionary<string, ISqlDialect>(StringComparer.InvariantCultureIgnoreCase);

        public IConfiguration Configuration { get; }

        public DbRegistry(IConfiguration configuration)
        {
            dialects["MySql.Data.MySqlClient.MySqlClientFactory"] = new MySQLDialect();
            dialects["System.Data.SQLite.SQLiteFactory"] = new SQLiteDialect();
            Configuration = configuration;
            Configure();
        }

        internal void RegisterDatabase(string databaseId, DbProviderFactory providerFactory, string connectionString)
        {
            if (string.IsNullOrEmpty(databaseId)) throw new ArgumentNullException(nameof(databaseId));
            if (providerFactory == null) throw new ArgumentNullException(nameof(providerFactory));

            if (!providers.ContainsKey(databaseId))
            {
                lock (syncRoot)
                {
                    if (!providers.ContainsKey(databaseId))
                    {
                        providers.Add(databaseId, providerFactory);
                        if (!string.IsNullOrEmpty(connectionString))
                        {
                            connnectionStrings.Add(databaseId, connectionString);
                        }
                    }
                }
            }
        }

        internal void RegisterDatabase(string databaseId, string providerInvariantName, string connectionString)
        {
            RegisterDatabase(databaseId, DbProviderFactories.GetFactory(providerInvariantName), connectionString);
        }

        public void RegisterDatabase(string databaseId, ConnectionStringSettings connectionString)
        {
            RegisterDatabase(databaseId, connectionString.ProviderName, connectionString.ConnectionString);
        }

        public void UnRegisterDatabase(string databaseId)
        {
            if (string.IsNullOrEmpty(databaseId)) throw new ArgumentNullException(nameof(databaseId));

            if (providers.ContainsKey(databaseId))
            {
                lock (syncRoot)
                {
                    if (providers.ContainsKey(databaseId))
                    {
                        providers.Remove(databaseId);
                    }
                }
            }

            if (connnectionStrings.ContainsKey(databaseId))
            {
                lock (syncRoot)
                {
                    if (connnectionStrings.ContainsKey(databaseId))
                    {
                        connnectionStrings.Remove(databaseId);
                    }
                }
            }
        }

        public bool IsDatabaseRegistered(string databaseId)
        {
            lock (syncRoot)
            {
                return providers.ContainsKey(databaseId);
            }
        }

        public DbConnection CreateDbConnection(string databaseId)
        {
            if (!providers.ContainsKey(databaseId))
            {
                databaseId = DEFAULT;
            }

            var connection = providers[databaseId].CreateConnection();
            if (connnectionStrings.ContainsKey(databaseId))
            {
                connection.ConnectionString = connnectionStrings[databaseId];
            }
            return connection;
        }

        public DbProviderFactory GetDbProviderFactory(string databaseId)
        {
            if (!providers.ContainsKey(databaseId))
            {
                databaseId = DEFAULT;
            }
            return providers.ContainsKey(databaseId) ? providers[databaseId] : null;
        }

        public ConnectionStringSettings GetConnectionString(string databaseId)
        {
            if (!connnectionStrings.ContainsKey(databaseId))
            {
                databaseId = DEFAULT;
            }
            return connnectionStrings.ContainsKey(databaseId) ? new ConnectionStringSettings(databaseId, connnectionStrings[databaseId], providers[databaseId].GetType().Name) : null;
        }

        public ISqlDialect GetSqlDialect(string databaseId)
        {
            var provider = GetDbProviderFactory(databaseId);
            if (provider != null && dialects.ContainsKey(provider.GetType().FullName))
            {
                return dialects[provider.GetType().FullName];
            }
            return SqlDialect.Default;
        }


        private void Configure()
        {
            foreach (var cs in Configuration.GetSettings<DbProviderFactoryCustom>("DbProviderFactories"))
            {
                DbProviderFactories.RegisterFactory(cs.Invariant, cs.Type);
            }

            var factories = DbProviderFactories.GetFactoryClasses();
            AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory); //SQLite

            foreach (var cs in Configuration.GetConnectionStrings())
            {
                var factory = factories.Rows.Find(cs.ProviderName);
                if (factory == null)
                {
                    throw new ConfigurationErrorsException("Db factory " + cs.ProviderName + " not found.");
                }
                RegisterDatabase(cs.Name, cs);
            }
        }
    }

    public static class DbRegistryExtension
    {
        public static IServiceCollection AddDbRegistryService(this IServiceCollection services)
        {
            services.TryAddSingleton<DbRegistry>();

            return services;
        }
    }
}