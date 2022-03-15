namespace ASC.Core.Encryption;

public interface ICrypt
{
    byte Version { get; }
    long GetFileSize(string filePath);
    Stream GetReadStream(string filePath);
    void DecryptFile(string filePath);
    void EncryptFile(string filePath);
    void Init(string storageName, EncryptionSettings encryptionSettings);
}
