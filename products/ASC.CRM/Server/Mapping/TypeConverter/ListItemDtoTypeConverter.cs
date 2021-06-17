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

using ASC.Common;
using ASC.CRM.ApiModels;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.CRM.Configuration;

using AutoMapper;

namespace ASC.CRM.Mapping
{

    [Scope]
    public sealed class ListItemDtoTypeConverter : ITypeConverter<ListItem, TaskCategoryBaseDto>,
                                                   ITypeConverter<ListItem, TaskCategoryDto>,
                                                   ITypeConverter<ListItem, HistoryCategoryDto>,
                                                   ITypeConverter<ListItem, ContactStatusDto>,
                                                   ITypeConverter<ListItem, ContactTypeDto>

    {
        private readonly WebImageSupplier _webImageSupplier;
        private readonly DaoFactory _daoFactory;

        public ListItemDtoTypeConverter(WebImageSupplier webImageSupplier,
                                        DaoFactory daoFactory)
        {
            _webImageSupplier = webImageSupplier;
            _daoFactory = daoFactory;
        }

        public TaskCategoryBaseDto Convert(ListItem source, TaskCategoryBaseDto destination, ResolutionContext context)
        {
            if (destination != null)
                throw new NotImplementedException();

            return new TaskCategoryDto(source)
            {
                ImagePath = _webImageSupplier.GetAbsoluteWebPath(source.AdditionalParams, ProductEntryPoint.ID)
            };
        }

        public HistoryCategoryDto Convert(ListItem source, HistoryCategoryDto destination, ResolutionContext context)
        {
            if (destination != null)
                throw new NotImplementedException();

            var result = new HistoryCategoryDto(source);

            result.RelativeItemsCount = _daoFactory.GetListItemDao().GetRelativeItemsCount(ListType.HistoryCategory, source.ID);

            return result;
        }

        public TaskCategoryDto Convert(ListItem source, TaskCategoryDto destination, ResolutionContext context)
        {
            if (destination != null)
                throw new NotImplementedException();

            var result = new TaskCategoryDto(source);

            result.RelativeItemsCount = _daoFactory.GetListItemDao().GetRelativeItemsCount(ListType.TaskCategory, source.ID);

            return result;
        }

        public ContactStatusDto Convert(ListItem source, ContactStatusDto destination, ResolutionContext context)
        {
            if (destination != null)
                throw new NotImplementedException();

            var result = new ContactStatusDto(source);

            result.RelativeItemsCount = _daoFactory.GetListItemDao().GetRelativeItemsCount(ListType.ContactStatus, source.ID);

            return result;

        }

        public ContactTypeDto Convert(ListItem source, ContactTypeDto destination, ResolutionContext context)
        {
            if (destination != null)
                throw new NotImplementedException();

            var result = new ContactTypeDto(source);

            result.RelativeItemsCount = _daoFactory.GetListItemDao().GetRelativeItemsCount(ListType.ContactType, source.ID);

            return result;

        }
    }
}