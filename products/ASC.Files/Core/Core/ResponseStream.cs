namespace ASC.Web.Files.Core;

public class ResponseStream : Stream
{
    private readonly Stream _stream;
    private readonly long _length;

    public ResponseStream(Stream stream, long length)
    {
        _stream = stream;
        _length = length;
    }

    public ResponseStream(HttpResponseMessage response)
    {
        _stream = response.Content.ReadAsStream();
        _length = _stream.Length;
        _response = response;
    }

    public override bool CanRead => _stream.CanRead;
    public override bool CanSeek => _stream.CanSeek;
    public override bool CanWrite => _stream.CanWrite;
    public override long Length => _length;

    public override long Position
    {
        get => _stream.Position;
        set => _stream.Position = value;
    }

    private readonly HttpResponseMessage _response;

    public override void Flush()
    {
        _stream.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return _stream.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return _stream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        _stream.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        _stream.Write(buffer, offset, count);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _stream.Dispose();
            _response.Dispose();
        }

        base.Dispose(disposing);
    }

    public override void Close()
    {
        _stream.Close();
        base.Close();
    }
}
