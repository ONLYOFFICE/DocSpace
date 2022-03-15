namespace ASC.Security.Cryptography;

public static class Hasher
{
    private const HashAlg DefaultAlg = HashAlg.SHA256;

    public static byte[] Hash(string data, HashAlg hashAlg)
    {
        return ComputeHash(data, hashAlg);
    }

    public static byte[] Hash(string data)
    {
        return Hash(data, DefaultAlg);
    }

    public static byte[] Hash(byte[] data, HashAlg hashAlg)
    {
        return ComputeHash(data, hashAlg);
    }

    public static byte[] Hash(byte[] data)
    {
        return Hash(data, DefaultAlg);
    }

    public static string Base64Hash(string data, HashAlg hashAlg)
    {
        return ComputeHash64(data, hashAlg);
    }

    public static string Base64Hash(string data)
    {
        return Base64Hash(data, DefaultAlg);
    }

    public static string Base64Hash(byte[] data, HashAlg hashAlg)
    {
        return ComputeHash64(data, hashAlg);
    }

    public static string Base64Hash(byte[] data)
    {
        return Base64Hash(data, DefaultAlg);
    }

    public static bool EqualHash(byte[] dataToCompare, byte[] hash)
    {
        return EqualHash(dataToCompare, hash, DefaultAlg);
    }

    public static bool EqualHash(string dataToCompare, string hash, HashAlg hashAlg)
    {
        return EqualHash(S2B(dataToCompare), S642B(hash), hashAlg);
    }

    public static bool EqualHash(string dataToCompare, string hash)
    {
        return EqualHash(dataToCompare, hash, DefaultAlg);
    }

    public static bool EqualHash(byte[] dataToCompare, byte[] hash, HashAlg hashAlg)
    {
        return string.Equals(
            ComputeHash64(dataToCompare, hashAlg),
            B2S64(hash)
            );
    }

    private static HashAlgorithm GetAlg(HashAlg hashAlg)
    {
        return hashAlg switch
        {
            HashAlg.MD5 => MD5.Create(),
            HashAlg.SHA1 => SHA1.Create(),
            HashAlg.SHA256 => SHA256.Create(),
            HashAlg.SHA512 => SHA512.Create(),
            _ => SHA256.Create()
        };
    }

    private static byte[] S2B(string str)
    {
        ArgumentNullException.ThrowIfNull(str);

        return Encoding.UTF8.GetBytes(str);
    }

    private static string B2S(byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return Encoding.UTF8.GetString(data);
    }

    private static byte[] S642B(string str)
    {
        ArgumentNullException.ThrowIfNull(str);

        return Convert.FromBase64String(str);
    }

    private static string B2S64(byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return Convert.ToBase64String(data);
    }

    private static byte[] ComputeHash(byte[] data, HashAlg hashAlg)
    {
        using var alg = GetAlg(hashAlg);

        return alg.ComputeHash(data);
    }

    private static byte[] ComputeHash(string data, HashAlg hashAlg)
    {
        return ComputeHash(S2B(data), hashAlg);
    }

    private static string ComputeHash64(byte[] data, HashAlg hashAlg)
    {
        return B2S64(ComputeHash(data, hashAlg));
    }

    private static string ComputeHash64(string data, HashAlg hashAlg)
    {
        return ComputeHash64(S2B(data), hashAlg);
    }
}