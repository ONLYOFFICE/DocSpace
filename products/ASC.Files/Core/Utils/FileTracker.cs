namespace ASC.Web.Files.Utils;

[Singletone]
public class FileTrackerHelper
{
    private const string Tracker = "filesTracker";
    private readonly ICache _cache;

    public static readonly TimeSpan TrackTimeout = TimeSpan.FromSeconds(12);
    public static readonly TimeSpan CacheTimeout = TimeSpan.FromSeconds(60);
    public static readonly TimeSpan CheckRightTimeout = TimeSpan.FromMinutes(1);

    public FileTrackerHelper(ICache cache)
    {
        _cache = cache;
    }

    public Guid Add<T>(Guid userId, T fileId)
    {
        var tabId = Guid.NewGuid();
        ProlongEditing(fileId, tabId, userId);

        return tabId;
    }

    public bool ProlongEditing<T>(T fileId, Guid tabId, Guid userId, bool editingAlone = false)
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
                tracker.EditingBy.Add(tabId, new TrackInfo(userId, tabId == userId, editingAlone));
            }
        }
        else
        {
            tracker = new FileTracker(tabId, userId, tabId == userId, editingAlone);
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
                _cache.Insert(Tracker + fileId, tracker, CacheTimeout);
            }
            else
            {
                _cache.Remove(Tracker + fileId);
            }
        }
    }
}

public class FileTracker
{

    internal Dictionary<Guid, TrackInfo> EditingBy { get; private set; }

    internal FileTracker(Guid tabId, Guid userId, bool newScheme, bool editingAlone)
    {
        EditingBy = new Dictionary<Guid, TrackInfo> { { tabId, new TrackInfo(userId, newScheme, editingAlone) } };
    }


    internal class TrackInfo
    {
        public DateTime CheckRightTime { get; set; }
        public DateTime TrackTime { get; set; }
        public Guid UserId { get; set; }
        public bool NewScheme { get; set; }
        public bool EditingAlone { get; set; }

        public TrackInfo() { }

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
