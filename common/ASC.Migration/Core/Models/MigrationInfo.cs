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

namespace ASC.Migration.Core.Models;

public abstract class MigrationInfo<TUser, TContacts, TCalendar, TFiles, TMail, TGroup> : IMigrationInfo
    where TUser : MigratingUser<TContacts, TCalendar, TFiles, TMail>
    where TContacts : MigratingContacts
    where TCalendar : MigratingCalendar
    where TFiles : MigratingFiles
    where TMail : MigratingMail
    where TGroup : MigratingGroup
{
    public Dictionary<string, TUser> Users = new Dictionary<string, TUser>();
    public string MigratorName { get; set; }
    public List<MigrationModules> Modules = new List<MigrationModules>();
    public List<string> failedArchives = new List<string>();
    public List<TGroup> Groups = new List<TGroup>();

    public virtual MigrationApiInfo ToApiInfo()
    {
        return new MigrationApiInfo()
        {
            Users = Users.Values.Select(u => u.ToApiInfo()).ToList(),
            MigratorName = MigratorName,
            Modules = Modules,
            FailedArchives = failedArchives,
            Groups = Groups.Select(g => g.ToApiInfo()).ToList()
        };
    }

    public virtual void Merge(MigrationApiInfo apiInfo)
    {
        foreach (var apiUser in apiInfo.Users)
        {
            if (!Users.ContainsKey(apiUser.Key))
            {
                continue;
            }

            var user = Users[apiUser.Key];
            user.ShouldImport = apiUser.ShouldImport;

            user.MigratingCalendar.ShouldImport = apiUser.MigratingCalendar.ShouldImport;
            user.MigratingContacts.ShouldImport = apiUser.MigratingContacts.ShouldImport;
            user.MigratingFiles.ShouldImport = apiUser.MigratingFiles.ShouldImport;
            user.MigratingMail.ShouldImport = apiUser.MigratingMail.ShouldImport;
        }
        foreach (var apiGroup in apiInfo.Groups)
        {
            if (!Groups.Exists(g => apiGroup.GroupName == g.GroupName))
            {
                continue;
            }

            var group = Groups.Find(g => apiGroup.GroupName == g.GroupName);
            group.ShouldImport = apiGroup.ShouldImport;
        }
    }
}
