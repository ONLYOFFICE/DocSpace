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
using ASC.Common.Utils;
using ASC.Core;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;
using ASC.Web.Core.Calendars;

using Autofac;

namespace ASC.Projects.Calendars
{
    [Scope]
    public sealed class ProjectCalendar : BaseCalendar
    {
        [AllDayLongUTC]
        private class Event : BaseEvent
        {
        }

        private Project project;
        private bool following;
        private TenantManager TenantManager { get; set; }
        private EngineFactory EngineFactory { get; set; }
        public ProjectCalendar(AuthContext authContext, TimeZoneConverter timeZoneConverter, EngineFactory engineFactory, TenantManager tenantManager) :base(authContext, timeZoneConverter )
        {
            TenantManager = tenantManager;
            EngineFactory = engineFactory;
        }

        public ProjectCalendar Init(Project project, string backgroundColor, string textColor, SharingOptions sharingOptions, bool following)
        {
            this.project = project;
            this.following = following;

            Context.HtmlBackgroundColor = backgroundColor;
            Context.HtmlTextColor = textColor;
            Context.CanChangeAlertType = false;
            Context.CanChangeTimeZone = false;
            Context.GetGroupMethod = () => Projects.Resources.ProjectsCommonResource.ProductName;
            Id = this.project.UniqID;
            EventAlertType = EventAlertType.Hour;
            Name = this.project.Title;
            Description = this.project.Description;
            SharingOptions = sharingOptions;
            return this;
        }

        public override List<IEvent> LoadEvents(Guid userId, DateTime startDate, DateTime endDate)
        {
            var tasks = new List<Task>();
            if (!following)
                tasks.AddRange(EngineFactory.GetTaskEngine().GetByProject(project.ID, TaskStatus.Open, userId));

            var milestones = EngineFactory.GetMilestoneEngine().GetByStatus(project.ID, MilestoneStatus.Open);

            var events = milestones
                .Select(m => new Event
                {
                    AlertType = EventAlertType.Never,
                    CalendarId = Id,
                    UtcStartDate = m.DeadLine,
                    UtcEndDate = m.DeadLine,
                    AllDayLong = true,
                    Id = m.UniqID,
                    Name = Projects.Resources.MilestoneResource.Milestone + ": " + m.Title,
                    Description = m.Description
                })
                .Where(e => IsVisibleEvent(startDate, endDate, e.UtcStartDate, e.UtcEndDate))
                .Cast<IEvent>()
                .ToList();

            foreach (var t in tasks)
            {
                var start = t.StartDate;

                if (!t.Deadline.Equals(DateTime.MinValue))
                {
                    start = start.Equals(DateTime.MinValue) ? t.Deadline : t.StartDate;
                }
                else
                {
                    start = DateTime.MinValue;
                }

                var projectEvent = new Event
                {
                    AlertType = EventAlertType.Never,
                    CalendarId = Id,
                    UtcStartDate = start,
                    UtcEndDate = t.Deadline,
                    AllDayLong = true,
                    Id = t.UniqID,
                    Name = Projects.Resources.TaskResource.Task + ": " + t.Title,
                    Description = t.Description
                };

                if (IsVisibleEvent(startDate, endDate, projectEvent.UtcStartDate, projectEvent.UtcEndDate))
                    events.Add(projectEvent);
            }

            return events;
        }

        public override TimeZoneInfo TimeZone
        {
            get { return TimeZoneInfo.FindSystemTimeZoneById(TenantManager.GetCurrentTenant().TimeZone); }
        }

        private bool IsVisibleEvent(DateTime startDate, DateTime endDate, DateTime eventStartDate, DateTime eventEndDate)
        {
            return (startDate <= eventStartDate && eventStartDate <= endDate) ||
                   (startDate <= eventEndDate && eventEndDate <= endDate) ||
                   (eventStartDate < startDate && eventEndDate > endDate);
        }
    }
}