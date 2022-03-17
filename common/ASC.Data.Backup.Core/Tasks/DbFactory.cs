/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
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

namespace ASC.Data.Backup.Tasks;

[Scope]
public class DbFactory
{
    public const string DefaultConnectionStringName = "default";

    internal ConnectionStringSettings ConnectionStringSettings
    {
        get
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                return _configurationExtension.GetConnectionStrings(DefaultConnectionStringName);
            }
            else
            {
                return _configurationExtension.GetConnectionStrings(_connectionString);
            }

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
    private string _connectionString;
    private string _path;

    public DbFactory(IConfiguration configuration, ConfigurationExtension configurationExtension)
    {
        _configuration = configuration;
        _configurationExtension = configurationExtension;
    }

    public DbConnection OpenConnection(string path = "default", string connectionString = DefaultConnectionStringName)//TODO
    {
        _connectionString = connectionString;
        _path = path;
        var connection = DbProviderFactory.CreateConnection();
        if (connection != null)
        {
            connection.ConnectionString = EnsureConnectionTimeout(ConnectionStringSettings.ConnectionString);
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
                ConnectionStringSettings.ProviderName.IndexOf("MySql", StringComparison.OrdinalIgnoreCase) != -1
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
