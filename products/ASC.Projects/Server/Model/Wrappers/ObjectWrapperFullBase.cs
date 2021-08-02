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

using ASC.Api.Core;
using ASC.Web.Api.Models;

namespace ASC.Api.Projects.Wrappers
{
    public class ObjectWrapperFullBase : ObjectWrapperBase
    { 
        public ApiDateTime Created { get; set; }
        public EmployeeWraper CreatedBy { get; set; }
        public Guid CreatedById { get; set; }

        private ApiDateTime updated;
        public ApiDateTime Updated
        {
            get { return updated < Created ? Created : updated; }
            set { updated = value; }
        }
        public EmployeeWraper UpdatedBy { get; set; }
        public Guid UpdatedById { get; set; }
    }
}