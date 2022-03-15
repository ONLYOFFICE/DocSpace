namespace ASC.Common.Data;

public static class StreamExtension
{
    public const int BufferSize = 2048; //NOTE: set to 2048 to fit in minimum tcp window

    public static void StreamCopyTo(this Stream srcStream, Stream dstStream, int length)
    {
        ArgumentNullException.ThrowIfNull(srcStream);
        ArgumentNullException.ThrowIfNull(dstStream);

        var buffer = new byte[BufferSize];
        int totalRead = 0;
        int readed;

        while ((readed = srcStream.Read(buffer, 0, length - totalRead > BufferSize ? BufferSize : length - totalRead)) > 0 && totalRead < length)
        {
            dstStream.Write(buffer, 0, readed);
            totalRead += readed;
        }
    }
}
