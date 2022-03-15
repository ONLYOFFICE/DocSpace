namespace ASC.Web.Studio.Core.Backup;

[Scope]
public class BackupFileUploadHandler
{
    private const long MaxBackupFileSize = 1024L * 1024L * 1024L;
    private const string BackupTempFolder = "backup";
    private const string BackupFileName = "backup.tmp";

    private readonly PermissionContext _permissionContext;
    private readonly TempPath _tempPath;
    private readonly TenantManager _tenantManager;

    public BackupFileUploadHandler(
        PermissionContext permissionContext,
        TempPath tempPath,
        TenantManager tenantManager)
    {
        _permissionContext = permissionContext;
        _tempPath = tempPath;
        _tenantManager = tenantManager;
    }

    public string ProcessUpload(IFormFile file)
    {
        if (file == null)
        {
            return "No files.";
        }

        if (!_permissionContext.CheckPermissions(SecutiryConstants.EditPortalSettings))
        {
            return "Access denied.";
        }

        if (file.Length <= 0 || file.Length > MaxBackupFileSize)
        {
            return $"File size must be greater than 0 and less than {MaxBackupFileSize} bytes";
        }

        try
        {
            var filePath = GetFilePath();

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            using (var fileStream = File.Create(filePath))
            {
                file.CopyTo(fileStream);
            }

            return string.Empty;
        }
        catch (Exception error)
        {
            return error.Message;
        }
    }

    internal string GetFilePath()
    {
        var folder = Path.Combine(_tempPath.GetTempPath(), BackupTempFolder, _tenantManager.GetCurrentTenant().Id.ToString());

        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        return Path.Combine(folder, BackupFileName);
    }
}
