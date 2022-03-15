namespace ASC.Data.Storage;

public class TenantQuotaController : IQuotaController
{
    private long CurrentSize
    {
        get
        {
            if (!_lazyCurrentSize.IsValueCreated)
            {
                return _currentSize = _lazyCurrentSize.Value;
            }

            return _currentSize;
        }
        set => _currentSize = value;
    }

    private readonly int _tenant;
    private readonly TenantManager _tenantManager;
    private Lazy<long> _lazyCurrentSize;
    private long _currentSize;

    public TenantQuotaController(int tenant, TenantManager tenantManager)
    {
        _tenant = tenant;
        _tenantManager = tenantManager;
        _lazyCurrentSize = new Lazy<long>(() => _tenantManager.FindTenantQuotaRows(tenant)
            .Where(r => UsedInQuota(r.Tag))
            .Sum(r => r.Counter));
    }

    #region IQuotaController Members

    public void QuotaUsedAdd(string module, string domain, string dataTag, long size, bool quotaCheckFileSize = true)
    {
        size = Math.Abs(size);
        if (UsedInQuota(dataTag))
        {
            QuotaUsedCheck(size, quotaCheckFileSize);
            CurrentSize += size;
        }

        SetTenantQuotaRow(module, domain, size, dataTag, true);
    }

    public void QuotaUsedDelete(string module, string domain, string dataTag, long size)
    {
        size = -Math.Abs(size);
        if (UsedInQuota(dataTag))
        {
            CurrentSize += size;
        }

        SetTenantQuotaRow(module, domain, size, dataTag, true);
    }

    public void QuotaUsedSet(string module, string domain, string dataTag, long size)
    {
        size = Math.Max(0, size);
        if (UsedInQuota(dataTag))
        {
            CurrentSize += size;
        }

        SetTenantQuotaRow(module, domain, size, dataTag, false);
    }

    public void QuotaUsedCheck(long size)
    {
        QuotaUsedCheck(size, true);
    }

    public void QuotaUsedCheck(long size, bool quotaCheckFileSize)
    {
        var quota = _tenantManager.GetTenantQuota(_tenant);
        if (quota != null)
        {
            if (quotaCheckFileSize && quota.MaxFileSize != 0 && quota.MaxFileSize < size)
            {
                throw new TenantQuotaException(string.Format("Exceeds the maximum file size ({0}MB)", BytesToMegabytes(quota.MaxFileSize)));
            }
            if (quota.MaxTotalSize != 0 && quota.MaxTotalSize < CurrentSize + size)
            {
                throw new TenantQuotaException(string.Format("Exceeded maximum amount of disk quota ({0}MB)", BytesToMegabytes(quota.MaxTotalSize)));
            }
        }
    }

    #endregion

    public long QuotaCurrentGet()
    {
        return CurrentSize;
    }

    private void SetTenantQuotaRow(string module, string domain, long size, string dataTag, bool exchange)
    {
            _tenantManager.SetTenantQuotaRow(
                new TenantQuotaRow { Tenant = _tenant, Path = $"/{module}/{domain}", Counter = size, Tag = dataTag },
                exchange);
    }

    private bool UsedInQuota(string tag)
    {
        return !string.IsNullOrEmpty(tag) && new Guid(tag) != Guid.Empty;
    }

    private double BytesToMegabytes(long bytes)
    {
        return Math.Round(bytes / 1024d / 1024d, 1);
    }
}
