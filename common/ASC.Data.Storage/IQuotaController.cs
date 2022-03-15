namespace ASC.Data.Storage;

public interface IQuotaController
{
    //quotaCheckFileSize:hack for Backup bug 48873
    void QuotaUsedAdd(string module, string domain, string dataTag, long size, bool quotaCheckFileSize = true);

    void QuotaUsedDelete(string module, string domain, string dataTag, long size);

    void QuotaUsedSet(string module, string domain, string dataTag, long size);

    void QuotaUsedCheck(long size);
}
