using System;
using System.Collections.Generic;
using System.Linq;

using ASC.Api.Core;
using ASC.Api.CRM;
using ASC.Common.Web;
using ASC.CRM.ApiModels;
using ASC.CRM.Core;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.CRM.Resources;
using ASC.MessagingSystem.Core;
using ASC.MessagingSystem.Models;
using ASC.Web.CRM.Core.Search;
using ASC.Web.CRM.Services.NotifyService;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;


namespace ASC.CRM.Api
{
    public class TasksController : BaseApiController
    {
        private readonly NotifyClient _notifyClient;
        private readonly ApiContext _apiContext;
        private readonly MessageService _messageService;
        private readonly MessageTarget _messageTarget;

        public TasksController(CrmSecurity crmSecurity,
                     DaoFactory daoFactory,
                     ApiContext apiContext,
                     MessageTarget messageTarget,
                     MessageService messageService,
                     NotifyClient notifyClient,
                     IMapper mapper,
                     FactoryIndexerCase factoryIndexerCase)
            : base(daoFactory, crmSecurity, mapper)
        {
            _apiContext = apiContext;
            _messageTarget = messageTarget;
            _messageService = messageService;
            _notifyClient = notifyClient;
            _mapper = mapper;
        }

        /// <summary>
        ///  Returns the detailed information about the task with the ID specified in the request
        /// </summary>
        /// <param name="taskid">Task ID</param>
        /// <returns>Task</returns>
        /// <short>Get task by ID</short> 
        /// <category>Tasks</category>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        [HttpGet(@"task/{taskid:int}")]
        public TaskDto GetTaskByID(int taskid)
        {
            if (taskid <= 0) throw new ArgumentException();

            var task = _daoFactory.GetTaskDao().GetByID(taskid);

            if (task == null) throw new ItemNotFoundException();

            if (!_crmSecurity.CanAccessTo(task))
            {
                throw _crmSecurity.CreateSecurityException();
            }

            return _mapper.Map<TaskDto>(task);
        }

        /// <summary>
        ///   Returns the list of tasks matching the creteria specified in the request
        /// </summary>
        /// <param optional="true" name="responsibleid">Task responsible</param>
        /// <param optional="true" name="categoryid">Task category ID</param>
        /// <param optional="true" name="isClosed">Show open or closed tasks only</param>
        /// <param optional="true" name="fromDate">Earliest task due date</param>
        /// <param optional="true" name="toDate">Latest task due date</param>
        /// <param name="entityType" remark="Allowed values: opportunity, contact or case">Related entity type</param>
        /// <param name="entityid">Related entity ID</param>
        /// <exception cref="ArgumentException"></exception>
        /// <short>Get task list</short> 
        /// <category>Tasks</category>
        /// <returns>
        ///   Task list
        /// </returns>
        [HttpGet(@"task/filter")]
        public IEnumerable<TaskDto> GetAllTasks(
            Guid responsibleid,
            int categoryid,
            bool? isClosed,
            ApiDateTime fromDate,
            ApiDateTime toDate,
            string entityType,
            int entityid)
        {
            TaskSortedByType taskSortedByType;

            if (!string.IsNullOrEmpty(entityType) &&
                !(
                     string.Equals(entityType, "contact", StringComparison.OrdinalIgnoreCase)||
                     string.Equals(entityType, "opportunity", StringComparison.OrdinalIgnoreCase)||
                     string.Equals(entityType, "case", StringComparison.OrdinalIgnoreCase))
                )
                throw new ArgumentException();

            var searchText = _apiContext.FilterValue;

            IEnumerable<TaskDto> result;

            OrderBy taskOrderBy;

            if (ASC.CRM.Classes.EnumExtension.TryParse(_apiContext.SortBy, true, out taskSortedByType))
            {
                taskOrderBy = new OrderBy(taskSortedByType, !_apiContext.SortDescending);
            }
            else if (string.IsNullOrEmpty(_apiContext.SortBy))
            {
                taskOrderBy = new OrderBy(TaskSortedByType.DeadLine, true);
            }
            else
            {
                taskOrderBy = null;
            }

            var fromIndex = (int)_apiContext.StartIndex;
            var count = (int)_apiContext.Count;

            if (taskOrderBy != null)
            {
                result = ToTaskListDto(
                    _daoFactory.GetTaskDao()
                        .GetTasks(
                            searchText,
                            responsibleid,
                            categoryid,
                            isClosed,
                            fromDate,
                            toDate,
                            ToEntityType(entityType),
                            entityid,
                            fromIndex,
                            count,
                            taskOrderBy)).ToList();

                _apiContext.SetDataPaginated();
                _apiContext.SetDataFiltered();
                _apiContext.SetDataSorted();
            }
            else
                result = ToTaskListDto(
                    _daoFactory
                        .GetTaskDao()
                        .GetTasks(
                            searchText,
                            responsibleid,
                            categoryid,
                            isClosed,
                            fromDate,
                            toDate,
                            ToEntityType(entityType),
                            entityid,
                            0,
                            0, null)).ToList();


            int totalCount;

            if (result.Count() < count)
            {
                totalCount = fromIndex + result.Count();
            }
            else
            {
                totalCount = _daoFactory
                    .GetTaskDao()
                    .GetTasksCount(
                        searchText,
                        responsibleid,
                        categoryid,
                        isClosed,
                        fromDate,
                        toDate,
                        ToEntityType(entityType),
                        entityid);
            }

            _apiContext.SetTotalCount(totalCount);

            return result;
        }

        /// <summary>
        ///   Open anew the task with the ID specified in the request
        /// </summary>
        /// <short>Resume task</short> 
        /// <category>Tasks</category>
        /// <param name="taskid">Task ID</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>
        ///   Task
        /// </returns>
        [HttpPut(@"task/{taskid:int}/reopen")]
        public TaskDto ReOpenTask(int taskid)
        {
            if (taskid <= 0) throw new ArgumentException();

            _daoFactory.GetTaskDao().OpenTask(taskid);

            var task = _daoFactory.GetTaskDao().GetByID(taskid);

            _messageService.Send(MessageAction.CrmTaskOpened, _messageTarget.Create(task.ID), task.Title);

            return _mapper.Map<TaskDto>(task);
        }

        /// <summary>
        ///   Close the task with the ID specified in the request
        /// </summary>
        /// <short>Close task</short> 
        /// <category>Tasks</category>
        /// <param name="taskid">Task ID</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>
        ///   Task
        /// </returns>
        [HttpPut(@"task/{taskid:int}/close")]
        public TaskDto CloseTask(int taskid)
        {
            if (taskid <= 0) throw new ArgumentException();

            _daoFactory.GetTaskDao().CloseTask(taskid);

            var task = _daoFactory.GetTaskDao().GetByID(taskid);
            _messageService.Send(MessageAction.CrmTaskClosed, _messageTarget.Create(task.ID), task.Title);

            return _mapper.Map<TaskDto>(task);

        }

        /// <summary>
        ///   Delete the task with the ID specified in the request
        /// </summary>
        /// <short>Delete task</short> 
        /// <category>Tasks</category>
        /// <param name="taskid">Task ID</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///  Deleted task
        /// </returns>
        [HttpDelete(@"task/{taskid:int}")]
        public TaskDto DeleteTask(int taskid)
        {
            if (taskid <= 0) throw new ArgumentException();

            var task = _daoFactory.GetTaskDao().GetByID(taskid);
            if (task == null) throw new ItemNotFoundException();

            _daoFactory.GetTaskDao().DeleteTask(taskid);
            _messageService.Send(MessageAction.CrmTaskDeleted, _messageTarget.Create(task.ID), task.Title);

            return _mapper.Map<TaskDto>(task);

        }

        /// <summary>
        ///  Creates the task with the parameters (title, description, due date, etc.) specified in the request
        /// </summary>
        /// <param name="title">Task title</param>
        /// <param optional="true"  name="description">Task description</param>
        /// <param name="deadline">Task due date</param>
        /// <param name="responsibleId">Task responsible ID</param>
        /// <param name="categoryId">Task category ID</param>
        /// <param optional="true"  name="contactId">Contact ID</param>
        /// <param optional="true"  name="entityType" remark="Allowed values: opportunity or case">Related entity type</param>
        /// <param optional="true"  name="entityId">Related entity ID</param>
        /// <param optional="true"  name="isNotify">Notify the responsible about the task</param>
        /// <param optional="true"  name="alertValue">Time period in minutes for reminder to the responsible about the task</param>
        /// <exception cref="ArgumentException"></exception>
        /// <short>Create task</short> 
        /// <category>Tasks</category>
        /// <returns>Task</returns>
        [HttpPost(@"task")]
        public TaskDto CreateTask(
[FromBody] CreateOrUpdateTaskRequestDto inDto)
        {
            var entityType = inDto.EntityType;
            var categoryId = inDto.CategoryId;
            var title = inDto.Title;
            var description = inDto.Description;
            var deadline = inDto.Deadline;
            var responsibleId = inDto.ResponsibleId;
            var contactId = inDto.ContactId;
            var entityId = inDto.EntityId;
            var alertValue = inDto.AlertValue;
            var isNotify = inDto.isNotify;

            if (!string.IsNullOrEmpty(entityType) &&
                !(
                     string.Equals(entityType, "opportunity", StringComparison.OrdinalIgnoreCase)||
                     string.Equals(entityType, "case", StringComparison.OrdinalIgnoreCase)
                 )
                || categoryId <= 0)
                throw new ArgumentException();

            var listItem = _daoFactory.GetListItemDao().GetByID(categoryId);
            if (listItem == null) throw new ItemNotFoundException(CRMErrorsResource.TaskCategoryNotFound);

            var task = new Task
            {
                Title = title,
                Description = description,
                ResponsibleID = responsibleId,
                CategoryID = categoryId,
                DeadLine = deadline,
                ContactID = contactId,
                EntityType = ToEntityType(entityType),
                EntityID = entityId,
                IsClosed = false,
                AlertValue = alertValue
            };

            task = _daoFactory.GetTaskDao().SaveOrUpdateTask(task);

            if (isNotify)
            {
                Contact taskContact = null;
                Cases taskCase = null;
                Deal taskDeal = null;

                if (task.ContactID > 0)
                {
                    taskContact = _daoFactory.GetContactDao().GetByID(task.ContactID);
                }

                if (task.EntityID > 0)
                {
                    switch (task.EntityType)
                    {
                        case EntityType.Case:
                            taskCase = _daoFactory.GetCasesDao().GetByID(task.EntityID);
                            break;
                        case EntityType.Opportunity:
                            taskDeal = _daoFactory.GetDealDao().GetByID(task.EntityID);
                            break;
                    }
                }

                _notifyClient.SendAboutResponsibleByTask(task, listItem.Title, taskContact, taskCase, taskDeal, null);
            }

            _messageService.Send(MessageAction.CrmTaskCreated, _messageTarget.Create(task.ID), task.Title);

            return _mapper.Map<TaskDto>(task);
        }

        /// <summary>
        ///  Creates the group of the same task with the parameters (title, description, due date, etc.) specified in the request for several contacts
        /// </summary>
        /// <param name="title">Task title</param>
        /// <param optional="true"  name="description">Task description</param>
        /// <param name="deadline">Task due date</param>
        /// <param name="responsibleId">Task responsible ID</param>
        /// <param name="categoryId">Task category ID</param>
        /// <param name="contactId">contact ID list</param>
        /// <param optional="true"  name="entityType" remark="Allowed values: opportunity or case">Related entity type</param>
        /// <param optional="true"  name="entityId">Related entity ID</param>
        /// <param optional="true"  name="isNotify">Notify the responsible about the task</param>
        /// <param optional="true"  name="alertValue">Time period in minutes for reminder to the responsible about the task</param>
        /// <exception cref="ArgumentException"></exception>
        /// <short>Create task list</short> 
        /// <category>Tasks</category>
        /// <returns>Tasks</returns>
        /// <visible>false</visible>
        [HttpPost(@"contact/task/group")]
        public IEnumerable<TaskDto> CreateTaskGroup([FromBody] CreateTaskGroupRequestDto inDto)
        {
            var entityType = inDto.EntityType;
            var contactId = inDto.ContactIds;
            var title = inDto.Title;
            var description = inDto.Description;
            var responsibleId = inDto.ResponsibleId;
            var entityId = inDto.EntityId;
            var categoryId = inDto.CategoryId;
            var alertValue = inDto.AlertValue;
            var deadline = inDto.Deadline;
            var isNotify = inDto.isNotify;



            var tasks = new List<Task>();

            if (
                !string.IsNullOrEmpty(entityType) &&
                !(string.Equals(entityType, "opportunity", StringComparison.OrdinalIgnoreCase) ||
                  string.Equals(entityType, "case", StringComparison.OrdinalIgnoreCase))
                )
                throw new ArgumentException();

            foreach (var cid in contactId)
            {
                tasks.Add(new Task
                {
                    Title = title,
                    Description = description,
                    ResponsibleID = responsibleId,
                    CategoryID = categoryId,
                    DeadLine = deadline,
                    ContactID = cid,
                    EntityType = ToEntityType(entityType),
                    EntityID = entityId,
                    IsClosed = false,
                    AlertValue = alertValue
                });
            }

            tasks = _daoFactory.GetTaskDao().SaveOrUpdateTaskList(tasks).ToList();

            string taskCategory = null;
            if (isNotify)
            {
                if (categoryId > 0)
                {
                    var listItem = _daoFactory.GetListItemDao().GetByID(categoryId);
                    if (listItem == null) throw new ItemNotFoundException();

                    taskCategory = listItem.Title;
                }
            }

            for (var i = 0; i < tasks.Count; i++)
            {
                if (!isNotify) continue;

                Contact taskContact = null;
                Cases taskCase = null;
                Deal taskDeal = null;

                if (tasks[i].ContactID > 0)
                {
                    taskContact = _daoFactory.GetContactDao().GetByID(tasks[i].ContactID);
                }

                if (tasks[i].EntityID > 0)
                {
                    switch (tasks[i].EntityType)
                    {
                        case EntityType.Case:
                            taskCase = _daoFactory.GetCasesDao().GetByID(tasks[i].EntityID);
                            break;
                        case EntityType.Opportunity:
                            taskDeal = _daoFactory.GetDealDao().GetByID(tasks[i].EntityID);
                            break;
                    }
                }

                _notifyClient.SendAboutResponsibleByTask(tasks[i], taskCategory, taskContact, taskCase, taskDeal, null);
            }

            if (tasks.Any())
            {
                var contacts = _daoFactory.GetContactDao().GetContacts(contactId);
                var task = tasks.First();
                _messageService.Send(MessageAction.ContactsCreatedCrmTasks, _messageTarget.Create(tasks.Select(x => x.ID)), contacts.Select(x => x.GetTitle()), task.Title);
            }

            return ToTaskListDto(tasks);
        }


        /// <summary>
        ///   Updates the selected task with the parameters (title, description, due date, etc.) specified in the request
        /// </summary>
        /// <param name="taskid">Task ID</param>
        /// <param name="title">Task title</param>
        /// <param name="description">Task description</param>
        /// <param name="deadline">Task due date</param>
        /// <param name="responsibleid">Task responsible ID</param>
        /// <param name="categoryid">Task category ID</param>
        /// <param name="contactid">Contact ID</param>
        /// <param name="entityType" remark="Allowed values: opportunity or case">Related entity type</param>
        /// <param name="entityid">Related entity ID</param>
        /// <param name="isNotify">Notify or not</param>
        /// <param optional="true"  name="alertValue">Time period in minutes for reminder to the responsible about the task</param>
        /// <short> Update task</short> 
        /// <category>Tasks</category>
        /// <returns>Task</returns>
        [HttpPut(@"task/{taskid:int}")]
        public TaskDto UpdateTask(
            [FromRoute] int taskid,
[FromBody] CreateOrUpdateTaskRequestDto inDto)
        {
            var entityType = inDto.EntityType;
            var categoryid = inDto.CategoryId;
            var title = inDto.Title;
            var description = inDto.Description;
            var deadline = inDto.Deadline;
            var responsibleid = inDto.ResponsibleId;
            var contactid = inDto.ContactId;
            var entityid = inDto.EntityId;
            var alertValue = inDto.AlertValue;
            var isNotify = inDto.isNotify;

            if (!string.IsNullOrEmpty(entityType) &&
                !(string.Equals(entityType, "opportunity", StringComparison.OrdinalIgnoreCase) ||
                  string.Equals(entityType, "case", StringComparison.OrdinalIgnoreCase)
                 ) || categoryid <= 0)
                throw new ArgumentException();

            var listItem = _daoFactory.GetListItemDao().GetByID(categoryid);
            if (listItem == null) throw new ItemNotFoundException(CRMErrorsResource.TaskCategoryNotFound);

            var task = new Task
            {
                ID = taskid,
                Title = title,
                Description = description,
                DeadLine = deadline,
                AlertValue = alertValue,
                ResponsibleID = responsibleid,
                CategoryID = categoryid,
                ContactID = contactid,
                EntityID = entityid,
                EntityType = ToEntityType(entityType)
            };


            task = _daoFactory.GetTaskDao().SaveOrUpdateTask(task);

            if (isNotify)
            {
                Contact taskContact = null;
                Cases taskCase = null;
                Deal taskDeal = null;

                if (task.ContactID > 0)
                {
                    taskContact = _daoFactory.GetContactDao().GetByID(task.ContactID);
                }

                if (task.EntityID > 0)
                {
                    switch (task.EntityType)
                    {
                        case EntityType.Case:
                            taskCase = _daoFactory.GetCasesDao().GetByID(task.EntityID);
                            break;
                        case EntityType.Opportunity:
                            taskDeal = _daoFactory.GetDealDao().GetByID(task.EntityID);
                            break;
                    }
                }

                _notifyClient.SendAboutResponsibleByTask(task, listItem.Title, taskContact, taskCase, taskDeal, null);
            }

            _messageService.Send(MessageAction.CrmTaskUpdated, _messageTarget.Create(task.ID), task.Title);

            return _mapper.Map<TaskDto>(task);
        }

        /// <visible>false</visible>
        [HttpPut(@"task/{taskid:int}/creationdate")]
        public void SetTaskCreationDate(int taskId, ApiDateTime creationDate)
        {
            var dao = _daoFactory.GetTaskDao();
            var task = dao.GetByID(taskId);

            if (task == null || !_crmSecurity.CanAccessTo(task))
                throw new ItemNotFoundException();

            dao.SetTaskCreationDate(taskId, creationDate);
        }

        /// <visible>false</visible>
        [HttpPut(@"task/{taskid:int}/lastmodifeddate")]
        public void SetTaskLastModifedDate(int taskId, ApiDateTime lastModifedDate)
        {
            var dao = _daoFactory.GetTaskDao();
            var task = dao.GetByID(taskId);

            if (task == null || !_crmSecurity.CanAccessTo(task))
                throw new ItemNotFoundException();

            dao.SetTaskLastModifedDate(taskId, lastModifedDate);
        }

        private IEnumerable<TaskDto> ToTaskListDto(IEnumerable<Task> itemList)
        {
            var result = new List<TaskDto>();

            var contactIDs = new List<int>();
            var taskIDs = new List<int>();
            var categoryIDs = new List<int>();
            var entityDtosIDs = new Dictionary<EntityType, List<int>>();

            foreach (var item in itemList)
            {
                taskIDs.Add(item.ID);

                if (!categoryIDs.Contains(item.CategoryID))
                {
                    categoryIDs.Add(item.CategoryID);
                }

                if (item.ContactID > 0 && !contactIDs.Contains(item.ContactID))
                {
                    contactIDs.Add(item.ContactID);
                }

                if (item.EntityID > 0)
                {
                    if (item.EntityType != EntityType.Opportunity && item.EntityType != EntityType.Case) continue;

                    if (!entityDtosIDs.ContainsKey(item.EntityType))
                    {
                        entityDtosIDs.Add(item.EntityType, new List<int>
                            {
                                item.EntityID
                            });
                    }
                    else if (!entityDtosIDs[item.EntityType].Contains(item.EntityID))
                    {
                        entityDtosIDs[item.EntityType].Add(item.EntityID);
                    }
                }
            }

            var entityDtos = new Dictionary<string, EntityDto>();

            foreach (var entityType in entityDtosIDs.Keys)
            {
                switch (entityType)
                {
                    case EntityType.Opportunity:
                        _daoFactory.GetDealDao().GetDeals(entityDtosIDs[entityType].Distinct().ToArray())
                                  .ForEach(item =>
                                  {
                                      if (item == null) return;

                                      entityDtos.Add(
                                          string.Format("{0}_{1}", (int)entityType, item.ID),
                                          new EntityDto
                                          {
                                              EntityId = item.ID,
                                              EntityTitle = item.Title,
                                              EntityType = "opportunity"
                                          });
                                  });
                        break;
                    case EntityType.Case:
                        _daoFactory.GetCasesDao().GetByID(entityDtosIDs[entityType].ToArray())
                                  .ForEach(item =>
                                  {
                                      if (item == null) return;

                                      entityDtos.Add(
                                          string.Format("{0}_{1}", (int)entityType, item.ID),
                                          new EntityDto
                                          {
                                              EntityId = item.ID,
                                              EntityTitle = item.Title,
                                              EntityType = "case"
                                          });
                                  });
                        break;
                }
            }

            var categories = _daoFactory.GetListItemDao().GetItems(categoryIDs.ToArray()).ToDictionary(x => x.ID, x => _mapper.Map<TaskCategoryDto>(x));
            var contacts = _daoFactory.GetContactDao().GetContacts(contactIDs.ToArray()).ToDictionary(item => item.ID, x => _mapper.Map<ContactBaseWithEmailDto>(x));
            var restrictedContacts = _daoFactory.GetContactDao().GetRestrictedContacts(contactIDs.ToArray()).ToDictionary(item => item.ID, x => _mapper.Map<ContactBaseWithEmailDto>(x));

            foreach (var item in itemList)
            {
                var taskDto = _mapper.Map<TaskDto>(item);

                taskDto.CanEdit = _crmSecurity.CanEdit(item);

                if (contacts.ContainsKey(item.ContactID))
                {
                    taskDto.Contact = contacts[item.ContactID];
                }

                if (restrictedContacts.ContainsKey(item.ContactID))
                {
                    taskDto.Contact = restrictedContacts[item.ContactID];
                    /*Hide some fields. Should be refactored! */
                    taskDto.Contact.Currency = null;
                    taskDto.Contact.Email = null;
                    taskDto.Contact.AccessList = null;
                }

                if (item.EntityID > 0)
                {
                    var entityStrKey = string.Format("{0}_{1}", (int)item.EntityType, item.EntityID);

                    if (entityDtos.ContainsKey(entityStrKey))
                    {
                        taskDto.Entity = entityDtos[entityStrKey];
                    }
                }

                if (categories.ContainsKey(item.CategoryID))
                {
                    taskDto.Category = categories[item.CategoryID];
                }

                result.Add(taskDto);
            }

            return result;
        }
    }

}