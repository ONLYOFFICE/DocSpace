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

namespace ASC.Web.Files.Utils;

[Singletone]
public class FileTrackerHelper
{
    private const string Tracker = "filesTracker";
    private readonly ICache _cache;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<FileTrackerHelper> _logger;
    public static readonly TimeSpan TrackTimeout = TimeSpan.FromSeconds(12);
    public static readonly TimeSpan CacheTimeout = TimeSpan.FromSeconds(60);
    public static readonly TimeSpan CheckRightTimeout = TimeSpan.FromMinutes(1);

    public FileTrackerHelper(ICache cache, IServiceScopeFactory serviceScopeFactory, ILogger<FileTrackerHelper> logger)
    {
        _cache = cache;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }


    public bool ProlongEditing<T>(T fileId, Guid tabId, Guid userId, int tenantId, bool editingAlone = false)
    {
        var checkRight = true;
        var tracker = GetTracker(fileId);
        if (tracker != null && IsEditing(fileId))
        {
            if (tracker.EditingBy.TryGetValue(tabId, out var trackInfo))
            {
                trackInfo.TrackTime = DateTime.UtcNow;
                checkRight = DateTime.UtcNow - tracker.EditingBy[tabId].CheckRightTime > CheckRightTimeout;
            }
            else
            {
                tracker.EditingBy.Add(tabId, new TrackInfo(userId, tabId == userId, editingAlone, tenantId));
            }
        }
        else
        {
            tracker = new FileTracker(tabId, userId, tabId == userId, editingAlone, tenantId);
        }

        SetTracker(fileId, tracker);

        return checkRight;
    }

    public void Remove<T>(T fileId, Guid tabId = default, Guid userId = default)
    {
        var tracker = GetTracker(fileId);
        if (tracker != null)
        {
            if (tabId != default)
            {
                tracker.EditingBy.Remove(tabId);
                SetTracker(fileId, tracker);

                return;
            }
            if (userId != default)
            {
                var listForRemove = tracker.EditingBy
                                           .Where(b => tracker.EditingBy[b.Key].UserId == userId);

                foreach (var editTab in listForRemove)
                {
                    tracker.EditingBy.Remove(editTab.Key);
                }

                SetTracker(fileId, tracker);

                return;
            }
        }

        SetTracker(fileId, null);
    }

    public void RemoveAllOther<T>(Guid userId, T fileId)
    {
        var tracker = GetTracker(fileId);
        if (tracker != null)
        {
            var listForRemove = tracker.EditingBy
                                       .Where(b => b.Value.UserId != userId);

            if (listForRemove.Count() != tracker.EditingBy.Count)
            {
                foreach (var forRemove in listForRemove)
                {
                    tracker.EditingBy.Remove(forRemove.Key);
                }

                SetTracker(fileId, tracker);

                return;
            }
        }
        SetTracker(fileId, null);
    }

    public bool IsEditing<T>(T fileId)
    {
        var tracker = GetTracker(fileId);
        if (tracker != null)
        {
            var listForRemove = tracker.EditingBy
                                       .Where(e => !e.Value.NewScheme && (DateTime.UtcNow - e.Value.TrackTime).Duration() > TrackTimeout);

            foreach (var editTab in listForRemove)
            {
                tracker.EditingBy.Remove(editTab.Key);
            }

            if (tracker.EditingBy.Count == 0)
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

    public bool IsEditingAlone<T>(T fileId)
    {
        var tracker = GetTracker(fileId);

        return tracker != null && tracker.EditingBy.Count == 1 && tracker.EditingBy.FirstOrDefault().Value.EditingAlone;
    }

    public void ChangeRight<T>(T fileId, Guid userId, bool check)
    {
        var tracker = GetTracker(fileId);
        if (tracker != null)
        {
            foreach (var value in tracker.EditingBy.Values)
            {
                if (value.UserId == userId || userId == Guid.Empty)
                {
                    value.CheckRightTime = check ? DateTime.MinValue : DateTime.UtcNow;
                }
            }

            SetTracker(fileId, tracker);
        }
        else
        {
            SetTracker(fileId, null);
        }
    }

    public List<Guid> GetEditingBy<T>(T fileId)
    {
        var tracker = GetTracker(fileId);

        return tracker != null && IsEditing(fileId) ? tracker.EditingBy.Values.Select(i => i.UserId).Distinct().ToList() : new List<Guid>();
    }

    private FileTracker GetTracker<T>(T fileId)
    {
        if (!EqualityComparer<T>.Default.Equals(fileId, default(T)))
        {
            return _cache.Get<FileTracker>(Tracker + fileId);
        }

        return null;
    }

    private void SetTracker<T>(T fileId, FileTracker tracker)
    {
        if (!EqualityComparer<T>.Default.Equals(fileId, default(T)))
        {
            if (tracker != null)
            {
                _cache.Insert(Tracker + fileId, tracker, CacheTimeout, EvictionCallback(fileId, tracker));
            }
            else
            {
                _cache.Remove(Tracker + fileId);
            }
        }
    }

    private Action<object, object, EvictionReason, object> EvictionCallback<T>(T fileId, FileTracker fileTracker)
    {
        return async (key, value, reason, state) =>
        {
            if (reason != EvictionReason.Expired)
            {
                return;
            }

            try
            {
                if (fileTracker.EditingBy == null || !fileTracker.EditingBy.Any())
                {
                    return;
                }

                var editedBy = fileTracker.EditingBy.FirstOrDefault();
                await using var scope = _serviceScopeFactory.CreateAsyncScope();
                var tenantManager = scope.ServiceProvider.GetRequiredService<TenantManager>();
                await tenantManager.SetCurrentTenantAsync(editedBy.Value.TenantId);

                var helper = scope.ServiceProvider.GetRequiredService<DocumentServiceHelper>();
                var tracker = scope.ServiceProvider.GetRequiredService<DocumentServiceTrackerHelper>();
                var daoFactory = scope.ServiceProvider.GetRequiredService<IDaoFactory>();
                var socketManager = scope.ServiceProvider.GetRequiredService<SocketManager>();

                var docKey = await helper.GetDocKeyAsync(await daoFactory.GetFileDao<T>().GetFileAsync(fileId));

                if (await tracker.StartTrackAsync(fileId.ToString(), docKey))
                {
                    _cache.Insert(Tracker + fileId, fileTracker, CacheTimeout, EvictionCallback(fileId, fileTracker));
                }
            }
            catch (Exception e)
            {
                _logger.ErrorWithException(e);
            }
        };
    }
}

public class FileTracker
{

    internal Dictionary<Guid, TrackInfo> EditingBy { get; private set; }

    internal FileTracker(Guid tabId, Guid userId, bool newScheme, bool editingAlone, int tenantId)
    {
        EditingBy = new Dictionary<Guid, TrackInfo> { { tabId, new TrackInfo(userId, newScheme, editingAlone, tenantId) } };
    }


    internal class TrackInfo
    {
        public DateTime CheckRightTime { get; set; }
        public DateTime TrackTime { get; set; }
        public Guid UserId { get; set; }
        public int TenantId { get; set; }
        public bool NewScheme { get; set; }
        public bool EditingAlone { get; set; }

        public TrackInfo() { }

        public TrackInfo(Guid userId, bool newScheme, bool editingAlone, int tenantId)
        {
            CheckRightTime = DateTime.UtcNow;
            TrackTime = DateTime.UtcNow;
            NewScheme = newScheme;
            UserId = userId;
            EditingAlone = editingAlone;
            TenantId = tenantId;
        }
    }
}
