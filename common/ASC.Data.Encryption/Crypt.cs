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

/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
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


namespace ASC.Data.Encryption;

[Transient]
public class Crypt : ICrypt
{
    private string Storage { get; set; }
    private EncryptionSettings Settings { get; set; }
    private string TempDir { get; set; }

    private IConfiguration Configuration { get; set; }
    public TempPath TempPath { get; }

    public void Init(string storageName, EncryptionSettings encryptionSettings)
    {
        Storage = storageName;
        Settings = encryptionSettings;
        TempDir = TempPath.GetTempPath();
    }

    public Crypt(IConfiguration configuration, TempPath tempPath)
    {
        Configuration = configuration;
        TempPath = tempPath;
    }

    public byte Version { get { return 1; } }

    public void EncryptFile(string filePath)
    {
        if (string.IsNullOrEmpty(Settings.Password)) return;

        var metadata = new Metadata(Configuration);

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
        var fileInfo = new FileInfo(filePath);

        if (fileInfo.IsReadOnly)
        {
            fileInfo.IsReadOnly = false;
        }

        var ecryptedFilePath = GetUniqFileName(filePath, ".enc");
        try
        {
            var metadata = new Metadata(Configuration);

            metadata.Initialize(Version, password, fileInfo.Length);

            using (var ecryptedFileStream = new FileStream(ecryptedFilePath, FileMode.Create))
            {
                metadata.WriteToStream(ecryptedFileStream);

                using (var algorithm = metadata.GetCryptographyAlgorithm())
                {
                    using var transform = algorithm.CreateEncryptor();
                    using var cryptoStream = new CryptoStreamWrapper(ecryptedFileStream, transform, CryptoStreamMode.Write);
                    using (var fileStream = File.OpenRead(filePath))
                    {
                        fileStream.CopyTo(cryptoStream);
                        fileStream.Close();
                    }

                    cryptoStream.FlushFinalBlock();

                    metadata.ComputeAndWriteHmacHash(ecryptedFileStream);

                    cryptoStream.Close();
                }

                ecryptedFileStream.Close();
            }

            ReplaceFile(ecryptedFilePath, filePath);
        }
        catch (Exception)
        {
            if (File.Exists(ecryptedFilePath))
            {
                File.Delete(ecryptedFilePath);
            }

            throw;
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
            var metadata = new Metadata(Configuration);

            metadata.Initialize(password);

            using (var fileStream = File.OpenRead(filePath))
            {
                if (!metadata.TryReadFromStream(fileStream, Version)) return;

                metadata.ComputeAndValidateHmacHash(fileStream);

                using (var decryptedFileStream = new FileStream(decryptedFilePath, FileMode.Create))
                {
                    using (var algorithm = metadata.GetCryptographyAlgorithm())
                    {
                        using var transform = algorithm.CreateDecryptor();
                        using var cryptoStream = new CryptoStreamWrapper(decryptedFileStream, transform, CryptoStreamMode.Write);
                        fileStream.CopyTo(cryptoStream);

                        cryptoStream.FlushFinalBlock();
                        cryptoStream.Close();
                    }

                    decryptedFileStream.Close();
                }

                fileStream.Close();
            }

            ReplaceFile(decryptedFilePath, filePath);
        }
        catch (Exception)
        {
            if (File.Exists(decryptedFilePath))
            {
                File.Delete(decryptedFilePath);
            }

            throw;
        }
    }

    private Stream GetReadMemoryStream(string filePath, string password)
    {
        var decryptedMemoryStream = new MemoryStream(); //TODO: MemoryStream or temporary decrypted file on disk?

        var metadata = new Metadata(Configuration);

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
            using var transform = algorithm.CreateDecryptor();
            using var cryptoStream = new CryptoStreamWrapper(fileStream, transform, CryptoStreamMode.Read);
            cryptoStream.CopyTo(decryptedMemoryStream);
            cryptoStream.Close();
        }

        fileStream.Close();

        decryptedMemoryStream.Seek(0, SeekOrigin.Begin);

        return decryptedMemoryStream;
    }

    private Stream GetReadStream(string filePath, string password)
    {
        var metadata = new Metadata(Configuration);

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
        var metadata = new Metadata(Configuration);

        metadata.Initialize(password);

        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, metadata.GetMetadataLength(), FileOptions.SequentialScan);
        if (metadata.TryReadFromStream(fileStream, Version))
        {
            return metadata.GetFileSize();
        }
        else
        {
            return new FileInfo(filePath).Length;
        }
    }


    private string GetUniqFileName(string filePath, string ext)
    {
        var dir = string.IsNullOrEmpty(TempDir) ? Path.GetDirectoryName(filePath) : TempDir;
        var name = Path.GetFileNameWithoutExtension(filePath);
        var result = CrossPlatform.PathCombine(dir, $"{Storage}_{name}{ext}");
        var index = 1;

        while (File.Exists(result))
        {
            result = CrossPlatform.PathCombine(dir, $"{Storage}_{name}({index++}){ext}");
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
        catch (Exception)
        {
            File.Move(tempFilePath, originalFilePath);
            throw;
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
