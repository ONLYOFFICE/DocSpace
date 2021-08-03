/*
 *
 * (c) Copyright Ascensio System Limited 2010-2021
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
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