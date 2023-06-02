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



namespace ASC.Migration.GoogleWorkspace.Models;

public class GWSMigratingGroups : MigratingGroup
{
    private string _groupName;
    private List<string> _userUidList;
    private readonly UserManager _userManager;
    private readonly string _rootFolder;
    private GroupInfo _groupinfo;
    public Guid Guid => _groupinfo.ID;
    public MigrationModules Module = new MigrationModules();
    public override List<string> UserUidList => _userUidList;
    public override string GroupName => _groupName;
    public override string ModuleName => MigrationResource.ModuleNameGroups;
    public GWSMigratingGroups(UserManager userManager, string rootFolder, Action<string, Exception> log) : base(log)
    {
        _userManager = userManager;
        _rootFolder = rootFolder;
    }

    public override void Parse()
    {
        _userUidList = new List<string>();
        var groupsFolder = Path.Combine(_rootFolder, "Groups");
        var groupInfo = Path.Combine(groupsFolder, "info.csv");
        using (var sr = new StreamReader(groupInfo))
        {
            var line = sr.ReadLine();
            line = sr.ReadLine();
            if (line != null)
            {
                _groupName = line.Split(',')[9];
                if (!string.IsNullOrWhiteSpace(_groupName))
                {
                    _groupinfo = new GroupInfo()
                    {
                        Name = _groupName
                    };
                }
            }
        }
        if (!string.IsNullOrWhiteSpace(_groupinfo.Name))
        {
            var groupMembers = Path.Combine(groupsFolder, "members.csv");
            using (var sr = new StreamReader(groupMembers))
            {
                var line = sr.ReadLine();
                while ((line = sr.ReadLine()) != null)
                {
                    var b = line.Split(',');
                    _userUidList.Add(line.Split(',')[1]);
                }
            }
        }
        if (_userUidList.Count > 0)
        {
            Module = new MigrationModules(ModuleName, MigrationResource.OnlyofficeModuleNamePeople);
        }
    }

    public override async Task MigrateAsync()
    {
        var existingGroups = (await _userManager.GetGroupsAsync()).ToList();
        var oldGroup = existingGroups.Find(g => g.Name == _groupinfo.Name);
        if (oldGroup != null)
        {
            _groupinfo = oldGroup;
        }
        else
        {
            _groupinfo = await _userManager.SaveGroupInfoAsync(_groupinfo);
        }
        foreach (var userEmail in _userUidList)
        {
            UserInfo user;
            try
            {
                user = await _userManager.GetUserByEmailAsync(userEmail);
                if (user == Constants.LostUser)
                {
                    throw new ArgumentNullException();
                }
                if (!await _userManager.IsUserInGroupAsync(user.Id, _groupinfo.ID))
                {
                    await _userManager.AddUserIntoGroupAsync(user.Id, _groupinfo.ID);
                }
            }
            catch (Exception ex)
            {
                //Think about the text of the error
                Log($"Couldn't to add user {userEmail} to group {_groupName} {Path.Combine(_rootFolder, "Groups")} ", ex);
            }
        }
    }
}
