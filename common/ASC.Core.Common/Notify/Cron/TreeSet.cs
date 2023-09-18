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

namespace ASC.Notify.Cron;

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
        var inserted = Contains(obj);
        if (!inserted)
        {
            base.Add(obj);
        }

        return !inserted;
    }
}
