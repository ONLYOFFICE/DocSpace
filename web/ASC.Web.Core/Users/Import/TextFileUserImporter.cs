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

namespace ASC.Web.Core.Users.Import
{
    public class TextFileUserImporter : IUserImporter
    {
        private readonly Stream stream;

        protected Dictionary<string, string> NameMapping { get; set; }

        protected IList<string> ExcludeList { get; private set; }


        public Encoding Encoding { get; set; }

        public char Separator { get; set; }

        public bool HasHeader { get; set; }

        public string TextDelmiter { get; set; }

        public string DefaultHeader { get; set; }


        public TextFileUserImporter(Stream stream)
        {
            this.stream = stream;
            Encoding = Encoding.UTF8;
            Separator = ';';
            HasHeader = false;
            TextDelmiter = "\"";
            ExcludeList = new List<string> { "ID", "Status" };
        }


        public virtual IEnumerable<UserInfo> GetDiscoveredUsers()
        {
            var users = new List<UserInfo>();

            var fileLines = new List<string>();
            using (var reader = new StreamReader(stream, Encoding, true))
            {
                fileLines.AddRange(reader.ReadToEnd().Split(new[] { Environment.NewLine, "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries));
            }

            if (!string.IsNullOrEmpty(DefaultHeader))
            {
                fileLines.Insert(0, DefaultHeader);
            }
            if (0 < fileLines.Count)
            {
                var mappedProperties = new Dictionary<int, PropertyInfo>();
                //Get the map
                var infos = typeof(UserInfo).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var fieldsCount = GetFieldsMapping(fileLines[0], infos, mappedProperties);

                //Begin read file
                for (var i = 1; i < fileLines.Count; i++)
                {
                    users.Add(GetExportedUser(fileLines[i], mappedProperties, fieldsCount));
                }
            }
            return users;
        }

        private UserInfo GetExportedUser(string line, IDictionary<int, PropertyInfo> mappedProperties, int fieldsCount)
        {
            var exportedUser = new UserInfo
            {
                Id = Guid.NewGuid()
            };

            var dataFields = GetDataFields(line);
            for (var j = 0; j < Math.Min(fieldsCount, dataFields.Length); j++)
            {
                //Get corresponding property
                var propinfo = mappedProperties[j];
                if (propinfo != null)
                {
                    //Convert value
                    var value = ConvertFromString(dataFields[j], propinfo.PropertyType);
                    if (value != null)
                    {
                        propinfo.SetValue(exportedUser, value, Array.Empty<object>());
                    }
                }
            }
            return exportedUser;
        }

        private string[] GetDataFields(string line)
        {
            var pattern = string.Format("{0}(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))", Separator);
            var result = Regex.Split(line, pattern);

            //remove TextDelmiter
            result = Array.ConvertAll<string, string>(result,
                original =>
                {
                    if (original.StartsWith(TextDelmiter) && original.EndsWith(TextDelmiter))
                    {
                        return original[1..^1];
                    }

                    return original;
                }
             );

            return result;
        }

        private int GetFieldsMapping(string firstLine, IEnumerable<PropertyInfo> infos, IDictionary<int, PropertyInfo> mappedProperties)
        {
            var fields = firstLine.Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                //Find apropriate field in UserInfo
                foreach (var info in infos)
                {
                    var propertyField = field.Trim();
                    if (NameMapping != null)
                    {
                        NameMapping.TryGetValue(propertyField, out propertyField);
                    }
                    if (!string.IsNullOrEmpty(propertyField) && !ExcludeList.Contains(propertyField) && propertyField.Equals(info.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        //Add to map
                        mappedProperties.Add(i, info);
                    }
                }
                if (!mappedProperties.ContainsKey(i))
                {
                    //No property was found
                    mappedProperties.Add(i, null);
                }
            }
            return fields.Length;
        }

        private static object ConvertFromString(string value, Type type)
        {
            var converter = TypeDescriptor.GetConverter(type);
            return converter != null && converter.CanConvertFrom(typeof(string)) ? converter.ConvertFromString(value) : null;
        }
    }
}