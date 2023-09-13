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

using Profile = AutoMapper.Profile;

namespace ASC.Files.Core;

[Flags]
public enum TagType
{
    New = 1,
    Favorite = 2,
    System = 4,
    Locked = 8,
    Recent = 16,
    Template = 32,
    Custom = 64,
    Pin = 128,
    Origin = 256
}

[DebuggerDisplay("{Name} ({Id}) entry {EntryType} ({EntryId})")]
public sealed class Tag : IMapFrom<DbFilesTag>
{
    public string Name { get; set; }
    public TagType Type { get; set; }
    public Guid Owner { get; set; }
    public object EntryId { get; set; }
    public FileEntryType EntryType { get; set; }
    public int Id { get; set; }
    public int Count { get; set; }

    public Tag() { }

    public Tag(string name, TagType type, Guid owner)
        : this(name, type, owner, 0)
    {
    }

    public Tag(string name, TagType type, Guid owner, int count)
    {
        Name = name;
        Type = type;
        Owner = owner;
        Count = count;
    }

    public Tag AddEntry<T>(FileEntry<T> entry)
    {
        if (entry != null)
        {
            EntryId = entry.Id;
            EntryType = entry.FileEntryType;
        }

        return this;
    }

    public static Tag New<T>(Guid owner, FileEntry<T> entry)
    {
        return New(owner, entry, 1);
    }

    public static Tag New<T>(Guid owner, FileEntry<T> entry, int count)
    {
        return new Tag("new", TagType.New, owner, count).AddEntry(entry);
    }

    public static Tag Recent<T>(Guid owner, FileEntry<T> entry)
    {
        return new Tag("recent", TagType.Recent, owner, 0).AddEntry(entry);
    }

    public static Tag Favorite<T>(Guid owner, FileEntry<T> entry)
    {
        return new Tag("favorite", TagType.Favorite, owner, 0).AddEntry(entry);
    }

    public static Tag Template<T>(Guid owner, FileEntry<T> entry)
    {
        return new Tag("template", TagType.Template, owner, 0).AddEntry(entry);
    }

    public static Tag Custom<T>(Guid owner, FileEntry<T> entry, string name)
    {
        return new Tag(name, TagType.Custom, owner, 0).AddEntry(entry);
    }

    public static Tag Pin<T>(Guid owner, FileEntry<T> entry)
    {
        return new Tag("pin", TagType.Pin, owner, 0).AddEntry(entry);
    }

    public static Tag Origin<T>(T entryId, FileEntryType type, T originId, Guid owner)
    {
        return new Tag(originId.ToString(), TagType.Origin, owner, 0)
        {
            EntryId = entryId,
            EntryType = type
        };
    }

    public override bool Equals(object obj)
    {
        return obj is Tag f && Equals(f);
    }

    public bool Equals(Tag f)
    {
        return f != null && f.Id == Id && f.EntryType == EntryType && Equals(f.EntryId, EntryId);
    }

    public override int GetHashCode()
    {
        return (Id + EntryType + EntryId.ToString()).GetHashCode();
    }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<DbFilesTag, Tag>();
        profile.CreateMap<DbFilesTagLink, Tag>();
    }
}
