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

namespace ASC.Files.Core;

[Transient]
[DebuggerDisplay("{ID} v{Version}")]
public class EditHistory
{
    private readonly ILogger<EditHistory> _logger;
    private readonly TenantUtil _tenantUtil;
    private readonly UserManager _userManager;
    private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;

    public EditHistory(
        ILogger<EditHistory> logger,
        TenantUtil tenantUtil,
        UserManager userManager,
        DisplayUserSettingsHelper displayUserSettingsHelper)
    {
        _logger = logger;
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
                            Id = r.User.Id ?? "",
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
                _logger.ErrorDeSerializeOldScheme(ex);
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

    public string Id { get; set; }

    private string _name;
    public string Name
    {
        get
        {
            if (!Guid.TryParse(Id, out var idInternal))
            {
                return _name;
            }

            UserInfo user;
            return
                idInternal.Equals(Guid.Empty)
                      || idInternal.Equals(ASC.Core.Configuration.Constants.Guest.ID)
                      || (user = _userManager.GetUsers(idInternal)).Equals(Constants.LostUser)
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

/// <summary>
/// </summary>
[DebuggerDisplay("{Version}")]
public class EditHistoryDataDto
{
    /// <summary>URL to the file changes</summary>
    /// <type>System.String, System</type>
    public string ChangesUrl { get; set; }

    /// <summary>Key</summary>
    /// <type>System.String, System</type>
    public string Key { get; set; }

    /// <summary>Previous version</summary>
    /// <type>ASC.Files.Core.EditHistoryUrl, ASC.Files.Core</type>
    public EditHistoryUrl Previous { get; set; }

    /// <summary>Token</summary>
    /// <type>System.String, System</type>
    public string Token { get; set; }

    /// <summary>File URL</summary>
    /// <type>System.String, System</type>
    public string Url { get; set; }

    /// <summary>File version</summary>
    /// <type>System.Int32, System</type>
    public int Version { get; set; }

    /// <summary>File type</summary>
    /// <type>System.String, System</type>
    public string FileType { get; set; }
}

[DebuggerDisplay("{Key} - {Url}")]
public class EditHistoryUrl
{
    public string Key { get; set; }
    public string Url { get; set; }
    public string FileType { get; set; }
}
