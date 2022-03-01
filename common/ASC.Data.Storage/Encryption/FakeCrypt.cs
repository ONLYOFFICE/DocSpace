namespace ASC.Data.Storage.Encryption;

public class FakeCrypt : ICrypt
{
    public byte Version => 1;

    public void EncryptFile(string filePath)
    {
        return;
    }

    public void DecryptFile(string filePath)
    {
        return;
    }

    public Stream GetReadStream(string filePath)
    {
        return File.OpenRead(filePath);
    }

    public long GetFileSize(string filePath)
    {
        return new FileInfo(filePath).Length;
    }

    public void Init(string storageName, EncryptionSettings encryptionSettings) { }
}
