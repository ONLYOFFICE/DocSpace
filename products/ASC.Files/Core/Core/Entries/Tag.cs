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
using System.Diagnostics;

namespace ASC.Files.Core
{
    [Flags]
    public enum TagType
    {
        New = 1,
        Favorite = 2,
        System = 4,
        Locked = 8,
        Recent = 16,
        Template = 32,
    }

    [Serializable]
    [DebuggerDisplay("{TagName} ({Id}) entry {EntryType} ({EntryId})")]
    public sealed class Tag
    {
        public string TagName { get; set; }

        public TagType TagType { get; set; }

        public Guid Owner { get; set; }

        public object EntryId { get; set; }

        public FileEntryType EntryType { get; set; }

        public int Id { get; set; }

        public int Count { get; set; }


        public Tag()
        {
        }

        public Tag(string name, TagType type, Guid owner)
            : this(name, type, owner, 0)
        {
        }

        public Tag(string name, TagType type, Guid owner, int count)
        {
            TagName = name;
            TagType = type;
            Owner = owner;
            Count = count;
        }

        public Tag AddEntry<T>(FileEntry<T> entry)
        {
            if (entry != null)
            {
                EntryId = entry.ID;
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


        public override bool Equals(object obj)
        {
            return obj is Tag f && Equals(f);
        }
        public bool Equals(Tag f)
        {
            return f.Id == Id && f.EntryType == EntryType && Equals(f.EntryId, EntryId);
        }

        public override int GetHashCode()
        {
            return (Id + EntryType + EntryId.ToString()).GetHashCode();
        }
    }
}