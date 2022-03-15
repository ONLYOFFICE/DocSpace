namespace ASC.Web.Core.ModuleManagement.Common
{
    public interface IShortcutProvider
    {
        string GetAbsoluteWebPathForShortcut(Guid shortcutID, string currentUrl);

        bool CheckPermissions(Guid shortcutID, string currentUrl);
    }
}
