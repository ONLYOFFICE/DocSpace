namespace ASC.Core.Common;

public static class ByteConverter
{
    public static long GetInMBytes(long bytes)
    {
        const long MB = 1024 * 1024;

        return bytes < MB * MB ? bytes / MB : bytes;
    }

    public static long GetInBytes(long bytes)
    {
        const long MB = 1024 * 1024;

        return bytes < MB ? bytes * MB : bytes;
    }
}
