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
using System.Runtime.Serialization;

using ASC.Common.Caching;

namespace ASC.Web.Files.Utils
{
    public class FileTracker
    {
        private const string TRACKER = "filesTracker";
        private static readonly ICache cache = AscCache.Memory;

        public static readonly TimeSpan TrackTimeout = TimeSpan.FromSeconds(12);
        public static readonly TimeSpan CacheTimeout = TimeSpan.FromSeconds(60);
        public static readonly TimeSpan CheckRightTimeout = TimeSpan.FromMinutes(1);

        private readonly Dictionary<Guid, TrackInfo> _editingBy;


        private FileTracker()
        {
        }

        private FileTracker(Guid tabId, Guid userId, bool newScheme, bool editingAlone)
        {
            _editingBy = new Dictionary<Guid, TrackInfo> { { tabId, new TrackInfo(userId, newScheme, editingAlone) } };
        }


        public static Guid Add<T>(Guid userId, T fileId)
        {
            var tabId = Guid.NewGuid();
            ProlongEditing(fileId, tabId, userId);
            return tabId;
        }

        public static bool ProlongEditing<T>(T fileId, Guid tabId, Guid userId, bool editingAlone = false)
        {
            var checkRight = true;
            var tracker = GetTracker(fileId);
            if (tracker != null && IsEditing(fileId))
            {
                if (tracker._editingBy.Keys.Contains(tabId))
                {
                    tracker._editingBy[tabId].TrackTime = DateTime.UtcNow;
                    checkRight = (DateTime.UtcNow - tracker._editingBy[tabId].CheckRightTime > CheckRightTimeout);
                }
                else
                {
                    tracker._editingBy.Add(tabId, new TrackInfo(userId, tabId == userId, editingAlone));
                }
            }
            else
            {
                tracker = new FileTracker(tabId, userId, tabId == userId, editingAlone);
            }

            SetTracker(fileId, tracker);

            return checkRight;
        }

        public static void Remove<T>(T fileId, Guid tabId = default, Guid userId = default)
        {
            var tracker = GetTracker(fileId);
            if (tracker != null)
            {
                if (tabId != default)
                {
                    tracker._editingBy.Remove(tabId);
                    SetTracker(fileId, tracker);
                    return;
                }
                if (userId != default)
                {
                    var listForRemove = tracker._editingBy
                                               .Where(b => tracker._editingBy[b.Key].UserId == userId)
                                               .ToList();
                    foreach (var editTab in listForRemove)
                    {
                        tracker._editingBy.Remove(editTab.Key);
                    }
                    SetTracker(fileId, tracker);
                    return;
                }
            }

            SetTracker(fileId, null);
        }

        public static void RemoveAllOther<T>(Guid userId, T fileId)
        {
            var tracker = GetTracker(fileId);
            if (tracker != null)
            {
                var listForRemove = tracker._editingBy
                                           .Where(b => b.Value.UserId != userId)
                                           .ToList();
                if (listForRemove.Count() != tracker._editingBy.Count)
                {
                    foreach (var forRemove in listForRemove)
                    {
                        tracker._editingBy.Remove(forRemove.Key);
                    }
                    SetTracker(fileId, tracker);
                    return;
                }
            }
            SetTracker(fileId, null);
        }

        public static bool IsEditing<T>(T fileId)
        {
            var tracker = GetTracker(fileId);
            if (tracker != null)
            {
                var listForRemove = tracker._editingBy
                                           .Where(e => !e.Value.NewScheme && (DateTime.UtcNow - e.Value.TrackTime).Duration() > TrackTimeout)
                                           .ToList();
                foreach (var editTab in listForRemove)
                {
                    tracker._editingBy.Remove(editTab.Key);
                }

                if (tracker._editingBy.Count == 0)
                {
                    SetTracker(fileId, null);
                    return false;
                }

                SetTracker(fileId, tracker);
                return true;
            }
            SetTracker(fileId, null);
            return false;
        }
        public static bool IsEditingAlone<T>(T fileId)
        {
            var tracker = GetTracker(fileId);
            return tracker != null && tracker._editingBy.Count == 1 && tracker._editingBy.FirstOrDefault().Value.EditingAlone;
        }

        public static void ChangeRight<T>(T fileId, Guid userId, bool check)
        {
            var tracker = GetTracker(fileId);
            if (tracker != null)
            {

                tracker._editingBy.Values
                       .ToList()
                       .ForEach(i =>
                           {
                               if (i.UserId == userId || userId == Guid.Empty)
                               {
                                   i.CheckRightTime = check ? DateTime.MinValue : DateTime.UtcNow;
                               }
                           });
                SetTracker(fileId, tracker);
            }
            else
            {
                SetTracker(fileId, null);
            }
        }

        public static List<Guid> GetEditingBy<T>(T fileId)
        {
            var tracker = GetTracker(fileId);
            return tracker != null && IsEditing(fileId) ? tracker._editingBy.Values.Select(i => i.UserId).Distinct().ToList() : new List<Guid>();
        }

        private static FileTracker GetTracker<T>(T fileId)
        {
            if (!fileId.Equals(default(T)))
            {
                return cache.Get<FileTracker>(TRACKER + fileId);
            }
            return null;
        }

        private static void SetTracker<T>(T fileId, FileTracker tracker)
        {
            if (!fileId.Equals(default(T)))
            {
                if (tracker != null)
                {
                    cache.Insert(TRACKER + fileId, tracker, CacheTimeout);
                }
                else
                {
                    cache.Remove(TRACKER + fileId);
                }
            }
        }


        
        internal class TrackInfo
        {
            public DateTime CheckRightTime;

            public DateTime TrackTime;

            public Guid UserId;

            public bool NewScheme;

            public bool EditingAlone;

            public TrackInfo()
            {
            }

            public TrackInfo(Guid userId, bool newScheme, bool editingAlone)
            {
                CheckRightTime = DateTime.UtcNow;
                TrackTime = DateTime.UtcNow;
                NewScheme = newScheme;
                UserId = userId;
                EditingAlone = editingAlone;
            }
        }
    }
}