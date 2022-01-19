namespace ASC.Data.Backup.ApiModels;

public class BackupRestoreDto
{
    public string BackupId { get; set; }
    public object StorageType { get; set; }
    public IEnumerable<ItemKeyValuePair<object, object>> StorageParams { get; set; }
    public bool Notify { get; set; }
}
