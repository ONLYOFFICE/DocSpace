using ASC.Common;
using ASC.Mail.Server.Core.Dao;
using ASC.Mail.Server.Core.Dao.Interfaces;

using Microsoft.EntityFrameworkCore.Storage;

namespace ASC.Mail.Core.MailServer.Core.Dao
{
    [Scope(typeof(MailServerDaoFactory), Additional = typeof(MailServerDaoFactoryExtension))]
    public interface IMailServerDaoFactory
    {
        MailServerDbContext GetContext();

        void SetServerDbConnectionString(string serverCs);

        IAliasDao GetAliasDao();

        IDkimDao GetDkimDao();

        IDomainDao GetDomainDao();

        IMailboxDao GetMailboxDao();

        public IDbContextTransaction BeginTransaction(System.Data.IsolationLevel? level = null);
    }
}

