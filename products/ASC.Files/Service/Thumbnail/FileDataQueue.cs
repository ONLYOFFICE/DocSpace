namespace ASC.Files.ThumbnailBuilder;

[Singletone(Additional = typeof(WorkerExtension))]
public class FileDataQueue
{
    internal static readonly ConcurrentDictionary<object, FileData<int>> Queue
        = new ConcurrentDictionary<object, FileData<int>>();
}