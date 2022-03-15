namespace System.IO;

[Singletone]
public class TempPath
{
    private readonly string _tempFolder;

    public TempPath(IConfiguration configuration)
    {
        string rootFolder = AppContext.BaseDirectory;
        if (string.IsNullOrEmpty(rootFolder))
        {
            rootFolder = Assembly.GetEntryAssembly().Location;
        }

        _tempFolder = configuration["temp"] ?? Path.Combine("..", "Data", "temp");
        if (!Path.IsPathRooted(_tempFolder))
        {
            _tempFolder = Path.GetFullPath(Path.Combine(rootFolder, _tempFolder));
        }

        if (!Directory.Exists(_tempFolder))
        {
            Directory.CreateDirectory(_tempFolder);
        }
    }

    public string GetTempPath()
    {
        return _tempFolder;
    }

    public string GetTempFileName()
    {
        FileStream f = null;
        string path;
        var count = 0;

        do
        {
            path = Path.Combine(_tempFolder, Path.GetRandomFileName());

            try
            {
                using (f = new FileStream(path, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read))
                {
                    return path;
                }
            }
            catch (IOException ex)
            {
                if (ex.HResult != -2147024816 || count++ > 65536)
                {
                    throw;
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                if (count++ > 65536)
                {
                    throw new IOException(ex.Message, ex);
                }
            }
        } while (f == null);

        return path;
    }
}