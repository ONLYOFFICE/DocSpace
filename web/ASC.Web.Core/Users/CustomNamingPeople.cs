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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;

using ASC.Common;
using ASC.Core.Common.Settings;
using ASC.Web.Core.PublicResources;

namespace ASC.Web.Core.Users
{
    [Serializable]
    public class PeopleNamesSettings : ISettings
    {
        public Guid ID
        {
            get { return new Guid("47F34957-6A70-4236-9681-C8281FB762FA"); }
        }

        public PeopleNamesItem Item { get; set; }

        public string ItemId { get; set; }

        public ISettings GetDefault(IServiceProvider serviceProvider)
        {
            return new PeopleNamesSettings { ItemId = PeopleNamesItem.DefaultID };
        }
    }

    public class PeopleNamesItem
    {
        private static readonly StringComparison cmp = StringComparison.InvariantCultureIgnoreCase;

        private string schemaName;

        private string userCaption;

        private string usersCaption;

        private string groupCaption;

        private string groupsCaption;

        private string userPostCaption;

        private string groupHeadCaption;

        private string regDateCaption;

        private string guestCaption;

        private string guestsCaption;


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
            get { return Id.Equals(CustomID, cmp) ? schemaName ?? string.Empty : GetResourceValue(schemaName); }
            set { schemaName = value; }
        }

        public string UserCaption
        {
            get { return Id.Equals(CustomID, cmp) ? userCaption ?? string.Empty : GetResourceValue(userCaption); }
            set { userCaption = value; }
        }

        public string UsersCaption
        {
            get { return Id.Equals(CustomID, cmp) ? usersCaption ?? string.Empty : GetResourceValue(usersCaption); }
            set { usersCaption = value; }
        }

        public string GroupCaption
        {
            get { return Id.Equals(CustomID, cmp) ? groupCaption ?? string.Empty : GetResourceValue(groupCaption); }
            set { groupCaption = value; }
        }

        public string GroupsCaption
        {
            get { return Id.Equals(CustomID, cmp) ? groupsCaption ?? string.Empty : GetResourceValue(groupsCaption); }
            set { groupsCaption = value; }
        }

        public string UserPostCaption
        {
            get { return Id.Equals(CustomID, cmp) ? userPostCaption ?? string.Empty : GetResourceValue(userPostCaption); }
            set { userPostCaption = value; }
        }

        public string GroupHeadCaption
        {
            get { return Id.Equals(CustomID, cmp) ? groupHeadCaption ?? string.Empty : GetResourceValue(groupHeadCaption); }
            set { groupHeadCaption = value; }
        }

        public string RegDateCaption
        {
            get { return Id.Equals(CustomID, cmp) ? regDateCaption ?? string.Empty : GetResourceValue(regDateCaption); }
            set { regDateCaption = value; }
        }

        public string GuestCaption
        {
            get { return Id.Equals(CustomID, cmp) ? guestCaption ?? NamingPeopleResource.CommonGuest : GetResourceValue(guestCaption); }
            set { guestCaption = value; }
        }

        public string GuestsCaption
        {
            get { return Id.Equals(CustomID, cmp) ? guestsCaption ?? NamingPeopleResource.CommonGuests : GetResourceValue(guestsCaption); }
            set { guestsCaption = value; }
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
        private static object Locked = new object();
        private static bool loaded = false;

        private static readonly List<PeopleNamesItem> items = new List<PeopleNamesItem>();
        private SettingsManager SettingsManager { get; }

        public CustomNamingPeople(SettingsManager settingsManager)
        {
            SettingsManager = settingsManager;
        }

        public PeopleNamesItem Current
        {
            get
            {
                var settings = SettingsManager.Load<PeopleNamesSettings>();
                return PeopleNamesItem.CustomID.Equals(settings.ItemId, StringComparison.InvariantCultureIgnoreCase) && settings.Item != null ?
                    settings.Item :
                    GetPeopleNames(settings.ItemId);
            }
        }

        public string Substitute<T>(string resourceKey) where T : class
        {
            var text = (string)typeof(T).GetProperty(resourceKey, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).GetValue(null, null);
            return Substitute(text);
        }

        public string Substitute(string text)
        {
            return SubstituteGuest(SubstituteUserPost(SubstituteRegDate(SubstituteGroupHead(SubstitutePost(SubstituteGroup(SubstituteUser(text)))))));
        }

        public Dictionary<string, string> GetSchemas()
        {
            Load();

            var dict = items.ToDictionary(i => i.Id.ToLower(), i => i.SchemaName);
            dict.Add(PeopleNamesItem.CustomID, Resource.CustomNamingPeopleSchema);
            return dict;
        }

        public PeopleNamesItem GetPeopleNames(string schemaId)
        {
            if (PeopleNamesItem.CustomID.Equals(schemaId, StringComparison.InvariantCultureIgnoreCase))
            {
                var settings = SettingsManager.Load<PeopleNamesSettings>();
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

            return items.Find(i => i.Id.Equals(schemaId, StringComparison.InvariantCultureIgnoreCase));
        }

        public void SetPeopleNames(string schemaId)
        {
            var settings = SettingsManager.Load<PeopleNamesSettings>();
            settings.ItemId = schemaId;
            SettingsManager.Save(settings);
        }

        public void SetPeopleNames(PeopleNamesItem custom)
        {
            var settings = SettingsManager.Load<PeopleNamesSettings>();
            custom.Id = PeopleNamesItem.CustomID;
            settings.ItemId = PeopleNamesItem.CustomID;
            settings.Item = custom;
            SettingsManager.Save(settings);
        }


        private void Load()
        {
            if (loaded)
            {
                return;
            }

            lock (Locked)
            {
                if (loaded)
                {
                    return;
                }

                loaded = true;
                var doc = new XmlDocument();
                doc.LoadXml(NamingPeopleResource.PeopleNames);

                items.Clear();
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
                    items.Add(item);
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
}