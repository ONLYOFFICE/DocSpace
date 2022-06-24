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


using System;
using System.Collections.Generic;
using System.Linq;

using ASC.Api.CRM;
using ASC.Common.Web;
using ASC.CRM.ApiModels;

using ASC.CRM.Core;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;
using ASC.Web.Api.Models;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;

namespace ASC.CRM.Api
{
    public class TaskTemplateController : BaseApiController
    {
        private readonly EmployeeDtoHelper _employeeDtoHelper;

        public TaskTemplateController(CrmSecurity crmSecurity,
                                    DaoFactory daoFactory,
                                    EmployeeDtoHelper employeeDtoHelper,
                                    IMapper mapper)
                        : base(daoFactory, crmSecurity, mapper)

        {
            _employeeDtoHelper = employeeDtoHelper;
        }



        /// <summary>
        ///   Creates a new task template container with the type and title specified in the request
        /// </summary>
        /// <param name="entityType">Type</param>
        /// <param name="title">Title</param>
        /// <short>Create task template container</short> 
        /// <category>Task Templates</category>
        /// <returns>
        ///     Task template container
        /// </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <visible>false</visible>
        [HttpPost(@"{entityType:regex(contact|person|company|opportunity|case)}/tasktemplatecontainer")]
        public TaskTemplateContainerDto CreateTaskTemplateContainer([FromRoute] string entityType, [FromBody] string title)
        {
            if (string.IsNullOrEmpty(title)) throw new ArgumentException();

            var taskTemplateContainer = new TaskTemplateContainer
            {
                EntityType = ToEntityType(entityType),
                Title = title
            };

            taskTemplateContainer.ID = _daoFactory.GetTaskTemplateContainerDao().SaveOrUpdate(taskTemplateContainer);
            return ToTaskTemplateContainerDto(taskTemplateContainer);
        }

        /// <summary>
        ///    Returns the complete list of all the task template containers available on the portal
        /// </summary>
        /// <param name="entityType">Type</param>
        /// <short>Get task template container list</short> 
        /// <category>Task Templates</category>
        /// <returns>
        ///     Task template container list
        /// </returns>
        /// <visible>false</visible>
        [HttpGet(@"{entityType:regex(contact|person|company|opportunity|case)}/tasktemplatecontainer")]
        public IEnumerable<TaskTemplateContainerDto> GetTaskTemplateContainers(string entityType)
        {
            return ToTaskListTemplateContainerDto(_daoFactory.GetTaskTemplateContainerDao().GetItems(ToEntityType(entityType)));
        }

        /// <summary>
        ///   Deletes the task template container with the ID specified in the request
        /// </summary>
        /// <param name="containerid">Task template container ID</param>
        /// <short>Delete task template container</short> 
        /// <category>Task Templates</category>
        /// <returns>
        ///    Deleted task template container
        /// </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <visible>false</visible>
        [HttpDelete(@"tasktemplatecontainer/{containerid:int}")]
        public TaskTemplateContainerDto DeleteTaskTemplateContainer(int containerid)
        {
            if (containerid <= 0) throw new ArgumentException();

            var result = ToTaskTemplateContainerDto(_daoFactory.GetTaskTemplateContainerDao().GetByID(containerid));
            if (result == null) throw new ItemNotFoundException();

            _daoFactory.GetTaskTemplateContainerDao().Delete(containerid);

            return result;
        }

        /// <summary>
        ///   Updates the task template container with the ID specified in the request
        /// </summary>
        /// <param name="containerid">Task template container ID</param>
        /// <param name="title">Title</param>
        /// <short>Update task template container</short> 
        /// <category>Task Templates</category>
        /// <returns>
        ///     Task template container
        /// </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <visible>false</visible>
        [HttpPut(@"tasktemplatecontainer/{containerid:int}")]
        public TaskTemplateContainerDto UpdateTaskTemplateContainer(int containerid, string title)
        {
            if (containerid <= 0 || string.IsNullOrEmpty(title)) throw new ArgumentException();

            var result = _daoFactory.GetTaskTemplateContainerDao().GetByID(containerid);
            if (result == null) throw new ItemNotFoundException();

            result.Title = title;

            _daoFactory.GetTaskTemplateContainerDao().SaveOrUpdate(result);

            return ToTaskTemplateContainerDto(result);
        }

        /// <summary>
        ///   Returns the detailed information on the task template container with the ID specified in the request
        /// </summary>
        /// <param name="containerid">Task template container ID</param>
        /// <short>Get task template container by ID</short> 
        /// <category>Task Templates</category>
        /// <returns>
        ///     Task template container
        /// </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <visible>false</visible>
        [HttpGet(@"tasktemplatecontainer/{containerid:int}")]
        public TaskTemplateContainerDto GetTaskTemplateContainerByID(int containerid)
        {
            if (containerid <= 0) throw new ArgumentException();

            var item = _daoFactory.GetTaskTemplateContainerDao().GetByID(containerid);
            if (item == null) throw new ItemNotFoundException();

            return ToTaskTemplateContainerDto(item);
        }

        /// <summary>
        ///   Returns the list of all tasks in the container with the ID specified in the request
        /// </summary>
        /// <param name="containerid">Task template container ID</param>
        /// <short>Get task template list by contaier ID</short> 
        /// <category>Task Templates</category>
        /// <returns>
        ///     Task template list
        /// </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <visible>false</visible>
        [HttpGet(@"tasktemplatecontainer/{containerid:int}/tasktemplate")]
        public IEnumerable<TaskTemplateDto> GetTaskTemplates(int containerid)
        {
            if (containerid <= 0) throw new ArgumentException();

            var container = _daoFactory.GetTaskTemplateContainerDao().GetByID(containerid);
            if (container == null) throw new ItemNotFoundException();

            return _daoFactory.GetTaskTemplateDao().GetList(containerid).ConvertAll(ToTaskTemplateDto);
        }

        /// <summary>
        ///   Creates a new task template with the parameters specified in the request in the container with the selected ID
        /// </summary>
        /// <param name="containerid">Task template container ID</param>
        /// <param name="title">Title</param>
        /// <param name="description">Description</param>
        /// <param name="responsibleid">Responsible ID</param>
        /// <param name="categoryid">Category ID</param>
        /// <param name="isNotify">Responsible notification: notify or not</param>
        /// <param name="offsetTicks">Ticks offset</param>
        /// <param name="deadLineIsFixed"></param>
        /// <short>Create task template</short> 
        /// <category>Task Templates</category>
        /// <returns>Task template</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <visible>false</visible>
        [HttpPost(@"tasktemplatecontainer/{containerid:int}/tasktemplate")]
        public TaskTemplateDto CreateTaskTemplate(
            [FromRoute] int containerid,
            [FromBody] CreateOrUpdateTaskTemplateRequestDto inDto)
        {
            var title = inDto.Title;
            var categoryid = inDto.Categoryid;
            var deadLineIsFixed = inDto.DeadLineIsFixed;
            var description = inDto.Description;
            var isNotify = inDto.isNotify;
            var responsibleid = inDto.Responsibleid;
            var offsetTicks = inDto.OffsetTicks;

            if (containerid <= 0 || string.IsNullOrEmpty(title) || categoryid <= 0) throw new ArgumentException();

            var container = _daoFactory.GetTaskTemplateContainerDao().GetByID(containerid);
            if (container == null) throw new ItemNotFoundException();

            var item = new TaskTemplate
            {
                CategoryID = categoryid,
                ContainerID = containerid,
                DeadLineIsFixed = deadLineIsFixed,
                Description = description,
                isNotify = isNotify,
                ResponsibleID = responsibleid,
                Title = title,
                Offset = TimeSpan.FromTicks(offsetTicks)
                
            };

            item.ID = _daoFactory.GetTaskTemplateDao().SaveOrUpdate(item);

            return ToTaskTemplateDto(item);
        }

        /// <summary>
        ///   Updates the selected task template with the parameters specified in the request in the container with the selected ID
        /// </summary>
        /// <param name="id">Task template ID</param>
        /// <param name="containerid">Task template container ID</param>
        /// <param name="title">Title</param>
        /// <param name="description">Description</param>
        /// <param name="responsibleid">Responsible ID</param>
        /// <param name="categoryid">Category ID</param>
        /// <param name="isNotify">Responsible notification: notify or not</param>
        /// <param name="offsetTicks">Ticks offset</param>
        /// <param name="deadLineIsFixed"></param>
        /// <short>Update task template</short> 
        /// <category>Task Templates</category>
        /// <returns>Task template</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <visible>false</visible>
        [HttpPut(@"tasktemplatecontainer/{containerid:int}/tasktemplate/{id:int}")]
        public TaskTemplateDto UpdateTaskTemplate(
            [FromRoute] int id,
            [FromRoute] int containerid,
            [FromBody] CreateOrUpdateTaskTemplateRequestDto inDto)
        {

            var title = inDto.Title;
            var categoryid = inDto.Categoryid;
            var deadLineIsFixed = inDto.DeadLineIsFixed;
            var description = inDto.Description;
            var isNotify = inDto.isNotify;
            var responsibleid = inDto.Responsibleid;
            var offsetTicks = inDto.OffsetTicks;
          
            if (containerid <= 0 || string.IsNullOrEmpty(title) || categoryid <= 0) throw new ArgumentException();

            var updatingItem = _daoFactory.GetTaskTemplateDao().GetByID(id);
            if (updatingItem == null) throw new ItemNotFoundException();

            var container = _daoFactory.GetTaskTemplateContainerDao().GetByID(containerid);
            if (container == null) throw new ItemNotFoundException();

            var item = new TaskTemplate
            {
                CategoryID = categoryid,
                ContainerID = containerid,
                DeadLineIsFixed = deadLineIsFixed,
                Description = description,
                isNotify = isNotify,
                ResponsibleID = responsibleid,
                Title = title,
                ID = id,
                Offset = TimeSpan.FromTicks(offsetTicks)
            };

            item.ID = _daoFactory.GetTaskTemplateDao().SaveOrUpdate(item);

            return ToTaskTemplateDto(item);
        }

        /// <summary>
        ///   Deletes the task template with the ID specified in the request
        /// </summary>
        /// <param name="id">Task template ID</param>
        /// <short>Delete task template</short> 
        /// <category>Task Templates</category>
        /// <returns>Task template</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <visible>false</visible>
        [HttpDelete(@"tasktemplatecontainer/tasktemplate/{id:int}")]
        public TaskTemplateDto DeleteTaskTemplate(int id)
        {
            if (id <= 0) throw new ArgumentException();

            var taskTemplate = _daoFactory.GetTaskTemplateDao().GetByID(id);
            if (taskTemplate == null) throw new ItemNotFoundException();

            var result = ToTaskTemplateDto(taskTemplate);

            _daoFactory.GetTaskTemplateDao().Delete(id);

            return result;
        }

        /// <summary>
        ///   Return the task template with the ID specified in the request
        /// </summary>
        /// <param name="id">Task template ID</param>
        /// <short>Get task template by ID</short> 
        /// <category>Task Templates</category>
        /// <returns>Task template</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <visible>false</visible>
        [HttpGet(@"tasktemplatecontainer/tasktemplate/{id:int}")]
        public TaskTemplateDto GetTaskTemplateByID(int id)
        {
            if (id <= 0) throw new ArgumentException();

            var taskTemplate = _daoFactory.GetTaskTemplateDao().GetByID(id);
            if (taskTemplate == null) throw new ItemNotFoundException();

            return ToTaskTemplateDto(taskTemplate);
        }

        protected TaskTemplateDto ToTaskTemplateDto(TaskTemplate taskTemplate)
        {
            // TODO: set task template category
            return new TaskTemplateDto
            {
                //                Category = GetTaskCategoryByID(taskTemplate.CategoryID),
                ContainerID = taskTemplate.ContainerID,
                DeadLineIsFixed = taskTemplate.DeadLineIsFixed,
                Description = taskTemplate.Description,
                Id = taskTemplate.ID,
                isNotify = taskTemplate.isNotify,
                Title = taskTemplate.Title,
                OffsetTicks = taskTemplate.Offset.Ticks,
                Responsible = _employeeDtoHelper.Get(taskTemplate.ResponsibleID)
            };
        }

        protected IEnumerable<TaskTemplateContainerDto> ToTaskListTemplateContainerDto(IEnumerable<TaskTemplateContainer> items)
        {
            var result = new List<TaskTemplateContainerDto>();

            var taskTemplateDictionary = _daoFactory.GetTaskTemplateDao().GetAll()
                                                   .GroupBy(item => item.ContainerID)
                                                   .ToDictionary(x => x.Key, y => y.Select(ToTaskTemplateDto));

            foreach (var item in items)
            {
                var taskTemplateContainer = new TaskTemplateContainerDto
                {
                    Title = item.Title,
                    EntityType = item.EntityType.ToString(),
                    Id = item.ID
                };

                if (taskTemplateDictionary.ContainsKey(taskTemplateContainer.Id))
                {
                    taskTemplateContainer.Items = taskTemplateDictionary[taskTemplateContainer.Id];
                }

                result.Add(taskTemplateContainer);
            }

            return result;
        }

        protected TaskTemplateContainerDto ToTaskTemplateContainerDto(TaskTemplateContainer item)
        {
            return ToTaskListTemplateContainerDto(new List<TaskTemplateContainer>
                {
                    item
                }).FirstOrDefault();
        }
    }
}