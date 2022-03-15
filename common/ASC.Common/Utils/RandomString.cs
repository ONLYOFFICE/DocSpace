namespace ASC.Common.Utils;

public static class RandomString
{
    public static string Generate(int length)
    {
        const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        var res = new StringBuilder();

        while (0 < length--)
        {
                res.Append(valid[RandomNumberGenerator.GetInt32(valid.Length)]);
        }

        return res.ToString();
    }
}
