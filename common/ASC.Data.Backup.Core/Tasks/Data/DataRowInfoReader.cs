namespace ASC.Data.Backup.Tasks.Data;

internal static class DataRowInfoReader
{
    private const string XmlSchemaNamespace = "http://www.w3.org/2001/XMLSchema";

    public static IEnumerable<DataRowInfo> ReadFromStream(Stream stream)
    {
        var readerSettings = new XmlReaderSettings
        {
            CheckCharacters = false,
            CloseInput = false
        };

        using var xmlReader = XmlReader.Create(stream, readerSettings);
        xmlReader.MoveToContent();
        xmlReader.ReadToFollowing("schema", XmlSchemaNamespace);

        var schema = new Dictionary<string, string>();

        if (XNode.ReadFrom(xmlReader) is XElement schemaElement)
        {
            foreach (var entry in schemaElement.Descendants(XName.Get("sequence", XmlSchemaNamespace)).Single().Elements(XName.Get("element", XmlSchemaNamespace)))
            {
                schema.Add(entry.Attribute("name").ValueOrDefault(), entry.Attribute("type").ValueOrDefault());
            }
        }

        while (xmlReader.Read())
        {
            if (xmlReader.NodeType == XmlNodeType.Element)
            {
                if (XNode.ReadFrom(xmlReader) is XElement el)
                {
                    var dataRowInfo = new DataRowInfo(el.Name.LocalName);
                    foreach (var column in schema)
                    {
                        var value = ConvertToType(el.Element(column.Key).ValueOrDefault(), column.Value);
                        dataRowInfo.SetValue(column.Key, value);
                    }

                    yield return dataRowInfo;
                }
            }
        }
    }

    private static object ConvertToType(string str, string schemaType)
    {
        if (str == null)
        {
            return null;
        }
        if (schemaType == "xs:boolean")
        {
            return Convert.ToBoolean(str);
        }
        if (schemaType == "xs:base64Binary")
        {
            return Convert.FromBase64String(str);
        }

        return str;
    }
}
