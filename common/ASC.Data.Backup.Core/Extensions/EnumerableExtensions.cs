namespace ASC.Data.Backup.Extensions;

public class TreeNode<TEntry>
{
    public TEntry Entry { get; set; }
    public TreeNode<TEntry> Parent { get; set; }
    public List<TreeNode<TEntry>> Children { get; private set; }

    public TreeNode()
    {
        Children = new List<TreeNode<TEntry>>();
    }

    public TreeNode(TEntry entry)
        : this()
    {
        Entry = entry;
        Parent = null;
    }
}

public static class EnumerableExtensions
{
    public static IEnumerable<TreeNode<TEntry>> ToTree<TEntry, TKey>(this IEnumerable<TEntry> elements,
                                                                     Func<TEntry, TKey> keySelector,
                                                                     Func<TEntry, TKey> parentKeySelector)
    {
        ArgumentNullException.ThrowIfNull(elements);
        ArgumentNullException.ThrowIfNull(keySelector);
        ArgumentNullException.ThrowIfNull(parentKeySelector);

        var dic = elements.ToDictionary(keySelector, x => new TreeNode<TEntry>(x));

        foreach (var keyValue in dic)
        {
            var parentKey = parentKeySelector(keyValue.Value.Entry);
            if (parentKey != null && dic.TryGetValue(parentKeySelector(keyValue.Value.Entry), out var parent))
            {
                parent.Children.Add(keyValue.Value);
                keyValue.Value.Parent = parent;
            }
        }

        return dic.Values.Where(x => x.Parent == null);
    }

    public static IEnumerable<IEnumerable<TEntry>> MakeParts<TEntry>(this IEnumerable<TEntry> collection, int partLength)
    {
        ArgumentNullException.ThrowIfNull(collection);

        if (partLength <= 0)
            throw new ArgumentOutOfRangeException(nameof(partLength), partLength, "Length must be positive integer");

        return MakePartsIterator(collection, partLength);
    }

    private static IEnumerable<IEnumerable<TEntry>> MakePartsIterator<TEntry>(this IEnumerable<TEntry> collection, int partLength)
    {
        var part = new List<TEntry>(partLength);

        foreach (var entry in collection)
        {
            part.Add(entry);

            if (part.Count == partLength)
            {
                yield return part.AsEnumerable();
                part = new List<TEntry>(partLength);
            }
        }

        if (part.Count > 0)
        {
            yield return part.AsEnumerable();
        }
    }
}
