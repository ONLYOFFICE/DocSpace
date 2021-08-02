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

using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;
using ASC.Projects.EF;
using ASC.Core.Tenants;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;

using Microsoft.Extensions.Options;
using Microsoft.Security.Application;
using ASC.Common;

namespace ASC.Projects.Data.DAO
{
    [Scope]
    public class CommentDao : BaseDao, ICommentDao
    {
        private TenantUtil TenantUtil { get; set; }
        private ILog Log { get; set; }

        public CommentDao(SecurityContext securityContext, DbContextManager<WebProjectsContext> dbContextManager, TenantUtil tenantUtil, IOptionsMonitor<ILog> options, TenantManager tenantManager)
            : base(securityContext, dbContextManager, tenantManager)
        {
            TenantUtil = tenantUtil;
            Log = options.CurrentValue;
        }


        public List<Comment> GetAll(DomainObject<int> target)
        {
            return WebProjectsContext.Comment
                 .Where(c => c.TargetUniqId == target.UniqID)
                 .OrderBy(c => c.CreateOn)
                 .Select(c=> ToComment(c))
                 .ToList();
        }

        public Comment GetById(Guid id)
        {
            return WebProjectsContext.Comment
                .Where(c => c.Id == id.ToString())
                .Select(c => ToComment(c))
                .SingleOrDefault();
        }

        public List<int> Count(List<ProjectEntity> targets)
        {
            return WebProjectsContext.Comment
                .Where(c => targets.ConvertAll(target => target.UniqID).Contains(c.TargetUniqId) && c.InActive == 0)
                .GroupBy(c => c.TargetUniqId)
                .Select(c => c.Count())
                .ToList();
        }

        public int Count(DomainObject<Int32> target)
        {
            return WebProjectsContext.Comment
                .Where(c => c.TargetUniqId == target.UniqID && c.InActive == 0)
                .Count();
        }


        public Comment Save(Comment comment)
        {
            DbComment dbComment = ToDbComment(comment);
            if (ToGuid(dbComment.Id) == default(Guid)) dbComment.Id = Guid.NewGuid().ToString();

            if (!string.IsNullOrWhiteSpace(comment.Content) && comment.Content.Contains("<w:WordDocument>"))
            {
                try
                {
                    dbComment.Content = Sanitizer.GetSafeHtmlFragment(dbComment.Content);
                }
                catch (Exception err)
                {
                    Log.Error(err);
                }
            }
            dbComment.CreateOn = TenantUtil.DateTimeToUtc(dbComment.CreateOn);
            WebProjectsContext.Comment.Add(dbComment);
            WebProjectsContext.SaveChanges();

            comment.ID = dbComment.CommentId;
            return comment;//todo check have id
        }

        public void Delete(Guid id)
        {
            var comment = new DbComment()
            {
                Id = id.ToString()
            };
            WebProjectsContext.Comment.Remove(comment);
            WebProjectsContext.SaveChanges();
        }

        private Comment ToComment(DbComment comment)
        {
            return new Comment
            {
                OldGuidId = ToGuid(comment.Id),
                TargetUniqID = comment.TargetUniqId,
                Content = comment.Content,
                Inactive = Convert.ToBoolean(comment.InActive),
                CreateBy = ToGuid(comment.CreateBy),
                CreateOn = TenantUtil.DateTimeFromUtc(Convert.ToDateTime(comment.CreateOn)),
                Parent = ToGuid(comment.ParentId),
                ID = Convert.ToInt32(comment.CommentId)
            };
        }

        public DbComment ToDbComment(Comment comment)
        {
            return new DbComment
            {
                Id = comment.OldGuidId.ToString(),
                TargetUniqId = comment.TargetUniqID,
                Content = comment.Content,
                InActive = Convert.ToInt32(comment.Inactive),
                CreateBy = comment.CreateBy.ToString(),
                CreateOn = TenantUtil.DateTimeFromUtc(Convert.ToDateTime(comment.CreateOn)),
                ParentId = comment.Parent.ToString(),
                CommentId = Convert.ToInt32(comment.ID)
            };
        }
    }
}