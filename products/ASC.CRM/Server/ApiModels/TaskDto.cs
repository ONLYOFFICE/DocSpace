/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
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


using ASC.Api.Core;
using ASC.Common;
using ASC.CRM.Core;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.Web.Api.Models;
using System;
using System.Runtime.Serialization;

namespace ASC.CRM.ApiModels
{
    /// <summary>
    ///  Task
    /// </summary>
    [DataContract(Name = "task", Namespace = "")]
    public class TaskDto
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        
        public EmployeeWraper CreateBy { get; set; }

        
        public ApiDateTime Created { get; set; }

        
        public ContactBaseWithEmailDto Contact { get; set; }

        
        public String Title { get; set; }

        
        public String Description { get; set; }

        
        public ApiDateTime DeadLine { get; set; }

        
        public int AlertValue { get; set; }

        
        public EmployeeWraper Responsible { get; set; }

        
        public bool IsClosed { get; set; }

        
        public TaskCategoryBaseDto Category { get; set; }

        
        public EntityDto Entity { get; set; }

        
        public bool CanEdit { get; set; }

        public static TaskDto GetSample()
        {
            return new TaskDto
            {
                Created = ApiDateTime.GetSample(),
                CreateBy = EmployeeWraper.GetSample(),
                DeadLine = ApiDateTime.GetSample(),
                IsClosed = false,
                Responsible = EmployeeWraper.GetSample(),
                //                 Category = TaskCategoryBaseDto.GetSample(),
                CanEdit = true,
                Title = "Send a commercial offer",
                AlertValue = 0
            };
        }
    }

    [DataContract(Name = "taskBase", Namespace = "")]
    public class TaskBaseDto
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        public TaskBaseDto()
        {

        }


        
        public String Title { get; set; }

        
        public String Description { get; set; }

        
        public ApiDateTime DeadLine { get; set; }

        
        public int AlertValue { get; set; }

        
        public EmployeeWraper Responsible { get; set; }

        
        public bool IsClosed { get; set; }

        
        public TaskCategoryBaseDto Category { get; set; }

        
        public EntityDto Entity { get; set; }

        
        public bool CanEdit { get; set; }

        public static TaskBaseDto GetSample()
        {
            return new TaskBaseDto
            {
                DeadLine = ApiDateTime.GetSample(),
                IsClosed = false,
                Responsible = EmployeeWraper.GetSample(),
                Category = TaskCategoryBaseDto.GetSample(),
                CanEdit = true,
                Title = "Send a commercial offer",
                AlertValue = 0
            };
        }
    }

    [Scope]
    public class TaskDtoHelper
    {
        public TaskDtoHelper(ApiDateTimeHelper apiDateTimeHelper,
                                 EmployeeWraperHelper employeeWraperHelper,
                                 CRMSecurity cRMSecurity,
                                 DaoFactory daoFactory,
                                 ContactDtoHelper contactBaseDtoHelper,
                                 EntityDtoHelper entityDtoHelper)
        {
            ApiDateTimeHelper = apiDateTimeHelper;
            EmployeeWraperHelper = employeeWraperHelper;
            CRMSecurity = cRMSecurity;
            DaoFactory = daoFactory;
            ContactBaseDtoHelper = contactBaseDtoHelper;
            EntityDtoHelper = entityDtoHelper;
        }

        public ContactDtoHelper ContactBaseDtoHelper { get; }
        public CRMSecurity CRMSecurity { get; }
        public ApiDateTimeHelper ApiDateTimeHelper { get; }
        public EmployeeWraperHelper EmployeeWraperHelper { get; }
        public DaoFactory DaoFactory { get; }
        public EntityDtoHelper EntityDtoHelper { get; }

        public TaskBaseDto GetTaskBaseDto(Task task)
        {
            return new TaskBaseDto { 
                Title = task.Title,
                Description = task.Description,
                DeadLine = ApiDateTimeHelper.Get(task.DeadLine),
                Responsible = EmployeeWraperHelper.Get(task.ResponsibleID),
                IsClosed = task.IsClosed,
                AlertValue = task.AlertValue
            };
        }

        public TaskDto GetTaskDto(Task task)
        {
            var result = new TaskDto
            {
                Title = task.Title,
                Description = task.Description,
                DeadLine = ApiDateTimeHelper.Get(task.DeadLine),
                Responsible = EmployeeWraperHelper.Get(task.ResponsibleID),
                IsClosed = task.IsClosed,
                AlertValue = task.AlertValue
            };

            if (task.CategoryID > 0)
            {
                var categoryItem = DaoFactory.GetListItemDao().GetByID(task.CategoryID);

                result.Category = new TaskCategoryDto(categoryItem)
                {
                    RelativeItemsCount = DaoFactory.GetListItemDao().GetRelativeItemsCount(ListType.TaskCategory, categoryItem.ID)
                };
                
            }

            if (task.ContactID > 0)
            {
                result.Contact = ContactBaseDtoHelper.GetContactBaseWithEmailDto(DaoFactory.GetContactDao().GetByID(task.ContactID));
            }

            if (task.EntityID > 0)
            {
                
                result.Entity = EntityDtoHelper.Get(task.EntityType, task.EntityID);
            }

            result.CanEdit = CRMSecurity.CanEdit(task);

            return result;
        }
    }
}