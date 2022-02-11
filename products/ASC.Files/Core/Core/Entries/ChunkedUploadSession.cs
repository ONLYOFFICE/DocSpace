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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core.ChunkedUploader;
using ASC.Web.Files.Utils;

using Microsoft.Extensions.Options;

namespace ASC.Files.Core
{
    [DebuggerDisplay("{Id} into {FolderId}")]
    [Serializable]
    public class ChunkedUploadSession<T> : CommonChunkedUploadSession
    {
        public T FolderId { get; set; }

        public File<T> File { get; set; }

        public bool Encrypted { get; set; }

        //hack for Backup bug 48873
        [NonSerialized]
        public bool CheckQuota = true;

        public ChunkedUploadSession(File<T> file, long bytesTotal) : base(bytesTotal)
        {
            File = file;
        }

        public override object Clone()
        {
            var clone = (ChunkedUploadSession<T>)MemberwiseClone();
            clone.File = (File<T>)File.Clone();
            return clone;
        }

        public override Stream Serialize()
        {
            var str = JsonSerializer.Serialize(this);
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(str));
            return stream;
        }

        public static ChunkedUploadSession<T> Deserialize(Stream stream, FileHelper fileHelper)
        {
            var chunkedUploadSession = JsonSerializer.Deserialize<ChunkedUploadSession<T>>(stream);
            chunkedUploadSession.File.FileHelper = fileHelper;
            chunkedUploadSession.TransformItems();
            return chunkedUploadSession;

        }
    }

    [Scope]
    public class ChunkedUploadSessionHelper
    {
        private EntryManager EntryManager { get; }
        public ILog Logger { get; }

        public ChunkedUploadSessionHelper(IOptionsMonitor<ILog> options, EntryManager entryManager)
        {
            EntryManager = entryManager;
            Logger = options.CurrentValue;
        }


        public async Task<object> ToResponseObjectAsync<T>(ChunkedUploadSession<T> session, bool appendBreadCrumbs = false)
        {
            var breadCrumbs = await EntryManager.GetBreadCrumbsAsync(session.FolderId);
            var pathFolder = appendBreadCrumbs
                                 ? breadCrumbs.Select(f =>
                                 {
                                     //todo: check how?
                                     if (f == null)
                                     {
                                         Logger.ErrorFormat("GetBreadCrumbs {0} with null", session.FolderId);
                                         return default;
                                     }
                                     if (f is Folder<string> f1) return (T)Convert.ChangeType(f1.ID, typeof(T));
                                     if (f is Folder<int> f2) return (T)Convert.ChangeType(f2.ID, typeof(T));
                                     return (T)Convert.ChangeType(0, typeof(T));
                                 })
                                 : new List<T> { session.FolderId };

            return new
            {
                id = session.Id,
                path = pathFolder,
                created = session.Created,
                expired = session.Expired,
                location = session.Location,
                bytes_uploaded = session.BytesUploaded,
                bytes_total = session.BytesTotal
            };
        }
    }
}
