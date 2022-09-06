// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.Data.Backup.Storage;

internal class S3BackupStorage : IBackupStorage
{
    private readonly string _accessKeyId;
    private readonly string _secretAccessKey;
    private readonly string _bucket;
    private readonly string _region;
    private readonly ILogger _logger;

    public S3BackupStorage(ILogger<S3BackupStorage> logger, string accessKeyId, string secretAccessKey, string bucket, string region)
    {
        _logger = logger;
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
        response.WriteResponseStreamToFileAsync(targetLocalPath, true, new CancellationToken());
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
            _logger.WarningWithException(ex);

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
