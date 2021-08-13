/*
 *
 * (c) Copyright Ascensio System Limited 2010-2021
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


using System;
using System.Collections.Generic;
using System.Linq;

using ASC.Common;
using ASC.Files.Core;
using ASC.Files.Core.Security;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;

namespace ASC.Projects.Classes
{
    [Transient]
    public class SecurityAdapter : IFileSecurity
    {
        private Project Project { get; set; }
        private ProjectSecurity ProjectSecurity { get; set; }
        private EngineFactory EngineFactory { get; set; }

        public SecurityAdapter(EngineFactory engineFactory, ProjectSecurity projectSecurity)
        {
            ProjectSecurity = projectSecurity;
            EngineFactory = engineFactory;
        }

        public SecurityAdapter Init(Project project)
        {
            Project = project;
            return this;
        }

        public SecurityAdapter Init(int projectId)
        {
            Project = EngineFactory.GetProjectEngine().GetByID(projectId);
            return this;
        }

        public bool CanRead<T>(FileEntry<T> entry, Guid userId)
        {
            return Can(entry, userId, SecurityAction.Read);
        }

        public bool CanCustomFilterEdit<T>(FileEntry<T> file, Guid userId)
        {
            return CanEdit(file, userId);
        }

        public bool CanComment<T>(FileEntry<T> entry, Guid userId)
        {
            return Can(entry, userId, SecurityAction.Edit);
        }

        public bool CanFillForms<T>(FileEntry<T> entry, Guid userId)
        {
            return Can(entry, userId, SecurityAction.Edit);
        }

        public bool CanReview<T>(FileEntry<T> entry, Guid userId)
        {
            return Can(entry, userId, SecurityAction.Edit);
        }

        public bool CanCreate<T>(FileEntry<T> entry, Guid userId)
        {
            return Can(entry, userId, SecurityAction.Create);
        }

        public bool CanDelete<T>(FileEntry<T> entry, Guid userId)
        {
            return Can(entry, userId, SecurityAction.Delete);
        }

        public bool CanEdit<T>(FileEntry<T> entry, Guid userId)
        {
            return Can(entry, userId, SecurityAction.Edit);
        }

        private bool Can<T>(FileEntry<T> entry, Guid userId, SecurityAction action)
        {
            if (entry == null || Project == null) return false;

            if (!ProjectSecurity.CanReadFiles(Project, userId)) return false;

            if (Project.Status == ProjectStatus.Closed
                && action != SecurityAction.Read)
                return false;

            if (ProjectSecurity.IsAdministrator(userId)) return true;

            var inTeam = EngineFactory.GetProjectEngine().IsInTeam(Project.ID, userId);

            switch (action)
            {
                case SecurityAction.Read:
                    return !Project.Private || inTeam;
                case SecurityAction.Create:
                case SecurityAction.Edit:
                    Folder<T> folder;
                    return inTeam
                            && (!ProjectSecurity.IsVisitor(userId)
                                || (folder = entry as Folder<T>) != null && folder.FolderType == FolderType.BUNCH);
                case SecurityAction.Delete:
                    return inTeam
                            && !ProjectSecurity.IsVisitor(userId)
                            && (Project.Responsible == userId ||
                                (entry.CreateBy == userId
                                && ((folder = entry as Folder<T>) == null || folder.FolderType == FolderType.DEFAULT)));
                default:
                    return false;
            }
        }

        public IEnumerable<Guid> WhoCanRead<T>(FileEntry<T> entry)
        {
            return EngineFactory.GetProjectEngine().GetTeam(Project.ID).Select(p => p.ID).ToList();
        }

        private enum SecurityAction
        {
            Read,
            Create,
            Edit,
            Delete,
        };
    }
}