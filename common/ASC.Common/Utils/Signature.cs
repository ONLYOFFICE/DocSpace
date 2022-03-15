namespace ASC.Common.Utils;

[Singletone]
public class Signature
{
    private readonly MachinePseudoKeys _machinePseudoKeys;

    public Signature(MachinePseudoKeys machinePseudoKeys)
    {
        _machinePseudoKeys = machinePseudoKeys;
    }

    public string Create<T>(T obj)
    {
        return Create(obj, Encoding.UTF8.GetString(_machinePseudoKeys.GetMachineConstant()));
    }

    public static string Create<T>(T obj, string secret)
    {
        var str = JsonConvert.SerializeObject(obj);
        var payload = GetHashBase64(str + secret) + "?" + str;

        return WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(payload));
    }

    public T Read<T>(string signature)
    {
        return Read<T>(signature, Encoding.UTF8.GetString(_machinePseudoKeys.GetMachineConstant()));
    }

    public static T Read<T>(string signature, string secret)
    {
        try
        {
            var rightSignature = signature.Replace("\"", "");
            var payloadParts = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(rightSignature)).Split('?');

            if (GetHashBase64(payloadParts[1] + secret) == payloadParts[0])
            {
                return JsonConvert.DeserializeObject<T>(payloadParts[1]); //Sig correct
            }
        }
        catch (Exception) { }

        return default;
    }

    private static string GetHashBase64(string str)
    {
        using var sha256 = SHA256.Create();

        return Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(str)));
    }

    private static string GetHashBase64MD5(string str)
    {
        using var md5 = MD5.Create();

        return Convert.ToBase64String(md5.ComputeHash(Encoding.UTF8.GetBytes(str)));
    }
}
