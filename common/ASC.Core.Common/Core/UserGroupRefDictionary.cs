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

namespace ASC.Core;

public class UserGroupRefDictionary : IDictionary<string, UserGroupRef>
{
    private readonly IDictionary<string, UserGroupRef> _dict = new Dictionary<string, UserGroupRef>();
    private IDictionary<Guid, IEnumerable<UserGroupRef>> _byUsers;
    private IDictionary<Guid, IEnumerable<UserGroupRef>> _byGroups;


    public int Count => _dict.Count;

    public bool IsReadOnly => _dict.IsReadOnly;

    public ICollection<string> Keys => _dict.Keys;

    public ICollection<UserGroupRef> Values => _dict.Values;

    public UserGroupRef this[string key]
    {
        get => _dict[key];
        set
        {
            _dict[key] = value;
            BuildIndexes();
        }
    }


    public UserGroupRefDictionary(IDictionary<string, UserGroupRef> dic)
    {
        foreach (var p in dic)
        {
            _dict.Add(p);
        }

        BuildIndexes();
    }


    public void Add(string key, UserGroupRef value)
    {
        _dict.Add(key, value);
        BuildIndexes();
    }

    public void Add(KeyValuePair<string, UserGroupRef> item)
    {
        _dict.Add(item);
        BuildIndexes();
    }

    public bool Remove(string key)
    {
        var result = _dict.Remove(key);
        BuildIndexes();

        return result;
    }

    public bool Remove(KeyValuePair<string, UserGroupRef> item)
    {
        var result = _dict.Remove(item);
        BuildIndexes();

        return result;
    }

    public void Clear()
    {
        _dict.Clear();
        BuildIndexes();
    }


    public bool TryGetValue(string key, out UserGroupRef value)
    {
        return _dict.TryGetValue(key, out value);
    }

    public bool ContainsKey(string key)
    {
        return _dict.ContainsKey(key);
    }

    public bool Contains(KeyValuePair<string, UserGroupRef> item)
    {
        return _dict.Contains(item);
    }

    public void CopyTo(KeyValuePair<string, UserGroupRef>[] array, int arrayIndex)
    {
        _dict.CopyTo(array, arrayIndex);
    }

    public IEnumerator<KeyValuePair<string, UserGroupRef>> GetEnumerator()
    {
        return _dict.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_dict).GetEnumerator();
    }

    public IEnumerable<UserGroupRef> GetByUser(Guid userId)
    {
        return _byUsers.ContainsKey(userId) ? _byUsers[userId].ToList() : new List<UserGroupRef>();
    }

    public IEnumerable<UserGroupRef> GetByGroups(Guid groupId)
    {
        return _byGroups.ContainsKey(groupId) ? _byGroups[groupId].ToList() : new List<UserGroupRef>();
    }

    private void BuildIndexes()
    {
        _byUsers = _dict.Values.GroupBy(r => r.UserId).ToDictionary(g => g.Key, g => g.AsEnumerable());
        _byGroups = _dict.Values.GroupBy(r => r.GroupId).ToDictionary(g => g.Key, g => g.AsEnumerable());
    }
}
