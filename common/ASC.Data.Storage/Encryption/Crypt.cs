/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/


using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;

using ASC.Common;

using Microsoft.Extensions.Configuration;

namespace ASC.Data.Storage.Encryption
{
    public class Crypt : ICrypt
    {
        private string Storage { get; set; }
        private EncryptionSettings Settings { get; set; }
        private string TempDir { get; set; }

        private IConfiguration Configuration { get; set; }
        private EncryptionFactory EncryptionFactory { get; set; }

        public void Init(string storageName, EncryptionSettings encryptionSettings)
        {
            Storage = storageName;
            Settings = encryptionSettings;
            TempDir = Configuration["storage:encryption:tempdir"] ?? Path.GetTempPath();
        }

        public Crypt(IConfiguration configuration, EncryptionFactory encryptionFactory)
        {
            Configuration = configuration;
            EncryptionFactory = encryptionFactory;
        }

        public byte Version { get { return 1; } }

        public void EncryptFile(string filePath)
        {
            var metadata = EncryptionFactory.GetMetadata();

            metadata.Initialize(Settings.Password);

            using (var fileStream = File.OpenRead(filePath))
            {
                if (metadata.TryReadFromStream(fileStream, Version))
                {
                    return;
                }
            }

            EncryptFile(filePath, Settings.Password);
        }

        public void DecryptFile(string filePath)
        {
            if (Settings.Status == EncryprtionStatus.Decrypted)
            {
                return;
            }

            DecryptFile(filePath, Settings.Password);
        }

        public Stream GetReadStream(string filePath)
        {
            if (Settings.Status == EncryprtionStatus.Decrypted)
            {
                return File.OpenRead(filePath);
            }

            return GetReadStream(filePath, Settings.Password);
        }

        public long GetFileSize(string filePath)
        {
            if (Settings.Status == EncryprtionStatus.Decrypted)
            {
                return new FileInfo(filePath).Length;
            }

            return GetFileSize(filePath, Settings.Password);
        }


        private void EncryptFile(string filePath, string password)
        {
            if (string.IsNullOrEmpty(password)) return;

            var fileInfo = new FileInfo(filePath);

            if (fileInfo.IsReadOnly)
            {
                fileInfo.IsReadOnly = false;
            }

            var ecryptedFilePath = GetUniqFileName(filePath, ".enc");

            try
            {
                var metadata = EncryptionFactory.GetMetadata();

                metadata.Initialize(Version, password, fileInfo.Length);

                using (var ecryptedFileStream = new FileStream(ecryptedFilePath, FileMode.Create))
                {
                    metadata.WriteToStream(ecryptedFileStream);

                    using (var algorithm = metadata.GetCryptographyAlgorithm())
                    {
                        using (var transform = algorithm.CreateEncryptor())
                        {
                            using (var cryptoStream = new CryptoStreamWrapper(ecryptedFileStream, transform, CryptoStreamMode.Write))
                            {
                                using (var fileStream = File.OpenRead(filePath))
                                {
                                    fileStream.CopyTo(cryptoStream);
                                    fileStream.Close();
                                }

                                cryptoStream.FlushFinalBlock();

                                metadata.ComputeAndWriteHmacHash(ecryptedFileStream);

                                cryptoStream.Close();
                            }
                        }
                    }

                    ecryptedFileStream.Close();
                }

                ReplaceFile(ecryptedFilePath, filePath);
            }
            catch (Exception exception)
            {
                if (File.Exists(ecryptedFilePath))
                {
                    File.Delete(ecryptedFilePath);
                }

                throw exception;
            }
        }

        private void DecryptFile(string filePath, string password)
        {
            var fileInfo = new FileInfo(filePath);

            if (fileInfo.IsReadOnly)
            {
                fileInfo.IsReadOnly = false;
            }

            var decryptedFilePath = GetUniqFileName(filePath, ".dec");

            try
            {
                var metadata = EncryptionFactory.GetMetadata();

                metadata.Initialize(password);

                using (var fileStream = File.OpenRead(filePath))
                {
                    if (!metadata.TryReadFromStream(fileStream, Version)) return;

                    metadata.ComputeAndValidateHmacHash(fileStream);

                    using (var decryptedFileStream = new FileStream(decryptedFilePath, FileMode.Create))
                    {
                        using (var algorithm = metadata.GetCryptographyAlgorithm())
                        {
                            using (var transform = algorithm.CreateDecryptor())
                            {
                                using (var cryptoStream = new CryptoStreamWrapper(decryptedFileStream, transform, CryptoStreamMode.Write))
                                {
                                    fileStream.CopyTo(cryptoStream);

                                    cryptoStream.FlushFinalBlock();
                                    cryptoStream.Close();
                                }
                            }
                        }

                        decryptedFileStream.Close();
                    }

                    fileStream.Close();
                }

                ReplaceFile(decryptedFilePath, filePath);
            }
            catch (Exception exception)
            {
                if (File.Exists(decryptedFilePath))
                {
                    File.Delete(decryptedFilePath);
                }

                throw exception;
            }
        }

        private Stream GetReadMemoryStream(string filePath, string password)
        {
            var decryptedMemoryStream = new MemoryStream(); //TODO: MemoryStream or temporary decrypted file on disk?

            var metadata = EncryptionFactory.GetMetadata();

            metadata.Initialize(password);

            var fileStream = File.OpenRead(filePath);

            if (!metadata.TryReadFromStream(fileStream, Version))
            {
                decryptedMemoryStream.Close();
                fileStream.Seek(0, SeekOrigin.Begin);
                return fileStream;
            }

            metadata.ComputeAndValidateHmacHash(fileStream);

            using (var algorithm = metadata.GetCryptographyAlgorithm())
            {
                using (var transform = algorithm.CreateDecryptor())
                {
                    using (var cryptoStream = new CryptoStreamWrapper(fileStream, transform, CryptoStreamMode.Read))
                    {
                        cryptoStream.CopyTo(decryptedMemoryStream);
                        cryptoStream.Close();
                    }
                }
            }

            fileStream.Close();

            decryptedMemoryStream.Seek(0, SeekOrigin.Begin);

            return decryptedMemoryStream;
        }

        private Stream GetReadStream(string filePath, string password)
        {
            var metadata = EncryptionFactory.GetMetadata();

            metadata.Initialize(password);

            var fileStream = File.OpenRead(filePath);

            if (!metadata.TryReadFromStream(fileStream, Version))
            {
                fileStream.Seek(0, SeekOrigin.Begin);
                return fileStream;
            }

            metadata.ComputeAndValidateHmacHash(fileStream);

            var wrapper = new StreamWrapper(fileStream, metadata);

            return wrapper;
        }

        private long GetFileSize(string filePath, string password)
        {
            var metadata = EncryptionFactory.GetMetadata();

            metadata.Initialize(password);

            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, metadata.GetMetadataLength(), FileOptions.SequentialScan))
            {
                if (metadata.TryReadFromStream(fileStream, Version))
                {
                    return metadata.GetFileSize();
                }
                else
                {
                    return new FileInfo(filePath).Length;
                }
            }
        }


        private string GetUniqFileName(string filePath, string ext)
        {
            var dir = string.IsNullOrEmpty(TempDir) ? Path.GetDirectoryName(filePath) : TempDir;
            var name = Path.GetFileNameWithoutExtension(filePath);
            var result = Path.Combine(dir, string.Format("{0}_{1}{2}", Storage, name, ext));
            var index = 1;

            while (File.Exists(result))
            {
                result = Path.Combine(dir, string.Format("{0}_{1}({2}){3}", Storage, name, index++, ext));
            }

            return result;
        }

        private void ReplaceFile(string modifiedFilePath, string originalFilePath)
        {
            var tempFilePath = GetUniqFileName(originalFilePath, ".tmp");

            File.Move(originalFilePath, tempFilePath);

            try
            {
                File.Move(modifiedFilePath, originalFilePath);
            }
            catch(Exception exception)
            {
                File.Move(tempFilePath, originalFilePath);
                throw exception;
            }
            finally
            {
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }
    }
}
