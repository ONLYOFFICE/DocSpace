namespace ASC.Data.Storage;

public static class Extensions
{
    private const int BufferSize = 2048;//NOTE: set to 2048 to fit in minimum tcp window

        public static async Task<Stream> IronReadStreamAsync(this IDataStore store, TempStream tempStream, string domain, string path, int tryCount)
    {
        var ms = tempStream.Create();
            await IronReadToStreamAsync(store, domain, path, tryCount, ms);
        ms.Seek(0, SeekOrigin.Begin);

        return ms;
    }

        public static Task IronReadToStreamAsync(this IDataStore store, string domain, string path, int tryCount, Stream readTo)
    {
        if (tryCount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(tryCount), "Must be greater or equal 1.");
        }

        if (!readTo.CanWrite)
        {
            throw new ArgumentException("stream cannot be written", nameof(readTo));
        }


            return InternalIronReadToStreamAsync(store, domain, path, tryCount, readTo);
        }

        private static async Task InternalIronReadToStreamAsync(this IDataStore store, string domain, string path, int tryCount, Stream readTo)
        {
        var tryCurrent = 0;
        var offset = 0;

        while (tryCurrent < tryCount)
        {
            try
            {
                tryCurrent++;
                    using var stream = await store.GetReadStreamAsync(domain, path, offset);
                var buffer = new byte[BufferSize];
                    int readed;
                    while ((readed = await stream.ReadAsync(buffer, 0, BufferSize)) > 0)
                {
                        await readTo.WriteAsync(buffer, 0, readed);
                    offset += readed;
                }
                break;
            }
            catch (Exception ex)
            {
                if (tryCurrent >= tryCount)
                {
                    throw new IOException("Can not read stream. Tries count: " + tryCurrent + ".", ex);
                }

                Thread.Sleep(tryCount * 50);
            }
        }
    }
}
