namespace ASC.Data.Backup.Exceptions;

public class DbBackupException : Exception
{
    public DbBackupException() { }

    public DbBackupException(string message)
        : this(message, null) { }

    public DbBackupException(string message, Exception innerException)
        : base(message, innerException) { }
}
