namespace ASC.Core;

public static class Crypto
{
    private static byte[] GetSK1(bool rewrite)
    {
        return GetSK(rewrite.GetType().Name.Length);
    }

    private static byte[] GetSK2(bool rewrite)
    {
        return GetSK(rewrite.GetType().Name.Length * 2);
    }

    private static byte[] GetSK(int seed)
    {
        var random = new AscRandom(seed);
        var randomKey = new byte[32];
        for (var i = 0; i < randomKey.Length; i++)
        {
            randomKey[i] = (byte)random.Next(byte.MaxValue);
        }

        return randomKey;
    }

    public static string GetV(string data, int keyno, bool reverse)
    {
        var hasher = Aes.Create();
        hasher.Key = keyno == 1 ? GetSK1(false) : GetSK2(false);
        hasher.IV = new byte[hasher.BlockSize >> 3];

        if (reverse)
        {
            using var ms = new MemoryStream();
            using var ss = new CryptoStream(ms, hasher.CreateEncryptor(), CryptoStreamMode.Write);
            using var plainTextStream = new MemoryStream(Convert.FromBase64String(data));
            plainTextStream.CopyTo(ss);
            ss.FlushFinalBlock();
            hasher.Clear();

            return Convert.ToBase64String(ms.ToArray());
        }
        else
        {
            using var ms = new MemoryStream(Convert.FromBase64String(data));
            using var ss = new CryptoStream(ms, hasher.CreateDecryptor(), CryptoStreamMode.Read);
            using var plainTextStream = new MemoryStream();
            ss.CopyTo(plainTextStream);
            hasher.Clear();

            return Encoding.Unicode.GetString(plainTextStream.ToArray());
        }
    }

    internal static byte[] GetV(byte[] data, int keyno, bool reverse)
    {
        var hasher = Aes.Create();
        hasher.Key = keyno == 1 ? GetSK1(false) : GetSK2(false);
        hasher.IV = new byte[hasher.BlockSize >> 3];

        if (reverse)
        {
            using var ms = new MemoryStream();
            using var ss = new CryptoStream(ms, hasher.CreateEncryptor(), CryptoStreamMode.Write);
            using var plainTextStream = new MemoryStream(data);
            plainTextStream.CopyTo(ss);
            ss.FlushFinalBlock();
            hasher.Clear();

            return ms.ToArray();
        }
        else
        {
            using var ms = new MemoryStream(data);
            using var ss = new CryptoStream(ms, hasher.CreateDecryptor(), CryptoStreamMode.Read);
            using var plainTextStream = new MemoryStream();
            ss.CopyTo(plainTextStream);
            hasher.Clear();

            return plainTextStream.ToArray();
        }
    }
}
