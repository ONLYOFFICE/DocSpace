namespace ASC.Web.Files.Core.Compress;

/// <summary>
/// Archives the data stream into the format .tar.gz
/// </summary>
[Scope]
public class CompressToTarGz : ICompress
{
    private GZipOutputStream _gzoStream;
    private TarOutputStream _gzip;
    private TarEntry _tarEntry;
    private readonly TempStream _tempStream;

    public CompressToTarGz(TempStream tempStream)
    {
        _tempStream = tempStream;
    }

    /// <summary></summary>
    /// <param name="stream">Accepts a new stream, it will contain an archive upon completion of work</param>
    public void SetStream(Stream stream)
    {
        _gzoStream = new GZipOutputStream(stream) { IsStreamOwner = false };
        _gzip = new TarOutputStream(_gzoStream, Encoding.UTF8);
        _gzoStream.IsStreamOwner = false;
    }

    /// <summary>
    /// The record name is created (the name of a separate file in the archive)
    /// </summary>
    /// <param name="title">File name with extension, this name will have the file in the archive</param>
    public void CreateEntry(string title)
    {
        _tarEntry = TarEntry.CreateTarEntry(title);
    }

    /// <summary>
    /// Transfer the file itself to the archive
    /// </summary>
    /// <param name="readStream">File data</param>
    public void PutStream(Stream readStream)
    {
        using (var buffered = _tempStream.GetBuffered(readStream))
        {
            _tarEntry.Size = buffered.Length;
            _gzip.PutNextEntry(_tarEntry);
            buffered.CopyTo(_gzip);
        }
    }

    /// <summary>
    /// Put an entry on the output stream.
    /// </summary>
    public void PutNextEntry()
    {
        _gzip.PutNextEntry(_tarEntry);
    }

    /// <summary>
    /// Closes the current entry.
    /// </summary>
    public void CloseEntry()
    {
        _gzip.CloseEntry();
    }

    /// <summary>
    /// Resource title (does not affect the work of the class)
    /// </summary>
    /// <returns></returns>
    public string Title => FilesUCResource.FilesWillBeCompressedTarGz;

    /// <summary>
    /// Extension the archive (does not affect the work of the class)
    /// </summary>
    /// <returns></returns>
    public string ArchiveExtension => CompressToArchive.TarExt;

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        _gzip?.Dispose();
        _gzoStream?.Dispose();
    }
}
