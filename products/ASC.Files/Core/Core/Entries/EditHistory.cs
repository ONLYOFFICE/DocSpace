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
public class EditHistoryData
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
