/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
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


using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;

using ASC.Common;
using ASC.Common.Utils;

namespace ASC.Data.Backup
{
    [Scope]
    public class DbBackupProvider : IBackupProvider
    {
        private readonly List<string> processedTables = new List<string>();
        private readonly DbHelper dbHelper;
        private readonly TempStream tempStream;

        public string Name
        {
            get { return "databases"; }
        }

        public DbBackupProvider(DbHelper dbHelper, TempStream tempStream)
        {
            this.dbHelper = dbHelper;
            this.tempStream = tempStream;
        }
        public IEnumerable<XElement> GetElements(int tenant, string[] configs, IDataWriteOperator writer)
        {
            processedTables.Clear();
            var xml = new List<XElement>();
            var connectionKeys = new Dictionary<string, string>();
            foreach (var connectionString in GetConnectionStrings(configs))
            {
                //do not save the base, having the same provider and connection string is not to duplicate
                //data, but also expose the ref attribute of repetitive bases for the correct recovery
                var node = new XElement(connectionString.Name);
                xml.Add(node);

                var connectionKey = connectionString.ProviderName + connectionString.ConnectionString;
                if (connectionKeys.ContainsKey(connectionKey))
                {
                    node.Add(new XAttribute("ref", connectionKeys[connectionKey]));
                }
                else
                {
                    connectionKeys.Add(connectionKey, connectionString.Name);
                    node.Add(BackupDatabase(tenant, connectionString, writer));
                }
            }

            return xml;
        }

        public void LoadFrom(IEnumerable<XElement> elements, int tenant, string[] configs, IDataReadOperator reader)
        {
            processedTables.Clear();
            foreach (var connectionString in GetConnectionStrings(configs))
            {
                RestoreDatabase(connectionString, elements, reader);
            }
        }

        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;


        private void OnProgressChanged(string status, int progress)
        {
            ProgressChanged?.Invoke(this, new ProgressChangedEventArgs(status, progress));
        }


        private Configuration GetConfiguration(string config)
        {
            if (config.Contains(Path.DirectorySeparatorChar) && !Uri.IsWellFormedUriString(config, UriKind.Relative))
            {
                var map = new ExeConfigurationFileMap
                {
                    ExeConfigFilename = string.Compare(Path.GetExtension(config), ".config", true) == 0 ? config : CrossPlatform.PathCombine(config, "Web.config")
                };
                return ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            }
            return ConfigurationManager.OpenExeConfiguration(config);
        }

        public IEnumerable<ConnectionStringSettings> GetConnectionStrings(string[] configs)
        {
            /*  if (configs.Length == 0)
              {
                  configs = new string[] { AppDomain.CurrentDomain.SetupInformation.ConfigurationFile }; 
              }
              var connectionStrings = new List<ConnectionStringSettings>();
              foreach (var config in configs)
              {
                  connectionStrings.AddRange(GetConnectionStrings(GetConfiguration(config)));
              }
              return connectionStrings.GroupBy(cs => cs.Name).Select(g => g.First());*/
            return null;
        }

        public IEnumerable<ConnectionStringSettings> GetConnectionStrings(Configuration cfg)
        {
            var connectionStrings = new List<ConnectionStringSettings>();
            foreach (ConnectionStringSettings connectionString in cfg.ConnectionStrings.ConnectionStrings)
            {
                if (connectionString.Name == "LocalSqlServer" || connectionString.Name == "readonly") continue;
                connectionStrings.Add(connectionString);
                if (connectionString.ConnectionString.Contains("|DataDirectory|"))
                {
                    connectionString.ConnectionString = connectionString.ConnectionString.Replace("|DataDirectory|", Path.GetDirectoryName(cfg.FilePath) + '\\');
                }
            }
            return connectionStrings;
        }

        private List<XElement> BackupDatabase(int tenant, ConnectionStringSettings connectionString, IDataWriteOperator writer)
        {
            var xml = new List<XElement>();
            var errors = 0;
            var timeout = TimeSpan.FromSeconds(1);
            var tables = dbHelper.GetTables();
            for (var i = 0; i < tables.Count; i++)
            {
                var table = tables[i];
                OnProgressChanged(table, (int)(i / (double)tables.Count * 100));

                if (processedTables.Contains(table, StringComparer.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                xml.Add(new XElement(table));
                DataTable dataTable = null;
                while (true)
                {
                    try
                    {
                        dataTable = dbHelper.GetTable(table, tenant);
                        break;
                    }
                    catch
                    {
                        errors++;
                        if (20 < errors) throw;
                        Thread.Sleep(timeout);
                    }
                }
                foreach (DataColumn c in dataTable.Columns)
                {
                    if (c.DataType == typeof(DateTime)) c.DateTimeMode = DataSetDateTime.Unspecified;
                }

                using (var file = tempStream.Create())
                {
                    dataTable.WriteXml(file, XmlWriteMode.WriteSchema);
                    writer.WriteEntry(string.Format("{0}\\{1}\\{2}", Name, connectionString.Name, table).ToLower(), file);
                }

                processedTables.Add(table);
            }
            return xml;
        }

        private void RestoreDatabase(ConnectionStringSettings connectionString, IEnumerable<XElement> elements, IDataReadOperator reader)
        {
            var dbName = connectionString.Name;
            var dbElement = elements.SingleOrDefault(e => string.Compare(e.Name.LocalName, connectionString.Name, true) == 0);
            if (dbElement != null && dbElement.Attribute("ref") != null)
            {
                dbName = dbElement.Attribute("ref").Value;
                dbElement = elements.Single(e => string.Compare(e.Name.LocalName, dbElement.Attribute("ref").Value, true) == 0);
            }
            if (dbElement == null) return;

            var tables = dbHelper.GetTables();
            for (var i = 0; i < tables.Count; i++)
            {
                var table = tables[i];
                OnProgressChanged(table, (int)(i / (double)tables.Count * 100));

                if (processedTables.Contains(table, StringComparer.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                if (dbElement.Element(table) != null)
                {
                    using (var stream = reader.GetEntry(string.Format("{0}\\{1}\\{2}", Name, dbName, table).ToLower()))
                    {
                        var data = new DataTable();
                        data.ReadXml(stream);
                        dbHelper.SetTable(data);
                    }
                    processedTables.Add(table);
                }
            }
        }
    }
}
