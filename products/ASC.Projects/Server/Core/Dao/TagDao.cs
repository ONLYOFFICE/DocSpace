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
using System.Data;
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
    public class TagDao : BaseDao, ITagDao
    {
        public TagDao(SecurityContext securityContext, DbContextManager<WebProjectsContext> dbContextManager, TenantManager tenantManager) : base(securityContext, dbContextManager, tenantManager)
        {
        }

        public string GetById(int id)
        {
            return WebProjectsContext.Tag
                .Where(t => t.Id == id)
                .Select(t=> t.Title)
                .SingleOrDefault();
        }

        public KeyValuePair<int, string> Create(string data)
        {
            var tag = new DbTag()
            {
                Title = data,
                LastModifiedOn = DateTime.UtcNow,
                TenantId = Tenant
            };
            WebProjectsContext.Tag.Add(tag);
            WebProjectsContext.SaveChanges();
            return new KeyValuePair<int, string>(tag.Id, data);
        }

        public Dictionary<int, string> GetTags()
        {
            return WebProjectsContext.Tag
                .Where(t=> t.TenantId == Tenant)
                .OrderBy(t=> t.Title)
                .ToDictionary(t=> t.Id, t=> t.Title);
        }

        public Dictionary<int, string> GetTags(string prefix)
        {
            return WebProjectsContext.Tag
                .Where(t => t.Title.Contains(prefix))
                .OrderBy(t => t.Title)
                .ToDictionary(t => t.Id, t => t.Title);

        }

        public int[] GetTagProjects(string tagName)
        {
            return WebProjectsContext.Tag.Join(WebProjectsContext.TagToProject,
                t => t.Id,
                tp => tp.TagId,
                (t, tp) => new
                {
                    Tag = t,
                    TagToProject = tp
                }).Where(q => q.Tag.TenantId == Tenant && tagName.ToLower() == q.Tag.Title.ToLower())
                .Select(q=> q.TagToProject.ProjectId).ToArray();
        }

        public int[] GetTagProjects(int tagID)
        {
            return WebProjectsContext.TagToProject.Where(tp => tp.TagId == tagID)
                .Select(tp => tp.ProjectId).ToArray();
        }

        public Dictionary<int, string> GetProjectTags(int projectId)
        {
            return WebProjectsContext.Tag.Join(WebProjectsContext.TagToProject,
                t => t.Id,
                tp => tp.TagId,
                (t, tp) => new
                {
                    Tag = t,
                    TagToProject = tp
                }).Where(q => q.TagToProject.ProjectId == projectId)
                .OrderBy(q => q.Tag.Title)
                .ToDictionary(q => q.Tag.Id, q => q.Tag.Title);
        }

        public void SetProjectTags(int projectId, string[] tags) // check 2 time!!!!
        {
            var tagsToDelete = WebProjectsContext.TagToProject.Where(tp => tp.ProjectId == projectId).ToList();
            WebProjectsContext.TagToProject.RemoveRange(tagsToDelete);

            foreach (var tag in tagsToDelete)
            {
                    var tagToDelete = WebProjectsContext.Tag
                        .Join(WebProjectsContext.TagToProject,
                        t=> t.Id,
                        tp=> tp.TagId,
                        (t, tp)=> new
                        {
                            Tag = t,
                            TagToProject = tp
                        }).Where(q => q.Tag.Id == tag.TagId && q.TagToProject.ProjectId == 0)
                        .Select(q=> q.Tag).SingleOrDefault();   
                    WebProjectsContext.Remove(tagToDelete);
            }


            foreach (var tag in tags)
            {
                var tagId = WebProjectsContext.Tag.Where(t => t.Title.ToLower() == tag.ToLower())
                    .Select(t=> t.Id)
                    .SingleOrDefault();
                if (tagId == 0)
                {
                    var t = new DbTag()
                    {
                        Title = tag,
                        LastModifiedOn = DateTime.UtcNow
                    };
                    WebProjectsContext.Tag.Add(t);
                }
                var tp = new DbTagToProject()
                {
                    TagId = tagId,
                    ProjectId = projectId
                };
                WebProjectsContext.TagToProject.Add(tp);
            }
            WebProjectsContext.SaveChanges();
        }

        public void SetProjectTags(int projectId, IEnumerable<int> tags)
        {
            var tp = WebProjectsContext.TagToProject.Where(tp => tp.ProjectId == projectId).ToList();
            WebProjectsContext.TagToProject.RemoveRange(tp);

            var tagsToDelete = WebProjectsContext.Tag.Join(WebProjectsContext.TagToProject,
                t => t.Id,
                tp => tp.TagId,
                (t, tp) => new
                {
                    Tag = t,
                    TagToProject = tp
                }).Where(q => q.TagToProject.TagId == null)
                .Select(q => q.Tag.Id)
                .Distinct()
                .ToList();


            foreach (var tag in tagsToDelete.Except(tags))
            {
                var tagToDelete = WebProjectsContext.Tag
                        .Join(WebProjectsContext.TagToProject,
                        t => t.Id,
                        tp => tp.TagId,
                        (t, tp) => new
                        {
                            Tag = t,
                            TagToProject = tp
                        }).Where(q => q.Tag.Id == tag && q.TagToProject.ProjectId == 0)
                        .Select(q => q.Tag).SingleOrDefault();
                WebProjectsContext.Remove(tagToDelete);
            }

            if (tags.Any())
            {
                foreach (var t in tags)
                {
                    var insert = new DbTagToProject()
                    {
                        TagId = t,
                        ProjectId = projectId
                    };
                    WebProjectsContext.TagToProject.Add(insert);
                }
            }
            WebProjectsContext.SaveChanges();
        }
    }
}
