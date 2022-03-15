namespace ASC.Data.Backup.Extensions;

public static class XmlExtensions
{
    public static string ValueOrDefault(this XElement el)
    {
        return el?.Value;
    }

    public static string ValueOrDefault(this XAttribute attr)
    {
        return attr?.Value;
    }

    public static void WriteTo(this XElement el, Stream stream)
    {
        WriteTo(el, stream, Encoding.UTF8);
    }

    public static void WriteTo(this XElement el, Stream stream, Encoding encoding)
    {
        var data = encoding.GetBytes(el.ToString(SaveOptions.None));
        stream.Write(data, 0, data.Length);
    }
}
