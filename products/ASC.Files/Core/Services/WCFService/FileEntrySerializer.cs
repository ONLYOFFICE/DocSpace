/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/

namespace ASC.Web.Files.Services.WCFService
{
    public class FileEntrySerializer
    {
        private static readonly IDictionary<Type, XmlObjectSerializer> serializers = new Dictionary<Type, XmlObjectSerializer>();
        private static readonly bool oldMonoSerializer = false;


        static FileEntrySerializer()
        {
            serializers[typeof(File<>)] = new DataContractSerializer(typeof(File<>));
            //serializers[typeof(List<FileEntry<>>)] = new DataContractSerializer(typeof(List<FileEntry<>>));
            serializers[typeof(DataWrapper<>)] = new DataContractSerializer(typeof(DataWrapper<>));

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

        public System.IO.MemoryStream ToXml(object o)
        {
            var result = new System.IO.MemoryStream();
            if (o == null)
            {
                return result;
            }

            using (var writer = XmlDictionaryWriter.CreateTextWriter(result, Encoding.UTF8, false))
            {
                var serializer = serializers[o.GetType()];
                serializer.WriteObject(writer, o);
            }
            result.Seek(0, System.IO.SeekOrigin.Begin);

            if (oldMonoSerializer)
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

                result = new System.IO.MemoryStream();
                xml.Save(result);
                result.Seek(0, System.IO.SeekOrigin.Begin);
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
}