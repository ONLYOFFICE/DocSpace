namespace ASC.Data.Storage.S3;

public class S3UploadGuard
{
    public Configuration.Storage Storage { get; }

    private readonly CoreSettings _coreSettings;
    private string _accessKey;
    private string _secretAccessKey;
    private string _bucket;
    private string _region;
    private bool _configErrors;
    private bool _configured;

    public S3UploadGuard(CoreSettings coreSettings, Configuration.Storage storage)
    {
        _coreSettings = coreSettings;
        Storage = storage;
    }

    public void DeleteExpiredUploadsAsync(TimeSpan trustInterval)
    {
            var task = new Task(async () =>
        {
                await DeleteExpiredUploadsActionAsync(trustInterval);
        }, TaskCreationOptions.LongRunning);

        task.Start();
    }

        private Task DeleteExpiredUploadsActionAsync(TimeSpan trustInterval)
    {
        Configure();

        if (_configErrors)
        {
                return Task.CompletedTask;
        }

            return InternalDeleteExpiredUploadsActionAsync(trustInterval);
        }

        private async Task InternalDeleteExpiredUploadsActionAsync(TimeSpan trustInterval)
        {
        using var s3 = GetClient();
        var nextKeyMarker = string.Empty;
        var nextUploadIdMarker = string.Empty;
        bool isTruncated;

        do
        {
            var request = new ListMultipartUploadsRequest { BucketName = _bucket };

            if (!string.IsNullOrEmpty(nextKeyMarker))
            {
                request.KeyMarker = nextKeyMarker;
            }

            if (!string.IsNullOrEmpty(nextUploadIdMarker))
            {
                request.UploadIdMarker = nextUploadIdMarker;
            }

                var response = await s3.ListMultipartUploadsAsync(request);

            foreach (var u in response.MultipartUploads.Where(x => x.Initiated + trustInterval <= DateTime.UtcNow))
            {
                    await AbortMultipartUploadAsync(u, s3);
            }

            isTruncated = response.IsTruncated;
            nextKeyMarker = response.NextKeyMarker;
            nextUploadIdMarker = response.NextUploadIdMarker;
        }
        while (isTruncated);
    }

        private async Task AbortMultipartUploadAsync(MultipartUpload u, AmazonS3Client client)
    {
        var request = new AbortMultipartUploadRequest
        {
            BucketName = _bucket,
            Key = u.Key,
            UploadId = u.UploadId,
        };

            await client.AbortMultipartUploadAsync(request);
    }

    private AmazonS3Client GetClient()
    {
        var s3Config = new AmazonS3Config { UseHttp = true, MaxErrorRetry = 3, RegionEndpoint = RegionEndpoint.GetBySystemName(_region) };

        return new AmazonS3Client(_accessKey, _secretAccessKey, s3Config);
    }

    private void Configure()
    {
        if (!_configured)
        {
            var handler = Storage.GetHandler("s3");
            if (handler != null)
            {
                var props = handler.GetProperties();
                _bucket = props["bucket"];
                _accessKey = props["acesskey"];
                _secretAccessKey = props["secretaccesskey"];
                _region = props["region"];
            }
            _configErrors = string.IsNullOrEmpty(_coreSettings.BaseDomain) //localhost
                            || string.IsNullOrEmpty(_accessKey)
                            || string.IsNullOrEmpty(_secretAccessKey)
                            || string.IsNullOrEmpty(_bucket)
                            || string.IsNullOrEmpty(_region);

            _configured = true;
        }
    }
}
