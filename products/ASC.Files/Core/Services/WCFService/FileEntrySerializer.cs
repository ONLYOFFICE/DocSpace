// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.Web.Files.Services.WCFService;

public class FileEntrySerializer
{
    private static readonly IDictionary<Type, XmlObjectSerializer> _serializers = new Dictionary<Type, XmlObjectSerializer>();
    //private static readonly bool _oldMonoSerializer;


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

        result.Seek(0, SeekOrigin.Begin);

        /*if (_oldMonoSerializer)
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
        */
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
