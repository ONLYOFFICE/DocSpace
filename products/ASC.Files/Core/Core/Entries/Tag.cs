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

    public Tag() { }

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
