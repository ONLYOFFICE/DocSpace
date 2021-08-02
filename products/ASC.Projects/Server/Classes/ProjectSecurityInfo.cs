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

namespace ASC.Projects.Classes
{
    public class ProjectSecurityInfo
    {
        public bool CanCreateMilestone { get; set; }
        public bool CanCreateMessage { get; set; }
        public bool CanCreateTask { get; set; }
        public bool CanCreateTimeSpend { get; set; }
        public bool CanEditTeam { get; set; }
        public bool CanReadFiles { get; set; }
        public bool CanReadMilestones { get; set; }
        public bool CanReadMessages { get; set; }
        public bool CanReadTasks { get; set; }
        public bool CanLinkContact { get; set; }
        public bool CanReadContacts { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool IsInTeam { get; set; }
    }

    public class CommonSecurityInfo
    { 
        public bool CanCreateProject { get; set; }
        public bool CanCreateTask { get; set; }
        public bool CanCreateMilestone { get; set; }
        public bool CanCreateMessage { get; set; }
        public bool CanCreateTimeSpend { get; set; }
    }

    public class TaskSecurityInfo
    {
        public bool CanEdit { get; set; }
        public bool CanCreateSubtask { get; set; }
        public bool CanCreateTimeSpend { get; set; }
        public bool CanDelete { get; set; }
        public bool CanReadFiles { get; set; }
    }
}