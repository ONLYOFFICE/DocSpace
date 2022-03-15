namespace ASC.Security.Cryptography;

[Singletone]
public class InstanceCrypto
{
    private readonly byte[] _eKey;

    public InstanceCrypto(MachinePseudoKeys machinePseudoKeys)
    {
        _eKey = machinePseudoKeys.GetMachineConstant(32);
    }

    public string Encrypt(string data)
    {
        return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(data)));
    }

    public byte[] Encrypt(byte[] data)
    {
        using var hasher = Aes.Create();
        hasher.Key = _eKey;
        hasher.IV = new byte[hasher.BlockSize >> 3];

        using var ms = new MemoryStream();
        using var ss = new CryptoStream(ms, hasher.CreateEncryptor(), CryptoStreamMode.Write);
        using var plainTextStream = new MemoryStream(data);

        plainTextStream.CopyTo(ss);
        ss.FlushFinalBlock();
        hasher.Clear();

        return ms.ToArray();
    }

    public string Decrypt(string data) => Decrypt(Convert.FromBase64String(data));

    public string Decrypt(byte[] data)
    {
        using var hasher = Aes.Create();
        hasher.Key = _eKey;
        hasher.IV = new byte[hasher.BlockSize >> 3];

        using var msDecrypt = new MemoryStream(data);
        using var csDecrypt = new CryptoStream(msDecrypt, hasher.CreateDecryptor(), CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);

        // Read the decrypted bytes from the decrypting stream
        // and place them in a string.
        return srDecrypt.ReadToEnd();
    }
}
