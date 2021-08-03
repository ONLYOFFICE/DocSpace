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

using ASC.Common;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;

using Autofac;

namespace ASC.Projects.Core.Engine
{
    [Scope]
    public class ProjectsReassign
    {
        private List<Project> FromUserProjects { get; set; }
        private List<Project> ToUserProjects { get; set; }
        private EngineFactory EngineFactory { get; set; }

        public ProjectsReassign(EngineFactory engineFactory)
        {
            FromUserProjects = new List<Project>();
            EngineFactory = engineFactory;
        }

        public void Reassign(Guid fromUserId, Guid toUserId)
        {
            FromUserProjects = EngineFactory.GetProjectEngine().GetByParticipant(fromUserId).ToList();
            ToUserProjects = EngineFactory.GetProjectEngine().GetByParticipant(toUserId).ToList();

            ReplaceTeam(fromUserId, toUserId);
            ReassignProjectManager(fromUserId, toUserId);
            ReassignMilestones(fromUserId, toUserId);
            ReassignTasks(fromUserId, toUserId);
            ReassignSubtasks(fromUserId, toUserId);
        }

        private void ReplaceTeam(Guid fromUserId, Guid toUserId)
        {
            foreach (var project in FromUserProjects)
            {
                var teamSecurity = EngineFactory.GetProjectEngine().GetTeamSecurity(project, fromUserId);

                if (!ToUserProjects.Exists(r => r.ID == project.ID))
                {
                    EngineFactory.GetProjectEngine().AddToTeam(project, toUserId, false);
                    EngineFactory.GetProjectEngine().SetTeamSecurity(project, toUserId, teamSecurity);
                }

                EngineFactory.GetProjectEngine().RemoveFromTeam(project, fromUserId, false);
            }
        }

        private void ReassignProjectManager(Guid fromUserId, Guid toUserId)
        {
            var filter = new TaskFilter { UserId = fromUserId, ProjectStatuses = new List<ProjectStatus> { ProjectStatus.Open, ProjectStatus.Paused } };
            var projects = EngineFactory.GetProjectEngine().GetByFilter(filter);

            foreach (var project in projects)
            {
                project.Responsible = toUserId;
                EngineFactory.GetProjectEngine().SaveOrUpdate(project, false);
            }
        }

        private void ReassignMilestones(Guid fromUserId, Guid toUserId)
        {
            var filter = new TaskFilter { UserId = fromUserId, MilestoneStatuses = new List<MilestoneStatus> { MilestoneStatus.Open } };
            var milestones = EngineFactory.GetMilestoneEngine().GetByFilter(filter);

            foreach (var milestone in milestones)
            {
                AddToTeam(milestone.Project, toUserId);
                milestone.Responsible = toUserId;
                EngineFactory.GetMilestoneEngine().SaveOrUpdate(milestone, false);
            }
        }

        private void ReassignTasks(Guid fromUserId, Guid toUserId)
        {
            var tasks = EngineFactory.GetTaskEngine().GetByResponsible(fromUserId, TaskStatus.Open);

            foreach (var task in tasks.Where(r => r.Responsibles.Any()))
            {
                AddToTeam(task.Project, toUserId);
                task.Responsibles = task.Responsibles.Where(r => r != fromUserId).ToList();
                task.Responsibles.Add(toUserId);
                EngineFactory.GetTaskEngine().SaveOrUpdate(task, null, false);
            }
        }

        private void ReassignSubtasks(Guid fromUserId, Guid toUserId)
        {
            var tasks = EngineFactory.GetSubtaskEngine().GetByResponsible(fromUserId, TaskStatus.Open);

            foreach (var task in tasks)
            {
                AddToTeam(task.Project, toUserId);
                foreach (var subtask in task.SubTasks)
                {
                    subtask.Responsible = toUserId;
                    EngineFactory.GetSubtaskEngine().SaveOrUpdate(subtask, task);
                }
            }
        }

        private void AddToTeam(Project project, Guid userId)
        {
            if (!FromUserProjects.Exists(r => r.ID == project.ID) && !ToUserProjects.Exists(r => r.ID == project.ID))
            {
                EngineFactory.GetProjectEngine().AddToTeam(project, userId, false);
                ToUserProjects.Add(project);
            }
        }
    }
}