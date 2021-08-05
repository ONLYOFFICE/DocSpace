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
                 .ToList()
                 .ConvertAll(c => ToComment(c));
        }

        public Comment GetById(Guid id)
        {
            var dbComment =  WebProjectsContext.Comment
                .Where(c => c.Id == id.ToString())
                .SingleOrDefault();
            return ToComment(dbComment);
        }

        public List<int> Count(List<ProjectEntity> targets)
        {
            var target = targets.ConvertAll(target => target.UniqID);
            var query = WebProjectsContext.Comment
                .Where(c => target.Contains(c.TargetUniqId) && c.InActive == 0)
                .AsEnumerable()
                .GroupBy(c => c.TargetUniqId)
                .ToList();
            return targets.ConvertAll(
                target =>
                {
                    var pair = query.Find(q => Equals(q.Key, target.UniqID));
                    return pair == null ? 0 : pair.Count();
                });
        }

        public int Count(DomainObject<Int32> target)
        {
            return WebProjectsContext.Comment
                .Where(c => c.TargetUniqId == target.UniqID && c.InActive == 0)
                .Count();
        }


        public Comment SaveOrUpdate(Comment comment)
        {
            if (comment.OldGuidId == default(Guid)) comment.OldGuidId = Guid.NewGuid();

            if (!string.IsNullOrWhiteSpace(comment.Content) && comment.Content.Contains("<w:WordDocument>"))
            {
                try
                {
                    comment.Content = Sanitizer.GetSafeHtmlFragment(comment.Content);
                }
                catch (Exception err)
                {
                    Log.Error(err);
                }
            }
            comment.CreateOn = TenantUtil.DateTimeToUtc(comment.CreateOn);
            if (WebProjectsContext.Comment.Where(c => c.Id == comment.OldGuidId.ToString()).Any())
            {
                var db = WebProjectsContext.Comment.Where(c => c.Id == comment.OldGuidId.ToString()).SingleOrDefault();
                db.TargetUniqId = comment.TargetUniqID;
                db.Content = comment.Content;
                db.InActive = Convert.ToInt32(comment.Inactive);
                db.CreateBy = comment.CreateBy.ToString();
                db.CreateOn = TenantUtil.DateTimeFromUtc(Convert.ToDateTime(comment.CreateOn));
                db.ParentId = comment.Parent.ToString();
                WebProjectsContext.Comment.Update(db);
                WebProjectsContext.SaveChanges();
                return ToComment(db);
            }
            else
            {
                DbComment dbComment = ToDbComment(comment);
                WebProjectsContext.Comment.Add(dbComment);
                WebProjectsContext.SaveChanges();
                comment.ID = dbComment.CommentId;
                return comment;
            }
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
                CommentId = Convert.ToInt32(comment.ID),
                TenantId = Tenant
            };
        }
    }
}