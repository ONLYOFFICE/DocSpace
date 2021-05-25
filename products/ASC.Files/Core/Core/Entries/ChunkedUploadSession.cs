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
using System.Text.Json.Serialization;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core.ChunkedUploader;
using ASC.Web.Files.Utils;

using Microsoft.Extensions.Options;


namespace ASC.Files.Core
{
    [DebuggerDisplay("{Id} into {FolderId}")]
    public class ChunkedUploadSession<T> : CommonChunkedUploadSession
    {
        public T FolderId { get; set; }

        public File<T> File { get; set; }

        public bool Encrypted { get; set; }

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

        public Stream Serialize()
        {
            var str = JsonSerializer.Serialize(Property<T>.Serialize(this));
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(str));
            return stream;
        }

        public static ChunkedUploadSession<T> Deserialize(Stream stream, File<T> file)
        {
            stream.Position = 0;
            var sr = new StreamReader(stream);
            string str = sr.ReadToEnd();
            var property = JsonSerializer.Deserialize<Property<T>>(str);
            return property.Deserialize(file);
        }
        [Serializable]
        public class Property<T>
        {
            public static Property<T> Serialize(ChunkedUploadSession<T> session)
            {
                var serializeFile = new Property<T>() {
                    Title = session.File.Title,
                    CreateBy = session.File.CreateBy,
                    CreateByString = session.File.CreateByString,
                    ModifiedBy = session.File.ModifiedBy,
                    ModifiedByString = session.File.ModifiedByString,
                    Error = session.File.Error,
                    Access = session.File.Access,
                    Shared = session.File.Shared,
                    ProviderId = session.File.ProviderId,
                    ProviderKey = session.File.ProviderKey,
                    CreateOn = session.File.CreateOn,
                    ModifiedOn = session.File.ModifiedOn,
                    RootFolderType = session.File.RootFolderType,
                    RootFolderCreator = session.File.RootFolderCreator,
                    FileEntryType = session.File.FileEntryType,
                    ID = session.File.ID,
                    FolderID = session.File.FolderID,
                    FolderIdDisplay = session.File.FolderIdDisplay,
                    RootFolderId = session.File.RootFolderId,
                    Version = session.File.Version,
                    VersionGroup = session.File.VersionGroup,
                    Comment = session.File.Comment,
                    PureTitle = session.File.PureTitle,
                    ContentLength = session.File.ContentLength,
                    ContentLengthString = session.File.ContentLengthString,
                    FileStatus = session.File.FileStatus,
                    Locked = session.File.Locked,
                    LockedBy = session.File.LockedBy,
                    IsNew = session.File.IsNew,
                    IsFavorite = session.File.IsFavorite,
                    IsTemplate = session.File.IsTemplate,
                    Encrypted = session.File.Encrypted,
                    Forcesave = session.File.Forcesave,
                    ConvertedType = session.File.ConvertedType,
                    NativeAccessor = session.File.NativeAccessor,
                    FolderId = session.FolderId,
                    EncryptedChunk = session.Encrypted,
                    Id = session.Id,
                    Created = session.Created,
                    Expired = session.Expired,
                    Location = session.Location,
                    BytesUploaded = session.BytesUploaded,
                    BytesTotal = session.BytesTotal,
                    TenantId = session.TenantId,
                    UserId = session.UserId,
                    UseChunks = session.UseChunks,
                    CultureName = session.CultureName,
                    Items = session.Items,
                    TempPath = session.TempPath,
                    UploadId = session.UploadId,
                    ChunksBuffer = session.ChunksBuffer
                };
                return serializeFile;
            }

            public string TempPath { get; set; }

            public string UploadId { get; set; }

            public string ChunksBuffer { get; set; }

            public T FolderId { get; set; }

            public bool EncryptedChunk { get; set; }

            public string Id { get; set; }

            public DateTime Created { get; set; }

            public DateTime Expired { get; set; }

            public string Location { get; set; }

            public long BytesUploaded { get; set; }

            public long BytesTotal { get; set; }

            public int TenantId { get; set; }

            public Guid UserId { get; set; }

            public bool UseChunks { get; set; }

            public string CultureName { get; set; }

            public Dictionary<string, object> Items { get; set; }

            public string Title { get; set; }

            [JsonPropertyName("create_by_id")]
            public Guid CreateBy { get; set; }

            [JsonPropertyName("create_by")]
            public string CreateByString { get; set; }

            [JsonPropertyName("modified_by_id")]
            public Guid ModifiedBy { get; set; }

            [JsonPropertyName("modified_by")]
            public string ModifiedByString { get; set; }

            public string Error { get; set; }

            public Security.FileShare Access { get; set; }

            public bool Shared { get; set; }

            [JsonPropertyName("provider_id")]
            public int ProviderId { get; set; }

            [JsonPropertyName("provider_key")]
            public string ProviderKey { get; set; }

            public DateTime CreateOn { get; set; }

            public DateTime ModifiedOn { get; set; }

            public FolderType RootFolderType { get; set; }

            public Guid RootFolderCreator { get; set; }

            public FileEntryType FileEntryType { get; set; }

            public T ID { get; set; }

            public T FolderID { get; set; }

            [JsonPropertyName("folder_id")]
            public T FolderIdDisplay { get; set; }

            public T RootFolderId { get; set; }

            public int Version { get; set; }

            [JsonPropertyName("version_group")]
            public int VersionGroup { get; set; }

            public string Comment { get; set; }

            public string PureTitle { get; set; }

            [JsonPropertyName("content_length")]
            public long ContentLength { get; set; }

            [JsonPropertyName("content_length_string")]
            public string ContentLengthString { get; set; }

            [JsonPropertyName("file_status")]
            public FileStatus FileStatus { get; set; }

            public bool Locked { get; set; }

            [JsonPropertyName("locked_by")]
            public string LockedBy { get; set; }

            public bool IsNew { get; set; }

            public bool IsFavorite { get; set; }

            public bool IsTemplate { get; set; }

            public bool Encrypted { get; set; }

            public ForcesaveType Forcesave { get; set; }

            public string ConvertedType { get; set; }

            public object NativeAccessor { get; set; }

            public ChunkedUploadSession<T> Deserialize(File<T> file)
            {
                file.Title = Title;
                file.CreateBy = CreateBy;
                file.CreateByString = CreateByString;
                file.ModifiedBy = ModifiedBy;
                file.ModifiedByString = ModifiedByString;
                file.Error = Error;
                file.Access = Access;
                file.Shared = Shared;
                file.ProviderId = ProviderId;
                file.ProviderKey = ProviderKey;
                file.CreateOn = CreateOn;
                file.ModifiedOn = ModifiedOn;
                file.RootFolderType = RootFolderType;
                file.RootFolderCreator = RootFolderCreator;
                file.FileEntryType = FileEntryType;
                file.ID = ID;
                file.FolderID = FolderID;
                file.FolderIdDisplay = FolderIdDisplay;
                file.RootFolderId = RootFolderId;
                file.Version = Version;
                file.VersionGroup = VersionGroup;
                file.Comment = Comment;
                file.PureTitle = PureTitle;
                file.ContentLength = ContentLength;
                file.ContentLengthString = ContentLengthString;
                file.FileStatus = FileStatus;
                file.Locked = Locked;
                file.LockedBy = LockedBy;
                file.IsNew = IsNew;
                file.IsFavorite = IsFavorite;
                file.IsTemplate = IsTemplate;
                file.Encrypted = Encrypted;
                file.Forcesave = Forcesave;
                file.ConvertedType = ConvertedType;
                file.NativeAccessor = NativeAccessor;

                var chunk = new ChunkedUploadSession<T>(file, BytesTotal);

                chunk.FolderId = FolderId;
                chunk.Encrypted = EncryptedChunk;
                chunk.Id = Id;
                chunk.Created = Created;
                chunk.Expired = Expired;
                chunk.Location = Location;
                chunk.BytesUploaded = BytesUploaded;
                chunk.TenantId = TenantId;
                chunk.UserId = UserId;
                chunk.UseChunks = UseChunks;
                chunk.CultureName = CultureName;
                chunk.Items = Items;
                chunk.TempPath = TempPath;
                chunk.UploadId = UploadId;
                chunk.ChunksBuffer = ChunksBuffer;
                return chunk;
            }
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


        public object ToResponseObject<T>(ChunkedUploadSession<T> session, bool appendBreadCrumbs = false)
        {
            var pathFolder = appendBreadCrumbs
                                 ? EntryManager.GetBreadCrumbs(session.FolderId).Select(f =>
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
