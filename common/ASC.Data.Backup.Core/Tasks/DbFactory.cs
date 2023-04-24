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

namespace ASC.Data.Backup.Tasks;

[Scope]
public class DbFactory
{
    public const string DefaultConnectionStringName = "default";

    internal string ConnectionStringSettings(string key = null, string connectionString = null, string region = "current")
    {
        if (!string.IsNullOrEmpty(connectionString))
        {
            return connectionString;
        }
        else
        {
            if (key != null)
            {
                return _configurationExtension.GetConnectionStrings(key, region).ConnectionString;
            }

            return _configurationExtension.GetConnectionStrings(DefaultConnectionStringName, region).ConnectionString;
        }
    }

    private DbProviderFactory DbProviderFactory
    {
        get
        {
            if (_dbProviderFactory == null)
            {
                var type = Type.GetType(_configuration["DbProviderFactories:mysql:type"], true);
                _dbProviderFactory = (DbProviderFactory)Activator.CreateInstance(type, true);
            }

            return _dbProviderFactory;
        }
    }

    private DbProviderFactory _dbProviderFactory;
    private readonly IConfiguration _configuration;
    private readonly ConfigurationExtension _configurationExtension;
    private readonly IServiceProvider _serviceProvider;
    private readonly ICache _cache;

    public DbFactory(IConfiguration configuration, ConfigurationExtension configurationExtension, IServiceProvider serviceProvider, ICache cache)
    {
        _configuration = configuration;
        _configurationExtension = configurationExtension;
        _serviceProvider = serviceProvider;
        _cache = cache;
    }

    public DbConnection OpenConnection(string path = "default", string connectionString = null, string region = "current")
    {
        var connection = DbProviderFactory.CreateConnection();
        if (connection != null)
        {
            connection.ConnectionString = EnsureConnectionTimeout(ConnectionStringSettings(path, connectionString, region));
            connection.Open();
        }

        return connection;
    }

    public IDbDataAdapter CreateDataAdapter()
    {
        var result = DbProviderFactory.CreateDataAdapter();
        if (result == null && DbProviderFactory is MySqlClientFactory)
        {
            result = new MySqlDataAdapter();
        }

        return result;
    }

    public DbCommand CreateLastInsertIdCommand()
    {
        var command = DbProviderFactory.CreateCommand();
        if (command != null)
        {
            command.CommandText =
                _configurationExtension.GetConnectionStrings(DefaultConnectionStringName).ProviderName.IndexOf("MySql", StringComparison.OrdinalIgnoreCase) != -1
                    ? "select Last_Insert_Id();"
                    : "select last_insert_rowid();";
        }

        return command;
    }

    public DbCommand CreateShowColumnsCommand(string tableName)
    {
        var command = DbProviderFactory.CreateCommand();
        if (command != null)
        {
            command.CommandText = "show columns from " + tableName + ";";
        }

        return command;
    }

    private static string EnsureConnectionTimeout(string connectionString)
    {
        if (!connectionString.Contains("Connection Timeout"))
        {
            connectionString = connectionString.TrimEnd(';') + ";Connection Timeout=90";
        }

        return connectionString;
    }
}
