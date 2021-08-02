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

namespace ASC.Api.Projects.Wrappers
{
    public class SimpleTaskWrapper
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ApiDateTime Deadline { get; set; }
        public int Status { get; set; }
        public int? CustomTaskStatus { get; set; }
        public ApiDateTime Created { get; set; }
        public Guid CreatedBy { get; set; }
        private ApiDateTime updated;
        public ApiDateTime Updated
        {
            get { return updated < Created ? Created : updated; }
            set { updated = value; }
        }
        public Guid UpdatedBy { get; set; }
        public ApiDateTime StartDate { get; set; }
        public int MilestoneId { get; set; }
        public TaskPriority Priority { get; set; }
        public int ProjectOwner { get; set; }
        public int Progress { get; set; }
        public int SubtasksCount { get; set; }
        public IEnumerable<TaskLinkWrapper> Links { get; set; }
        public List<Guid> Responsibles { get; set; }
        public bool CanEdit { get; set; }
        public bool CanCreateSubtask { get; set; }
        public bool CanCreateTimeSpend { get; set; }
        public bool CanDelete { get; set; }
    }
}