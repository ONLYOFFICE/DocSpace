namespace ASC.Data.Backup.Tasks;

internal static class KeyHelper
{
    private const string Databases = "databases";

    public static string GetDumpKey()
    {
        return "dump";
    }

    public static string GetDatabaseSchema()
    {
            return $"{Databases}/scheme";
    }

    public static string GetDatabaseData()
    {
            return $"{Databases}/data";
    }

    public static string GetDatabaseSchema(string table)
    {
            return $"{GetDatabaseSchema()}/{table}";
    }

    public static string GetDatabaseData(string table)
    {
            return $"{GetDatabaseData()}/{table}";
    }

    public static string GetTableZipKey(IModuleSpecifics module, string tableName)
    {
            return $"{Databases}/{module.ConnectionStringName}/{tableName}";
    }

    public static string GetZipKey(this BackupFileInfo file)
    {
        return CrossPlatform.PathCombine(file.Module, file.Domain, file.Path);
    }

    public static string GetStorage()
    {
        return "storage";
    }
    public static string GetStorageRestoreInfoZipKey()
    {
            return $"{GetStorage()}/restore_info";
    }
}
