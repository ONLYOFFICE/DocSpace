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


using ASC.Common;
using ASC.Core.Common.EF;
using ASC.Mail.Core.Dao;
using ASC.Mail.Core.Dao.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace ASC.Mail.Core
{
    public class DaoFactory : IDaoFactory
    {
        public MailDbContext MailDb { get; }
        public AccountDao AccountDao { get; }
        public AlertDao AlertDao { get; }
        public ChainDao ChainDao { get; }
        public ContactCardDao ContactCardDao { get; }
        public ContactDao ContactDao { get; }
        public ContactInfoDao ContactInfoDao { get; }
        public CrmContactDao CrmContactDao { get; }
        public CrmLinkDao CrmLinkDao { get; }
        public MailboxAutoreplyDao MailboxAutoreplyDao { get; }
        public MailboxAutoreplyHistoryDao MailboxAutoreplyHistoryDao { get; }
        public MailboxDao MailboxDao { get; }
        public MailboxServerDao MailboxServerDao { get; }
        public MailboxDomainDao MailboxDomainDao { get; }
        public MailboxProviderDao MailboxProviderDao { get; }
        public DisplayImagesAddressDao DisplayImagesAddressDao { get; }
        public FilterDao FilterDao { get; }
        public MailboxSignatureDao MailboxSignatureDao { get; }
        public MailDao MailDao { get; }
        public MailGarbageDao MailGarbageDao { get; }
        public MailInfoDao MailInfoDao { get; }
        public ServerDao ServerDao { get; }
        public ServerDnsDao ServerDnsDao { get; }
        public ServerDomainDao ServerDomainDao { get; }
        public ServerGroupDao ServerGroupDao { get; }
        public TagAddressDao TagAddressDao { get; }
        public TagDao TagDao { get; }
        public TagMailDao TagMailDao { get; }
        public UserFolderDao UserFolderDao { get; }
        public UserFolderTreeDao UserFolderTreeDao { get; }
        public UserFolderXMailDao UserFolderXMailDao { get; }
        public AttachmentDao AttachmentDao { get; }
        public FolderDao FolderDao { get; }
        public ImapFlagsDao ImapFlagsDao { get; }
        public ImapSpecialMailboxDao ImapSpecialMailboxDao { get; }

        public DaoFactory(
            DbContextManager<MailDbContext> dbContext,
            AccountDao accountDao,
            AlertDao alertDao,
            AttachmentDao attachmentDao,
            ChainDao chainDao,
            ContactCardDao contactCardDao,
            ContactDao contactDao,
            ContactInfoDao contactInfoDao,
            CrmContactDao crmContactDao,
            CrmLinkDao crmLinkDao,
            DisplayImagesAddressDao displayImagesAddressDao,
            FilterDao filterDao,
            FolderDao folderDao,
            ImapFlagsDao imapFlagsDao,
            ImapSpecialMailboxDao imapSpecialMailboxDao,
            MailboxAutoreplyDao mailboxAutoreplyDao,
            MailboxAutoreplyHistoryDao mailboxAutoreplyHistoryDao,
            MailboxDao mailboxDao,
            MailboxDomainDao mailboxDomainDao,
            MailboxProviderDao mailboxProviderDao,
            MailboxServerDao mailboxServerDao,
            MailboxSignatureDao mailboxSignatureDao,
            MailDao mailDao,
            MailGarbageDao mailGarbageDao,
            MailInfoDao mailInfoDao,
            ServerDao serverDao,
            ServerDnsDao serverDnsDao,
            ServerDomainDao serverDomainDao,
            ServerGroupDao serverGroupDao,
            TagAddressDao tagAddressDao,
            TagDao tagDao,
            TagMailDao tagMailDao,
            UserFolderDao userFolderDao,
            UserFolderTreeDao userFolderTreeDao,
            UserFolderXMailDao userFolderXMailDao)
        {
            MailDb = dbContext.Get("mail");
            AccountDao = accountDao;
            AlertDao = alertDao;
            ChainDao = chainDao;
            ContactCardDao = contactCardDao;
            ContactDao = contactDao;
            ContactInfoDao = contactInfoDao;
            CrmContactDao = crmContactDao;
            CrmLinkDao = crmLinkDao;
            MailboxAutoreplyDao = mailboxAutoreplyDao;
            MailboxAutoreplyHistoryDao = mailboxAutoreplyHistoryDao;
            MailboxDao = mailboxDao;
            MailboxServerDao = mailboxServerDao;
            MailboxDomainDao = mailboxDomainDao;
            MailboxProviderDao = mailboxProviderDao;
            DisplayImagesAddressDao = displayImagesAddressDao;
            FilterDao = filterDao;
            MailboxSignatureDao = mailboxSignatureDao;
            MailDao = mailDao;
            MailGarbageDao = mailGarbageDao;
            MailInfoDao = mailInfoDao;
            ServerDao = serverDao;
            ServerDnsDao = serverDnsDao;
            ServerDomainDao = serverDomainDao;
            ServerGroupDao = serverGroupDao;
            TagAddressDao = tagAddressDao;
            TagDao = tagDao;
            TagMailDao = tagMailDao;
            UserFolderDao = userFolderDao;
            UserFolderTreeDao = userFolderTreeDao;
            UserFolderXMailDao = userFolderXMailDao;
            AttachmentDao = attachmentDao;
            FolderDao = folderDao;
            ImapFlagsDao = imapFlagsDao;
            ImapSpecialMailboxDao = imapSpecialMailboxDao;
        }

        public IDbContextTransaction BeginTransaction() {
            return MailDb.Database.BeginTransaction();
        }
    }

    public static class DaoFactoryExtension
    {
        public static DIHelper AddDaoFactoryService(this DIHelper services)
        {
            services.TryAddScoped<DaoFactory>();

            return services
                .AddMailDbContextService()
                .AddMailboxProviderDaoService()
                .AddMailboxDomainDaoService()
                .AddMailboxServerDaoService()
                .AddMailboxAutoreplyDaoService()
                .AddMailboxAutoreplyHistoryDaoService()
                .AddMailboxDaoService()
                .AddAccountDaoService()
                .AddAlertDaoService()
                .AddDisplayImagesAddressDaoService()
                .AddMailboxSignatureDaoService()
                .AddTagDaoService()
                .AddAttachmentDaoService();
        }
    }
}
