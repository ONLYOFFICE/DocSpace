namespace ASC.Common;

[Singletone]
public class TempStream
{
    private readonly TempPath _tempPath;

    public TempStream(TempPath tempPath)
    {
        _tempPath = tempPath;
    }

    public Stream GetBuffered(Stream srcStream)
    {
        ArgumentNullException.ThrowIfNull(srcStream);
        if (!srcStream.CanSeek || srcStream.CanTimeout)
        {
            //Buffer it
            var memStream = Create();
            srcStream.CopyTo(memStream);
            memStream.Position = 0;

            return memStream;
        }

        return srcStream;
    }

    public Stream Create()
    {
        return new FileStream(_tempPath.GetTempFileName(), FileMode.OpenOrCreate,
            FileAccess.ReadWrite, FileShare.Read, 4096, FileOptions.DeleteOnClose);
    }
}