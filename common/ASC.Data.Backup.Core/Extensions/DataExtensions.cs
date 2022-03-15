namespace ASC.Data.Backup.Extensions;

public static class DataExtensions
{
    public static DbCommand WithTimeout(this DbCommand command, int timeout)
    {
        if (command != null)
        {
            command.CommandTimeout = timeout;
        }

        return command;
    }

    public static DbConnection Fix(this DbConnection connection)
    {
        if (connection != null && connection.State != ConnectionState.Open)
        {
            connection.Close();
            connection.Open();
        }

        return connection;
    }
}
