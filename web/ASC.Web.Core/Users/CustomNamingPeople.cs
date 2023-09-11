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

namespace ASC.Web.Core.Users;

public class PeopleNamesSettings : ISettings<PeopleNamesSettings>
{
    [JsonIgnore]
    public Guid ID
    {
        get { return new Guid("47F34957-6A70-4236-9681-C8281FB762FA"); }
    }

    public PeopleNamesItem Item { get; set; }

    public string ItemId { get; set; }

    public PeopleNamesSettings GetDefault()
    {
        return new PeopleNamesSettings { ItemId = PeopleNamesItem.DefaultID };
    }
}

public class PeopleNamesItem
{
    private static readonly StringComparison _cmp = StringComparison.InvariantCultureIgnoreCase;

    private string _schemaName;

    private string _userCaption;

    private string _usersCaption;

    private string _groupCaption;

    private string _groupsCaption;

    private string _userPostCaption;

    private string _groupHeadCaption;

    private string _regDateCaption;

    private string _guestCaption;

    private string _guestsCaption;


    public static string DefaultID
    {
        get { return "common"; }
    }

    public static string CustomID
    {
        get { return "custom"; }
    }

    public string Id { get; set; }

    public string SchemaName
    {
        get { return Id.Equals(CustomID, _cmp) ? _schemaName ?? string.Empty : GetResourceValue(_schemaName); }
        set { _schemaName = value; }
    }

    public string UserCaption
    {
        get { return Id.Equals(CustomID, _cmp) ? _userCaption ?? string.Empty : GetResourceValue(_userCaption); }
        set { _userCaption = value; }
    }

    public string UsersCaption
    {
        get { return Id.Equals(CustomID, _cmp) ? _usersCaption ?? string.Empty : GetResourceValue(_usersCaption); }
        set { _usersCaption = value; }
    }

    public string GroupCaption
    {
        get { return Id.Equals(CustomID, _cmp) ? _groupCaption ?? string.Empty : GetResourceValue(_groupCaption); }
        set { _groupCaption = value; }
    }

    public string GroupsCaption
    {
        get { return Id.Equals(CustomID, _cmp) ? _groupsCaption ?? string.Empty : GetResourceValue(_groupsCaption); }
        set { _groupsCaption = value; }
    }

    public string UserPostCaption
    {
        get { return Id.Equals(CustomID, _cmp) ? _userPostCaption ?? string.Empty : GetResourceValue(_userPostCaption); }
        set { _userPostCaption = value; }
    }

    public string GroupHeadCaption
    {
        get { return Id.Equals(CustomID, _cmp) ? _groupHeadCaption ?? string.Empty : GetResourceValue(_groupHeadCaption); }
        set { _groupHeadCaption = value; }
    }

    public string RegDateCaption
    {
        get { return Id.Equals(CustomID, _cmp) ? _regDateCaption ?? string.Empty : GetResourceValue(_regDateCaption); }
        set { _regDateCaption = value; }
    }

    public string GuestCaption
    {
        get { return Id.Equals(CustomID, _cmp) ? _guestCaption ?? NamingPeopleResource.CommonGuest : GetResourceValue(_guestCaption); }
        set { _guestCaption = value; }
    }

    public string GuestsCaption
    {
        get { return Id.Equals(CustomID, _cmp) ? _guestsCaption ?? NamingPeopleResource.CommonGuests : GetResourceValue(_guestsCaption); }
        set { _guestsCaption = value; }
    }

    private static string GetResourceValue(string resourceKey)
    {
        if (string.IsNullOrEmpty(resourceKey))
        {
            return string.Empty;
        }
        return (string)typeof(NamingPeopleResource).GetProperty(resourceKey, BindingFlags.Static | BindingFlags.Public).GetValue(null, null);
    }
}

[Scope]
public class CustomNamingPeople
{
    private static readonly object _locked = new object();
    private static bool _loaded;

    private static readonly List<PeopleNamesItem> _items = new List<PeopleNamesItem>();
    private readonly SettingsManager _settingsManager;

    public CustomNamingPeople(SettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
    }

    public PeopleNamesItem Current
    {
        get
        {
            var settings = _settingsManager.Load<PeopleNamesSettings>();
            return PeopleNamesItem.CustomID.Equals(settings.ItemId, StringComparison.InvariantCultureIgnoreCase) && settings.Item != null ?
                settings.Item :
                GetPeopleNames(settings.ItemId);
        }
    }

    public string Substitute<T>(string resourceKey) where T : class
    {
        var prop = typeof(T).GetProperty(resourceKey, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        if (prop != null)
        {
            var text = (string)prop.GetValue(null, null);
            return Substitute(text);
        }
        return null;
    }

    public string Substitute(string text)
    {
        return SubstituteGuest(SubstituteUserPost(SubstituteRegDate(SubstituteGroupHead(SubstitutePost(SubstituteGroup(SubstituteUser(text)))))));
    }

    public Dictionary<string, string> GetSchemas()
    {
        Load();

        var dict = _items.ToDictionary(i => i.Id.ToLower(), i => i.SchemaName);
        dict.Add(PeopleNamesItem.CustomID, Resource.CustomNamingPeopleSchema);
        return dict;
    }

    public async Task<PeopleNamesItem> GetPeopleNamesAsync(string schemaId)
    {
        if (PeopleNamesItem.CustomID.Equals(schemaId, StringComparison.InvariantCultureIgnoreCase))
        {
            var settings = await _settingsManager.LoadAsync<PeopleNamesSettings>();
            var result = settings.Item;
            if (result == null)
            {
                result = new PeopleNamesItem
                {
                    Id = PeopleNamesItem.CustomID,
                    GroupCaption = string.Empty,
                    GroupHeadCaption = string.Empty,
                    GroupsCaption = string.Empty,
                    RegDateCaption = string.Empty,
                    UserCaption = string.Empty,
                    UserPostCaption = string.Empty,
                    UsersCaption = string.Empty,
                    GuestCaption = string.Empty,
                    GuestsCaption = string.Empty
                };
            }

            result.SchemaName = Resource.CustomNamingPeopleSchema;

            return result;
        }

        Load();

        return _items.Find(i => i.Id.Equals(schemaId, StringComparison.InvariantCultureIgnoreCase));
    }

    public PeopleNamesItem GetPeopleNames(string schemaId)
    {
        if (PeopleNamesItem.CustomID.Equals(schemaId, StringComparison.InvariantCultureIgnoreCase))
        {
            var settings = _settingsManager.Load<PeopleNamesSettings>();
            var result = settings.Item;
            if (result == null)
            {
                result = new PeopleNamesItem
                {
                    Id = PeopleNamesItem.CustomID,
                    GroupCaption = string.Empty,
                    GroupHeadCaption = string.Empty,
                    GroupsCaption = string.Empty,
                    RegDateCaption = string.Empty,
                    UserCaption = string.Empty,
                    UserPostCaption = string.Empty,
                    UsersCaption = string.Empty,
                    GuestCaption = string.Empty,
                    GuestsCaption = string.Empty
                };
            }

            result.SchemaName = Resource.CustomNamingPeopleSchema;

            return result;
        }

        Load();

        return _items.Find(i => i.Id.Equals(schemaId, StringComparison.InvariantCultureIgnoreCase));
    }

    public async Task SetPeopleNamesAsync(string schemaId)
    {
        var settings = await _settingsManager.LoadAsync<PeopleNamesSettings>();
        settings.ItemId = schemaId;
        await _settingsManager.SaveAsync(settings);
    }

    public async Task SetPeopleNamesAsync(PeopleNamesItem custom)
    {
        var settings = await _settingsManager.LoadAsync<PeopleNamesSettings>();
        custom.Id = PeopleNamesItem.CustomID;
        settings.ItemId = PeopleNamesItem.CustomID;
        settings.Item = custom;
        await _settingsManager.SaveAsync(settings);
    }


    private void Load()
    {
        if (_loaded)
        {
            return;
        }

        lock (_locked)
        {
            if (_loaded)
            {
                return;
            }

            _loaded = true;
            var doc = new XmlDocument();
            doc.LoadXml(NamingPeopleResource.PeopleNames);

            _items.Clear();
            foreach (XmlNode node in doc.SelectNodes("/root/item"))
            {
                var item = new PeopleNamesItem
                {
                    Id = node.SelectSingleNode("id").InnerText,
                    SchemaName = node.SelectSingleNode("names/schemaname").InnerText,
                    GroupHeadCaption = node.SelectSingleNode("names/grouphead").InnerText,
                    GroupCaption = node.SelectSingleNode("names/group").InnerText,
                    GroupsCaption = node.SelectSingleNode("names/groups").InnerText,
                    UserCaption = node.SelectSingleNode("names/user").InnerText,
                    UsersCaption = node.SelectSingleNode("names/users").InnerText,
                    UserPostCaption = node.SelectSingleNode("names/userpost").InnerText,
                    RegDateCaption = node.SelectSingleNode("names/regdate").InnerText,
                    GuestCaption = node.SelectSingleNode("names/guest").InnerText,
                    GuestsCaption = node.SelectSingleNode("names/guests").InnerText,
                };
                _items.Add(item);
            }
        }
    }

    private string SubstituteUser(string text)
    {
        var item = Current;
        if (item != null)
        {
            return text
                .Replace("{!User}", item.UserCaption)
                .Replace("{!user}", item.UserCaption.ToLower())
                .Replace("{!Users}", item.UsersCaption)
                .Replace("{!users}", item.UsersCaption.ToLower());
        }
        return text;
    }

    private string SubstituteGroup(string text)
    {
        var item = Current;
        if (item != null)
        {
            return text
                .Replace("{!Group}", item.GroupCaption)
                .Replace("{!group}", item.GroupCaption.ToLower())
                .Replace("{!Groups}", item.GroupsCaption)
                .Replace("{!groups}", item.GroupsCaption.ToLower());
        }
        return text;
    }

    private string SubstituteGuest(string text)
    {
        var item = Current;
        if (item != null)
        {
            return text
                .Replace("{!Guest}", item.GuestCaption)
                .Replace("{!guest}", item.GuestCaption.ToLower())
                .Replace("{!Guests}", item.GuestsCaption)
                .Replace("{!guests}", item.GuestsCaption.ToLower());
        }
        return text;
    }

    private string SubstitutePost(string text)
    {
        var item = Current;
        if (item != null)
        {
            return text
                .Replace("{!Post}", item.UserPostCaption)
                .Replace("{!post}", item.UserPostCaption.ToLower());
        }
        return text;
    }

    private string SubstituteGroupHead(string text)
    {
        var item = Current;
        if (item != null)
        {
            return text
                .Replace("{!Head}", item.GroupHeadCaption)
                .Replace("{!head}", item.GroupHeadCaption.ToLower());
        }
        return text;
    }

    private string SubstituteRegDate(string text)
    {
        var item = Current;
        if (item != null)
        {
            return text
                .Replace("{!Regdate}", item.RegDateCaption)
                .Replace("{!regdate}", item.RegDateCaption.ToLower());
        }
        return text;
    }

    private string SubstituteUserPost(string text)
    {
        var item = Current;
        if (item != null)
        {
            return text
                .Replace("{!Userpost}", item.UserPostCaption)
                .Replace("{!userpost}", item.UserPostCaption.ToLower());
        }
        return text;
    }
}
