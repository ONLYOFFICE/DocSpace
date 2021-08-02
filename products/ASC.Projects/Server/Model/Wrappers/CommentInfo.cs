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

namespace ASC.Api.Projects.Wrappers
{
    public class CommentInfo
    { 
        public string CommentID { get; set; }
        public Guid UserID { get; set; }
        public string UserPost { get; set; }
        public string UserFullName { get; set; }
        public string UserProfileLink { get; set; }
        public string UserAvatarPath { get; set; }
        public string CommentBody { get; set; }
        public bool Inactive { get; set; }
        public bool IsRead { get; set; }
        public bool IsEditPermissions { get; set; }
        public bool IsResponsePermissions { get; set; }
        public DateTime TimeStamp { get; set; }
        public string TimeStampStr { get; set; }
        public IList<CommentInfo> CommentList { get; set; }

    }
}