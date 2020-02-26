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
using System.Linq;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core.ChunkedUploader;
using ASC.Web.Files.Utils;

using Microsoft.Extensions.Options;

namespace ASC.Files.Core
{
    [DebuggerDisplay("{Id} into {FolderId}")]
    [Serializable]
    public class ChunkedUploadSession : CommonChunkedUploadSession
    {
        public string FolderId { get; set; }

        public File File { get; set; }

        public bool Encrypted { get; set; }

        public ChunkedUploadSession(File file, long bytesTotal) : base(bytesTotal)
        {
            File = file;
        }

        public override object Clone()
        {
            var clone = (ChunkedUploadSession)MemberwiseClone();
            clone.File = (File)File.Clone();
            return clone;
        }
    }

    public class ChunkedUploadSessionHelper
    {
        public EntryManager EntryManager { get; }
        public ILog Logger { get; }

        public ChunkedUploadSessionHelper(IOptionsMonitor<ILog> options, EntryManager entryManager)
        {
            EntryManager = entryManager;
            Logger = options.CurrentValue;
        }


        public object ToResponseObject(ChunkedUploadSession session, bool appendBreadCrumbs = false)
        {
            var pathFolder = appendBreadCrumbs
                                 ? EntryManager.GetBreadCrumbs(session.FolderId).Select(f =>
                                 {
                                     //todo: check how?
                                     if (f == null)
                                     {
                                         Logger.ErrorFormat("GetBreadCrumbs {0} with null", session.FolderId);
                                         return string.Empty;
                                     }
                                     return f.ID;
                                 })
                                 : new List<object> { session.FolderId };

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

    public static class ChunkedUploadSessionHelperExtention
    {
        public static DIHelper AddChunkedUploadSessionHelperService(this DIHelper services)
        {
            services.TryAddScoped<ChunkedUploadSessionHelper>();
            return services.AddEntryManagerService();
        }
    }
}
