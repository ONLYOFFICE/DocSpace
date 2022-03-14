/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using System.Linq;
using System.Threading.Tasks;

using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

using ASC.Core;

namespace ASC.Data.Storage.S3
{
    public class S3UploadGuard
    {
        private string accessKey;
        private string secretAccessKey;
        private string bucket;
        private string region;
        private bool configErrors;
        private bool configured;

        private CoreSettings CoreSettings { get; }
        public Configuration.Storage Storage { get; }

        public S3UploadGuard(CoreSettings coreSettings, Configuration.Storage storage)
        {
            CoreSettings = coreSettings;
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

            if (configErrors)
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
                var request = new ListMultipartUploadsRequest { BucketName = bucket };

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
                BucketName = bucket,
                Key = u.Key,
                UploadId = u.UploadId,
            };

            await client.AbortMultipartUploadAsync(request);
        }

        private AmazonS3Client GetClient()
        {
            var s3Config = new AmazonS3Config { UseHttp = true, MaxErrorRetry = 3, RegionEndpoint = RegionEndpoint.GetBySystemName(region) };
            return new AmazonS3Client(accessKey, secretAccessKey, s3Config);
        }

        private void Configure()
        {
            if (!configured)
            {
                var handler = Storage.GetHandler("s3");
                if (handler != null)
                {
                    var props = handler.GetProperties();
                    bucket = props["bucket"];
                    accessKey = props["acesskey"];
                    secretAccessKey = props["secretaccesskey"];
                    region = props["region"];
                }
                configErrors = string.IsNullOrEmpty(CoreSettings.BaseDomain) //localhost
                                || string.IsNullOrEmpty(accessKey)
                                || string.IsNullOrEmpty(secretAccessKey)
                                || string.IsNullOrEmpty(bucket)
                                || string.IsNullOrEmpty(region);

                configured = true;
            }
        }
    }
}
