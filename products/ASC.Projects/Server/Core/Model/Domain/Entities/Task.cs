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

using System;
using System.Collections.Generic;

using ASC.Projects.Classes;

namespace ASC.Projects.Core.Domain
{
    public class Task : ProjectEntity
    {
        public override EntityType EntityType { get { return EntityType.Task; } }
        public override string ItemPath { get { return "{0}Tasks.aspx?prjID={1}&ID={2}"; } }
        public TaskPriority Priority { get; set; }
        public TaskStatus Status { get; set; }
        public int? CustomTaskStatus { get; set; }
        public int Milestone { get; set; }
        public int SortOrder { get; set; }
        public DateTime Deadline { get; set; }
        public List<Subtask> SubTasks { get; set; }
        public List<Guid> Responsibles { get; set; }
        public List<TaskLink> Links { get; set; }
        public Milestone MilestoneDesc { get; set; }
        public DateTime StatusChangedOn { get; set; }
        public DateTime StartDate { get; set; }
        public TaskSecurityInfo Security { get; set; }
        private int progress;

        public int Progress
        {
            get { return progress; }
            set
            {
                if (value < 0)
                {
                    progress = 0;
                }
                else if (value > 100)
                {
                    progress = 100;
                }
                else
                {
                    progress = value;
                }
            }
        }

        public Task()
        {
            Responsibles = new List<Guid>();
            SubTasks = new List<Subtask>();
            Links = new List<TaskLink>();
        }
    }
}
