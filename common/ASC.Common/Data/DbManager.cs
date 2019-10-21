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


using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

using ASC.Common.Data.AdoProxy;
using ASC.Common.Data.Sql;
using ASC.Common.Logging;
using ASC.Common.Web;

using Microsoft.Extensions.Options;

namespace ASC.Common.Data
{
    public class DbOptionsManager : OptionsManager<DbManager>, IDisposable
    {
        private Dictionary<string, DbManager> Pairs { get; set; }

        public DbOptionsManager(IOptionsFactory<DbManager> factory) : base(factory)
        {
            Pairs = new Dictionary<string, DbManager>();
        }

        public override DbManager Get(string name)
        {
            var result = base.Get(name);
            if (!Pairs.ContainsKey(name))
            {
                Pairs.Add(name, result);
            }
            return result;
        }

        public void Dispose()
        {
            foreach (var v in Pairs)
            {
                v.Value.Dispose();
            }
        }
    }
    public class ConfigureDbManager : IConfigureNamedOptions<DbManager>
    {
        public DbRegistry DbRegistry { get; }
        public IOptionsMonitor<LogNLog> Option { get; }

        public ConfigureDbManager(DbRegistry dbRegistry, IOptionsMonitor<LogNLog> option)
        {
            DbRegistry = dbRegistry;
            Option = option;
        }

        public void Configure(string name, DbManager dbManager)
        {
            dbManager.DbRegistry = DbRegistry;
            dbManager.DatabaseId = string.IsNullOrEmpty(name) ? "default" : name;
            dbManager.Logger = Option.Get("ASC.SQL");

        }

        public void Configure(DbManager dbManager)
        {
            Configure("default", dbManager);
        }
    }


    public class DbManager : IDbManager
    {
        public ILog Logger { get; internal set; }
        private readonly ProxyContext proxyContext;
        private readonly bool shared;

        private DbCommand command;
        private ISqlDialect dialect;
        private volatile bool disposed;

        public int? CommandTimeout { get; set; }

        private DbCommand Command
        {
            get
            {
                CheckDispose();
                if (command == null)
                {
                    command = OpenConnection().CreateCommand();
                }
                if (command.Connection.State == ConnectionState.Closed || command.Connection.State == ConnectionState.Broken)
                {
                    command = OpenConnection().CreateCommand();
                }

                if (CommandTimeout.HasValue)
                {
                    command.CommandTimeout = CommandTimeout.Value;
                }

                return command;
            }
        }

        public string DatabaseId
        {
            get;
            set;
        }

        public bool InTransaction
        {
            get { return Command.Transaction != null; }
        }

        public DbConnection Connection
        {
            get { return Command.Connection; }
        }

        public DbRegistry DbRegistry { get; internal set; }

        public DbManager()
        {

        }

        public DbManager(DbRegistry dbRegistry, string databaseId, int? commandTimeout = null)
            : this(dbRegistry, databaseId, true, commandTimeout)
        {
        }

        public DbManager(DbRegistry dbRegistry, string databaseId, bool shared, int? commandTimeout = null)
        {
            DbRegistry = dbRegistry ?? throw new ArgumentNullException(nameof(dbRegistry));
            DatabaseId = databaseId ?? throw new ArgumentNullException(nameof(databaseId));
            this.shared = shared;

            if (Logger.IsDebugEnabled)
            {
                proxyContext = new ProxyContext(AdoProxyExecutedEventHandler);
            }

            if (commandTimeout.HasValue)
            {
                this.CommandTimeout = commandTimeout;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            lock (this)
            {
                if (disposed) return;
                disposed = true;
                if (command != null)
                {
                    if (command.Connection != null) command.Connection.Dispose();
                    command.Dispose();
                    command = null;
                }
            }
        }

        #endregion

        private DbConnection OpenConnection()
        {
            var connection = GetConnection();
            connection.Open();
            return connection;
        }

        private DbConnection GetConnection()
        {
            CheckDispose();
            string key = null;
            DbConnection connection;
            if (shared && HttpContext.Current != null)
            {
                key = string.Format("Connection {0}|{1}", GetDialect(), DbRegistry.GetConnectionString(DatabaseId));
                connection = DisposableHttpContext.Current[key] as DbConnection;
                if (connection != null)
                {
                    var state = ConnectionState.Closed;
                    var disposed = false;
                    try
                    {
                        state = connection.State;
                    }
                    catch (ObjectDisposedException)
                    {
                        disposed = true;
                    }
                    if (!disposed && (state == ConnectionState.Closed || state == ConnectionState.Broken))
                    {
                        if (string.IsNullOrEmpty(connection.ConnectionString))
                        {
                            connection.ConnectionString = DbRegistry.GetConnectionString(DatabaseId).ConnectionString;
                        }
                        return connection;
                    }
                }
            }
            connection = DbRegistry.CreateDbConnection(DatabaseId);
            if (proxyContext != null)
            {
                connection = new DbConnectionProxy(connection, proxyContext);
            }
            if (shared && HttpContext.Current != null) DisposableHttpContext.Current[key] = connection;
            return connection;
        }

        public IDbTransaction BeginTransaction()
        {
            if (InTransaction) throw new InvalidOperationException("Transaction already open.");

            Command.Transaction = Command.Connection.BeginTransaction();

            var tx = new DbTransaction(Command.Transaction);
            tx.Unavailable += TransactionUnavailable;
            return tx;
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            if (InTransaction) throw new InvalidOperationException("Transaction already open.");

            il = GetDialect().GetSupportedIsolationLevel(il);
            Command.Transaction = Command.Connection.BeginTransaction(il);

            var tx = new DbTransaction(Command.Transaction);
            tx.Unavailable += TransactionUnavailable;
            return tx;
        }

        public IDbTransaction BeginTransaction(bool nestedIfAlreadyOpen)
        {
            return nestedIfAlreadyOpen && InTransaction ? new DbNestedTransaction(Command.Transaction) : BeginTransaction();
        }

        public List<object[]> ExecuteList(string sql, params object[] parameters)
        {
            return Command.ExecuteList(sql, parameters);
        }

        public Task<List<object[]>> ExecuteListAsync(string sql, params object[] parameters)
        {
            return Command.ExecuteListAsync(sql, parameters);
        }

        public List<object[]> ExecuteList(ISqlInstruction sql)
        {
            return Command.ExecuteList(sql, GetDialect());
        }

        public Task<List<object[]>> ExecuteListAsync(ISqlInstruction sql)
        {
            return Command.ExecuteListAsync(sql, GetDialect());
        }

        public List<T> ExecuteList<T>(ISqlInstruction sql, Converter<IDataRecord, T> converter)
        {
            return Command.ExecuteList(sql, GetDialect(), converter);
        }

        public List<T> ExecuteList<T>(ISqlInstruction sql, Converter<object[], T> converter)
        {
            return Command.ExecuteList(sql, GetDialect(), converter);
        }

        public T ExecuteScalar<T>(string sql, params object[] parameters)
        {
            return Command.ExecuteScalar<T>(sql, parameters);
        }

        public T ExecuteScalar<T>(ISqlInstruction sql)
        {
            return Command.ExecuteScalar<T>(sql, GetDialect());
        }

        public int ExecuteNonQuery(string sql, params object[] parameters)
        {
            return Command.ExecuteNonQuery(sql, parameters);
        }

        public Task<int> ExecuteNonQueryAsync(string sql, params object[] parameters)
        {
            return Command.ExecuteNonQueryAsync(sql, parameters);
        }

        public int ExecuteNonQuery(ISqlInstruction sql)
        {
            return Command.ExecuteNonQuery(sql, GetDialect());
        }

        public int ExecuteBatch(IEnumerable<ISqlInstruction> batch)
        {
            if (batch == null) throw new ArgumentNullException(nameof(batch));

            var affected = 0;
            using (var tx = BeginTransaction())
            {
                foreach (var sql in batch)
                {
                    affected += ExecuteNonQuery(sql);
                }
                tx.Commit();
            }
            return affected;
        }

        private void TransactionUnavailable(object sender, EventArgs e)
        {
            if (Command.Transaction != null)
            {
                Command.Transaction = null;
            }
        }

        private void CheckDispose()
        {
            if (disposed) throw new ObjectDisposedException(GetType().FullName);
        }

        private ISqlDialect GetDialect()
        {
            return dialect ?? (dialect = DbRegistry.GetSqlDialect(DatabaseId));
        }

        private void AdoProxyExecutedEventHandler(ExecutedEventArgs a)
        {
            Logger.DebugWithProps(a.SqlMethod,
                new KeyValuePair<string, object>("duration", a.Duration.TotalMilliseconds),
                new KeyValuePair<string, object>("sql", RemoveWhiteSpaces(a.Sql)),
                new KeyValuePair<string, object>("sqlParams", RemoveWhiteSpaces(a.SqlParameters))
                );
        }

        private string RemoveWhiteSpaces(string str)
        {
            return !string.IsNullOrEmpty(str) ?
                str.Replace(Environment.NewLine, " ").Replace("\n", "").Replace("\r", "").Replace("\t", " ") :
                string.Empty;
        }

        public ISqlDialect GetSqlDialect(string databaseId)
        {
            return DbRegistry.GetSqlDialect(databaseId);
        }
    }

    public class DbManagerProxy : IDbManager
    {
        private DbManager dbManager { get; set; }

        public DbManagerProxy(DbManager dbManager)
        {
            this.dbManager = dbManager;
        }

        public void Dispose()
        {
            if (HttpContext.Current == null)
            {
                dbManager.Dispose();
            }
        }

        public DbConnection Connection { get { return dbManager.Connection; } }
        public string DatabaseId { get { return dbManager.DatabaseId; } }
        public bool InTransaction { get { return dbManager.InTransaction; } }

        public IDbTransaction BeginTransaction()
        {
            return dbManager.BeginTransaction();
        }

        public IDbTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return dbManager.BeginTransaction(isolationLevel);
        }

        public IDbTransaction BeginTransaction(bool nestedIfAlreadyOpen)
        {
            return dbManager.BeginTransaction(nestedIfAlreadyOpen);
        }

        public List<object[]> ExecuteList(string sql, params object[] parameters)
        {
            return dbManager.ExecuteList(sql, parameters);
        }

        public List<object[]> ExecuteList(ISqlInstruction sql)
        {
            return dbManager.ExecuteList(sql);
        }

        public Task<List<object[]>> ExecuteListAsync(ISqlInstruction sql)
        {
            return dbManager.ExecuteListAsync(sql);
        }

        public List<T> ExecuteList<T>(ISqlInstruction sql, Converter<IDataRecord, T> converter)
        {
            return dbManager.ExecuteList(sql, converter);
        }

        public List<T> ExecuteList<T>(ISqlInstruction sql, Converter<object[], T> converter)
        {
            return dbManager.ExecuteList<T>(sql, converter);
        }

        public T ExecuteScalar<T>(string sql, params object[] parameters)
        {
            return dbManager.ExecuteScalar<T>(sql, parameters);
        }

        public T ExecuteScalar<T>(ISqlInstruction sql)
        {
            return dbManager.ExecuteScalar<T>(sql);
        }

        public int ExecuteNonQuery(string sql, params object[] parameters)
        {
            return dbManager.ExecuteNonQuery(sql, parameters);
        }

        public int ExecuteNonQuery(ISqlInstruction sql)
        {
            return dbManager.ExecuteNonQuery(sql);
        }

        public int ExecuteBatch(IEnumerable<ISqlInstruction> batch)
        {
            return dbManager.ExecuteBatch(batch);
        }

        public ISqlDialect GetSqlDialect(string databaseId)
        {
            return dbManager.GetSqlDialect(databaseId);
        }
    }
}