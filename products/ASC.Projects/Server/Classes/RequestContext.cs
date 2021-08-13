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
