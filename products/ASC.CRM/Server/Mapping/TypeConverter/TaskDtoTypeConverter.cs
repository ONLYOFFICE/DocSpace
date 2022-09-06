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

using ASC.Api.Core;
using ASC.Common;
using ASC.CRM.ApiModels;
using ASC.CRM.Core;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.Web.Api.Models;

using AutoMapper;

namespace ASC.CRM.Mapping
{
    [Scope]
    public class TaskDtoTypeConverter : ITypeConverter<Task, TaskDto>,
                                        ITypeConverter<Task, TaskBaseDto>
    {
        private readonly CrmSecurity _crmSecurity;
        private readonly ApiDateTimeHelper _apiDateTimeHelper;
        private readonly EmployeeDtoHelper _employeeDtoHelper;
        private readonly DaoFactory _daoFactory;
        private readonly EntityDtoHelper _entityDtoHelper;

        public TaskDtoTypeConverter(ApiDateTimeHelper apiDateTimeHelper,
                                 EmployeeDtoHelper employeeDtoHelper,
                                 CrmSecurity crmSecurity,
                                 DaoFactory daoFactory,
                                 EntityDtoHelper entityDtoHelper)
        {
            _apiDateTimeHelper = apiDateTimeHelper;
            _employeeDtoHelper = employeeDtoHelper;
            _crmSecurity = crmSecurity;
            _daoFactory = daoFactory;
            _entityDtoHelper = entityDtoHelper;
        }

        public TaskDto Convert(Task source, TaskDto destination, ResolutionContext context)
        {
            var result = new TaskDto
            {
                Title = source.Title,
                Description = source.Description,
                DeadLine = _apiDateTimeHelper.Get(source.DeadLine),
                Responsible = _employeeDtoHelper.Get(source.ResponsibleID),
                IsClosed = source.IsClosed,
                AlertValue = source.AlertValue,
                Created = _apiDateTimeHelper.Get(source.CreateOn)
            };

            if (source.CategoryID > 0)
            {
                var categoryItem = _daoFactory.GetListItemDao().GetByID(source.CategoryID);

                result.Category = new TaskCategoryDto(categoryItem)
                {
                    RelativeItemsCount = _daoFactory.GetListItemDao().GetRelativeItemsCount(ListType.TaskCategory, categoryItem.ID)
                };

            }

            if (source.ContactID > 0)
            {
                result.Contact = context.Mapper.Map<ContactBaseWithEmailDto>(_daoFactory.GetContactDao().GetByID(source.ContactID));
            }

            if (source.EntityID > 0)
            {

                result.Entity = _entityDtoHelper.Get(source.EntityType, source.EntityID);
            }

            result.CanEdit = _crmSecurity.CanEdit(source);

            return result;
        }

        public TaskBaseDto Convert(Task source, TaskBaseDto destination, ResolutionContext context)
        {
            if (destination != null)
                throw new NotImplementedException();

            return new TaskBaseDto
            {
                Title = source.Title,
                Description = source.Description,
                DeadLine = _apiDateTimeHelper.Get(source.DeadLine),
                Responsible = _employeeDtoHelper.Get(source.ResponsibleID),
                IsClosed = source.IsClosed,
                AlertValue = source.AlertValue
            };
        }
    }
}


