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

using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;
using ASC.Projects.EF;
using ASC.Projects.Core.DataInterfaces;
using ASC.Common;

namespace ASC.Projects.Data.DAO
{
    [Scope]
    public class ParticipantDao : BaseDao, IParticipantDao
    {
        private IDaoFactory DaoFactory { get; set; }
        public ParticipantDao(SecurityContext securityContext, DbContextManager<WebProjectsContext> dbContextManager, IDaoFactory daoFactory, TenantManager tenantManager) : base(securityContext, dbContextManager, tenantManager)
        {
            DaoFactory = daoFactory;
        }

        public int[] GetFollowingProjects(Guid participant)
        {
            return WebProjectsContext.FollowingProject
                .Where(ptp=> ptp.ParticipantId == participant.ToString())
                .Select(ptp=> ptp.ProjectId)
                .ToArray();
        }

        public int[] GetMyProjects(Guid participant)
        {
            return WebProjectsContext.Participant
                .Where(ptp => ptp.ParticipantId == participant.ToString())
                .Select(ptp => ptp.ProjectId)
                .ToArray();
        }
        public List<int> GetInterestedProjects(Guid participant)
        {
            var q1 = WebProjectsContext.Participant
                .Where(ptp => ptp.ParticipantId == participant.ToString())
                .Select(ptp => ptp.ProjectId)
                .ToList();

            var q2 = WebProjectsContext.FollowingProject
                .Where(ptp => ptp.ParticipantId == participant.ToString())
                .Select(ptp => ptp.ProjectId)
                .ToList();
            return q1.Union(q2).ToList();
        }

        public void AddToFollowingProjects(int project, Guid participant)
        {
            var followingProject = new DbFollowingProject();
            followingProject.ParticipantId = participant.ToString();
            followingProject.ProjectId = project;
            WebProjectsContext.FollowingProject.Add(followingProject);
            WebProjectsContext.SaveChanges();

            DaoFactory.GetProjectDao().UpdateLastModified(project);
        }

        public void RemoveFromFollowingProjects(int project, Guid participant)
        {
            var followingProject = WebProjectsContext.FollowingProject.Where(fp => fp.ProjectId == project && fp.ParticipantId == participant.ToString()).SingleOrDefault();
            WebProjectsContext.FollowingProject.Remove(followingProject);
            WebProjectsContext.SaveChanges();

            DaoFactory.GetProjectDao().UpdateLastModified(project);
        }
    }
}
