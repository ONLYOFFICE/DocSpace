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


using System.Collections.Generic;

using ASC.Projects.Classes;

namespace ASC.Api.Projects.Wrappers
{
    public class ProjectWrapperFull : ObjectWrapperFullBase
    { 
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public ProjectSecurityInfo Security { get; set; }
        public object ProjectFolder { get; set; }
        public bool IsPrivate { get; set; }
        public int TaskCount { get; set; }
        public int TaskCountTotal { get; set; }
        public int MilestoneCount { get; set; }
        public int DiscussionCount { get; set; }
        public int ParticipantCount { get; set; }
        public string TimeTrackingTotal { get; set; }
        public int DocumentsCount { get; set; }
        public bool IsFollow { get; set; }
        public IEnumerable<string> Tags { get; set; }

    }
}