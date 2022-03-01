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

namespace ASC.Data.Backup;

public class ZipWriteOperator : IDataWriteOperator
{

    private readonly TarOutputStream _tarOutputStream;
    private readonly TempStream _tempStream;

    public ZipWriteOperator(TempStream tempStream, string targetFile)
    {
        var file = new FileStream(targetFile, FileMode.Create);
        var gZipOutputStream = new GZipOutputStream(file);
        _tarOutputStream = new TarOutputStream(gZipOutputStream, Encoding.UTF8);
        _tempStream = tempStream;
    }

    public void WriteEntry(string key, Stream stream)
    {
        using (var buffered = _tempStream.GetBuffered(stream))
        {
            var entry = TarEntry.CreateTarEntry(key);
            entry.Size = buffered.Length;
            _tarOutputStream.PutNextEntry(entry);
            buffered.Position = 0;
            buffered.CopyTo(_tarOutputStream);
            _tarOutputStream.CloseEntry();
        }
    }

    public void Dispose()
    {
        _tarOutputStream.Close();
        _tarOutputStream.Dispose();
    }
}

public class ZipReadOperator : IDataReadOperator
{
    private readonly string _tmpDir;

    public ZipReadOperator(string targetFile)
    {
        _tmpDir = Path.Combine(Path.GetDirectoryName(targetFile), Path.GetFileNameWithoutExtension(targetFile).Replace('>', '_').Replace(':', '_').Replace('?', '_'));

        using (var stream = File.OpenRead(targetFile))
        using (var reader = new GZipInputStream(stream))
        using (var tarOutputStream = TarArchive.CreateInputTarArchive(reader, Encoding.UTF8))
        {
            tarOutputStream.ExtractContents(_tmpDir);
        }

        File.Delete(targetFile);
    }

    public Stream GetEntry(string key)
    {
        var filePath = Path.Combine(_tmpDir, key);

        return File.Exists(filePath)
            ? File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read)
            : null;
    }

    public IEnumerable<string> GetEntries(string key)
    {
        var path = Path.Combine(_tmpDir, key);
        var files = Directory.EnumerateFiles(path);

        return files;
    }

    public void Dispose()
    {
        if (Directory.Exists(_tmpDir))
        {
            Directory.Delete(_tmpDir, true);
        }
    }
}
