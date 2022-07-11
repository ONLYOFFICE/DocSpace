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
using System.Linq;
using System.Threading.Tasks;

using ASC.Common;
using ASC.CRM.Core.Dao;
using ASC.Files.Core;
using ASC.Files.Core.Security;
using ASC.Web.Files.Api;

using Autofac;

namespace ASC.CRM.Core
{
    public class FileSecurity : IFileSecurity
    {
        private readonly DaoFactory _daoFactory;
        private readonly FilesIntegration _filesIntegration;
        private readonly CrmSecurity _crmSecurity;

        public FileSecurity(FilesIntegration filesIntegration,
                            CrmSecurity crmSecurity,
                            DaoFactory daoFactory)
        {
            _filesIntegration = filesIntegration;
            _crmSecurity = crmSecurity;
            _daoFactory = daoFactory;
        }

        public Task<bool> CanCreateAsync<T>(FileEntry<T> entry, Guid userId)
        {
            return Task.FromResult(true);
        }

        public Task<bool> CanCommentAsync<T>(FileEntry<T> entry, Guid userId)
        {
            return CanEditAsync(entry, userId);
        }

        public Task<bool> CanFillFormsAsync<T>(FileEntry<T> entry, Guid userId)
        {
            return CanEditAsync(entry, userId);
        }

        public Task<bool> CanReviewAsync<T>(FileEntry<T> entry, Guid userId)
        {
            return CanEditAsync(entry, userId);
        }

        public Task<bool> CanDeleteAsync<T>(FileEntry<T> entry, Guid userId)
        {
            return CanEditAsync(entry, userId);
        }

        public async Task<bool> CanEditAsync<T>(FileEntry<T> entry, Guid userId)
        {
            return
                await CanReadAsync(entry, userId) &&
                entry.CreateBy == userId || entry.ModifiedBy == userId || _crmSecurity.IsAdministrator(userId);
        }

        public Task<bool> CanReadAsync<T>(FileEntry<T> entry, Guid userId)
        {
            if (entry.FileEntryType == FileEntryType.Folder) return Task.FromResult(false);

            var invoice = _daoFactory.GetInvoiceDao().GetByFileId(Convert.ToInt32(entry.ID));
            if (invoice != null)
                return Task.FromResult(_crmSecurity.CanAccessTo(invoice, userId));

            return InternalCanReadAsync(entry, userId);

        }

        private async Task<bool> InternalCanReadAsync<T>(FileEntry<T> entry, Guid userId)
        {
            var reportFile = await _daoFactory.GetReportDao().GetFileAsync(Convert.ToInt32(entry.ID), userId);

            if (reportFile != null)
                return true;

            var tagDao = _filesIntegration.DaoFactory.GetTagDao<T>();

            var eventIds = await tagDao.GetTagsAsync(entry.ID, FileEntryType.File, TagType.System)
                .Where(x => x.TagName.StartsWith("RelationshipEvent_"))
                .Select(x => Convert.ToInt32(x.TagName.Split(new[] { '_' })[1]))
                .ToListAsync();

            if (eventIds.Count == 0) return false;

            var eventItem = _daoFactory.GetRelationshipEventDao().GetByID(eventIds.First());

            return _crmSecurity.CanAccessTo(eventItem, userId);

        }

        public Task<IEnumerable<Guid>> WhoCanReadAsync<T>(FileEntry<T> entry)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CanCustomFilterEditAsync<T>(FileEntry<T> entry, Guid userId)
        {
            return CanEditAsync(entry, userId);
        }
    }

    [Scope]
    public class FileSecurityProvider : IFileSecurityProvider
    {
        public FileSecurityProvider(FilesIntegration filesIntegration,
                                    CrmSecurity crmSecurity,
                                    DaoFactory daoFactory)
        {
            FilesIntegration = filesIntegration;
            CRMSecurity = crmSecurity;
            DaoFactory = daoFactory;
        }

        public DaoFactory DaoFactory { get; }
        public FilesIntegration FilesIntegration { get; }
        public CrmSecurity CRMSecurity { get; }

        public IFileSecurity GetFileSecurity(string data)
        {
            return new FileSecurity(FilesIntegration, CRMSecurity, DaoFactory);
        }

        public Dictionary<object, IFileSecurity> GetFileSecurity(Dictionary<string, string> data)
        {
            return data.ToDictionary<KeyValuePair<string, string>, object, IFileSecurity>(d => d.Key, d => new FileSecurity(FilesIntegration, CRMSecurity, DaoFactory));
        }
    }
}