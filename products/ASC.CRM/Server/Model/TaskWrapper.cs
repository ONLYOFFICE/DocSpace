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
using ASC.Web.Api.Models;
using System;
using System.Runtime.Serialization;

namespace ASC.Api.CRM.Wrappers
{
    /// <summary>
    ///  Task
    /// </summary>
    [DataContract(Name = "task", Namespace = "")]
    public class TaskWrapper
    {
        //public TaskWrapper(Task task)
        //{
        //    CreateBy = EmployeeWraper.Get(task.CreateBy);
        //    Created = (ApiDateTime)task.CreateOn;
        //    Title = task.Title;
        //    Description = task.Description;
        //    DeadLine = (ApiDateTime)task.DeadLine;
        //    Responsible = EmployeeWraper.Get(task.ResponsibleID);
        //    IsClosed = task.IsClosed;
        //    AlertValue = task.AlertValue;
        //}

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public EmployeeWraper CreateBy { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ApiDateTime Created { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ContactBaseWithEmailWrapper Contact { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public String Title { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public String Description { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ApiDateTime DeadLine { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public int AlertValue { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public EmployeeWraper Responsible { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsClosed { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public TaskCategoryBaseWrapper Category { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public EntityWrapper Entity { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool CanEdit { get; set; }

        public static TaskWrapper GetSample()
        {
            return new TaskWrapper
            {
                Created = ApiDateTime.GetSample(),
                CreateBy = EmployeeWraper.GetSample(),
                DeadLine = ApiDateTime.GetSample(),
                IsClosed = false,
                Responsible = EmployeeWraper.GetSample(),
                //                 Category = TaskCategoryBaseWrapper.GetSample(),
                CanEdit = true,
                Title = "Send a commercial offer",
                AlertValue = 0
            };
        }
    }

    [DataContract(Name = "taskBase", Namespace = "")]
    public class TaskBaseWrapper
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        public TaskBaseWrapper()
        {

        }

        
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public String Title { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public String Description { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ApiDateTime DeadLine { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public int AlertValue { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public EmployeeWraper Responsible { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsClosed { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public TaskCategoryBaseWrapper Category { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public EntityWrapper Entity { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool CanEdit { get; set; }

        public static TaskBaseWrapper GetSample()
        {
            return new TaskBaseWrapper
            {
                DeadLine = ApiDateTime.GetSample(),
                IsClosed = false,
                Responsible = EmployeeWraper.GetSample(),
                Category = TaskCategoryBaseWrapper.GetSample(),
                CanEdit = true,
                Title = "Send a commercial offer",
                AlertValue = 0
            };
        }
    }

    public class TaskWrapperHelper
    {
        
        public TaskWrapperHelper(ApiDateTimeHelper apiDateTimeHelper, 
                                 EmployeeWraperHelper employeeWraperHelper,
                                 CRMSecurity cRMSecurity,
                                 DaoFactory daoFactory,
                                 ContactBaseWrapperHelper contactBaseWrapperHelper)
        {
            ApiDateTimeHelper = apiDateTimeHelper;
            EmployeeWraperHelper = employeeWraperHelper;
            CRMSecurity = cRMSecurity;
            DaoFactory = daoFactory;
            ContactBaseWrapperHelper = contactBaseWrapperHelper;
        }

        public ContactBaseWrapperHelper ContactBaseWrapperHelper { get; }
        public CRMSecurity CRMSecurity { get; }
        public ApiDateTimeHelper ApiDateTimeHelper { get; }
        public EmployeeWraperHelper EmployeeWraperHelper { get; }
        public DaoFactory DaoFactory { get; }

        public TaskWrapper Get(Task task)
        {            
            var result = new TaskWrapper
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
                result.Category = GetTaskCategoryByID(task.CategoryID);
            }

            if (task.ContactID > 0)
            {
                result.Contact = ContactBaseWrapperHelper.Get(DaoFactory.GetContactDao().GetByID(task.ContactID));
            }

            if (task.EntityID > 0)
            {
                result.Entity = ToEntityWrapper(task.EntityType, task.EntityID);
            }

            result.CanEdit = CRMSecurity.CanEdit(task);

            return result;
        }
    }

    public static class TaskWrapperHelperExtension
    {
        public static DIHelper AddTaskWrapperHelperService(this DIHelper services)
        {
            services.TryAddTransient<TaskWrapperHelper>();

            return services.AddApiDateTimeHelper()
                           .AddEmployeeWraper()
                           .AddCRMSecurityService();
        }
    }
}

//private TaskWrapper ToTaskWrapper(Task task)
//{
//    var result = new TaskWrapper(task);

//    return result;
//}