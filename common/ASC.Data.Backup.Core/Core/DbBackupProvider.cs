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

using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace ASC.Data.Backup;

[Scope]
public class DbBackupProvider : IBackupProvider
{
    public string Name => "databases";

    private readonly List<string> _processedTables = new List<string>();
    private readonly DbHelper _dbHelper;
    private readonly TempStream _tempStream;

    public DbBackupProvider(DbHelper dbHelper, TempStream tempStream)
    {
        _dbHelper = dbHelper;
        _tempStream = tempStream;
    }

    public event EventHandler<ProgressChangedEventArgs> ProgressChanged;

    public async Task<IEnumerable<XElement>> GetElements(int tenant, string[] configs, IDataWriteOperator writer)
    {
        _processedTables.Clear();
        var xml = new List<XElement>();
        var connectionKeys = new Dictionary<string, string>();

        foreach (var connectionString in GetConnectionStrings(configs))
        {
            //do not save the base, having the same provider and connection string is not to duplicate
            //data, but also expose the ref attribute of repetitive bases for the correct recovery
            var node = new XElement(connectionString.Name);
            xml.Add(node);

            var connectionKey = connectionString.ProviderName + connectionString.ConnectionString;
            if (connectionKeys.TryGetValue(connectionKey, out var value))
            {
                node.Add(new XAttribute("ref", value));
            }
            else
            {
                connectionKeys.Add(connectionKey, connectionString.Name);
                node.Add(await BackupDatabase(tenant, connectionString, writer));
            }
        }
       
        return xml.AsEnumerable();
    }

    public async Task LoadFromAsync(IEnumerable<XElement> elements, int tenant, string[] configs, IDataReadOperator reader)
    {
        _processedTables.Clear();

        foreach (var connectionString in GetConnectionStrings(configs))
        {
            await RestoreDatabaseAsync(connectionString, elements, reader);
        }
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
            if (connectionString.Name == "LocalSqlServer" || connectionString.Name == "readonly")
            {
                continue;
            }

            connectionStrings.Add(connectionString);
            if (connectionString.ConnectionString.Contains("|DataDirectory|"))
            {
                connectionString.ConnectionString = connectionString.ConnectionString.Replace("|DataDirectory|", Path.GetDirectoryName(cfg.FilePath) + '\\');
            }
        }

        return connectionStrings;
    }

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
                ExeConfigFilename = string.Equals(Path.GetExtension(config), ".config", StringComparison.OrdinalIgnoreCase) ? config : CrossPlatform.PathCombine(config, "Web.config")
            };
            return ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
        }
        return ConfigurationManager.OpenExeConfiguration(config);
    }

    private async Task<List<XElement>> BackupDatabase(int tenant, ConnectionStringSettings connectionString, IDataWriteOperator writer)
    {
        var xml = new List<XElement>();
        var errors = 0;
        var timeout = TimeSpan.FromSeconds(1);
        var tables = _dbHelper.GetTables();

        for (var i = 0; i < tables.Count; i++)
        {
            var table = tables[i];
            OnProgressChanged(table, (int)(i / (double)tables.Count * 100));

            if (_processedTables.Contains(table, StringComparer.InvariantCultureIgnoreCase))
            {
                continue;
            }

            xml.Add(new XElement(table));
            DataTable dataTable;

            while (true)
            {
                try
                {
                    dataTable = _dbHelper.GetTable(table, tenant);
                    break;
                }
                catch
                {
                    errors++;
                    if (20 < errors)
                    {
                        throw;
                    }

                    Thread.Sleep(timeout);
                }
            }

            foreach (DataColumn c in dataTable.Columns)
            {
                if (c.DataType == typeof(DateTime))
                {
                    c.DateTimeMode = DataSetDateTime.Unspecified;
                }
            }

            await using (var file = _tempStream.Create())
            {
                dataTable.WriteXml(file, XmlWriteMode.WriteSchema);
                await writer.WriteEntryAsync($"{Name}\\{connectionString.Name}\\{table}".ToLower(), file);
            }

            _processedTables.Add(table);
        }

        return xml;
    }

    private async Task RestoreDatabaseAsync(ConnectionStringSettings connectionString, IEnumerable<XElement> elements, IDataReadOperator reader)
    {
        var dbName = connectionString.Name;
        var dbElement = elements.SingleOrDefault(e => string.Equals(e.Name.LocalName, connectionString.Name, StringComparison.OrdinalIgnoreCase));
        if (dbElement != null && dbElement.Attribute("ref") != null)
        {
            dbName = dbElement.Attribute("ref").Value;
            dbElement = elements.Single(e => string.Equals(e.Name.LocalName, dbElement.Attribute("ref").Value, StringComparison.OrdinalIgnoreCase));
        }

        if (dbElement == null)
        {
            return;
        }

        var tables = _dbHelper.GetTables();

        for (var i = 0; i < tables.Count; i++)
        {
            var table = tables[i];
            OnProgressChanged(table, (int)(i / (double)tables.Count * 100));

            if (_processedTables.Contains(table, StringComparer.InvariantCultureIgnoreCase))
            {
                continue;
            }

            if (dbElement.Element(table) != null)
            {
                await using (var stream = reader.GetEntry($"{Name}\\{dbName}\\{table}".ToLower()))
                {
                    var data = new DataTable();
                    data.ReadXml(stream);
                    await _dbHelper.SetTableAsync(data);
                }
                _processedTables.Add(table);
            }
        }
    }
}
