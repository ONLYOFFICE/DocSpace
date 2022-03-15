namespace ASC.Data.Storage;

public class ProgressStream : Stream
{
    public override bool CanRead => _stream.CanRead;
    public override bool CanSeek => _stream.CanSeek;
    public override bool CanWrite => _stream.CanWrite;
    public override long Length => _length;
    public override long Position
    {
        get => _stream.Position;
        set => _stream.Position = value;
    }

    private readonly Stream _stream;
    private long _length = long.MaxValue;

    public ProgressStream(Stream stream)
    {
        _stream = stream ?? throw new ArgumentNullException(nameof(stream));

        try
        {
            _length = stream.Length;
        }
        catch (Exception) { }
    }

    public event Action<ProgressStream, int> OnReadProgress;

    public void InvokeOnReadProgress(int progress)
    {
        OnReadProgress?.Invoke(this, progress);
    }

    public override void Flush()
    {
        _stream.Flush();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return _stream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        _stream.SetLength(value);
        _length = value;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        var readed = _stream.Read(buffer, offset, count);
        OnReadProgress(this, (int)(_stream.Position / (double)_length * 100));

        return readed;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        _stream.Write(buffer, offset, count);
    }
}
