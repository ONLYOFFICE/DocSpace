namespace ASC.Notify.Cron;

[Serializable]
public class TreeSet : ArrayList, ISortedSet
{
    public IComparer Comparator { get; } = Comparer.Default;

    public TreeSet() { }

    public TreeSet(ICollection c)
    {
        AddAll(c);
    }

    public TreeSet(IComparer c)
    {
        Comparator = c;
    }
    public new bool Add(object obj)
    {
        var inserted = AddWithoutSorting(obj);
        Sort(Comparator);
        return inserted;
    }

    public bool AddAll(ICollection c)
    {
        var e = new ArrayList(c).GetEnumerator();
        var added = false;
        while (e.MoveNext())
        {
            if (AddWithoutSorting(e.Current))
            {
                added = true;
            }
        }
        Sort(Comparator);

        return added;
    }

    public object First()
    {
        return this[0];
    }

    public override bool Contains(object item)
    {
        var tempEnumerator = GetEnumerator();
        while (tempEnumerator.MoveNext())
        {
            if (Comparator.Compare(tempEnumerator.Current, item) == 0)
            {
                return true;
            }
        }

        return false;
    }

    public ISortedSet TailSet(object limit)
    {
        ISortedSet newList = new TreeSet();
        var i = 0;
        while ((i < Count) && (Comparator.Compare(this[i], limit) < 0))
        {
            i++;
        }

        for (; i < Count; i++)
        {
            newList.Add(this[i]);
        }

        return newList;
    }

    public static TreeSet UnmodifiableTreeSet(ICollection collection)
    {
        var items = new ArrayList(collection);
        items = ReadOnly(items);

        return new TreeSet(items);
    }

    private bool AddWithoutSorting(object obj)
    {
        bool inserted = Contains(obj);
        if (!inserted)
        {
            base.Add(obj);
        }

        return !inserted;
    }
}
