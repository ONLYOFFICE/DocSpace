namespace ASC.Data.Backup.ApiModels;

public class BackupDto
{
    public string StorageType { get; set; }
    public bool BackupMail { get; set; }
    public IEnumerable<ItemKeyValuePair<object, object>> StorageParams { get; set; }
}
