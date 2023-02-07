// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

using static ASC.Files.Core.Security.FileSecurity;

namespace ASC.Files.Core;

[Serializable]
public abstract class FileEntry : ICloneable
{
    [JsonIgnore]
    public FileHelper FileHelper { get; set; }

    [JsonIgnore]
    public Global Global { get; set; }

    protected FileEntry() { }

    protected FileEntry(FileHelper fileHelper, Global global)
    {
        FileHelper = fileHelper;
        Global = global;
    }

    public virtual string Title { get; set; }
    public Guid CreateBy { get; set; }

    [JsonIgnore]
    public string CreateByString
    {
        get => !CreateBy.Equals(Guid.Empty) ? Global.GetUserName(CreateBy) : _createByString;
        set => _createByString = value;
    }

    public Guid ModifiedBy { get; set; }

    [JsonIgnore]
    public string ModifiedByString
    {
        get => !ModifiedBy.Equals(Guid.Empty) ? Global.GetUserName(ModifiedBy) : _modifiedByString;
        set => _modifiedByString = value;
    }

    [JsonIgnore]
    public string CreateOnString => CreateOn.Equals(default) ? null : CreateOn.ToString("g");

    [JsonIgnore]
    public string ModifiedOnString => ModifiedOn.Equals(default) ? null : ModifiedOn.ToString("g");

    public string Error { get; set; }
    public FileShare Access { get; set; }
    public bool Shared { get; set; }
    public int ProviderId { get; set; }
    public string ProviderKey { get; set; }

    [JsonIgnore]
    public bool ProviderEntry => !string.IsNullOrEmpty(ProviderKey);

    public DateTime CreateOn { get; set; }
    public DateTime ModifiedOn { get; set; }
    public FolderType RootFolderType { get; set; }
    public Guid RootCreateBy { get; set; }
    public abstract bool IsNew { get; set; }
    public FileEntryType FileEntryType { get; set; }
    public IEnumerable<Tag> Tags { get; set; }

    private string _modifiedByString;
    private string _createByString;

    public override string ToString()
    {
        return Title;
    }

    public object Clone()
    {
        return MemberwiseClone();
    }
}

public interface IFileEntry<in T>
{
    string UniqID { get; }
}


[Serializable]
public abstract class FileEntry<T> : FileEntry, ICloneable, IFileEntry<T>
{
    public T Id { get; set; }
    public T ParentId { get; set; }

    public IDictionary<FilesSecurityActions, bool> Security { get; set; }

    private T _folderIdDisplay;
    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly FilesSettingsHelper _filesSettingsHelper;
    private readonly FileDateTime _fileDateTime;


    protected FileEntry() { }

    protected FileEntry(
        FileHelper fileHelper,
        Global global,
        GlobalFolderHelper globalFolderHelper,
        FilesSettingsHelper filesSettingsHelper,
        FileDateTime fileDateTime) : base(fileHelper, global)
    {
        _globalFolderHelper = globalFolderHelper;
        _filesSettingsHelper = filesSettingsHelper;
        _fileDateTime = fileDateTime;
    }

    public T FolderIdDisplay
    {
        get
        {
            if (_folderIdDisplay != null)
            {
                return _folderIdDisplay;
            }

            return ParentId;
        }
        set => _folderIdDisplay = value;
    }

    public string DeletedPermanentlyOnString
    {
        get
        {
            if (!ModifiedOn.Equals(default(DateTime)) && Equals(FolderIdDisplay, _globalFolderHelper.FolderTrash) && _filesSettingsHelper.AutomaticallyCleanUp.IsAutoCleanUp)
            {
                var deletedPermanentlyOn = _fileDateTime.GetModifiedOnWithAutoCleanUp(ModifiedOn, _filesSettingsHelper.AutomaticallyCleanUp.Gap);
                return deletedPermanentlyOn.ToString("g");
            }
            else
            {
                return null;
            }
        }
    }

    public bool DenyDownload { get; set; }

    public bool DenySharing { get; set; }

    public T RootId { get; set; }

    [JsonIgnore]
    public virtual string UniqID => $"{GetType().Name.ToLower()}_{Id}";

    public override bool Equals(object obj)
    {
        return obj is FileEntry<T> f && Equals(f.Id, Id);
    }

    public virtual bool Equals(FileEntry<T> obj)
    {
        return Equals(obj.Id, Id);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public override string ToString()
    {
        return Title;
    }
}
