namespace ASC.Web.Files.Core.Compress;

/// <summary>
/// Archives the data stream in the format selected in the settings
/// </summary>
[Scope]
public class CompressToArchive : ICompress
{
    private readonly ICompress _compress;

    internal static readonly string TarExt = ".tar.gz";
    internal static readonly string ZipExt = ".zip";
    private static List<string> _exts = new List<string>(2) { TarExt, ZipExt };

    public CompressToArchive(FilesSettingsHelper filesSettings, CompressToTarGz compressToTarGz, CompressToZip compressToZip)
    {
        _compress = filesSettings.DownloadTarGz
            ? compressToTarGz
            : compressToZip;
    }

    public static string GetExt(IServiceProvider serviceProvider, string ext)
    {
        if (_exts.Contains(ext))
        {
            return ext;
        }

        using var zip = serviceProvider.GetService<CompressToArchive>();

        return zip.ArchiveExtension;
    }

    public void SetStream(Stream stream)
    {
        _compress.SetStream(stream);
    }

    /// <summary>
    /// The record name is created (the name of a separate file in the archive)
    /// </summary>
    /// <param name="title">File name with extension, this name will have the file in the archive</param>
    public void CreateEntry(string title)
    {
        _compress.CreateEntry(title);
    }

    /// <summary>
    /// Transfer the file itself to the archive
    /// </summary>
    /// <param name="readStream">File data</param>
    public void PutStream(Stream readStream)
    {
        _compress.PutStream(readStream);
    }

    /// <summary>
    /// Put an entry on the output stream.
    /// </summary>
    public void PutNextEntry()
    {
        _compress.PutNextEntry();
    }

    /// <summary>
    /// Closes the current entry.
    /// </summary>
    public void CloseEntry()
    {
        _compress.CloseEntry();
    }

    /// <summary>
    /// Resource title (does not affect the work of the class)
    /// </summary>
    /// <returns></returns>
    public string Title => _compress.Title;

    /// <summary>
    /// Extension the archive (does not affect the work of the class)
    /// </summary>
    /// <returns></returns>
    public string ArchiveExtension => _compress.ArchiveExtension;

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        _compress.Dispose();
    }
}
