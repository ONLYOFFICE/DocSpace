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



namespace ASC.Migration.GoogleWorkspace
{
    [ApiMigrator("GoogleWorkspace")]
    public class GoogleWorkspaceMigration : AbstractMigration<GwsMigrationInfo, GwsMigratingUser, GwsMigratingContacts, GwsMigratingCalendar, GwsMigratingFiles, GwsMigratingMail>
    {
        private string[] _takeouts;
        private readonly UserManager _userManager;
        private readonly SecurityContext _securityContext;
        private readonly TempPath _tempPath;

        public GoogleWorkspaceMigration(MigrationLogger migrationLogger, UserManager userManager, SecurityContext securityContext, TempPath tempPath) : base(migrationLogger)
        {
            _userManager = userManager;
            _securityContext = securityContext;
            _tempPath = tempPath;
        }

        public override void Init(string path, CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            var tempTakeouts = new List<string>();
            var files = Directory.GetFiles(path);
            if (!files.Any() || !files.Any(f => f.EndsWith(".zip")))
            {
                throw new Exception("Folder must not be empty and should contain .zip files.");
            }
            foreach (var item in files)
            {
                if (item.EndsWith(".zip"))
                {
                    tempTakeouts.Add(item);
                }
            }
            _takeouts = tempTakeouts.ToArray();

            _migrationInfo = new GwsMigrationInfo();
            _migrationInfo.MigratorName = GetType().CustomAttributes.First().ConstructorArguments.First().Value.ToString();
        }

        public override Task<MigrationApiInfo> Parse()
        {
            ReportProgress(0, MigrationResource.StartOfDataProcessing);

            var progressStep = 100 / _takeouts.Length;
            var i = 1;
            foreach (var takeout in _takeouts)
            {
                if (_cancellationToken.IsCancellationRequested) { ReportProgress(100, MigrationResource.MigrationCanceled); return null; }
                ReportProgress(GetProgress() + progressStep, MigrationResource.DataProcessing + $" {takeout} ({i++}/{_takeouts.Length})");
                var tmpFolder = Path.Combine(_tempPath.GetTempPath(), Path.GetFileNameWithoutExtension(takeout));
                try
                {
                    ZipFile.ExtractToDirectory(takeout, tmpFolder);

                    var rootFolder = Path.Combine(tmpFolder, "Takeout");

                    if (!Directory.Exists(rootFolder))
                    {
                        throw new Exception("Takeout zip does not contain root 'Takeout' folder.");
                    }
                    var directories = Directory.GetDirectories(rootFolder);
                    if (directories.Length == 1 && directories[0].Split(Path.DirectorySeparatorChar).Last() == "Groups")
                    {
                        var group = new GWSMigratingGroups(_userManager, rootFolder, Log);
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
                    else
                    {
                        var user = new GwsMigratingUser(takeout, rootFolder, Log);
                        user.Parse();
                        foreach (var element in user.ModulesList)
                        {
                            if (!_migrationInfo.Modules.Exists(x => x.MigrationModule == element.MigrationModule))
                            {
                                _migrationInfo.Modules.Add(new MigrationModules(element.MigrationModule, element.Module));
                            }
                        }
                        _migrationInfo.Users.Add(takeout, user);
                    }
                }
                catch (Exception ex)
                {
                    _migrationInfo.failedArchives.Add(Path.GetFileName(takeout));
                    Log($"Couldn't parse user from {Path.GetFileNameWithoutExtension(takeout)} archive", ex);
                }
                finally
                {
                    if (Directory.Exists(tmpFolder))
                    {
                        Directory.Delete(tmpFolder, true);
                    }
                }
            }
            ReportProgress(100, MigrationResource.DataProcessingCompleted);
            return Task.FromResult(_migrationInfo.ToApiInfo());
        }

        public override async Task Migrate(MigrationApiInfo migrationApiInfo)
        {
            ReportProgress(0, MigrationResource.PreparingForMigration);
            _migrationInfo.Merge(migrationApiInfo);

            var usersForImport = _migrationInfo.Users
                .Where(u => u.Value.ShouldImport)
                .Select(u => u.Value);

            _importedUsers = new List<Guid>();
            var failedUsers = new List<GwsMigratingUser>();
            var usersCount = usersForImport.Count();
            var progressStep = 25 / usersCount;
            // Add all users first
            var i = 1;
            foreach (var user in usersForImport)
            {
                if (_cancellationToken.IsCancellationRequested) { ReportProgress(100, MigrationResource.MigrationCanceled); return; }
                ReportProgress(GetProgress() + progressStep, string.Format(MigrationResource.UserMigration, user.DisplayName, i++, usersCount));
                try
                {
                    user.DataСhange(migrationApiInfo.Users.Find(element => element.Key == user.Key));
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
                        await group.MigrateAsync();
                    }
                    catch (Exception ex)
                    {
                        Log($"Couldn't migrate group {group.GroupName} ", ex);
                    }
                }
            }

            // Add files, contacts and other stuff
            i = 1;
            foreach (var user in usersForImport)
            {
                if (_cancellationToken.IsCancellationRequested) { ReportProgress(100, MigrationResource.MigrationCanceled); return; }
                if (failedUsers.Contains(user))
                {
                    ReportProgress(GetProgress() + progressStep, string.Format(MigrationResource.UserSkipped, user.DisplayName, i, usersCount));
                    continue;
                }

                var smallStep = progressStep / 4;

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

            foreach (var item in _takeouts)
            {
                File.Delete(item);
            }

            ReportProgress(100, MigrationResource.MigrationCompleted);
        }
    }
}
