namespace ASC.Data.Backup
{
    [Scope]
    public class DbHelper : IDisposable
    {
        private readonly DbProviderFactory _factory;
        private readonly DbConnection _connect;
        private readonly DbCommandBuilder _builder;
        private readonly DataTable _columns;
        private readonly bool _mysql;
        private readonly ILog _logger;
        private readonly BackupsContext _backupsContext;
        private readonly TenantDbContext _tenantDbContext;
        private readonly CoreDbContext _coreDbContext;
        private readonly IDictionary<string, string> _whereExceptions 
            = new Dictionary<string, string>();

        public DbHelper(
            IOptionsMonitor<ILog> options, 
            ConnectionStringSettings connectionString, 
            BackupsContext backupsContext, 
            TenantDbContext tenantDbContext, 
            CoreDbContext coreDbContext)
        {
            _logger = options.CurrentValue;
            _backupsContext = backupsContext;
            _tenantDbContext = tenantDbContext;
            _coreDbContext = coreDbContext;

            var file = connectionString.ElementInformation.Source;

            if ("web.connections.config".Equals(Path.GetFileName(file), StringComparison.InvariantCultureIgnoreCase))
            {
                file = CrossPlatform.PathCombine(Path.GetDirectoryName(file), "Web.config");
            }

            var xconfig = XDocument.Load(file);
            var provider = xconfig.XPathSelectElement("/configuration/system.data/DbProviderFactories/add[@invariant='" + connectionString.ProviderName + "']");
            _factory = (DbProviderFactory)Activator.CreateInstance(Type.GetType(provider.Attribute("type").Value, true));
            _builder = _factory.CreateCommandBuilder();
            _connect = _factory.CreateConnection();
            _connect.ConnectionString = connectionString.ConnectionString;
            _connect.Open();

            _mysql = connectionString.ProviderName.ToLower().Contains("mysql");
            if (_mysql)
            {
                CreateCommand("set @@session.sql_mode = concat(@@session.sql_mode, ',NO_AUTO_VALUE_ON_ZERO')").ExecuteNonQuery();
            }

            _columns = _connect.GetSchema("Columns");

            _whereExceptions["calendar_calendar_item"] = " where calendar_id in (select id from calendar_calendars where tenant = {0}) ";
            _whereExceptions["calendar_calendar_user"] = " where calendar_id in (select id from calendar_calendars where tenant = {0}) ";
            _whereExceptions["calendar_event_item"] = " inner join calendar_events on calendar_event_item.event_id = calendar_events.id where calendar_events.tenant = {0} ";
            _whereExceptions["calendar_event_user"] = " inner join calendar_events on calendar_event_user.event_id = calendar_events.id where calendar_events.tenant = {0} ";
            _whereExceptions["crm_entity_contact"] = " inner join crm_contact on crm_entity_contact.contact_id = crm_contact.id where crm_contact.tenant_id = {0} ";
            _whereExceptions["crm_entity_tag"] = " inner join crm_tag on crm_entity_tag.tag_id = crm_tag.id where crm_tag.tenant_id = {0} ";
            _whereExceptions["files_folder_tree"] = " inner join files_folder on folder_id = id where tenant_id = {0} ";
            _whereExceptions["forum_answer_variant"] = " where answer_id in (select id from forum_answer where tenantid = {0})";
            _whereExceptions["forum_topic_tag"] = " where topic_id in (select id from forum_topic where tenantid = {0})";
            _whereExceptions["forum_variant"] = " where question_id in (select id from forum_question where tenantid = {0})";
            _whereExceptions["projects_project_participant"] = " inner join projects_projects on projects_project_participant.project_id = projects_projects.id where projects_projects.tenant_id = {0} ";
            _whereExceptions["projects_following_project_participant"] = " inner join projects_projects on projects_following_project_participant.project_id = projects_projects.id where projects_projects.tenant_id = {0} ";
            _whereExceptions["projects_project_tag"] = " inner join projects_projects on projects_project_tag.project_id = projects_projects.id where projects_projects.tenant_id = {0} ";
            _whereExceptions["tenants_tenants"] = " where id = {0}";
            _whereExceptions["core_acl"] = " where tenant = {0} or tenant = -1";
            _whereExceptions["core_subscription"] = " where tenant = {0} or tenant = -1";
            _whereExceptions["core_subscriptionmethod"] = " where tenant = {0} or tenant = -1";
        }

        public List<string> GetTables()
        {
            var allowTables = new List<string>
            {
                "blogs_",
                "bookmarking_",
                "calendar_",
                "core_",
                "crm_",
                "events_",
                "files_",
                "forum_",
                "photo_",
                "projects_",
                "tenants_",
                "webstudio_",
                "wiki_",
            };

            var disallowTables = new List<string>
            {
                "core_settings",
                "webstudio_uservisit",
                "webstudio_useractivity",
                "tenants_forbiden",
            };

            IEnumerable<string> tables;

            if (_mysql)
            {
                tables = ExecuteList(CreateCommand("show tables"));
            }
            else
            {
                tables = _connect
                    .GetSchema("Tables")
                    .Select(@"TABLE_TYPE <> 'SYSTEM_TABLE'")
                    .Select(row => ((string)row["TABLE_NAME"]));
            }

            return tables
                .Where(t => allowTables.Any(a => t.StartsWith(a)) && !disallowTables.Any(d => t.StartsWith(d)))
                .ToList();
        }

        public DataTable GetTable(string table, int tenant)
        {
            try
            {
                var dataTable = new DataTable(table);
                var adapter = _factory.CreateDataAdapter();
                adapter.SelectCommand = CreateCommand("select " + Quote(table) + ".* from " + Quote(table) + GetWhere(table, tenant));

                _logger.Debug(adapter.SelectCommand.CommandText);

                adapter.Fill(dataTable);

                return dataTable;
            }
            catch (Exception error)
            {
                _logger.ErrorFormat("Table {0}: {1}", table, error);
                throw;
            }
        }


        public void SetTable(DataTable table)
        {
            using var tx = _connect.BeginTransaction();
            try
            {
                if ("tenants_tenants".Equals(table.TableName, StringComparison.InvariantCultureIgnoreCase))
                {
                    // remove last tenant
                    var tenant = _tenantDbContext.Tenants.LastOrDefault();
                    if (tenant != null)
                    {
                        _tenantDbContext.Tenants.Remove(tenant);
                        _tenantDbContext.SaveChanges();
                    }
                    /*  var tenantid = CreateCommand("select id from tenants_tenants order by id desc limit 1").ExecuteScalar();
                         CreateCommand("delete from tenants_tenants where id = " + tenantid).ExecuteNonQuery();*/
                    if (table.Columns.Contains("mappeddomain"))
                    {
                        foreach (var r in table.Rows.Cast<DataRow>())
                        {
                            r[table.Columns["mappeddomain"]] = null;
                            if (table.Columns.Contains("id"))
                            {
                                var tariff = _coreDbContext.Tariffs.FirstOrDefault(t => t.Tenant == tenant.Id);
                                tariff.Tenant = (int)r[table.Columns["id"]];
                                //  CreateCommand("update tenants_tariff set tenant = " + r[table.Columns["id"]] + " where tenant = " + tenantid).ExecuteNonQuery();
                                _coreDbContext.Entry(tariff).State = EntityState.Modified;
                                _coreDbContext.SaveChanges();
                            }
                        }
                    }
                }

                var sql = new StringBuilder("replace into " + Quote(table.TableName) + "(");

                var tableColumns = GetColumnsFrom(table.TableName)
                    .Intersect(table.Columns.Cast<DataColumn>().Select(c => c.ColumnName), StringComparer.InvariantCultureIgnoreCase)
                    .ToList();

                tableColumns.ForEach(column => sql.AppendFormat("{0}, ", Quote(column)));
                sql.Replace(", ", ") values (", sql.Length - 2, 2);

                var insert = _connect.CreateCommand();
                tableColumns.ForEach(column =>
                {
                    sql.AppendFormat("@{0}, ", column);
                    var p = insert.CreateParameter();
                    p.ParameterName = "@" + column;
                    insert.Parameters.Add(p);
                });
                sql.Replace(", ", ")", sql.Length - 2, 2);
                insert.CommandText = sql.ToString();

                foreach (var r in table.Rows.Cast<DataRow>())
                {
                    foreach (var c in tableColumns)
                    {
                        ((IDbDataParameter)insert.Parameters["@" + c]).Value = r[c];
                    }

                    insert.ExecuteNonQuery();
                }

                tx.Commit();
            }
            catch (Exception e)
            {
                _logger.ErrorFormat("Table {0}: {1}", table, e);
            }
        }

        public void Dispose()
        {
            _builder.Dispose();
            _connect.Dispose();
        }

        public DbCommand CreateCommand(string sql)
        {
            var command = _connect.CreateCommand();
            command.CommandText = sql;

            return command;
        }

        public List<string> ExecuteList(DbCommand command)
        {
            var list = new List<string>();
            using (var result = command.ExecuteReader())
            {
                while (result.Read())
                {
                    list.Add(result.GetString(0));
                }
            }

            return list;
        }

        private string Quote(string identifier)
        {
            return identifier;
        }

        private IEnumerable<string> GetColumnsFrom(string table)
        {
            if (_mysql)
            {
                return ExecuteList(CreateCommand("show columns from " + Quote(table)));
            }
            else
            {
                return _columns.Select(string.Format("TABLE_NAME = '{0}'", table))
                    .Select(r => r["COLUMN_NAME"].ToString());
            }
        }

        private string GetWhere(string tableName, int tenant)
        {
            if (tenant == -1)
            {
                return string.Empty;
            }

            if (_whereExceptions.ContainsKey(tableName.ToLower()))
            {
                return string.Format(_whereExceptions[tableName.ToLower()], tenant);
            }
            var tenantColumn = GetColumnsFrom(tableName).FirstOrDefault(c => c.ToLower().StartsWith("tenant"));

            return tenantColumn != null ?
                " where " + Quote(tenantColumn) + " = " + tenant :
                " where 1 = 0";
        }
    }
}
