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
using System.Linq;

using ASC.Common;
using ASC.Projects.Core.Domain;
using ASC.Projects.Data.DAO;
using ASC.Projects.EF;

#endregion

namespace ASC.Projects.Core.DataInterfaces
{
    [Scope(typeof(MilestoneDao), typeof(CachedMilestoneDao))]
    public interface IMilestoneDao
    {
        List<Milestone> GetAll();

        List<Milestone> GetByProject(int projectId);

        List<Milestone> GetByStatus(int projectId, MilestoneStatus milestoneStatus);

        List<Milestone> GetUpcomingMilestones(int offset, int max, params int[] projects);

        List<Milestone> GetLateMilestones(int offset, int max);

        List<Milestone> GetByDeadLine(DateTime deadline);

        List<Milestone> GetByFilter(TaskFilter filter, bool isAdmin, bool checkAccess);

        int GetByFilterCount(TaskFilter filter, bool isAdmin, bool checkAccess);

        List<Tuple<Guid, int, int>> GetByFilterCountForReport(TaskFilter filter, bool isAdmin, bool checkAccess);

        List<object[]> GetInfoForReminder(DateTime deadline);

        Milestone GetById(int id);

        List<Milestone> GetById(int[] id);

        bool IsExists(int id);

        Milestone SaveOrUpdate(Milestone milestone);

        void Delete(int id);

        string GetLastModified();

        DbMilestone ToDbMilestone(Milestone milestone);

        List<Milestone> GetMilestones(string text, int projectId, IEnumerable<string> keywords);
    }
}
