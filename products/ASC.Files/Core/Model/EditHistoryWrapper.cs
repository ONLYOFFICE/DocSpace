
using System.Collections.Generic;
using System.Linq;

using ASC.Api.Core;
using ASC.Core;
using ASC.Web.Core.Users;

namespace ASC.Files.Core.Model
{
    public class EditHistoryWrapper
    {
        public int ID { get; set; }
        public string Key { get; set; }
        public int Version { get; set; }
        public int VersionGroup { get; set; }
        public EditHistoryAuthor User { get; set; }
        public ApiDateTime Created { get; set; }
        public string ChangesHistory { get; set; }
        public List<EditHistoryChangesWrapper> Changes { get; set; }
        public string ServerVersion { get; set; }

        public EditHistoryWrapper(EditHistory editHistory, ApiDateTimeHelper apiDateTimeHelper, UserManager userManager, DisplayUserSettingsHelper displayUserSettingsHelper)
        {
            ID = editHistory.ID;
            Key = editHistory.Key;
            Version = editHistory.Version;
            VersionGroup = editHistory.VersionGroup;
            Changes = editHistory.Changes.Select(r => new EditHistoryChangesWrapper(r, apiDateTimeHelper)).ToList();
            ChangesHistory = editHistory.ChangesString;
            Created = apiDateTimeHelper.Get(editHistory.ModifiedOn);
            User = new EditHistoryAuthor(userManager, displayUserSettingsHelper) { Id = editHistory.ModifiedBy };
            ServerVersion = editHistory.ServerVersion;
        }
    }

    public class EditHistoryChangesWrapper
    {
        public EditHistoryAuthor User { get; set; }

        public ApiDateTime Created { get; set; }

        public EditHistoryChangesWrapper(EditHistoryChanges historyChanges, ApiDateTimeHelper apiDateTimeHelper)
        {
            User = historyChanges.Author;
            Created = apiDateTimeHelper.Get(historyChanges.Date);
        }
    }
}
