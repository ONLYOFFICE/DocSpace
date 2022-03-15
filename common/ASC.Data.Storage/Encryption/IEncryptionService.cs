namespace ASC.Data.Storage.Encryption;

public interface IEncryptionService
{
    void Start(EncryptionSettingsProto encryptionSettingsProto);

    void Stop();
}
