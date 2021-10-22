using System;
using System.Data;

using ASC.Common;
using ASC.Core.Common.EF;
using ASC.Mail.Server.Core.Dao;
using ASC.Mail.Server.Core.Dao.Interfaces;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace ASC.Mail.Core.MailServer.Core.Dao
{
    [Scope]
    public class MailServerDaoFactory : IMailServerDaoFactory
    {
        private IServiceProvider ServiceProvider { get; }
        private MailServerDbContext MailServerDbContext { get; }

        public MailServerDaoFactory(
            IServiceProvider serviceProvider,
            DbContextManager<MailServerDbContext> dbContextManager)
        {
            ServiceProvider = serviceProvider;
            MailServerDbContext = dbContextManager.Get("mailServer");
        }

        public IDbContextTransaction BeginTransaction(IsolationLevel? level = null)
        {
            return level.HasValue
                ? MailServerDbContext.Database.BeginTransaction(level.Value)
                : MailServerDbContext.Database.BeginTransaction();
        }

        public void SetServerDbConnectionString(string serverCs)
        {
            MailServerDbContext.Database.SetConnectionString(serverCs);
        }

        public MailServerDbContext GetContext()
        {
            return MailServerDbContext;
        }

        public IAliasDao GetAliasDao()
        {
            return ServiceProvider.GetService<IAliasDao>();
        }

        public IDkimDao GetDkimDao()
        {
            return ServiceProvider.GetService<IDkimDao>();
        }

        public IDomainDao GetDomainDao()
        {
            return ServiceProvider.GetService<IDomainDao>();
        }

        public IMailboxDao GetMailboxDao()
        {
            return ServiceProvider.GetService<IMailboxDao>();
        }
    }
    public class MailServerDaoFactoryExtension
    {
        public static void Register(DIHelper services)
        {
            services.TryAdd<IAliasDao, AliasDao>();
            services.TryAdd<IDkimDao, DkimDao>();
            services.TryAdd<IDomainDao, DomainDao>();
            services.TryAdd<IMailboxDao, MailboxDao>();
        }
    }
}
