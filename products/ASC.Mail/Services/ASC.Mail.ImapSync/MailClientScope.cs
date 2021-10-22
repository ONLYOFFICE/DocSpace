using ASC.Common;
using ASC.Core;
using ASC.Data.Storage;
using ASC.Mail.Core;
using ASC.Mail.Core.Engine;
using ASC.Mail.Utils;

using Microsoft.Extensions.DependencyInjection;

namespace ASC.Mail.ImapSync
{
    [Scope]
    public class MailClientScope
    {

        private TenantManager TenantManager { get; }
        private CoreBaseSettings CoreBaseSettings { get; }
        private StorageFactory StorageFactory { get; }
        private MailEnginesFactory MailEnginesFactory { get; }
        private SecurityContext SecurityContext { get; }
        private ApiHelper ApiHelper { get; }
        private IMailDaoFactory MailDaoFactory { get; }

        private MailboxEngine MailboxEngine { get; }
        private FolderEngine FolderEngine { get; }
        private ServiceProvider ServiceProvider { get; }

        public MailClientScope(
            ServiceProvider serviceProvider,
            TenantManager tenantManager,
            CoreBaseSettings coreBaseSettings,
            StorageFactory storageFactory,
            MailEnginesFactory mailEnginesFactory,
            SecurityContext securityContext,
            ApiHelper apiHelper,
            IMailDaoFactory mailDaoFactory,
            MailboxEngine mailboxEngine,
            FolderEngine folderEngine)
        {
            ServiceProvider = serviceProvider;
            CoreBaseSettings = coreBaseSettings;
            TenantManager = tenantManager;
            StorageFactory = storageFactory;
            MailEnginesFactory = mailEnginesFactory;
            SecurityContext = securityContext;
            ApiHelper = apiHelper;
            MailDaoFactory = mailDaoFactory;
            MailboxEngine = mailboxEngine;
            FolderEngine = folderEngine;
        }
    }
}
