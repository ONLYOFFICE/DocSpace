﻿using System;
using System.Collections.Generic;

using ASC.Common;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Context
{
    public class MySqlMessagesContext : MessagesContext { }
    public class PostgreSqlMessagesContext : MessagesContext { }
    public class MessagesContext : BaseDbContext
    {
        public DbSet<LoginEvents> LoginEvents { get; set; }
        public DbSet<User> Users { get; set; }

        protected override Dictionary<Provider, Func<BaseDbContext>> ProviderContext =>
            new Dictionary<Provider, Func<BaseDbContext>>()
            {
                { Provider.MySql, () => new MySqlMessagesContext() } ,
                { Provider.PostgreSql, () => new PostgreSqlMessagesContext() } ,
            };

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ModelBuilderWrapper
                .From(modelBuilder, Provider)
                .AddLoginEvents()
                .AddUser()
                .AddDbFunction();
        }
    }

    public static class MessagesContextExtension
    {
        public static DIHelper AddMessagesContextService(this DIHelper services)
        {
            return services.AddDbContextManagerService<MessagesContext>();
        }
    }
}