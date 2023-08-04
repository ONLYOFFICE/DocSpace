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

namespace ASC.Migration.OwnCloud.Models;

public class OCMigratingUser : MigratingUser<OCMigratingContacts, OCMigratingCalendar, OCMigratingFiles, OCMigratingMail>
{
    public override string Email => _userInfo.Email;

    public override string DisplayName => _userInfo.ToString();

    public List<MigrationModules> ModulesList = new List<MigrationModules>();

    public Guid Guid => _userInfo.Id;

    public override string ModuleName => MigrationResource.ModuleNameUsers;

    public string ConnectionString { get; set; }
    private readonly string _rootFolder;
    private bool _hasPhoto;
    private string _pathToPhoto;
    private UserInfo _userInfo;
    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly IDaoFactory _daoFactory;
    private readonly FileStorageService _fileStorageService;
    private readonly TenantManager _tenantManager;
    private readonly UserManager _userManager;
    private readonly OCUser _user;
    private readonly Regex _emailRegex = new Regex(@"(\S*@\S*\.\S*)");

    public OCMigratingUser(
        GlobalFolderHelper globalFolderHelper,
        IDaoFactory daoFactory,
        FileStorageService fileStorageService,
        TenantManager tenantManager,
        UserManager userManager,
        string key,
        OCUser userData,
        string rootFolder,
        Action<string, Exception> log) : base(log)
    {
        Key = key;
        _globalFolderHelper = globalFolderHelper;
        _daoFactory = daoFactory;
        _fileStorageService = fileStorageService;
        _tenantManager = tenantManager;
        _userManager = userManager;
        _user = userData;
        _rootFolder = rootFolder;
    }

    public override void Parse()
    {
        ModulesList.Add(new MigrationModules(ModuleName, MigrationResource.OnlyofficeModuleNamePeople));
        _userInfo = new UserInfo()
        {
            Id = Guid.NewGuid()
        };
        var drivePath = Directory.Exists(Path.Combine(_rootFolder, "data", Key, "cache")) ?
            Path.Combine(_rootFolder, "data", Key, "cache") : null;
        if (drivePath == null)
        {
            _hasPhoto = false;
        }
        else
        {
            _pathToPhoto = File.Exists(Path.Combine(drivePath, "avatar_upload")) ? Directory.GetFiles(drivePath, "avatar_upload")[0] : null;
            _hasPhoto = _pathToPhoto != null ? true : false;
        }

        var userName = _user.Data.DisplayName.Split(' ');
        _userInfo.FirstName = userName[0];
        if (userName.Length > 1)
        {
            _userInfo.LastName = userName[1];
        }
        if (_user.Data.Email != null && _user.Data.Email != "" && _user.Data.Email != "NULL")
        {
            var email = _emailRegex.Match(_user.Data.Email);
            if (email.Success)
            {
                _userInfo.Email = email.Groups[1].Value;
            }
            _userInfo.UserName = _userInfo.Email.Split('@').First();
        }
        _userInfo.ActivationStatus = EmployeeActivationStatus.Pending;
        Action<string, Exception> log = (m, e) => { Log($"{DisplayName} ({Email}): {m}", e); };

        MigratingContacts = new OCMigratingContacts(_tenantManager, this, _user.Addressbooks, log);
        MigratingContacts.Parse();
        if (MigratingContacts.ContactsCount != 0)
        {
            ModulesList.Add(new MigrationModules(MigratingContacts.ModuleName, MigrationResource.OnlyofficeModuleNameMail));
        }

        MigratingCalendar = new OCMigratingCalendar(_user.Calendars, log);
        //MigratingCalendar.Parse();
        if (MigratingCalendar.CalendarsCount != 0)
        {
            ModulesList.Add(new MigrationModules(MigratingCalendar.ModuleName, MigrationResource.OnlyofficeModuleNameCalendar));
        }

        MigratingFiles = new OCMigratingFiles(_globalFolderHelper, _daoFactory, _fileStorageService, this, _user.Storages, _rootFolder, log);
        MigratingFiles.Parse();
        if (MigratingFiles.FoldersCount != 0 || MigratingFiles.FilesCount != 0)
        {
            ModulesList.Add(new MigrationModules(MigratingFiles.ModuleName, MigrationResource.OnlyofficeModuleNameDocuments));
        }

        MigratingMail = new OCMigratingMail(log);
    }

    public void dataСhange(MigratingApiUser frontUser)
    {
        if (_userInfo.Email == null)
        {
            _userInfo.Email = frontUser.Email;
            if (_userInfo.UserName == null)
            {
                _userInfo.UserName = _userInfo.Email.Split('@').First();
            }
        }
        if (_userInfo.LastName == null)
        {
            _userInfo.LastName = "NOTPROVIDED";
        }
    }

    public override async Task MigrateAsync()
    {
        if (string.IsNullOrWhiteSpace(_userInfo.FirstName))
        {
            _userInfo.FirstName = FilesCommonResource.UnknownFirstName;
        }
        if (string.IsNullOrWhiteSpace(_userInfo.LastName))
        {
            _userInfo.LastName = FilesCommonResource.UnknownLastName;
        }

        var saved = await _userManager.GetUserByEmailAsync(_userInfo.Email);
        if (saved != Constants.LostUser)
        {
            saved.ContactsList = saved.ContactsList.Union(_userInfo.ContactsList).ToList();
            _userInfo.Id = saved.Id;
        }
        else
        {
            saved = await _userManager.SaveUserInfo(_userInfo);
        }
        if (_hasPhoto)
        {
            using (var ms = new MemoryStream())
            {
                using (var fs = File.OpenRead(_pathToPhoto))
                {
                    fs.CopyTo(ms);
                }
                await _userManager.SaveUserPhotoAsync(saved.Id, ms.ToArray());
            }
        }
    }
}
