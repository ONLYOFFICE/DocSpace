namespace ASC.Files.Core;

public class FileOptions<T>
{
    public bool Renamed { get; set; }
    public File<T> File { get; set; }
    public FileShare FileShare { get; set; }
    public Configuration<T> Configuration { get; set; }
}
