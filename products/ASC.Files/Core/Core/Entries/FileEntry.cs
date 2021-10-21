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
using System.Text.Json.Serialization;

using ASC.Files.Core.Security;

namespace ASC.Files.Core
{
    [Serializable]
    public abstract class FileEntry : ICloneable
    {
        [JsonIgnore]
        public FileHelper FileHelper { get; set; }

        protected FileEntry()
        {

        }

        public FileEntry(FileHelper fileHelper)
        {
            FileHelper = fileHelper;
        }

        public virtual string Title { get; set; }

        public Guid CreateBy { get; set; }

        [JsonIgnore]
        public string CreateByString { get => FileHelper.GetCreateByString(this); }

        public Guid ModifiedBy { get; set; }

        [JsonIgnore]
        public string ModifiedByString { get => FileHelper.GetModifiedByString(this); }

        [JsonIgnore]
        public string CreateOnString
        {
            get { return CreateOn.Equals(default) ? null : CreateOn.ToString("g"); }
        }

        [JsonIgnore]
        public string ModifiedOnString
        {
            get { return ModifiedOn.Equals(default) ? null : ModifiedOn.ToString("g"); }
        }

        public string Error { get; set; }

        public FileShare Access { get; set; }

        public bool Shared { get; set; }

        public int ProviderId { get; set; }

        public string ProviderKey { get; set; }

        [JsonIgnore]
        public bool ProviderEntry
        {
            get { return !string.IsNullOrEmpty(ProviderKey); }
        }

        public DateTime CreateOn { get; set; }

        public DateTime ModifiedOn { get; set; }

        public FolderType RootFolderType { get; set; }

        public Guid RootFolderCreator { get; set; }

        public abstract bool IsNew { get; set; }

        public FileEntryType FileEntryType;

        public string _modifiedByString;
        public string _createByString;

        public override string ToString()
        {
            return Title;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    [Serializable]
    public abstract class FileEntry<T> : FileEntry, ICloneable
    {
        public T ID { get; set; }

        public T FolderID { get; set; }

        private T _folderIdDisplay;

        protected FileEntry()
        {

        }

        protected FileEntry(FileHelper fileHelper) : base(fileHelper)
        {
        }

        public T FolderIdDisplay
        {
            get
            {
                if (_folderIdDisplay != null) return _folderIdDisplay;

                return FolderID;
            }
            set { _folderIdDisplay = value; }
        }

        public T RootFolderId { get; set; }

        [JsonIgnore]
        public virtual string UniqID
        {
            get { return $"{GetType().Name.ToLower()}_{ID}"; }
        }

        public override bool Equals(object obj)
        {
            return obj is FileEntry<T> f && Equals(f.ID, ID);
        }

        public bool Equals(FileEntry<T> obj)
        {
            return Equals(obj.ID, ID);
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override string ToString()
        {
            return Title;
        }
    }
}