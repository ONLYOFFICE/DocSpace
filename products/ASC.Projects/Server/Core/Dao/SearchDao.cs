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
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;
using ASC.ElasticSearch;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;
using ASC.Projects.EF;

namespace ASC.Projects.Data.DAO
{
    [Scope]
    public class SearchDao : BaseDao, ISearchDao
    {
        private IDaoFactory DaoFactory { get; set; }

        public SearchDao(SecurityContext securityContext, DbContextManager<WebProjectsContext> dbContextManager, IDaoFactory daoFactory, TenantManager tenantManager) : base(securityContext, dbContextManager, tenantManager)
        {
            DaoFactory = daoFactory;
        }

        public IEnumerable<DomainObject<int>> Search(String text, int projectId)
        {
            var result = new List<DomainObject<int>>();
            result.AddRange(GetProjects(text, projectId));
            result.AddRange(GetTasks(text, projectId));
            result.AddRange(GetSubtasks(text));
            result.AddRange(GetMilestones(text, projectId));
            result.AddRange(GetMessages(text, projectId));
            result.AddRange(GetComments(text));
            return result;
        }
        
        private IEnumerable<DomainObject<int>> GetProjects(String text, int projectId)
        {
            return DaoFactory.GetProjectDao().GetProjects(text, projectId, GetKeywords(text));
        }

        private IEnumerable<DomainObject<int>> GetMilestones(String text, int projectId)
        {
            return DaoFactory.GetMilestoneDao().GetMilestones(text, projectId, GetKeywords(text));
        }

        private IEnumerable<DomainObject<int>> GetTasks(String text, int projectId)
        {
            return DaoFactory.GetTaskDao().GetTasks(text, projectId, GetKeywords(text));
        }

        private IEnumerable<DomainObject<int>> GetMessages(String text, int projectId)
        {
            return DaoFactory.GetMessageDao().GetMessages(text, projectId, GetKeywords(text));
        }

        private IEnumerable<DomainObject<int>> GetComments(String text)
        {
            return DaoFactory.GetCommentDao().GetComments(text, GetKeywords(text));
        }

        private IEnumerable<DomainObject<int>> GetSubtasks(String text)
        {
            return DaoFactory.GetSubtaskDao().GetSubtasks(text, GetKeywords(text));
        }

        private IEnumerable<string> GetKeywords(string text)
        {
            return text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(k => 3 <= k.Trim().Length)
                .ToArray();
            
        }
    }
}
