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

using ASC.Migration.OwnCloud.Models;

namespace ASC.Migration.OwnCloud;

[ApiMigrator("OwncloudMigrate")]
public class OwnCloudMigration : AbstractMigration<OCMigrationInfo, OCMigratingUser, OCMigratingContacts, OCMigratingCalendar, OCMigratingFiles, OCMigratingMail>
{
    private string _takeouts;
    public string[] TempParse;
    private string _tmpFolder;
    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly IDaoFactory _daoFactory;
    private readonly FileSecurity _fileSecurity;
    private readonly FileStorageService _fileStorageService;
    private readonly SecurityContext _securityContext;
    private readonly TenantManager _tenantManager;
    private readonly UserManager _userManager;

    public OwnCloudMigration(
        GlobalFolderHelper globalFolderHelper,
        IDaoFactory daoFactory,
        FileSecurity fileSecurity,
        FileStorageService fileStorageService,
        SecurityContext securityContext,
        TenantManager tenantManager,
        UserManager userManager,
        MigrationLogger migrationLogger) : base(migrationLogger)
    {
        _globalFolderHelper = globalFolderHelper;
        _daoFactory = daoFactory;
        _fileSecurity = fileSecurity;
        _fileStorageService = fileStorageService;
        _securityContext = securityContext;
        _tenantManager = tenantManager;
        _userManager = userManager;
    }

    public override void Init(string path, CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
        var files = Directory.GetFiles(path);
        if (!files.Any() || !files.Any(f => f.EndsWith(".zip")))
        {
            throw new Exception("Folder must not be empty and should contain only .zip files.");
        }
        for (var i = 0; i < files.Length; i++)
        {
            if (files[i].EndsWith(".zip"))
            {
                var creationTime = File.GetCreationTimeUtc(files[i]);
                _takeouts = files[i];
            }
        }

        _migrationInfo = new OCMigrationInfo();
        _migrationInfo.MigratorName = GetType().CustomAttributes.First().ConstructorArguments.First().Value.ToString();
        _tmpFolder = path;
    }
    public override Task<MigrationApiInfo> Parse()
    {
        ReportProgress(0, MigrationResource.Unzipping);
        try
        {
            try
            {
                ZipFile.ExtractToDirectory(_takeouts, _tmpFolder);
            }
            catch (Exception ex)
            {
                Log($"Couldn't to unzip {_takeouts}", ex);
            }
            if (_cancellationToken.IsCancellationRequested) { ReportProgress(100, MigrationResource.MigrationCanceled); return null; }
            ReportProgress(30, MigrationResource.UnzippingFinished);
            var bdFile = "";
            try
            {
                bdFile = Directory.GetFiles(Directory.GetDirectories(_tmpFolder)[0], "*.bak")[0];
                if (bdFile == null)
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                _migrationInfo.failedArchives.Add(Path.GetFileName(_takeouts));
                Log("Archive must not be empty and should contain .bak files.", ex);
            }

            ReportProgress(40, MigrationResource.DumpParse);
            var users = DBExtractUser(bdFile);
            var progress = 40;
            foreach (var item in users)
            {
                if (_cancellationToken.IsCancellationRequested) { ReportProgress(100, MigrationResource.MigrationCanceled); return null; }
                ReportProgress(progress, MigrationResource.DataProcessing);
                progress += 30 / users.Count;
                if (item.Data.DisplayName != null)
                {
                    try
                    {
                        var userName = item.Data.DisplayName.Split(' ');
                        item.Data.DisplayName = userName.Length > 1 ? string.Format("{0} {1}", userName[0], userName[1]).Trim() : userName[0].Trim();
                        var user = new OCMigratingUser(_globalFolderHelper, _daoFactory, _fileStorageService, _tenantManager, _userManager, item.Uid, item, Directory.GetDirectories(_tmpFolder)[0], Log);
                        user.Parse();
                        foreach (var element in user.ModulesList)
                        {
                            if (!_migrationInfo.Modules.Exists(x => x.MigrationModule == element.MigrationModule))
                            {
                                _migrationInfo.Modules.Add(new MigrationModules(element.MigrationModule, element.Module));
                            }
                        }
                        _migrationInfo.Users.Add(item.Uid, user);
                    }
                    catch (Exception ex)
                    {
                        Log($"Couldn't parse user {item.Data.DisplayName}", ex);
                    }
                }
            }

            var groups = DBExtractGroup(bdFile);
            progress = 80;
            foreach (var item in groups)
            {
                ReportProgress(progress, MigrationResource.DataProcessing);
                progress += 10 / groups.Count;
                var group = new OCMigratingGroups(_userManager, item, Log);
                group.Parse();
                if (group.Module.MigrationModule != null)
                {
                    _migrationInfo.Groups.Add(group);
                    if (!_migrationInfo.Modules.Exists(x => x.MigrationModule == group.Module.MigrationModule))
                    {
                        _migrationInfo.Modules.Add(new MigrationModules(group.Module.MigrationModule, group.Module.Module));
                    }
                }
            }
            ReportProgress(90, MigrationResource.ClearingTemporaryData);
        }
        catch (Exception ex)
        {
            _migrationInfo.failedArchives.Add(Path.GetFileName(_takeouts));
            Log($"Couldn't parse users from {Path.GetFileNameWithoutExtension(_takeouts)} archive", ex);
        }
        ReportProgress(100, MigrationResource.DataProcessingCompleted);
        return Task.FromResult(_migrationInfo.ToApiInfo());
    }

    public List<OCGroup> DBExtractGroup(string dbFile)
    {
        var groups = new List<OCGroup>();

        var sqlFile = File.ReadAllText(dbFile);

        var groupList = GetDumpChunk("oc_groups", sqlFile);
        if (groupList == null)
        {
            return groups;
        }

        foreach (var group in groupList)
        {
            groups.Add(new OCGroup
            {
                GroupGid = group.Trim('\''),
                UsersUid = new List<string>()
            });
        }

        var usersInGroups = GetDumpChunk("oc_group_user", sqlFile);
        foreach (var user in usersInGroups)
        {
            var userGroupGid = user.Split(',').First().Trim('\'');
            var userUid = user.Split(',').Last().Trim('\'');
            groups.Find(ggid => userGroupGid == ggid.GroupGid).UsersUid.Add(userUid);
        }

        return groups;
    }

    public List<OCUser> DBExtractUser(string dbFile)
    {
        var userDataList = new Dictionary<string, OCUser>();

        var sqlFile = File.ReadAllText(dbFile);

        var accountsData = GetDumpChunk("oc_accounts_data", sqlFile);
        if (accountsData != null)
        {
            throw new Exception();
        }

        var accounts = GetDumpChunk("oc_accounts", sqlFile);
        if (accounts == null)
        {
            return userDataList.Values.ToList();
        }

        foreach (var account in accounts)
        {

            var userId = account.Split(',')[2].Trim('\'');

            userDataList.Add(userId, new OCUser
            {
                Uid = account.Split(',')[2].Trim('\''),
                Data = new OCUserData
                {
                    DisplayName = account.Split(',')[4].Trim('\''),
                    Email = account.Split(',')[1].Trim('\'')
                },
                Addressbooks = null,
                Calendars = new List<OCCalendars>(),
                Storages = new OCStorages()
            });
        }

        var calendarsData = GetDumpChunk("oc_calendars", sqlFile);
        if (calendarsData != null)
        {
            foreach (var calendarData in calendarsData)
            {
                var values = calendarData.Split(',')
                    .Select(s => s.Trim('\'')).ToArray();
                var uid = values[1].Split('/').Last();
                userDataList.TryGetValue(uid, out var user);
                if (user == null)
                {
                    continue;
                }

                user.Calendars.Add(new OCCalendars()
                {
                    Id = int.Parse(values[0]),
                    CalendarObject = new List<OCCalendarObjects>(),
                    DisplayName = values[2]
                });
            }
        }

        var calendars = userDataList.Values
            .SelectMany(u => u.Calendars)
            .ToDictionary(c => c.Id, c => c);
        var calendarObjects = GetDumpChunk("oc_calendarobjects", sqlFile);
        if (calendarObjects != null)
        {
            foreach (var calendarObject in calendarObjects)
            {
                var values = calendarObject.Split(',')
                    .Select(s => s.Trim('\'')).ToArray();
                var calId = int.Parse(values[3]);
                calendars.TryGetValue(calId, out var cal);
                if (cal == null)
                {
                    continue;
                }

                cal.CalendarObject.Add(new OCCalendarObjects()
                {
                    Id = int.Parse(values[0]),
                    CalendarData = Encoding.UTF8.GetBytes(values[1]
                                                    .Replace("\\r", "")
                                                    .Replace("\\n", "\n")),
                });
            }
        }

        var addressBooks = GetDumpChunk("oc_addressbooks", sqlFile);
        if (addressBooks != null)
        {
            foreach (var addressBook in addressBooks)
            {
                var values = addressBook.Split(',')
                    .Select(s => s.Trim('\'')).ToArray();
                var uid = values[1].Split('/').Last();
                userDataList.TryGetValue(uid, out var user);
                if (user == null)
                {
                    continue;
                }

                user.Addressbooks = new OCAddressbooks();
                user.Addressbooks.Id = int.Parse(values[0]);
                user.Addressbooks.Cards = new List<OCCards>();
            }
        }

        var addressBooksDict = userDataList.Values
            .Select(u => u.Addressbooks)
            .Where(x => x != null)
            .ToDictionary(b => b.Id, b => b);
        var cards = GetDumpChunk("oc_cards", sqlFile);
        if (cards != null)
        {
            foreach (var card in cards)
            {
                var values = card.Split(',')
                    .Select(s => s.Trim('\'')).ToArray();
                var bookId = int.Parse(values[1]);
                addressBooksDict.TryGetValue(bookId, out var book);
                if (book == null)
                {
                    continue;
                }

                book.Cards.Add(new OCCards()
                {
                    Id = int.Parse(values[0]),
                    CardData = Encoding.UTF8.GetBytes(values[2]
                                                    .Replace("\\r", "")
                                                    .Replace("\\n", "\n")),
                });
            }
        }

        var storages = GetDumpChunk("oc_storages", sqlFile);
        if (storages != null)
        {
            foreach (var storage in storages)
            {
                var values = storage.Split(',')
                           .Select(s => s.Trim('\'')).ToArray();
                var uid = values[0].Split(':').Last();
                userDataList.TryGetValue(uid, out var user);
                if (user == null)
                {
                    continue;
                }

                user.Storages.NumericId = int.Parse(values[1]);
                user.Storages.Id = values[0];
                user.Storages.FileCache = new List<OCFileCache>();
            }
        }

        var storagesDict = userDataList.Values
            .Select(u => u.Storages)
            .ToDictionary(s => s.NumericId, s => s);
        var fileCaches = GetDumpChunk("oc_filecache", sqlFile);
        if (fileCaches != null)
        {
            foreach (var cache in fileCaches)
            {
                var values = cache.Split(',')
                           .Select(s => s.Trim('\'')).ToArray();
                var storageId = int.Parse(values[1]);
                storagesDict.TryGetValue(storageId, out var storage);
                if (storage == null)
                {
                    continue;
                }

                storage.FileCache.Add(new OCFileCache()
                {
                    FileId = int.Parse(values[0]),
                    Path = values[2],
                    Share = new List<OCShare>()
                });
            }
        }

        var files = userDataList.Values
            .SelectMany(u => u.Storages.FileCache)
            .ToDictionary(f => f.FileId, f => f);
        var shares = GetDumpChunk("oc_share", sqlFile);
        if (shares != null)
        {
            foreach (var share in shares)
            {
                var values = share.Split(',')
                           .Select(s => s.Trim('\'')).ToArray();
                var fileId = int.Parse(values[9]);
                files.TryGetValue(fileId, out var file);
                if (file == null)
                {
                    continue;
                }

                file.Share.Add(new OCShare()
                {
                    Id = int.Parse(values[0]),
                    ShareWith = values[2],
                    Premissions = int.Parse(values[11])
                });
            }
        }

        return userDataList.Values.ToList();
    }

    private IEnumerable<string> GetDumpChunk(string tableName, string dump)
    {
        var regex = new Regex($"INSERT INTO `{tableName}` VALUES (.*);");
        var match = regex.Match(dump);
        if (!match.Success)
        {
            return null;
        }

        var entryRegex = new Regex(@"(\(.*?\))[,;]");
        var accountDataMatches = entryRegex.Matches(match.Groups[1].Value + ";");
        return accountDataMatches.Cast<Match>()
            .Select(m => m.Groups[1].Value.Trim(new[] { '(', ')' }));
    }

    public override async Task Migrate(MigrationApiInfo migrationApiInfo)
    {
        ReportProgress(0, MigrationResource.PreparingForMigration);
        _migrationInfo.Merge(migrationApiInfo);

        var usersForImport = _migrationInfo.Users
            .Where(u => u.Value.ShouldImport)
            .Select(u => u.Value);

        _importedUsers = new List<Guid>();
        var failedUsers = new List<OCMigratingUser>();
        var usersCount = usersForImport.Count();
        var progressStep = 25 / usersCount;
        var i = 1;
        foreach (var user in usersForImport)
        {
            if (_cancellationToken.IsCancellationRequested) { ReportProgress(100, MigrationResource.MigrationCanceled); return; }
            ReportProgress(GetProgress() + progressStep, string.Format(MigrationResource.UserMigration, user.DisplayName, i++, usersCount));
            try
            {
                user.dataСhange(migrationApiInfo.Users.Find(element => element.Key == user.Key));
                await user.MigrateAsync();
                _importedUsers.Add(user.Guid);
            }
            catch (Exception ex)
            {
                failedUsers.Add(user);
                Log($"Couldn't migrate user {user.DisplayName} ({user.Email})", ex);
            }
        }

        var groupsForImport = _migrationInfo.Groups
            .Where(g => g.ShouldImport)
            .Select(g => g);
        var groupsCount = groupsForImport.Count();
        if (groupsCount != 0)
        {
            progressStep = 25 / groupsForImport.Count();
            //Create all groups
            i = 1;
            foreach (var group in groupsForImport)
            {
                if (_cancellationToken.IsCancellationRequested) { ReportProgress(100, MigrationResource.MigrationCanceled); return; }
                ReportProgress(GetProgress() + progressStep, string.Format(MigrationResource.GroupMigration, group.GroupName, i++, groupsCount));
                try
                {
                    group.UsersGuidList = _migrationInfo.Users
                    .Where(user => group.UserUidList.Exists(u => user.Key == u))
                    .Select(u => u)
                    .ToDictionary(k => k.Key, v => v.Value.Guid);
                    await group.MigrateAsync();
                }
                catch (Exception ex)
                {
                    Log($"Couldn't migrate group {group.GroupName} ", ex);
                }
            }
        }

        i = 1;
        foreach (var user in usersForImport)
        {
            if (_cancellationToken.IsCancellationRequested) { ReportProgress(100, MigrationResource.MigrationCanceled); return; }
            if (failedUsers.Contains(user))
            {
                ReportProgress(GetProgress() + progressStep, string.Format(MigrationResource.UserSkipped, user.DisplayName, i, usersCount));
                continue;
            }

            var smallStep = progressStep / 3;

            try
            {
                await user.MigratingContacts.MigrateAsync();
            }
            catch (Exception ex)
            {
                Log($"Couldn't migrate user {user.DisplayName} ({user.Email}) contacts", ex);
            }
            finally
            {
                ReportProgress(GetProgress() + smallStep, string.Format(MigrationResource.MigratingUserContacts, user.DisplayName, i, usersCount));
            }

            /*try
            {
                user.MigratingCalendar.Migrate();
            }
            catch (Exception ex)
            {
                Log($"Couldn't migrate user {user.DisplayName} ({user.Email}) calendar", ex);
            }
            finally
            {
                ReportProgress(GetProgress() + smallStep, String.Format(MigrationResource.UserCalendarMigration, user.DisplayName, i, usersCount));
            }*/

            try
            {
                var currentUser = _securityContext.CurrentAccount;
                await _securityContext.AuthenticateMeAsync(user.Guid);
                user.MigratingFiles.SetUsersDict(usersForImport.Except(failedUsers));
                user.MigratingFiles.SetGroupsDict(groupsForImport);
                await user.MigratingFiles.MigrateAsync();
                await _securityContext.AuthenticateMeAsync(currentUser.ID);
            }
            catch (Exception ex)
            {
                Log($"Couldn't migrate user {user.DisplayName} ({user.Email}) files", ex);
            }
            finally
            {
                ReportProgress(GetProgress() + smallStep, string.Format(MigrationResource.MigratingUserFiles, user.DisplayName, i, usersCount));
            }
            i++;
        }

        if (Directory.Exists(_tmpFolder))
        {
            Directory.Delete(_tmpFolder, true);
        }
        ReportProgress(100, MigrationResource.MigrationCompleted);
    }
}
