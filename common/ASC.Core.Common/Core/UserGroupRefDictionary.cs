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
