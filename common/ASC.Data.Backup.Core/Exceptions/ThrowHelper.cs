namespace ASC.Data.Backup.Exceptions;

internal static class ThrowHelper
{
    public static DbBackupException CantDetectTenant(string tableName)
    {
            return new DbBackupException($"Can't detect tenant column for table {tableName}.");
    }

    public static DbBackupException CantOrderTables(IEnumerable<string> conflictingTables)
    {
            return new DbBackupException($"Can't order tables [\"{string.Join("\", \"", conflictingTables.ToArray())}\"].");
    }

    public static DbBackupException CantOrderModules(IEnumerable<Type> conflictingTypes)
    {
            return new DbBackupException($"Can't order modules [\"{string.Join("\", \"", conflictingTypes.Select(x => x.Name).ToArray())}\"].");
    }

    public static DbBackupException CantRestoreTable(string tableName, Exception reason)
    {
            return new DbBackupException($"Can't restore table {tableName}.", reason);
    }

    public static DbBackupException CantBackupTable(string tableName, Exception reason)
    {
            return new DbBackupException($"Can't backup table {tableName}.", reason);
    }

    public static DbBackupException CantDeleteTable(string tableName, Exception reason)
    {
            return new DbBackupException($"Can't delete table {tableName}.", reason);
    }
}
