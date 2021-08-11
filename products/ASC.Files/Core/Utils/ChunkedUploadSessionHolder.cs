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
using System.IO;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core.ChunkedUploader;
using ASC.Files.Core;
using ASC.Web.Files.Classes;
using ASC.Web.Studio.Core;

using Microsoft.Extensions.Options;

namespace ASC.Web.Files.Utils
{
    [Scope]
    public class ChunkedUploadSessionHolder
    {
        public static readonly TimeSpan SlidingExpiration = TimeSpan.FromHours(12);

        private IOptionsMonitor<ILog> Options { get; }
        private GlobalStore GlobalStore { get; }
        private SetupInfo SetupInfo { get; }
        private TempPath TempPath { get; }
        private FileHelper FileHelper { get; }

        public ChunkedUploadSessionHolder(
            IOptionsMonitor<ILog> options,
            GlobalStore globalStore,
            SetupInfo setupInfo,
            TempPath tempPath,
            FileHelper fileHelper)
        {
            Options = options;
            GlobalStore = globalStore;
            SetupInfo = setupInfo;
            TempPath = tempPath;
            FileHelper = fileHelper;

            // clear old sessions
            //TODO
            //try
            //{
            //    CommonSessionHolder(false).DeleteExpired();
            //}
            //catch (Exception err)
            //{
            //    options.CurrentValue.Error(err);
            //}
        }

        public void StoreSession<T>(ChunkedUploadSession<T> s)
        {
            CommonSessionHolder(false).Store(s);
        }

        public void RemoveSession<T>(ChunkedUploadSession<T> s)
        {
            CommonSessionHolder(false).Remove(s);
        }

        public ChunkedUploadSession<T> GetSession<T>(string sessionId)
        {
            using var stream = CommonSessionHolder(false).GetStream(sessionId);
            var chunkedUploadSession =  ChunkedUploadSession<T>.Deserialize(stream, FileHelper);
            return chunkedUploadSession;
        }


        public ChunkedUploadSession<T> CreateUploadSession<T>(File<T> file, long contentLength)
        {
            var result = new ChunkedUploadSession<T>(file, contentLength);
            CommonSessionHolder().Init(result);
            return result;
        }

        public void UploadChunk<T>(ChunkedUploadSession<T> uploadSession, Stream stream, long length)
        {
            CommonSessionHolder().UploadChunk(uploadSession, stream, length);
        }

        public void FinalizeUploadSession<T>(ChunkedUploadSession<T> uploadSession)
        {
            CommonSessionHolder().Finalize(uploadSession);
        }

        public void Move<T>(ChunkedUploadSession<T> chunkedUploadSession, string newPath)
        {
            CommonSessionHolder().Move(chunkedUploadSession, newPath, chunkedUploadSession.CheckQuota);
        }

        public void AbortUploadSession<T>(ChunkedUploadSession<T> uploadSession)
        {
            CommonSessionHolder().Abort(uploadSession);
        }

        public Stream UploadSingleChunk<T>(ChunkedUploadSession<T> uploadSession, Stream stream, long chunkLength)
        {
            return CommonSessionHolder().UploadSingleChunk(uploadSession, stream, chunkLength);
        }

        private CommonChunkedUploadSessionHolder CommonSessionHolder(bool currentTenant = true)
        {
            return new CommonChunkedUploadSessionHolder(TempPath, Options, GlobalStore.GetStore(currentTenant), FileConstant.StorageDomainTmp, SetupInfo.ChunkUploadSize);
        }
    }
}