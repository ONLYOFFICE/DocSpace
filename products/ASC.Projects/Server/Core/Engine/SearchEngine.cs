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
using System.Globalization;
using System.Linq;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Common.Utils;
using ASC.Common.Web;
using ASC.Files.Core;
using ASC.Projects.Classes;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;
using ASC.Web.Core.Files;
using ASC.Web.Files.Api;

using Autofac;

using Microsoft.Extensions.Options;

using IDaoFactory = ASC.Projects.Core.DataInterfaces.IDaoFactory;

namespace ASC.Projects.Engine
{
    public class SearchItem
    {
        public EntityType EntityType { get; private set; }
        public string ItemPath { get; private set; }
        public string ID { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public DateTime CreateOn { get; private set; }
        public SearchItem Container { get; set; }
        private PathProvider PathProvider { get; set; }
        public SearchItem(EntityType entityType, string id, string title, DateTime createOn, PathProvider pathProvider, SearchItem container = null, string desc = "", string itemPath = "")
        {
            Container = container;
            EntityType = entityType;
            ID = id;
            Title = title;
            Description = desc;
            CreateOn = createOn;
            PathProvider = pathProvider;

            if (!string.IsNullOrEmpty(itemPath))
            {
                ItemPath = ItemPathToAbsolute(itemPath);
            }
            else if (container != null)
            {
                ItemPath = container.ItemPath;
            }
        }

        public SearchItem(ProjectEntity entity, PathProvider pathProvider)
            : this(entity.EntityType, entity.ID.ToString(CultureInfo.InvariantCulture), entity.Title, entity.CreateOn, pathProvider, new SearchItem(entity.Project, pathProvider), entity.Description, entity.ItemPath)
        {
        }

        public SearchItem(Project entity, PathProvider pathProvider)
            : this(entity.EntityType, entity.ID.ToString(CultureInfo.InvariantCulture), entity.Title, entity.CreateOn, pathProvider, desc: entity.Description, itemPath: entity.ItemPath)
        {
        }

        public Dictionary<string, object> GetAdditional()
        {
            var result = new Dictionary<string, object>
                             {
                                 { "Type", EntityType },
                                 { "Hint", LocalizedEnumConverter.ConvertToString(EntityType) }
                             };

            if (Container != null)
            {
                result.Add("ContainerValue", Container.Title);
                result.Add("ContainerTitle", LocalizedEnumConverter.ConvertToString(Container.EntityType));
                result.Add("ContainerPath", Container.ItemPath);
            }

            return result;
        }

        private string ItemPathToAbsolute(string itemPath)
        {
            var projectID = ID;
            var container = Container;

            while (true)
            {
                if (container == null) break;

                projectID = container.ID;

                container = container.Container;
            }

            return string.Format(itemPath, VirtualPathUtility.ToAbsolute(PathProvider.BaseVirtualPath), projectID, ID);
        }
    }

    [Scope]
    public class SearchEngine
    {
        public Files.Core.IDaoFactory DaoFilesFactory { get; set; }
        public ProjectSecurity ProjectSecurity { get; set; }
        public FilesIntegration FilesIntegration { get; set; }
        public FilesLinkUtility FilesLinkUtility { get; set; }
        public FileUtility FileUtility { get; set; }
        public Web.Files.Classes.PathProvider PathProvider { get; set; }
        public PathProvider PathProviderProjects { get; set; }
        public EngineFactory EngineFactory { get; set; }
        public ILog Log { get; set; }
        public IDaoFactory DaoFactory { get; set; }

        private readonly List<SearchItem> searchItems;

        public SearchEngine(EngineFactory engineFactory, IOptionsMonitor<ILog> options, ProjectSecurity projectSecurity, IDaoFactory daoFactory, Files.Core.IDaoFactory daoFilesFactory, FilesIntegration filesIntegration, FilesLinkUtility filesLinkUtility, FileUtility fileUtility, Web.Files.Classes.PathProvider pathProvider, PathProvider pathProviderProjects)
        {
            Log = options.CurrentValue;
            ProjectSecurity = projectSecurity;
            searchItems = new List<SearchItem>();
            DaoFilesFactory = daoFilesFactory;
            FilesIntegration = filesIntegration;
            FilesLinkUtility = filesLinkUtility;
            FileUtility = fileUtility;
            PathProvider = pathProvider;
            PathProviderProjects = pathProviderProjects;
            EngineFactory = engineFactory;
            DaoFactory = daoFactory;
        }

        public IEnumerable<SearchItem> Search(string searchText, int projectId = 0)
        {
            var queryResult = DaoFactory.GetSearchDao().Search(searchText, projectId);

            foreach (var r in queryResult)
            {
                switch (r.EntityType)
                {
                    case EntityType.Project:
                        var project = (Project)r;
                        if (ProjectSecurity.CanRead(project))
                        {
                            searchItems.Add(new SearchItem(project, PathProviderProjects));
                        }
                        continue;
                    case EntityType.Milestone:
                        var milestone = (Milestone)r;
                        if (ProjectSecurity.CanRead(milestone))
                        {
                            searchItems.Add(new SearchItem(milestone, PathProviderProjects));
                        }
                        continue;
                    case EntityType.Message:
                        var message = (Message)r;
                        if (ProjectSecurity.CanRead(message))
                        {
                            searchItems.Add(new SearchItem(message, PathProviderProjects));
                        }
                        continue;
                    case EntityType.Task:
                        var task = (Task)r;
                        if (ProjectSecurity.CanRead(task))
                        {
                            searchItems.Add(new SearchItem(task, PathProviderProjects));
                        }
                        continue;
                    case EntityType.Comment:
                        var comment = (Comment)r;
                        var entity = EngineFactory.GetCommentEngine().GetEntityByTargetUniqId(comment);
                        if (entity == null) continue;
                        searchItems.Add(new SearchItem(comment.EntityType,
                            comment.ID.ToString(CultureInfo.InvariantCulture), HtmlUtil.GetText(comment.Content),
                            comment.CreateOn, PathProviderProjects, new SearchItem(entity, PathProviderProjects)));
                        continue;
                    case EntityType.SubTask:
                        var subtask = (Subtask)r;
                        var parentTask = EngineFactory.GetTaskEngine().GetByID(subtask.Task);
                        if (parentTask == null) continue;

                        searchItems.Add(new SearchItem(subtask.EntityType,
                            subtask.ID.ToString(CultureInfo.InvariantCulture), subtask.Title, subtask.CreateOn, PathProviderProjects,
                            new SearchItem(parentTask, PathProviderProjects)));
                        continue;
                }
            }

            try
            {
                // search in files
                var fileEntries = new List<FileEntry<int>>();
                var folderDao = DaoFilesFactory.GetFolderDao<int>();
                var fileDao = DaoFilesFactory.GetFileDao<int>();
                fileEntries.AddRange(folderDao.SearchFolders(searchText, true));
                fileEntries.AddRange(fileDao.Search(searchText, true));

                var projectIds = projectId != 0
                                        ? new List<int> { projectId }
                                        : fileEntries.GroupBy(f => f.RootFolderId)
                                            .Select(g => folderDao.GetFolder(g.Key))
                                            .Select(f => f != null ? folderDao.GetBunchObjectID(f.RootFolderId).Split('/').Last() : null)
                                            .Where(s => !string.IsNullOrEmpty(s))
                                            .Select(int.Parse);

                var rootProject = projectIds.ToDictionary(id => FilesIntegration.RegisterBunch<int>("projects", "project", id.ToString(CultureInfo.InvariantCulture)));
                fileEntries.RemoveAll(f => !rootProject.ContainsKey(f.RootFolderId));


                foreach (var f in fileEntries)
                {
                    var security = FilesIntegration.GetFileSecurity(folderDao.GetBunchObjectID(f.RootFolderId));
                    var id = rootProject[f.RootFolderId];
                    var project = DaoFactory.GetProjectDao().GetById(id);

                    if (ProjectSecurity.CanReadFiles(project))
                    {
                        var itemId = f.FileEntryType == FileEntryType.File
                                            ? FilesLinkUtility.GetFileWebPreviewUrl(FileUtility, f.Title, f.ID)
                                            : PathProvider.GetFolderUrl((Folder<int>)f, project.ID);
                        searchItems.Add(new SearchItem(EntityType.File, itemId, f.Title, f.CreateOn, PathProviderProjects, new SearchItem(project, PathProviderProjects), itemPath: "{2}"));
                    }
                }
            }
            catch (Exception err)
            {
                Log.Error(err);
            }
            return searchItems;
        }
    }
}