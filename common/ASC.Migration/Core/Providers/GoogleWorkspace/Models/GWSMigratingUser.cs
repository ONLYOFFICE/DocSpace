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

public class GwsMigratingUser : MigratingUser<GwsMigratingContacts, GwsMigratingCalendar, GwsMigratingFiles, GwsMigratingMail>
{
    public override string Email => _userInfo.Email;
    public Guid Guid => _userInfo.Id;
    public List<MigrationModules> ModulesList = new List<MigrationModules>();
    public override string ModuleName => MigrationResource.ModuleNameUsers;

    public override string DisplayName => $"{_userInfo.FirstName} {_userInfo.LastName}".Trim();

    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly IDaoFactory _daoFactory;
    private readonly FileSecurity _fileSecurity;
    private readonly FileStorageService _fileStorageService;
    private readonly UserManager _userManager;

    public GwsMigratingUser(
        GlobalFolderHelper globalFolderHelper,
        IDaoFactory daoFactory,
        FileSecurity fileSecurity,
        FileStorageService fileStorageService,
        UserManager userManager)
    {
        _globalFolderHelper = globalFolderHelper;
        _daoFactory = daoFactory;
        _fileSecurity = fileSecurity;
        _fileStorageService = fileStorageService;
        _userManager = userManager;
    }

    public override void Parse()
    {
        ModulesList.Add(new MigrationModules(ModuleName, MigrationResource.OnlyofficeModuleNamePeople));
        _userInfo = new UserInfo()
        {
            Id = Guid.NewGuid()
        };

        ParseRootHtml();
        ParseProfile();
        ParseAccount();

        Action<string, Exception> log = (m, e) => { Log($"{DisplayName} ({Email}): {m}", e); };

        MigratingContacts = new GwsMigratingContacts(_rootFolder, this, log);
        MigratingContacts.Parse();
        if (MigratingContacts.ContactsCount != 0)
        {
            ModulesList.Add(new MigrationModules(MigratingContacts.ModuleName, MigrationResource.OnlyofficeModuleNameMail));
        }

        MigratingCalendar = new GwsMigratingCalendar(_rootFolder, log);
        //MigratingCalendar.Parse();
        if (MigratingCalendar.CalendarsCount != 0)
        {
            ModulesList.Add(new MigrationModules(MigratingCalendar.ModuleName, MigrationResource.OnlyofficeModuleNameCalendar));
        }

        MigratingFiles = new GwsMigratingFiles(_globalFolderHelper, _daoFactory, _fileSecurity, _fileStorageService, _rootFolder, this, log);
        MigratingFiles.Parse();
        if (MigratingFiles.FoldersCount != 0 || MigratingFiles.FilesCount != 0)
        {
            ModulesList.Add(new MigrationModules(MigratingFiles.ModuleName, MigrationResource.OnlyofficeModuleNameDocuments));
        }

        MigratingMail = new GwsMigratingMail(_rootFolder, this, log);
        //MigratingMail.Parse();

        _userInfo.UserName = _userInfo.Email.Split('@').First();
        if (_userInfo.FirstName == null || _userInfo.FirstName == "")
        {
            _userInfo.FirstName = _userInfo.Email.Split('@').First();
        }
        _userInfo.ActivationStatus = EmployeeActivationStatus.Pending;
    }

    public void DataСhange(MigratingApiUser frontUser)
    {
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
            using (var fs = File.OpenRead(Key))
            {
                using (var zip = new ZipArchive(fs))
                {
                    using (var ms = new MemoryStream())
                    {
                        using (var imageStream = zip.GetEntry(string.Join("/", "Takeout", "Profile", "ProfilePhoto.jpg")).Open())
                        {
                            imageStream.CopyTo(ms);
                        }
                        await _userManager.SaveUserPhotoAsync(saved.Id, ms.ToArray());
                    }
                }
            }
        }
    }

    public GwsMigratingUser(string key, string rootFolder, Action<string, Exception> log) : base(log)
    {
        Key = key;
        this._rootFolder = rootFolder;
    }

    private readonly Regex _emailRegex = new Regex(@"(\S*@\S*\.\S*)");
    private readonly Regex _phoneRegex = new Regex(@"(\+?\d+)");

    private readonly string _rootFolder;
    private UserInfo _userInfo;
    private bool _hasPhoto;

    private void ParseRootHtml()
    {
        var htmlFiles = Directory.GetFiles(_rootFolder, "*.html");
        if (htmlFiles.Count() != 1)
        {
            throw new Exception("Incorrect Takeout format.");
        }

        var htmlPath = htmlFiles[0];

        var doc = new HtmlDocument();
        doc.Load(htmlPath);

        var emailNode = doc.DocumentNode.SelectNodes("//body//h1[@class='header_title']")[0];
        var matches = _emailRegex.Match(emailNode.InnerText);
        if (!matches.Success)
        {
            throw new Exception("Couldn't parse root html.");
        }

        _userInfo.Email = matches.Groups[1].Value;
    }

    private void ParseProfile()
    {
        var profilePath = Path.Combine(_rootFolder, "Profile", "Profile.json");
        if (!File.Exists(profilePath))
        {
            return;
        }

        var googleProfile = JsonConvert.DeserializeObject<GwsProfile>(File.ReadAllText(profilePath));

        if (googleProfile.Birthday != null)
        {
            _userInfo.BirthDate = googleProfile.Birthday.Value.DateTime;
        }

        if (googleProfile.Gender != null)
        {
            switch (googleProfile.Gender.Type)
            {
                case "male":
                    _userInfo.Sex = true;
                    break;

                case "female":
                    _userInfo.Sex = false;
                    break;

                default:
                    _userInfo.Sex = null;
                    break;
            }
        }

        _userInfo.FirstName = googleProfile.Name.GivenName;
        _userInfo.LastName = googleProfile.Name.FamilyName;

        if (googleProfile.Emails != null)
        {
            foreach (var email in googleProfile.Emails.Distinct())
            {
                AddEmailToUser(_userInfo, email.Value);
            }
        }

        var profilePhotoPath = Path.Combine(_rootFolder, "Profile", "ProfilePhoto.jpg");
        _hasPhoto = File.Exists(profilePhotoPath);
    }

    private void ParseAccount()
    {
        var accountPath = Path.Combine(_rootFolder, "Google Account");
        if (!Directory.Exists(accountPath))
        {
            return;
        }

        var htmlFiles = Directory.GetFiles(accountPath, "*.SubscriberInfo.html");
        if (htmlFiles.Count() != 1)
        {
            return;
        }

        var htmlPath = htmlFiles[0];

        var doc = new HtmlDocument();
        doc.Load(htmlPath);

        var alternateEmails = _emailRegex.Matches(doc.DocumentNode.SelectNodes("//div[@class='section'][1]/ul/li[2]")[0].InnerText);
        foreach (Match match in alternateEmails)
        {
            AddEmailToUser(_userInfo, match.Value);
        }

        var contactEmail = _emailRegex.Match(doc.DocumentNode.SelectNodes("//div[@class='section'][3]/ul/li[1]")[0].InnerText);
        if (contactEmail.Success)
        {
            AddEmailToUser(_userInfo, contactEmail.Groups[1].Value);
        }

        var recoveryEmail = _emailRegex.Match(doc.DocumentNode.SelectNodes("//div[@class='section'][3]/ul/li[2]")[0].InnerText);
        if (recoveryEmail.Success)
        {
            AddEmailToUser(_userInfo, recoveryEmail.Groups[1].Value);
        }

        var recoverySms = _phoneRegex.Match(doc.DocumentNode.SelectNodes("//div[@class='section'][3]/ul/li[3]")[0].InnerText);
        if (recoverySms.Success)
        {
            AddPhoneToUser(_userInfo, recoverySms.Groups[1].Value);
        }
    }

    private void AddEmailToUser(UserInfo userInfo, string email)
    {
        if (userInfo.Email != email && !userInfo.Contacts.Contains(email))
        {
            userInfo.ContactsList.Add(email.EndsWith("@gmail.com") ? "gmail" : "mail"); // SocialContactsManager.ContactType_gmail in ASC.WebStudio
            userInfo.ContactsList.Add(email);
        }
    }

    private void AddPhoneToUser(UserInfo userInfo, string phone)
    {
        if (userInfo.MobilePhone != phone && !userInfo.Contacts.Contains(phone))
        {
            userInfo.ContactsList.Add("mobphone"); // SocialContactsManager.ContactType_mobphone in ASC.WebStudio
            userInfo.ContactsList.Add(phone);
        }
    }
}
