namespace ASC.Web.Files.Services.WCFService;

public class FileEntrySerializer
{
    private static readonly IDictionary<Type, XmlObjectSerializer> _serializers = new Dictionary<Type, XmlObjectSerializer>();
    private static readonly bool _oldMonoSerializer = false;


    static FileEntrySerializer()
    {
        _serializers[typeof(File<>)] = new DataContractSerializer(typeof(File<>));
        //serializers[typeof(List<FileEntry<>>)] = new DataContractSerializer(typeof(List<FileEntry<>>));
        _serializers[typeof(DataWrapper<>)] = new DataContractSerializer(typeof(DataWrapper<>));

        //if (WorkContext.IsMono && !string.IsNullOrEmpty(WorkContext.MonoVersion))
        //{
        //    // in version higher 4.0 use standard DataContractResolver
        //    var version = WorkContext.MonoVersion.Split('.', ' ');
        //    if (2 <= version.Length && (Convert.ToInt32(version[0]) * 1000 + Convert.ToInt32(version[1])) < 4002)
        //    {
        //        oldMonoSerializer = true;
        //        serializers[typeof(List<FileEntry>)] = new DataContractSerializer(typeof(List<FileEntry>), Type.EmptyTypes, ushort.MaxValue, false, false, null, new FileEntryResolver());
        //        serializers[typeof(DataWrapper)] = new DataContractSerializer(typeof(DataWrapper), Type.EmptyTypes, ushort.MaxValue, false, false, null, new FileEntryResolver());
        //    }
        //}
    }

    public MemoryStream ToXml(object o)
    {
        var result = new MemoryStream();
        if (o == null)
        {
            return result;
        }

        using (var writer = XmlDictionaryWriter.CreateTextWriter(result, Encoding.UTF8, false))
        {
            var serializer = _serializers[o.GetType()];
            serializer.WriteObject(writer, o);
        }

        result.Seek(0, System.IO.SeekOrigin.Begin);

        if (_oldMonoSerializer)
        {
            var xml = new XmlDocument
            {
                PreserveWhitespace = true
            };
            xml.Load(result);
            result.Close();

            //remove incorrect ns
            foreach (XmlNode entry in xml.SelectNodes("//entry"))
            {
                var nsattr = entry.Attributes.Cast<XmlAttribute>().FirstOrDefault(a => a.Value == typeof(FileEntry<>).Name);
                if (nsattr != null)
                {
                    foreach (XmlAttribute a in entry.Attributes)
                    {
                        if (a.Value.StartsWith(nsattr.LocalName + ":"))
                        {
                            a.Value = a.Value.Substring(nsattr.LocalName.Length + 1);
                        }
                    }

                    entry.Attributes.Remove(nsattr);
                }
            }

            //http://stackoverflow.com/questions/13483138/mono-does-not-honor-system-runtime-serialization-datamemberattribute-emitdefault
            var nsmanager = new XmlNamespaceManager(xml.NameTable);
            nsmanager.AddNamespace("i", "http://www.w3.org/2001/XMLSchema-instance");
            foreach (XmlNode nil in xml.SelectNodes("//*[@i:nil='true']", nsmanager))
            {
                nil.ParentNode.RemoveChild(nil);
            }

            result = new MemoryStream();
            xml.Save(result);
            result.Seek(0, SeekOrigin.Begin);
        }

        return result;
    }


    private class FileEntryResolver : DataContractResolver
    {
        public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver knownTypeResolver)
        {
            return null;
        }

        public override bool TryResolveType(Type type, Type declaredType, DataContractResolver knownTypeResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
        {
            typeName = XmlDictionaryString.Empty;
            typeNamespace = XmlDictionaryString.Empty;

            if (declaredType == typeof(FileEntry<>))
            {
                typeName = new XmlDictionaryString(XmlDictionary.Empty, type.Name.ToLower(), 0);
                typeNamespace = new XmlDictionaryString(XmlDictionary.Empty, declaredType.Name, 0);

                return true;
            }

            return false;
        }
    }
}
