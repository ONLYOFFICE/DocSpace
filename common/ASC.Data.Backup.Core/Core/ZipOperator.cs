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
