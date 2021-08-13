/*
 *
 * (c) Copyright Ascensio System Limited 2010-2021
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;

using ASC.Common;
using ASC.Core;
using ASC.Core.Users;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;
using ASC.Projects.Core.Domain.Reports;
using ASC.Web.Core.Users;

using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.Projects.Engine
{
    [Scope]
    public class ReportEngine
    {
        private ProjectSecurity ProjectSecurity { get; set; }
        private SecurityContext SecurityContext { get; set; }
        private UserManager UserManager { get; set; }
        private DisplayUserSettingsHelper DisplayUserSettingsHelper { get; set; }
        private EngineFactory EngineFactory { get; set; }
        private IDaoFactory DaoFactory { get; set; }

        public ReportEngine(IDaoFactory daoFactory, ProjectSecurity projectSecurity, EngineFactory engineFactory, SecurityContext securityContext, UserManager userManager, DisplayUserSettingsHelper displayUserSettingsHelper)
        {
            ProjectSecurity = projectSecurity;
            SecurityContext = securityContext;
            UserManager = userManager;
            DisplayUserSettingsHelper = displayUserSettingsHelper;
            EngineFactory = engineFactory;
            DaoFactory = daoFactory;
        }

        public List<ReportTemplate> GetTemplates(Guid userId)
        {
            if (ProjectSecurity.IsVisitor(SecurityContext.CurrentAccount.ID)) throw new SecurityException("Access denied.");

            return DaoFactory.GetReportDao().GetTemplates(userId);
        }

        public List<ReportTemplate> GetAutoTemplates()
        {
            if (ProjectSecurity.IsVisitor(SecurityContext.CurrentAccount.ID)) throw new SecurityException("Access denied.");

            return DaoFactory.GetReportDao().GetAutoTemplates();
        }

        public ReportTemplate GetTemplate(int id)
        {
            if (ProjectSecurity.IsVisitor(SecurityContext.CurrentAccount.ID)) throw new SecurityException("Access denied.");

            return DaoFactory.GetReportDao().GetTemplate(id);
        }

        public ReportTemplate SaveTemplate(ReportTemplate template)
        {
            if (ProjectSecurity.IsVisitor(SecurityContext.CurrentAccount.ID)) throw new SecurityException("Access denied.");

            return DaoFactory.GetReportDao().SaveTemplate(template);
        }

        public ReportFile Save(ReportFile reportFile)
        {
            if (ProjectSecurity.IsVisitor(SecurityContext.CurrentAccount.ID)) throw new SecurityException("Access denied.");

            return DaoFactory.GetReportDao().Save(reportFile);
        }

        public void DeleteTemplate(int id)
        {
            if (ProjectSecurity.IsVisitor(SecurityContext.CurrentAccount.ID)) throw new SecurityException("Access denied.");

            DaoFactory.GetReportDao().DeleteTemplate(id);
        }

        public IEnumerable<ReportFile> Get()
        {
            if (ProjectSecurity.IsVisitor(SecurityContext.CurrentAccount.ID)) throw new SecurityException("Access denied.");

            return DaoFactory.GetReportDao().Get();
        }

        public ReportFile GetByFileId(int fileid)
        {
            if (ProjectSecurity.IsVisitor(SecurityContext.CurrentAccount.ID)) throw new SecurityException("Access denied.");

            return DaoFactory.GetReportDao().GetByFileId(fileid);
        }

        public void Remove(ReportFile report)
        {
            if (ProjectSecurity.IsVisitor(SecurityContext.CurrentAccount.ID)) throw new SecurityException("Access denied.");

            DaoFactory.GetReportDao().Remove(report);

            EngineFactory.GetFileEngine().MoveToTrash(report.FileId);
        }

        public IList<object[]> BuildUsersWithoutActiveTasks(TaskFilter filter)
        {
            var result = new List<object[]>();

            var users = new List<Guid>();
            if (filter.UserId != Guid.Empty) users.Add(filter.UserId);
            else if (filter.DepartmentId != Guid.Empty)
            {
                users.AddRange(UserManager.GetUsersByGroup(filter.DepartmentId).Select(u => u.ID));
            }
            else if (filter.HasProjectIds)
            {
                users.AddRange(EngineFactory.GetProjectEngine().GetTeam(filter.ProjectIds).Select(r => r.ID).Distinct());
            }
            else if (!filter.HasProjectIds)
            {
                users.AddRange(EngineFactory.GetProjectEngine().GetTeam(EngineFactory.GetProjectEngine().GetAll().Select(r => r.ID).ToList()).Select(r => r.ID).Distinct());
            }

            var data = EngineFactory.GetTaskEngine().GetByFilterCountForStatistic(filter);

            foreach (var row in data)
            {
                users.Remove(row.UserId);
                if (row.TasksOpen == 0)
                {
                    result.Add(new object[]
                    {
                        row.UserId, 0, row.TasksOpen, row.TasksClosed
                    });
                }
            }
            result.AddRange(users.Select(u => new object[] { u, 0, 0, 0 }));

            return result.Select(x => new[] { DisplayUserSettingsHelper.GetFullUserName(UserManager.GetUsers((Guid)x[0]), false), x[2], x[3] }).ToList();
        }

        public IList<object[]> BuildUsersWorkload(TaskFilter filter)
        {
            if (filter.ViewType == 0)
            {
                return EngineFactory.GetTaskEngine().GetByFilterCountForStatistic(filter).Select(r => new object[]
                {
                    DisplayUserSettingsHelper.GetFullUserName(UserManager.GetUsers(r.UserId), false), r.TasksOpen, r.TasksClosed, r.TasksTotal,
                    UserManager.GetUserGroups(r.UserId).Select(x=>x.Name)
                }).ToList();
            }

            var tasks = EngineFactory.GetTaskEngine().GetByFilter(filter).FilterResult;
            var projects = tasks.Select(r => r.Project).Distinct();

            var result = new List<object[]>();

            foreach (var pr in projects)
            {
                var prTasks = tasks.Where(r => r.Project.ID == pr.ID && r.Responsibles.Count > 0).ToList();
                if (!prTasks.Any())
                {
                    continue;
                }

                var users = filter.ParticipantId.HasValue ? new List<Guid> { filter.ParticipantId.Value } : prTasks.SelectMany(r => r.Responsibles).Distinct();

                var usersResult = new List<object[]>();
                foreach (var user in users)
                {
                    var tasksOpened = prTasks.Count(r => r.Responsibles.Contains(user) && r.Status == TaskStatus.Open);
                    var tasksClosed = prTasks.Count(r => r.Responsibles.Contains(user) && r.Status == TaskStatus.Closed);

                    usersResult.Add(new object[] {
                        DisplayUserSettingsHelper.GetFullUserName(UserManager.GetUsers(user), false),
                        tasksOpened,
                        tasksClosed,
                        tasksOpened + tasksClosed
                    });
                }

                result.Add(new object[] { pr.Title, usersResult });
            }

            return result;
        }

        public IList<object[]> BuildUsersActivity(TaskFilter filter)
        {
            var result = new List<object[]>();
            var tasks = EngineFactory.GetTaskEngine().GetByFilterCountForReport(filter);
            var milestones = EngineFactory.GetMilestoneEngine().GetByFilterCountForReport(filter);
            var messages = EngineFactory.GetMessageEngine().GetByFilterCountForReport(filter);

            if (filter.ViewType == 1)
            {
                var projectIds = GetProjects(tasks, milestones, messages);
                var projects = EngineFactory.GetProjectEngine().GetByID(projectIds).ToList();

                foreach (var p in projects)
                {
                    var userIds = GetUsers(p.ID, tasks, milestones, messages);

                    foreach (var userId in userIds)
                    {
                        var userName = UserManager.GetUsers(userId).DisplayUserName(DisplayUserSettingsHelper);
                        var tasksCount = GetCount(tasks, p.ID, userId);
                        var milestonesCount = GetCount(milestones, p.ID, userId);
                        var messagesCount = GetCount(messages, p.ID, userId);

                        result.Add(new object[]
                        {
                            p.ID, p.Title, userName, tasksCount, milestonesCount, messagesCount,
                            tasksCount + milestonesCount + messagesCount
                        });
                    }
                }
            }
            else
            {
                var userIds = GetUsers(-1, tasks, milestones, messages);

                foreach (var userId in userIds)
                {
                    var group = UserManager.GetUserGroups(userId).FirstOrDefault();
                    var userName = UserManager.GetUsers(userId).DisplayUserName(DisplayUserSettingsHelper);

                    var tasksCount = GetCount(tasks, userId);
                    var milestonesCount = GetCount(milestones, userId);
                    var messagesCount = GetCount(messages, userId);

                    result.Add(new object[]
                        {
                            group != null ? group.ID : Guid.Empty,
                            group != null ? group.Name : "", userName,
                            tasksCount,
                            milestonesCount,
                            messagesCount,
                            tasksCount + milestonesCount + messagesCount
                        });
                }
            }

            return result;
        }

        private static List<int> GetProjects(params IEnumerable<Tuple<Guid, int, int>>[] data)
        {
            var result = new List<int>();

            foreach (var item in data)
            {
                result.AddRange(item.Select(r => r.Item2));
            }

            return result.Distinct().ToList();
        }

        private static List<Guid> GetUsers(int pId, params IEnumerable<Tuple<Guid, int, int>>[] data)
        {
            var result = new List<Guid>();

            foreach (var item in data)
            {
                result.AddRange(item.Where(r => pId == -1 || r.Item2 == pId).Select(r => r.Item1));
            }

            return result.Distinct().ToList();
        }

        private static int GetCount(IEnumerable<Tuple<Guid, int, int>> data, Guid userId)
        {
            return data.Where(r => r.Item1 == userId).Sum(r => r.Item3);
        }
        private static int GetCount(IEnumerable<Tuple<Guid, int, int>> data, int pId, Guid userId)
        {
            var count = 0;
            var item = data.FirstOrDefault(r => r.Item2 == pId && r.Item1 == userId);
            if (item != null)
            {
                count = item.Item3;
            }

            return count;
        }
    }
}
