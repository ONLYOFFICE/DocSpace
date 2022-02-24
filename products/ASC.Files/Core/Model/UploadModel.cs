namespace ASC.Files.Model;

public class UploadModel : IModelWithFile, IDisposable
{
    public IFormFile File { get; set; }
    public ContentType ContentType { get; set; }
    public ContentDisposition ContentDisposition { get; set; }
    public IEnumerable<IFormFile> Files { get; set; }
    public bool? CreateNewIfExist { get; set; }
    public bool? StoreOriginalFileFlag { get; set; }
    public bool KeepConvertStatus { get; set; }

    private Stream _stream;
    private bool _disposedValue;

    public Stream Stream
    {
        get => File?.OpenReadStream() ?? _stream;
        set => _stream = value;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing && _stream != null)
            {
                _stream.Close();
                _stream.Dispose();
                _stream = null;
            }

            _disposedValue = true;
        }
    }

    ~UploadModel()
    {
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
