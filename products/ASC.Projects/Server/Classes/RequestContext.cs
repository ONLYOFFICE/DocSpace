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

using ASC.Common;
using ASC.Core;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;

namespace ASC.Projects.Classes
{
    [Scope]
    public class RequestContext
    {
        public bool IsInConcreteProject { get; private set; }
        private SecurityContext SecurityContext { get; set; }
        private UrlParameters UrlParameters { get; set; }
        private EngineFactory EngineFactory { get; set; }
        private Project currentProject;
        
        public RequestContext(EngineFactory engineFactory, SecurityContext securityContext, UrlParameters urlParameters)
        {
            SecurityContext = securityContext;
            UrlParameters = urlParameters;
            IsInConcreteProject = UrlParameters.ProjectID >= 0;
            EngineFactory = engineFactory;
        }

        private IEnumerable<Project> currentUserProjects;
        public IEnumerable<Project> CurrentUserProjects
        {
            get
            {
                return currentUserProjects ??
                       (currentUserProjects =
                           EngineFactory.GetProjectEngine().GetByParticipant(SecurityContext.CurrentAccount.ID));
            }
        }

        #region Project

        public Project GetCurrentProject(bool isthrow = true)
        {
            if (currentProject != null) return currentProject;

            currentProject = EngineFactory.GetProjectEngine().GetByID(GetCurrentProjectId(isthrow));

            if (currentProject != null || !isthrow)
            {
                return currentProject;
            }

            throw new ApplicationException("ProjectFat not finded");
        }

        public int GetCurrentProjectId(bool isthrow = true)
        {
            var pid = UrlParameters.ProjectID;

            if (pid >= 0 || !isthrow)
                return pid;

            throw new ApplicationException("ProjectFat Id parameter invalid");
        }

        #endregion
    }
}
