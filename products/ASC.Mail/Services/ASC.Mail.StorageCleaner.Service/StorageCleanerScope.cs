
using ASC.Common;
using ASC.Core;
using ASC.Data.Storage;
using ASC.Mail.Core;
using ASC.Mail.Core.Engine;
using ASC.Mail.Utils;

namespace ASC.Mail.StorageCleaner.Service
{
    [Scope]
    public class StorageCleanerScope
    {
        private Server.Core.ServerEngine ServerEngine { get; }
        private SecurityContext SecurityContext { get; }
        private UserManager UserManager { get; }
        private IMailDaoFactory MailDaoFactory { get; }
        private ApiHelper ApiHelper { get; }
        private MailboxEngine MailboxEngine { get; }
        private TenantManager TenantManager { get; }
        private ServerMailboxEngine ServerMailboxEngine { get; }
        private ServerDomainEngine ServerDomainEngine { get; }
        private UserFolderEngine UserFolderEngine { get; }
        private OperationEngine OperationEngine { get; }
        private StorageFactory StorageFactory { get; }

        public StorageCleanerScope(
            Server.Core.ServerEngine serverEngine,
            SecurityContext securityContext,
            UserManager userManager,
            IMailDaoFactory mailDaoFactory,
            ApiHelper apiHelper,
            MailboxEngine mailboxEngine,
            TenantManager tenantManager,
            ServerMailboxEngine serverMailboxEngine,
            ServerDomainEngine serverDomainEngine,
            UserFolderEngine userFolderEngine,
            OperationEngine operationEngine,
            StorageFactory storageFactory)
        {
            ServerEngine = serverEngine;
            SecurityContext = securityContext;
            UserManager = userManager;
            MailDaoFactory = mailDaoFactory;
            ApiHelper = apiHelper;
            MailboxEngine = mailboxEngine;
            TenantManager = tenantManager;
            ServerMailboxEngine = serverMailboxEngine;
            ServerDomainEngine = serverDomainEngine;
            UserFolderEngine = userFolderEngine;
            OperationEngine = operationEngine;
            StorageFactory = storageFactory;
        }
    }
}
