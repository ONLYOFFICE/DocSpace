namespace ASC.FederatedLogin.Helpers;

public static class HashHelper
{
    public static int CombineHashCodes(int hash1, int hash2)
    {
        return hash2 == 0 ? hash1 : ((hash1 << 5) + hash1) ^ hash2;
    }

    //Use this luke!!!
    public static int StringHash(string text)
    {
        return text.GetHashCode();
    }

    public static string MD5(string text)
    {
        return MD5(text, Encoding.Default);
    }

    public static string MD5(string text, Encoding encoding)
    {
        return MD5String(encoding.GetBytes(text));
    }

    public static string MD5String(byte[] data)
    {
        var hash = MD5(data);
        var sb = new StringBuilder();
        for (var i = 0; i < hash.Length; i++)
        {
            sb.Append(hash[i].ToString("X2"));
        }

        return sb.ToString();
    }

    public static byte[] MD5(byte[] data)
    {
        using var md5 = System.Security.Cryptography.MD5.Create();

        return md5.ComputeHash(data);
    }
}
