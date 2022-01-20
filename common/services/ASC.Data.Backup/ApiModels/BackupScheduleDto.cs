namespace ASC.Data.Backup.ApiModels;

public class BackupScheduleDto
{
    public string StorageType { get; set; }
    public IEnumerable<ItemKeyValuePair<object, object>> StorageParams { get; set; }
    public string BackupsStored { get; set; }
    public Cron CronParams { get; set; }
    public bool BackupMail { get; set; }
}

public class Cron
{
    public string Period { get; set; }
    public string Hour { get; set; }
    public string Day { get; set; }
}
