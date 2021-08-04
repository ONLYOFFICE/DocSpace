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

using ASC.Core.Common.Settings;
using ASC.Web.Files.Classes;

using Microsoft.Extensions.DependencyInjection;

namespace ASC.Projects
{
    [Serializable]
    public class ProjectsCommonSettings : ISettings
    {
        private int FolderMy { get; set; }
        public bool EverebodyCanCreate { get; set; }
        public bool HideEntitiesInPausedProjects { get; set; }

        public StartModuleType StartModuleType { get; set; }

        private object folderId;

        public object FolderId
        {
            get
            {
                return folderId ?? FolderMy;
            }
            set
            {
                folderId = value ?? FolderMy;
            }
        }

        public Guid ID
        {
            get { return new Guid("{F833803D-0A84-4156-A73F-7680F522FE07}"); }
        }

        public ISettings GetDefault(IServiceProvider serviceProvider)
        {
            var globalFolderHelper = serviceProvider.GetService<GlobalFolderHelper>();
            return new ProjectsCommonSettings
            {
                EverebodyCanCreate = false,
                StartModuleType = StartModuleType.Tasks,
                HideEntitiesInPausedProjects = true,
                FolderMy = globalFolderHelper.FolderMy
            };
        }
    }

    public enum StartModuleType
    {
        Projects,
        Tasks,
        Discussions,
        TimeTracking
    }
}