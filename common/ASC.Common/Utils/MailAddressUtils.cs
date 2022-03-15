namespace ASC.Common.Utils;

public static class MailAddressUtils
{
    public static MailAddress Create(string address)
    {
        if (!string.IsNullOrEmpty(address))
        {
            var firstPos = address.IndexOf('"');
            var lastPos = address.LastIndexOf('"');

            if (firstPos != -1 && firstPos < lastPos && address.IndexOf('"', firstPos + 1, lastPos - firstPos - 1) != -1)
            {
                address = new StringBuilder(address).Replace("\"", string.Empty, firstPos + 1, lastPos - firstPos - 1).ToString();
            }
        }

        return new MailAddress(address);
    }

    public static MailAddress Create(string address, string displayName)
    {
        if (!string.IsNullOrEmpty(displayName))
        {
            displayName = displayName.Replace("\"", string.Empty);

            if (125 < displayName.Length)
            {
                displayName = displayName.Substring(0, 125);
            }
        }

        return Create(ToSmtpAddress(address, displayName));
    }

    public static string ToEncodedString(this MailAddress m)
    {
        return ToSmtpAddress(m.Address, MimeHeaderUtils.EncodeMime(m.DisplayName));
    }

    private static string ToSmtpAddress(string address, string displayName)
    {
        return $"\"{displayName}\" <{address}>";
    }
}
