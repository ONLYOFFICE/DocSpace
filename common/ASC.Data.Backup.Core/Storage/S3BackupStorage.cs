namespace ASC.Data.Backup.Storage;

internal class S3BackupStorage : IBackupStorage
{
    private readonly string _accessKeyId;
    private readonly string _secretAccessKey;
    private readonly string _bucket;
    private readonly string _region;
    private readonly ILog _logger;

    public S3BackupStorage(IOptionsMonitor<ILog> options, string accessKeyId, string secretAccessKey, string bucket, string region)
    {
        _logger = options.CurrentValue;
        _accessKeyId = accessKeyId;
        _secretAccessKey = secretAccessKey;
        _bucket = bucket;
        _region = region;
    }

    public string Upload(string storageBasePath, string localPath, Guid userId)
    {
        string key;

        if (string.IsNullOrEmpty(storageBasePath))
        {
            key = "backup/" + Path.GetFileName(localPath);
        }
        else
        {
            key = string.Concat(storageBasePath.Trim(new char[] { ' ', '/', '\\' }), "/", Path.GetFileName(localPath));
        }

        using (var fileTransferUtility = new TransferUtility(_accessKeyId, _secretAccessKey, RegionEndpoint.GetBySystemName(_region)))
        {
            fileTransferUtility.Upload(
                new TransferUtilityUploadRequest
                {
                    BucketName = _bucket,
                    FilePath = localPath,
                    StorageClass = S3StorageClass.StandardInfrequentAccess,
                    PartSize = 6291456, // 6 MB.
                        Key = key
                });
        }


        return key;
    }

    public void Download(string storagePath, string targetLocalPath)
    {
        var request = new GetObjectRequest
        {
            BucketName = _bucket,
            Key = GetKey(storagePath),
        };

        using var s3 = GetClient();
        using var response = s3.GetObjectAsync(request).Result;
        response.WriteResponseStreamToFileAsync(targetLocalPath, true, new System.Threading.CancellationToken());
    }

    public void Delete(string storagePath)
    {
        using var s3 = GetClient();
        s3.DeleteObjectAsync(new DeleteObjectRequest
        {
            BucketName = _bucket,
            Key = GetKey(storagePath)
        });
    }

    public bool IsExists(string storagePath)
    {
        using var s3 = GetClient();
        try
        {
            var request = new ListObjectsRequest { BucketName = _bucket, Prefix = GetKey(storagePath) };
            var response = s3.ListObjectsAsync(request).Result;

            return response.S3Objects.Count > 0;
        }
        catch (AmazonS3Exception ex)
        {
            _logger.Warn(ex);

            return false;
        }
    }

    public string GetPublicLink(string storagePath)
    {
        using var s3 = GetClient();

        return s3.GetPreSignedURL(
            new GetPreSignedUrlRequest
            {
                BucketName = _bucket,
                Key = GetKey(storagePath),
                Expires = DateTime.UtcNow.AddDays(1),
                Verb = HttpVerb.GET
            });
    }

    private string GetKey(string fileName)
    {
        // return "backup/" + Path.GetFileName(fileName);
        return fileName;
    }

    private AmazonS3Client GetClient()
    {
        return new AmazonS3Client(_accessKeyId, _secretAccessKey,
            new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(_region)
            });
    }
}
