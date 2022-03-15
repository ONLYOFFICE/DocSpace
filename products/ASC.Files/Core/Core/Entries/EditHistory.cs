namespace ASC.Files.Core;

[Transient]
[DebuggerDisplay("{ID} v{Version}")]
public class EditHistory
{
    private readonly ILog _logger;
    private readonly TenantUtil _tenantUtil;
    private readonly UserManager _userManager;
    private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;

    public EditHistory(
        IOptionsMonitor<ILog> options,
        TenantUtil tenantUtil,
        UserManager userManager,
        DisplayUserSettingsHelper displayUserSettingsHelper)
    {
        _logger = options.CurrentValue;
        _tenantUtil = tenantUtil;
        _userManager = userManager;
        _displayUserSettingsHelper = displayUserSettingsHelper;
    }

    public int ID { get; set; }
    public string Key { get; set; }
    public int Version { get; set; }
    public int VersionGroup { get; set; }
    public DateTime ModifiedOn { get; set; }
    public Guid ModifiedBy { get; set; }
    public string ChangesString { get; set; }
    public string ServerVersion { get; set; }

    public List<EditHistoryChanges> Changes
    {
        get
        {
            var changes = new List<EditHistoryChanges>();
            if (string.IsNullOrEmpty(ChangesString))
            {
                return changes;
            }

            try
            {
                var options = new JsonSerializerOptions
                {
                    AllowTrailingCommas = true,
                    PropertyNameCaseInsensitive = true
                };

                var jObject = JsonSerializer.Deserialize<ChangesDataList>(ChangesString, options);
                ServerVersion = jObject.ServerVersion;

                if (string.IsNullOrEmpty(ServerVersion))
                {
                    return changes;
                }

                changes = jObject.Changes.Select(r =>
                {
                    var result = new EditHistoryChanges()
                    {
                        Author = new EditHistoryAuthor(_userManager, _displayUserSettingsHelper)
                        {
                            Id = new Guid(r.User.Id ?? Guid.Empty.ToString()),
                            Name = r.User.Name,
                        }
                    };


                    if (DateTime.TryParse(r.Created, out var _date))
                    {
                        _date = _tenantUtil.DateTimeFromUtc(_date);
                    }
                    result.Date = _date;

                    return result;
                })
                .ToList();

                return changes;
            }
            catch (Exception ex)
            {
                _logger.Error("DeSerialize old scheme exception", ex);
            }

            return changes;
        }
        set => throw new NotImplementedException();
    }
}

class ChangesDataList
{
    public string ServerVersion { get; set; }
    public ChangesData[] Changes { get; set; }
}

class ChangesData
{
    public string Created { get; set; }
    public ChangesUserData User { get; set; }
}

class ChangesUserData
{
    public string Id { get; set; }
    public string Name { get; set; }
}

[Transient]
[DebuggerDisplay("{Id} {Name}")]
public class EditHistoryAuthor
{
    private readonly UserManager _userManager;
    private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;

    public EditHistoryAuthor(
        UserManager userManager,
        DisplayUserSettingsHelper displayUserSettingsHelper)
    {
        _userManager = userManager;
        _displayUserSettingsHelper = displayUserSettingsHelper;
    }

    public Guid Id { get; set; }

    private string _name;
    public string Name
    {
        get
        {
            UserInfo user;
            return
                Id.Equals(Guid.Empty)
                      || Id.Equals(ASC.Core.Configuration.Constants.Guest.ID)
                      || (user = _userManager.GetUsers(Id)).Equals(Constants.LostUser)
                          ? string.IsNullOrEmpty(_name)
                                ? FilesCommonResource.Guest
                                : _name
                          : user.DisplayUserName(false, _displayUserSettingsHelper);
        }
        set => _name = value;
    }
}

[DebuggerDisplay("{Author.Name}")]
public class EditHistoryChanges
{
    public EditHistoryAuthor Author { get; set; }
    public DateTime Date { get; set; }
}

[DebuggerDisplay("{Version}")]
public class EditHistoryDataDto
{
    public string ChangesUrl { get; set; }
    public string Key { get; set; }
    public EditHistoryUrl Previous { get; set; }
    public string Token { get; set; }
    public string Url { get; set; }
    public int Version { get; set; }
    public string FileType { get; set; }
}

[DebuggerDisplay("{Key} - {Url}")]
public class EditHistoryUrl
{
    public string Key { get; set; }
    public string Url { get; set; }
    public string FileType { get; set; }
}
