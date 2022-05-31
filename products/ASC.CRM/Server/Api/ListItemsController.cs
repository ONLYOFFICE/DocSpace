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
using ASC.CRM.Core.Enums;
using ASC.MessagingSystem.Core;
using ASC.MessagingSystem.Models;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;

namespace ASC.CRM.Api
{
    public class ListItemsController : BaseApiController
    {
        private readonly MessageService _messageService;
        private readonly MessageTarget _messageTarget;

        public ListItemsController(CrmSecurity crmSecurity,
                     DaoFactory daoFactory,
                     MessageTarget messageTarget,
                     MessageService messageService,
                     IMapper mapper)
            : base(daoFactory, crmSecurity, mapper)
        {
            _messageTarget = messageTarget;
            _messageService = messageService;
        }


        /// <summary>
        ///   Creates an opportunity stage with the parameters (title, description, success probability, etc.) specified in the request
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="description">Description</param>
        /// <param name="color">Color</param>
        /// <param name="successProbability">Success probability</param>
        /// <param name="stageType" remark="Allowed values: 0 (Open), 1 (ClosedAndWon),2 (ClosedAndLost)">Stage type</param>
        /// <short>Create opportunity stage</short> 
        /// <category>Opportunities</category>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>
        ///    Opportunity stage
        /// </returns>
        [HttpPost(@"opportunity/stage")]
        public DealMilestoneDto CreateDealMilestone([FromBody] CreateOrUpdateDealMilestoneRequestDto inDto)
        {
            var title = inDto.Title;
            var successProbability = inDto.SuccessProbability;
            var description = inDto.Description;
            var color = inDto.Color;
            var stageType = inDto.StageType;

            if (!(_crmSecurity.IsAdmin)) throw _crmSecurity.CreateSecurityException();

            if (string.IsNullOrEmpty(title)) throw new ArgumentException();

            if (successProbability < 0) successProbability = 0;

            var dealMilestone = new DealMilestone
            {
                Title = title,
                Color = color,
                Description = description,
                Probability = successProbability,
                Status = stageType
            };

            dealMilestone.ID = _daoFactory.GetDealMilestoneDao().Create(dealMilestone);
            _messageService.Send(MessageAction.OpportunityStageCreated, _messageTarget.Create(dealMilestone.ID), dealMilestone.Title);

            return ToDealMilestoneDto(dealMilestone);
        }

        /// <summary>
        ///    Updates the selected opportunity stage with the parameters (title, description, success probability, etc.) specified in the request
        /// </summary>
        /// <param name="id">Opportunity stage ID</param>
        /// <param name="title">Title</param>
        /// <param name="description">Description</param>
        /// <param name="color">Color</param>
        /// <param name="successProbability">Success probability</param>
        /// <param name="stageType" remark="Allowed values: Open, ClosedAndWon, ClosedAndLost">Stage type</param>
        /// <short>Update opportunity stage</short> 
        /// <category>Opportunities</category>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///    Opportunity stage
        /// </returns>
        [HttpPut(@"opportunity/stage/{id:int}")]
        public DealMilestoneDto UpdateDealMilestone([FromRoute] int id, [FromBody] CreateOrUpdateDealMilestoneRequestDto inDto)
        {
            var title = inDto.Title;
            var successProbability = inDto.SuccessProbability;
            var description = inDto.Description;
            var color = inDto.Color;
            var stageType = inDto.StageType;

            if (!(_crmSecurity.IsAdmin)) throw _crmSecurity.CreateSecurityException();

            if (id <= 0 || string.IsNullOrEmpty(title)) throw new ArgumentException();

            if (successProbability < 0) successProbability = 0;

            var curDealMilestoneExist = _daoFactory.GetDealMilestoneDao().IsExist(id);
            if (!curDealMilestoneExist) throw new ItemNotFoundException();

            var dealMilestone = new DealMilestone
            {
                Title = title,
                Color = color,
                Description = description,
                Probability = successProbability,
                Status = stageType,
                ID = id
            };

            _daoFactory.GetDealMilestoneDao().Edit(dealMilestone);
            _messageService.Send(MessageAction.OpportunityStageUpdated, _messageTarget.Create(dealMilestone.ID), dealMilestone.Title);

            return ToDealMilestoneDto(dealMilestone);
        }

        /// <summary>
        ///    Updates the selected opportunity stage with the color specified in the request
        /// </summary>
        /// <param name="id">Opportunity stage ID</param>
        /// <param name="color">Color</param>
        /// <short>Update opportunity stage color</short> 
        /// <category>Opportunities</category>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///    Opportunity stage
        /// </returns>
        [HttpPut(@"opportunity/stage/{id:int}/color")]
        public DealMilestoneDto UpdateDealMilestoneColor([FromRoute] int id, [FromBody] string color)
        {
            if (!(_crmSecurity.IsAdmin)) throw _crmSecurity.CreateSecurityException();

            if (id <= 0) throw new ArgumentException();

            var dealMilestone = _daoFactory.GetDealMilestoneDao().GetByID(id);
            if (dealMilestone == null) throw new ItemNotFoundException();

            dealMilestone.Color = color;

            _daoFactory.GetDealMilestoneDao().ChangeColor(id, color);
            _messageService.Send(MessageAction.OpportunityStageUpdatedColor, _messageTarget.Create(dealMilestone.ID), dealMilestone.Title);

            return ToDealMilestoneDto(dealMilestone);
        }

        /// <summary>
        ///    Updates the available opportunity stages order with the list specified in the request
        /// </summary>
        /// <short>
        ///    Update opportunity stages order
        /// </short>
        /// <param name="ids">Opportunity stage ID list</param>
        /// <category>Opportunities</category>
        /// <returns>
        ///    Opportunity stages
        /// </returns>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        [HttpPut(@"opportunity/stage/reorder")]
        public IEnumerable<DealMilestoneDto> UpdateDealMilestonesOrder(IEnumerable<int> ids)
        {
            if (!(_crmSecurity.IsAdmin)) throw _crmSecurity.CreateSecurityException();

            if (ids == null) throw new ArgumentException();

            var idsList = ids.ToList();

            var result = idsList.Select(id => _daoFactory.GetDealMilestoneDao().GetByID(id)).ToList();

            _daoFactory.GetDealMilestoneDao().Reorder(idsList.ToArray());
            _messageService.Send(MessageAction.OpportunityStagesUpdatedOrder, _messageTarget.Create(idsList), result.Select(x => x.Title));

            return result.Select(ToDealMilestoneDto);
        }

        /// <summary>
        ///   Deletes the opportunity stage with the ID specified in the request
        /// </summary>
        /// <short>Delete opportunity stage</short> 
        /// <category>Opportunities</category>
        /// <param name="id">Opportunity stage ID</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///   Opportunity stage
        /// </returns>
        [HttpDelete(@"opportunity/stage/{id:int}")]
        public DealMilestoneDto DeleteDealMilestone(int id)
        {
            if (!(_crmSecurity.IsAdmin)) throw _crmSecurity.CreateSecurityException();

            if (id <= 0) throw new ArgumentException();

            var dealMilestone = _daoFactory.GetDealMilestoneDao().GetByID(id);
            if (dealMilestone == null) throw new ItemNotFoundException();

            var result = ToDealMilestoneDto(dealMilestone);

            _daoFactory.GetDealMilestoneDao().Delete(id);
            _messageService.Send(MessageAction.OpportunityStageDeleted, _messageTarget.Create(dealMilestone.ID), dealMilestone.Title);

            return result;
        }

        /// <summary>
        ///   Creates a new history category with the parameters (title, description, etc.) specified in the request
        /// </summary>
        ///<param name="title">Title</param>
        ///<param name="description">Description</param>
        ///<param name="sortOrder">Order</param>
        ///<param name="imageName">Image name</param>
        ///<short>Create history category</short> 
        /// <category>History</category>
        ///<returns>History category</returns>
        ///<exception cref="ArgumentException"></exception>
        [HttpPost(@"history/category")]
        public HistoryCategoryDto CreateHistoryCategory([FromBody] CreateListItemCategoryRequestDto inDto)
        {

            var title = inDto.Title;
            var description = inDto.Description;
            var sortOrder = inDto.SortOrder;
            var imageName = inDto.ImageName;

            if (!(_crmSecurity.IsAdmin)) throw _crmSecurity.CreateSecurityException();

            if (string.IsNullOrEmpty(title)) throw new ArgumentException();

            var listItem = new ListItem
            {
                Title = title,
                Description = description,
                SortOrder = sortOrder,
                AdditionalParams = imageName
            };

            listItem.ID = _daoFactory.GetListItemDao().CreateItem(ListType.HistoryCategory, listItem);
            _messageService.Send(MessageAction.HistoryEventCategoryCreated, _messageTarget.Create(listItem.ID), listItem.Title);

            return _mapper.Map<HistoryCategoryDto>(listItem);
        }

        /// <summary>
        ///   Updates the selected history category with the parameters (title, description, etc.) specified in the request
        /// </summary>
        ///<param name="id">History category ID</param>
        ///<param name="title">Title</param>
        ///<param name="description">Description</param>
        ///<param name="sortOrder">Order</param>
        ///<param name="imageName">Image name</param>
        ///<short>Update history category</short> 
        ///<category>History</category>
        ///<returns>History category</returns>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        [HttpPut(@"history/category/{id:int}")]
        public HistoryCategoryDto UpdateHistoryCategory(int id, string title, string description, string imageName, int sortOrder)
        {
            if (!(_crmSecurity.IsAdmin)) throw _crmSecurity.CreateSecurityException();

            if (id <= 0 || string.IsNullOrEmpty(title)) throw new ArgumentException();

            var curHistoryCategoryExist = _daoFactory.GetListItemDao().IsExist(id);
            if (!curHistoryCategoryExist) throw new ItemNotFoundException();

            var listItem = new ListItem
            {
                Title = title,
                Description = description,
                SortOrder = sortOrder,
                AdditionalParams = imageName,
                ID = id
            };

            _daoFactory.GetListItemDao().EditItem(ListType.HistoryCategory, listItem);
            _messageService.Send(MessageAction.HistoryEventCategoryUpdated, _messageTarget.Create(listItem.ID), listItem.Title);

            return _mapper.Map<HistoryCategoryDto>(listItem);
        }

        /// <summary>
        ///    Updates the icon of the selected history category
        /// </summary>
        /// <param name="id">History category ID</param>
        /// <param name="imageName">icon name</param>
        /// <short>Update history category icon</short> 
        /// <category>History</category>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///    History category
        /// </returns>
        [HttpPut(@"history/category/{id:int}/icon")]
        public HistoryCategoryDto UpdateHistoryCategoryIcon(int id, string imageName)
        {
            if (!(_crmSecurity.IsAdmin)) throw _crmSecurity.CreateSecurityException();

            if (id <= 0) throw new ArgumentException();

            var historyCategory = _daoFactory.GetListItemDao().GetByID(id);
            if (historyCategory == null) throw new ItemNotFoundException();

            historyCategory.AdditionalParams = imageName;

            _daoFactory.GetListItemDao().ChangePicture(id, imageName);
            _messageService.Send(MessageAction.HistoryEventCategoryUpdatedIcon, _messageTarget.Create(historyCategory.ID), historyCategory.Title);

            return _mapper.Map<HistoryCategoryDto>(historyCategory);
        }

        /// <summary>
        ///    Updates the history categories order with the list specified in the request
        /// </summary>
        /// <short>
        ///    Update history categories order
        /// </short>
        /// <param name="titles">History category title list</param>
        /// <category>History</category>
        /// <returns>
        ///    History categories
        /// </returns>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        [HttpPut(@"history/category/reorder")]
        public IEnumerable<HistoryCategoryDto> UpdateHistoryCategoriesOrder(IEnumerable<string> titles)
        {
            if (!(_crmSecurity.IsAdmin)) throw _crmSecurity.CreateSecurityException();

            if (titles == null) throw new ArgumentException();

            var result = titles.Select(title => _daoFactory.GetListItemDao().GetByTitle(ListType.HistoryCategory, title)).ToList();

            _daoFactory.GetListItemDao().ReorderItems(ListType.HistoryCategory, titles.ToArray());
            _messageService.Send(MessageAction.HistoryEventCategoriesUpdatedOrder, _messageTarget.Create(result.Select(x => x.ID)), result.Select(x => x.Title));

            return _mapper.Map<List<ListItem>, List<HistoryCategoryDto>>(result);
        }

        /// <summary>
        ///   Deletes the selected history category with the ID specified in the request
        /// </summary>
        /// <short>Delete history category</short> 
        /// <category>History</category>
        /// <param name="id">History category ID</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <returns>History category</returns>
        [HttpDelete(@"history/category/{id:int}")]
        public HistoryCategoryDto DeleteHistoryCategory(int id)
        {
            if (!(_crmSecurity.IsAdmin)) throw _crmSecurity.CreateSecurityException();

            if (id <= 0) throw new ArgumentException();

            var dao = _daoFactory.GetListItemDao();
            var listItem = dao.GetByID(id);
            if (listItem == null) throw new ItemNotFoundException();

            if (dao.GetItemsCount(ListType.HistoryCategory) < 2)
            {
                throw new ArgumentException("The last history category cannot be deleted");
            }

            var result = _mapper.Map<HistoryCategoryDto>(listItem);

            dao.DeleteItem(ListType.HistoryCategory, id, 0);

            _messageService.Send(MessageAction.HistoryEventCategoryDeleted, _messageTarget.Create(listItem.ID), listItem.Title);

            return result;
        }

        /// <summary>
        ///   Creates a new task category with the parameters (title, description, etc.) specified in the request
        /// </summary>
        ///<param name="title">Title</param>
        ///<param name="description">Description</param>
        ///<param name="sortOrder">Order</param>
        ///<param name="imageName">Image name</param>
        ///<short>Create task category</short> 
        ///<category>Tasks</category>
        ///<returns>Task category</returns>
        ///<exception cref="ArgumentException"></exception>
        ///<returns>
        ///    Task category
        ///</returns>
        [HttpPost(@"task/category")]
        public TaskCategoryDto CreateTaskCategory(
            [FromBody] CreateListItemCategoryRequestDto inDto)
        {
            if (!(_crmSecurity.IsAdmin)) throw _crmSecurity.CreateSecurityException();

            var listItem = new ListItem
            {
                Title = inDto.Title,
                Description = inDto.Description,
                SortOrder = inDto.SortOrder,
                AdditionalParams =inDto.ImageName
            };

            listItem.ID = _daoFactory.GetListItemDao().CreateItem(ListType.TaskCategory, listItem);
            _messageService.Send(MessageAction.CrmTaskCategoryCreated, _messageTarget.Create(listItem.ID), listItem.Title);

            return _mapper.Map<TaskCategoryDto>(listItem);
        }

        /// <summary>
        ///   Updates the selected task category with the parameters (title, description, etc.) specified in the request
        /// </summary>
        ///<param name="id">Task category ID</param>
        ///<param name="title">Title</param>
        ///<param name="description">Description</param>
        ///<param name="sortOrder">Order</param>
        ///<param name="imageName">Image name</param>
        ///<short>Update task category</short> 
        ///<category>Tasks</category>
        ///<returns>Task category</returns>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        /// <exception cref="SecurityException"></exception>
        ///<returns>
        ///    Task category
        ///</returns>
        [HttpPut(@"task/category/{id:int}")]
        public TaskCategoryDto UpdateTaskCategory(int id, string title, string description, string imageName, int sortOrder)
        {
            if (!(_crmSecurity.IsAdmin)) throw _crmSecurity.CreateSecurityException();

            if (id <= 0 || string.IsNullOrEmpty(title)) throw new ArgumentException();

            var curTaskCategoryExist = _daoFactory.GetListItemDao().IsExist(id);
            if (!curTaskCategoryExist) throw new ItemNotFoundException();

            var listItem = new ListItem
            {
                Title = title,
                Description = description,
                SortOrder = sortOrder,
                AdditionalParams = imageName,
                ID = id
            };

            _daoFactory.GetListItemDao().EditItem(ListType.TaskCategory, listItem);
            _messageService.Send(MessageAction.CrmTaskCategoryUpdated, _messageTarget.Create(listItem.ID), listItem.Title);

            return _mapper.Map<TaskCategoryDto>(listItem);
        }

        /// <summary>
        ///    Updates the icon of the task category with the ID specified in the request
        /// </summary>
        /// <param name="id">Task category ID</param>
        /// <param name="imageName">icon name</param>
        /// <short>Update task category icon</short> 
        /// <category>Tasks</category>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///    Task category
        /// </returns>
        [HttpPut(@"task/category/{id:int}/icon")]
        public TaskCategoryDto UpdateTaskCategoryIcon(int id, string imageName)
        {
            if (!(_crmSecurity.IsAdmin)) throw _crmSecurity.CreateSecurityException();

            if (id <= 0) throw new ArgumentException();

            var taskCategory = _daoFactory.GetListItemDao().GetByID(id);
            if (taskCategory == null) throw new ItemNotFoundException();

            taskCategory.AdditionalParams = imageName;

            _daoFactory.GetListItemDao().ChangePicture(id, imageName);
            _messageService.Send(MessageAction.CrmTaskCategoryUpdatedIcon, _messageTarget.Create(taskCategory.ID), taskCategory.Title);

            return _mapper.Map<TaskCategoryDto>(taskCategory);
        }

        /// <summary>
        ///    Updates the task categories order with the list specified in the request
        /// </summary>
        /// <short>
        ///    Update task categories order
        /// </short>
        /// <param name="titles">Task category title list</param>
        /// <category>Tasks</category>
        /// <returns>
        ///    Task categories
        /// </returns>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        [HttpPut(@"task/category/reorder")]
        public IEnumerable<TaskCategoryDto> UpdateTaskCategoriesOrder([FromBody] IEnumerable<string> titles)
        {
            if (!(_crmSecurity.IsAdmin)) throw _crmSecurity.CreateSecurityException();

            if (titles == null) throw new ArgumentException();

            var result = titles.Select(title => _daoFactory.GetListItemDao().GetByTitle(ListType.TaskCategory, title)).ToList();

            _daoFactory.GetListItemDao().ReorderItems(ListType.TaskCategory, titles.ToArray());

            _messageService.Send(MessageAction.CrmTaskCategoriesUpdatedOrder, _messageTarget.Create(result.Select(x => x.ID)), result.Select(x => x.Title));

            return _mapper.Map<List<ListItem>, List<TaskCategoryDto>>(result);
        }

        /// <summary>
        ///   Deletes the task category with the ID specified in the request
        /// </summary>
        /// <short>Delete task category</short> 
        /// <category>Tasks</category>
        /// <param name="categoryid">Task category ID</param>
        /// <param name="newcategoryid">Task category ID for replace in task with current category stage</param>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        ///<exception cref="SecurityException"></exception>
        [HttpDelete(@"task/category/{categoryid:int}")]
        public TaskCategoryDto DeleteTaskCategory([FromRoute] int categoryid, [FromBody] int newcategoryid)
        {
            if (!(_crmSecurity.IsAdmin)) throw _crmSecurity.CreateSecurityException();

            if (categoryid <= 0 || newcategoryid < 0) throw new ArgumentException();

            var dao = _daoFactory.GetListItemDao();
            var listItem = dao.GetByID(categoryid);
            if (listItem == null) throw new ItemNotFoundException();

            if (dao.GetItemsCount(ListType.TaskCategory) < 2)
            {
                throw new ArgumentException("The last task category cannot be deleted");
            }

            dao.DeleteItem(ListType.TaskCategory, categoryid, newcategoryid);
            _messageService.Send(MessageAction.CrmTaskCategoryDeleted, _messageTarget.Create(listItem.ID), listItem.Title);

            return _mapper.Map<TaskCategoryDto>(listItem);
        }

        /// <summary>
        ///   Creates a new contact status with the parameters (title, description, etc.) specified in the request
        /// </summary>
        ///<param name="title">Title</param>
        ///<param name="description">Description</param>
        ///<param name="color">Color</param>
        ///<param name="sortOrder">Order</param>
        ///<returns>Contact status</returns>
        /// <short>Create contact status</short> 
        /// <category>Contacts</category>
        ///<exception cref="ArgumentException"></exception>
        /// <returns>
        ///    Contact status
        /// </returns>
        [HttpPost(@"contact/status")]
        public ContactStatusDto CreateContactStatus(
            [FromBody] CreateOrUpdateContactStatusRequestDto inDto)
        {

            var title = inDto.Title;
            var description = inDto.Description;
            var color = inDto.Color;
            var sortOrder = inDto.SortOrder;

            if (!(_crmSecurity.IsAdmin)) throw _crmSecurity.CreateSecurityException();

            var listItem = new ListItem
            {
                Title = title,
                Description = description,
                Color = color,
                SortOrder = sortOrder
            };

            listItem.ID = _daoFactory.GetListItemDao().CreateItem(ListType.ContactStatus, listItem);
            _messageService.Send(MessageAction.ContactTemperatureLevelCreated, _messageTarget.Create(listItem.ID), listItem.Title);

            return _mapper.Map<ContactStatusDto>(listItem);
        }

        /// <summary>
        ///   Updates the selected contact status with the parameters (title, description, etc.) specified in the request
        /// </summary>
        ///<param name="id">Contact status ID</param>
        ///<param name="title">Title</param>
        ///<param name="description">Description</param>
        ///<param name="color">Color</param>
        ///<param name="sortOrder">Order</param>
        ///<returns>Contact status</returns>
        /// <short>Update contact status</short> 
        /// <category>Contacts</category>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <returns>
        ///    Contact status
        /// </returns>
        [HttpPut(@"contact/status/{id:int}")]
        public ContactStatusDto UpdateContactStatus(
            [FromRoute] int id,
            [FromBody] CreateOrUpdateContactStatusRequestDto inDto)
        {

            var title = inDto.Title;
            var description = inDto.Description;
            var color = inDto.Color;
            var sortOrder = inDto.SortOrder;

            if (!(_crmSecurity.IsAdmin)) throw _crmSecurity.CreateSecurityException();

            if (id <= 0 || string.IsNullOrEmpty(title)) throw new ArgumentException();

            var curListItemExist = _daoFactory.GetListItemDao().IsExist(id);
            if (!curListItemExist) throw new ItemNotFoundException();

            var listItem = new ListItem
            {
                ID = id,
                Title = title,
                Description = description,
                Color = color,
                SortOrder = sortOrder
            };

            _daoFactory.GetListItemDao().EditItem(ListType.ContactStatus, listItem);
            _messageService.Send(MessageAction.ContactTemperatureLevelUpdated, _messageTarget.Create(listItem.ID), listItem.Title);

            return _mapper.Map<ContactStatusDto>(listItem);
        }

        /// <summary>
        ///    Updates the color of the selected contact status with the new color specified in the request
        /// </summary>
        /// <param name="id">Contact status ID</param>
        /// <param name="color">Color</param>
        /// <short>Update contact status color</short> 
        /// <category>Contacts</category>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///    Contact status
        /// </returns>
        [HttpPut(@"contact/status/{id:int}/color")]
        public ContactStatusDto UpdateContactStatusColor([FromRoute] int id, [FromBody] string color)
        {
            if (!(_crmSecurity.IsAdmin)) throw _crmSecurity.CreateSecurityException();

            if (id <= 0) throw new ArgumentException();

            var contactStatus = _daoFactory.GetListItemDao().GetByID(id);
            if (contactStatus == null) throw new ItemNotFoundException();

            contactStatus.Color = color;

            _daoFactory.GetListItemDao().ChangeColor(id, color);
            _messageService.Send(MessageAction.ContactTemperatureLevelUpdatedColor, _messageTarget.Create(contactStatus.ID), contactStatus.Title);

            return _mapper.Map<ContactStatusDto>(contactStatus);
        }

        /// <summary>
        ///    Updates the contact statuses order with the list specified in the request
        /// </summary>
        /// <short>
        ///    Update contact statuses order
        /// </short>
        /// <param name="titles">Contact status title list</param>
        /// <category>Contacts</category>
        /// <returns>
        ///    Contact statuses
        /// </returns>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        [HttpPut(@"contact/status/reorder")]
        public IEnumerable<ContactStatusDto> UpdateContactStatusesOrder([FromBody] IEnumerable<string> titles)
        {
            if (!(_crmSecurity.IsAdmin)) throw _crmSecurity.CreateSecurityException();

            if (titles == null) throw new ArgumentException();

            var result = titles.Select(title => _daoFactory.GetListItemDao().GetByTitle(ListType.ContactStatus, title)).ToList();

            _daoFactory.GetListItemDao().ReorderItems(ListType.ContactStatus, titles.ToArray());
            _messageService.Send(MessageAction.ContactTemperatureLevelsUpdatedOrder, _messageTarget.Create(result.Select(x => x.ID)), result.Select(x => x.Title));

            return _mapper.Map<List<ListItem>, List<ContactStatusDto>>(result);
        }

        /// <summary>
        ///   Deletes the contact status with the ID specified in the request
        /// </summary>
        /// <short>Delete contact status</short> 
        /// <category>Contacts</category>
        /// <param name="contactStatusid">Contact status ID</param>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        ///<exception cref="SecurityException"></exception>
        /// <returns>
        ///  Contact status
        /// </returns>
        [HttpDelete(@"contact/status/{contactStatusid:int}")]
        public ContactStatusDto DeleteContactStatus(int contactStatusid)
        {
            if (!(_crmSecurity.IsAdmin)) throw _crmSecurity.CreateSecurityException();

            if (contactStatusid <= 0) throw new ArgumentException();

            var dao = _daoFactory.GetListItemDao();
            var listItem = dao.GetByID(contactStatusid);
            if (listItem == null) throw new ItemNotFoundException();

            if (dao.GetItemsCount(ListType.ContactStatus) < 2)
            {
                throw new ArgumentException("The last contact status cannot be deleted");
            }

            var contactStatus = _mapper.Map<ContactStatusDto>(listItem);

            dao.DeleteItem(ListType.ContactStatus, contactStatusid, 0);
            _messageService.Send(MessageAction.ContactTemperatureLevelDeleted, _messageTarget.Create(contactStatus.Id), contactStatus.Title);

            return contactStatus;
        }

        /// <summary>
        ///   Returns the status of the contact for the ID specified in the request
        /// </summary>
        /// <param name="contactStatusid">Contact status ID</param>
        /// <returns>Contact status</returns>
        /// <short>Get contact status</short> 
        /// <category>Contacts</category>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        [HttpGet(@"contact/status/{contactStatusid:int}")]
        public ContactStatusDto GetContactStatusByID(int contactStatusid)
        {
            if (contactStatusid <= 0) throw new ArgumentException();

            var listItem = _daoFactory.GetListItemDao().GetByID(contactStatusid);
            if (listItem == null) throw new ItemNotFoundException();

            return _mapper.Map<ContactStatusDto>(listItem);
        }

        /// <summary>
        ///   Creates a new contact type with the parameters (title, etc.) specified in the request
        /// </summary>
        ///<param name="title">Title</param>
        ///<param name="sortOrder">Order</param>
        ///<returns>Contact type</returns>
        /// <short>Create contact type</short> 
        /// <category>Contacts</category>
        ///<exception cref="ArgumentException"></exception>
        /// <returns>
        ///    Contact type
        /// </returns>
        [HttpPost(@"contact/type")]
        public ContactTypeDto CreateContactType([FromBody] CreateOrUpdateContactTypeRequestDto inDto)
        {
            var title = inDto.Title;
            var sortOrder = inDto.SortOrder;

            if (!(_crmSecurity.IsAdmin)) throw _crmSecurity.CreateSecurityException();

            var listItem = new ListItem
            {
                Title = title,
                Description = string.Empty,
                SortOrder = sortOrder
            };

            listItem.ID = _daoFactory.GetListItemDao().CreateItem(ListType.ContactType, listItem);
            _messageService.Send(MessageAction.ContactTypeCreated, _messageTarget.Create(listItem.ID), listItem.Title);

            return _mapper.Map<ContactTypeDto>(listItem);
        }

        /// <summary>
        ///   Updates the selected contact type with the parameters (title, description, etc.) specified in the request
        /// </summary>
        ///<param name="id">Contact type ID</param>
        ///<param name="title">Title</param>
        ///<param name="sortOrder">Order</param>
        ///<returns>Contact type</returns>
        /// <short>Update contact type</short> 
        /// <category>Contacts</category>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <returns>
        ///    Contact type
        /// </returns>
        [HttpPut(@"contact/type/{id:int}")]
        public ContactTypeDto UpdateContactType(int id, [FromBody] CreateOrUpdateContactTypeRequestDto inDto)
        {
            var title = inDto.Title;
            var sortOrder = inDto.SortOrder;

            if (!(_crmSecurity.IsAdmin)) throw _crmSecurity.CreateSecurityException();

            if (id <= 0 || string.IsNullOrEmpty(title)) throw new ArgumentException();

            var curListItemExist = _daoFactory.GetListItemDao().IsExist(id);
            if (!curListItemExist) throw new ItemNotFoundException();

            var listItem = new ListItem
            {
                ID = id,
                Title = title,
                SortOrder = sortOrder
            };

            _daoFactory.GetListItemDao().EditItem(ListType.ContactType, listItem);
            _messageService.Send(MessageAction.ContactTypeUpdated, _messageTarget.Create(listItem.ID), listItem.Title);

            return _mapper.Map<ContactTypeDto>(listItem);
        }

        /// <summary>
        ///    Updates the contact types order with the list specified in the request
        /// </summary>
        /// <short>
        ///    Update contact types order
        /// </short>
        /// <param name="titles">Contact type title list</param>
        /// <category>Contacts</category>
        /// <returns>
        ///    Contact types
        /// </returns>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        [HttpPut(@"contact/type/reorder")]
        public IEnumerable<ContactTypeDto> UpdateContactTypesOrder([FromBody] IEnumerable<string> titles)
        {
            if (!(_crmSecurity.IsAdmin)) throw _crmSecurity.CreateSecurityException();

            if (titles == null) throw new ArgumentException();

            var result = titles.Select(title => _daoFactory.GetListItemDao().GetByTitle(ListType.ContactType, title)).ToList();

            _daoFactory.GetListItemDao().ReorderItems(ListType.ContactType, titles.ToArray());
            _messageService.Send(MessageAction.ContactTypesUpdatedOrder, _messageTarget.Create(result.Select(x => x.ID)), result.Select(x => x.Title));

            return _mapper.Map<List<ListItem>, List<ContactTypeDto>>(result);
        }

        /// <summary>
        ///   Deletes the contact type with the ID specified in the request
        /// </summary>
        /// <short>Delete contact type</short> 
        /// <category>Contacts</category>
        /// <param name="contactTypeid">Contact type ID</param>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        ///<exception cref="SecurityException"></exception>
        /// <returns>
        ///  Contact type
        /// </returns>
        [HttpDelete(@"contact/type/{contactTypeid:int}")]
        public ContactTypeDto DeleteContactType(int contactTypeid)
        {
            if (!(_crmSecurity.IsAdmin)) throw _crmSecurity.CreateSecurityException();

            if (contactTypeid <= 0) throw new ArgumentException();
            var dao = _daoFactory.GetListItemDao();

            var listItem = dao.GetByID(contactTypeid);
            if (listItem == null) throw new ItemNotFoundException();

            if (dao.GetItemsCount(ListType.ContactType) < 2)
            {
                throw new ArgumentException("The last contact type cannot be deleted");
            }

            var contactType = _mapper.Map<ContactTypeDto>(listItem);

            dao.DeleteItem(ListType.ContactType, contactTypeid, 0);
            _messageService.Send(MessageAction.ContactTypeDeleted, _messageTarget.Create(listItem.ID), listItem.Title);

            return contactType;
        }

        /// <summary>
        ///   Returns the type of the contact for the ID specified in the request
        /// </summary>
        /// <param name="contactTypeid">Contact type ID</param>
        /// <returns>Contact type</returns>
        /// <short>Get contact type</short> 
        /// <category>Contacts</category>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        [HttpGet(@"contact/type/{contactTypeid:int}")]
        public ContactTypeDto GetContactTypeByID(int contactTypeid)
        {
            if (contactTypeid <= 0) throw new ArgumentException();

            var listItem = _daoFactory.GetListItemDao().GetByID(contactTypeid);
            if (listItem == null) throw new ItemNotFoundException();

            return _mapper.Map<ContactTypeDto>(listItem);
        }

        /// <summary>
        ///  Returns the stage of the opportunity with the ID specified in the request
        /// </summary>
        /// <param name="stageid">Opportunity stage ID</param>
        /// <returns>Opportunity stage</returns>
        /// <short>Get opportunity stage</short> 
        /// <category>Opportunities</category>
        ///<exception cref="ItemNotFoundException"></exception>
        ///<exception cref="ArgumentException"></exception>
        [HttpGet(@"opportunity/stage/{stageid:int}")]
        public DealMilestoneDto GetDealMilestoneByID(int stageid)
        {
            if (stageid <= 0) throw new ArgumentException();

            var dealMilestone = _daoFactory.GetDealMilestoneDao().GetByID(stageid);
            if (dealMilestone == null) throw new ItemNotFoundException();

            return ToDealMilestoneDto(dealMilestone);
        }

        /// <summary>
        ///    Returns the category of the task with the ID specified in the request
        /// </summary>
        /// <param name="categoryid">Task category ID</param>
        /// <returns>Task category</returns>
        /// <short>Get task category</short> 
        /// <category>Tasks</category>
        ///<exception cref="ItemNotFoundException"></exception>
        ///<exception cref="ArgumentException"></exception>
        [HttpGet(@"task/category/{categoryid:int}")]
        public TaskCategoryDto GetTaskCategoryByID(int categoryid)
        {
            if (categoryid <= 0) throw new ArgumentException();

            var listItem = _daoFactory.GetListItemDao().GetByID(categoryid);
            if (listItem == null) throw new ItemNotFoundException();

            return _mapper.Map<TaskCategoryDto>(listItem);
        }

        /// <summary>
        ///    Returns the list of all history categories available on the portal
        /// </summary>
        /// <short>Get all history categories</short> 
        /// <category>History</category>
        /// <returns>
        ///    List of all history categories
        /// </returns>
        [HttpGet(@"history/category")]
        public IEnumerable<HistoryCategoryDto> GetHistoryCategoryDto()
        {
            var result = _daoFactory.GetListItemDao().GetItems(ListType.HistoryCategory).ConvertAll(item => new HistoryCategoryDto(item));

            var relativeItemsCount = _daoFactory.GetListItemDao().GetRelativeItemsCount(ListType.HistoryCategory);

            result.ForEach(x =>
                {
                    if (relativeItemsCount.ContainsKey(x.Id))
                        x.RelativeItemsCount = relativeItemsCount[x.Id];
                });
            return result;
        }

        /// <summary>
        ///    Returns the list of all task categories available on the portal
        /// </summary>
        /// <short>Get all task categories</short> 
        /// <category>Tasks</category>
        /// <returns>
        ///    List of all task categories
        /// </returns>
        [HttpGet(@"task/category")]
        public IEnumerable<TaskCategoryDto> GetTaskCategories()
        {
            var result = _daoFactory.GetListItemDao().GetItems(ListType.TaskCategory).ConvertAll(item => (TaskCategoryDto)_mapper.Map<TaskCategoryDto>(item));

            var relativeItemsCount = _daoFactory.GetListItemDao().GetRelativeItemsCount(ListType.TaskCategory);

            result.ForEach(x =>
                {
                    if (relativeItemsCount.ContainsKey(x.Id))
                        x.RelativeItemsCount = relativeItemsCount[x.Id];
                });
            return result;
        }

        /// <summary>
        ///    Returns the list of all contact statuses available on the portal
        /// </summary>
        /// <short>Get all contact statuses</short> 
        /// <category>Contacts</category>
        /// <returns>
        ///    List of all contact statuses
        /// </returns>
        [HttpGet(@"contact/status")]
        public IEnumerable<ContactStatusDto> GetContactStatuses()
        {
            var result = _daoFactory.GetListItemDao().GetItems(ListType.ContactStatus).ConvertAll(item => new ContactStatusDto(item));

            var relativeItemsCount = _daoFactory.GetListItemDao().GetRelativeItemsCount(ListType.ContactStatus);

            result.ForEach(x =>
                {
                    if (relativeItemsCount.ContainsKey(x.Id))
                        x.RelativeItemsCount = relativeItemsCount[x.Id];
                });
            return result;
        }

        /// <summary>
        ///    Returns the list of all contact types available on the portal
        /// </summary>
        /// <short>Get all contact types</short> 
        /// <category>Contacts</category>
        /// <returns>
        ///    List of all contact types
        /// </returns>
        [HttpGet(@"contact/type")]
        public IEnumerable<ContactTypeDto> GetContactTypes()
        {
            var result = _daoFactory.GetListItemDao().GetItems(ListType.ContactType).ConvertAll(item => new ContactTypeDto(item));

            var relativeItemsCount = _daoFactory.GetListItemDao().GetRelativeItemsCount(ListType.ContactType);

            result.ForEach(x =>
                {
                    if (relativeItemsCount.ContainsKey(x.Id))
                        x.RelativeItemsCount = relativeItemsCount[x.Id];
                });

            return result;
        }

        /// <summary>
        ///    Returns the list of all opportunity stages available on the portal
        /// </summary>
        /// <short>Get all opportunity stages</short> 
        /// <category>Opportunities</category>
        /// <returns>
        ///   List of all opportunity stages
        /// </returns>
        [HttpGet(@"opportunity/stage")]
        public IEnumerable<DealMilestoneDto> GetDealMilestones()
        {
            var result = _daoFactory.GetDealMilestoneDao().GetAll().ConvertAll(item => new DealMilestoneDto(item));

            var relativeItemsCount = _daoFactory.GetDealMilestoneDao().GetRelativeItemsCount();

            result.ForEach(x =>
                {
                    if (relativeItemsCount.ContainsKey(x.Id))
                        x.RelativeItemsCount = relativeItemsCount[x.Id];
                });

            return result;
        }

        private DealMilestoneDto ToDealMilestoneDto(DealMilestone dealMilestone)
        {
            var result = new DealMilestoneDto(dealMilestone)
            {
                RelativeItemsCount = _daoFactory.GetDealMilestoneDao().GetRelativeItemsCount(dealMilestone.ID)
            };
            return result;
        }
    }
}