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

using ASC.Api.Core;
using ASC.Projects.Core.Domain;
using ASC.Web.Api.Models;

namespace ASC.Api.Projects.Wrappers
{
    public class TaskWrapper : ObjectWrapperFullBase
    {
        public bool CanEdit { get; set; }
        public bool CanCreateSubtask { get; set; }
        public bool CanCreateTimeSpend { get; set; }
        public bool CanDelete { get; set; }
        public bool CanReadFiles { get; set; }
        public ApiDateTime Deadline { get; set; }
        public ApiDateTime StartDate { get; set; }
        public int MilestoneId { get; set; }
        public TaskPriority Priority { get; set; }
        public SimpleProjectWrapper ProjectOwner { get; set; }
        public int Progress { get; set; }
        public List<SubtaskWrapper> Subtasks { get; set; }
        public IEnumerable<TaskLinkWrapper> Links { get; set; }
        public List<EmployeeWraper> Responsibles { get; set; }
        public List<Guid> ResponsibleIds { get; set; }
        public SimpleMilestoneWrapper Milestone { get; set; }
        public int? CustomTaskStatus { get; set; }

    }
}