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
            command.CommandText =
                ConnectionStringSettings.ProviderName.IndexOf("MySql", StringComparison.OrdinalIgnoreCase) != -1
                    ? "select Last_Insert_Id();"
                    : "select last_insert_rowid();";

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
