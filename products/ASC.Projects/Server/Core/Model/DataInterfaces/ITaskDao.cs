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
 * Pursuant to Section 7 � 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 � 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/

#region Usings

using System;
using System.Collections.Generic;

using ASC.Common;
using ASC.Projects.Core.Domain;
using ASC.Projects.Data.DAO;
using ASC.Projects.EF;

#endregion

namespace ASC.Projects.Core.DataInterfaces
{
    [Scope(typeof(TaskDao), typeof(CachedTaskDao))]
    public interface ITaskDao
    {
        List<Task> GetAll();

        List<Task> GetByProject(int projectId, TaskStatus? status, Guid participant);

        List<Task> GetByResponsible(Guid responsibleId, IEnumerable<TaskStatus> statuses);

        List<Task> GetMilestoneTasks(int milestoneId);

        List<Task> GetByFilter(TaskFilter filter, bool isAdmin, bool checkAccess);

        TaskFilterCountOperationResult GetByFilterCount(TaskFilter filter, bool isAdmin, bool checkAccess);

        List<Tuple<Guid, int, int>> GetByFilterCountForReport(TaskFilter filter, bool isAdmin, bool checkAccess);

        IEnumerable<TaskFilterCountOperationResult> GetByFilterCountForStatistic(TaskFilter filter, bool isAdmin, bool checkAccess);

        List<Task> GetById(ICollection<int> ids);

        Task GetById(int id);

        bool IsExists(int id);

        List<object[]> GetTasksForReminder(DateTime deadline);


        Task Create(Task task);
        Task Update(Task task);

        void Delete(Task task);


        void SaveRecurrence(Task task, string cron, DateTime startDate, DateTime endDate);

        void DeleteReccurence(int taskId);

        List<object[]> GetRecurrence(DateTime date);


        void AddLink(TaskLink link);

        void RemoveLink(TaskLink link);

        IEnumerable<TaskLink> GetLinks(int taskID);

        IEnumerable<TaskLink> GetLinks(List<Task> tasks);

        bool IsExistLink(TaskLink link);
        DbTask ToDbTask(Task task);

        List<Task> GetTasks(string text, int projectId, IEnumerable<string> keywords);
    }
}
