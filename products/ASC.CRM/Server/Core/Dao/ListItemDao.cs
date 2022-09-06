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
using System.Collections.Specialized;
using System.Linq;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.CRM.Classes;
using ASC.CRM.Core.EF;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.CRM.Resources;

using AutoMapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ASC.CRM.Core.Dao
{
    [Scope]
    public class ListItemDao : AbstractDao
    {
        public ListItemDao(
            CrmSecurity crmSecurity,
            DbContextManager<CrmDbContext> dbContextManager,
            TenantManager tenantManager,
            SecurityContext securityContext,
            ILogger logger,
            ICache ascCache,
            IMapper mapper)
            : base(dbContextManager,
                  tenantManager,
                  securityContext,
                  logger,
                  ascCache,
                  mapper)
        {

        }

        public bool IsExist(ListType listType, string title)
        {
            return Query(CrmDbContext.ListItem)
                    .Where(x => x.TenantId == TenantID && x.ListType == listType && x.Title.ToLower() == title.ToLower())
                    .Any();
        }

        public bool IsExist(int id)
        {
            return Query(CrmDbContext.ListItem).Where(x => x.Id == id).Any();
        }

        public List<ListItem> GetItems()
        {
            var dbListItems = Query(CrmDbContext.ListItem)
                                .AsNoTracking()
                                .OrderBy(x => x.SortOrder)
                                .ToList();

            return _mapper.Map<List<DbListItem>, List<ListItem>>(dbListItems);
        }

        public List<ListItem> GetItems(ListType listType)
        {
            var dbEntities = Query(CrmDbContext.ListItem)
                                .AsNoTracking()
                                .Where(x => x.ListType == listType)
                                .OrderBy(x => x.SortOrder)
                                .ToList();

            return _mapper.Map<List<DbListItem>, List<ListItem>>(dbEntities);
        }

        public int GetItemsCount(ListType listType)
        {
            return Query(CrmDbContext.ListItem)
                .Where(x => x.ListType == listType)
                .OrderBy(x => x.SortOrder)
                .Count();
        }

        public ListItem GetSystemListItem(int id)
        {
            switch (id)
            {
                case (int)HistoryCategorySystem.TaskClosed:
                    return new ListItem
                    {
                        ID = -1,
                        Title = HistoryCategorySystem.TaskClosed.ToLocalizedString(),
                        AdditionalParams = "event_category_close.png"
                    };
                case (int)HistoryCategorySystem.FilesUpload:
                    return new ListItem
                    {
                        ID = -2,
                        Title = HistoryCategorySystem.FilesUpload.ToLocalizedString(),
                        AdditionalParams = "event_category_attach_file.png"
                    };
                case (int)HistoryCategorySystem.MailMessage:
                    return new ListItem
                    {
                        ID = -3,
                        Title = HistoryCategorySystem.MailMessage.ToLocalizedString(),
                        AdditionalParams = "event_category_email.png"
                    };
                default:
                    return null;
            }

        }

        public List<ListItem> GetSystemItems()
        {
            return new List<ListItem>
            {
                new ListItem
                        {
                            ID = (int)HistoryCategorySystem.TaskClosed,
                            Title = HistoryCategorySystem.TaskClosed.ToLocalizedString(),
                            AdditionalParams = "event_category_close.png"
                        },
                new ListItem
                        {
                            ID = (int)HistoryCategorySystem.FilesUpload,
                            Title = HistoryCategorySystem.FilesUpload.ToLocalizedString(),
                            AdditionalParams = "event_category_attach_file.png"
                        },
                new ListItem
                        {
                            ID =(int)HistoryCategorySystem.MailMessage,
                            Title = HistoryCategorySystem.MailMessage.ToLocalizedString(),
                            AdditionalParams = "event_category_email.png"
                        }
            };
        }

        public ListItem GetByID(int id)
        {
            if (id < 0) return GetSystemListItem(id);

            var dbEntity = CrmDbContext.ListItem.Find(id);

            if (dbEntity.TenantId != TenantID) return null;

            return _mapper.Map<ListItem>(dbEntity);
        }

        public List<ListItem> GetItems(int[] id)
        {
            var dbEntities = Query(CrmDbContext.ListItem)
                                         .AsNoTracking()
                                         .Where(x => id.Contains(x.Id))
                                         .ToList();

            var entities = _mapper.Map<List<DbListItem>, List<ListItem>>(dbEntities);

            var systemItem = id.Where(item => item < 0).Select(GetSystemListItem);

            return systemItem.Any() ? entities.Union(systemItem).ToList() : entities;
        }

        public List<ListItem> GetAll()
        {
            var dbListItems = Query(CrmDbContext.ListItem)
                            .AsNoTracking()
                            .ToList();

            return _mapper.Map<List<DbListItem>, List<ListItem>>(dbListItems);
        }

        public void ChangeColor(int id, string newColor)
        {
            var dbEntity = CrmDbContext.ListItem.Find(id);

            dbEntity.Color = newColor;

            CrmDbContext.SaveChanges();
        }

        public NameValueCollection GetColors(ListType listType)
        {
            var result = new NameValueCollection();

            Query(CrmDbContext.ListItem)
                           .Where(x => x.ListType == listType)
                           .Select(x => new { x.Id, x.Color })
                           .ToList()
                           .ForEach(x => result.Add(x.Id.ToString(), x.Color));

            return result;

        }

        public ListItem GetByTitle(ListType listType, string title)
        {
            var dbEntity = Query(CrmDbContext.ListItem)
                                .FirstOrDefault(x => x.Title.ToLower() == title.ToLower() && x.ListType == listType);

            return _mapper.Map<ListItem>(dbEntity);
        }

        public int GetRelativeItemsCount(ListType listType, int id)
        {
            int result;

            switch (listType)
            {
                case ListType.ContactStatus:
                    result = Query(CrmDbContext.Contacts).Where(x => x.StatusId == id).Count();
                    break;
                case ListType.ContactType:
                    result = Query(CrmDbContext.Contacts).Where(x => x.ContactTypeId == id).Count();
                    break;
                case ListType.TaskCategory:
                    result = Query(CrmDbContext.Tasks).Where(x => x.CategoryId == id).Count();
                    break;
                case ListType.HistoryCategory:
                    result = Query(CrmDbContext.RelationshipEvent).Where(x => x.CategoryId == id).Count();
                    break;
                default:
                    throw new ArgumentException();

            }

            return result;
        }

        public Dictionary<int, int> GetRelativeItemsCount(ListType listType)
        {

            Dictionary<int, int> result;

            switch (listType)
            {
                case ListType.ContactStatus:
                {
                    result = Query(CrmDbContext.ListItem)
                            .Join(Query(CrmDbContext.Contacts),
                                       x => x.Id,
                                       y => y.StatusId,
                                       (x, y) => new { x, y })
                            .Where(x => x.x.ListType == listType)
                            .GroupBy(x => x.x.Id)
                            .Select(x => new { Id = x.Key, Count = x.Count() })
                            .ToDictionary(x => x.Id, x => x.Count);

                    break;
                }
                case ListType.ContactType:
                {
                    result = Query(CrmDbContext.ListItem)
                            .Join(Query(CrmDbContext.Contacts),
                                       x => x.Id,
                                       y => y.ContactTypeId,
                                       (x, y) => new { x, y })
                            .Where(x => x.x.ListType == listType)
                            .GroupBy(x => x.x.Id)
                            .Select(x => new { Id = x.Key, Count = x.Count() })
                            .ToDictionary(x => x.Id, x => x.Count);

                    break;
                }
                case ListType.TaskCategory:
                {
                    result = Query(CrmDbContext.ListItem)
                            .Join(Query(CrmDbContext.Tasks),
                                       x => x.Id,
                                       y => y.CategoryId,
                                       (x, y) => new { x, y })
                            .Where(x => x.x.ListType == listType)
                            .GroupBy(x => x.x.Id)
                            .Select(x => new { Id = x.Key, Count = x.Count() })
                            .ToDictionary(x => x.Id, x => x.Count);

                    break;
                }
                case ListType.HistoryCategory:
                {

                    result = Query(CrmDbContext.ListItem)
                            .Join(Query(CrmDbContext.RelationshipEvent),
                                       x => x.Id,
                                       y => y.CategoryId,
                                       (x, y) => new { x, y })
                            .Where(x => x.x.ListType == listType)
                            .GroupBy(x => x.x.Id)
                            .Select(x => new { Id = x.Key, Count = x.Count() })
                            .ToDictionary(x => x.Id, x => x.Count);

                    break;
                }
                default:
                    throw new ArgumentException();
            }

            return result;
        }

        public int CreateItem(ListType listType, ListItem enumItem)
        {

            if (IsExist(listType, enumItem.Title))
                return GetByTitle(listType, enumItem.Title).ID;

            if (string.IsNullOrEmpty(enumItem.Title))
                throw new ArgumentException();

            if (listType == ListType.TaskCategory || listType == ListType.HistoryCategory)
                if (string.IsNullOrEmpty(enumItem.AdditionalParams))
                    throw new ArgumentException();
                else
                    enumItem.AdditionalParams = System.IO.Path.GetFileName(enumItem.AdditionalParams);

            if (listType == ListType.ContactStatus)
                if (string.IsNullOrEmpty(enumItem.Color))
                    throw new ArgumentException();

            var sortOrder = enumItem.SortOrder;

            if (sortOrder == 0)
            {
                if(Query(CrmDbContext.ListItem).Any(x => x.ListType == listType))
                sortOrder = Query(CrmDbContext.ListItem)
                    .Where(x => x.ListType == listType)
                    .Max(x => x.SortOrder) + 1;
            }

            var listItem = new DbListItem
            {
                ListType = listType,
                Description = enumItem.Description,
                Title = enumItem.Title,
                AdditionalParams = enumItem.AdditionalParams,
                Color = enumItem.Color,
                SortOrder = sortOrder,
                TenantId = TenantID
            };

            CrmDbContext.Add(listItem);

            CrmDbContext.SaveChanges();

            return listItem.Id;
        }

        public void EditItem(ListType listType, ListItem enumItem)
        {
            if (HaveRelativeItemsLink(listType, enumItem.ID))
            {
                switch (listType)
                {
                    case ListType.ContactStatus:
                    case ListType.ContactType:
                        throw new ArgumentException($"{CRMErrorsResource.BasicCannotBeEdited}. {CRMErrorsResource.HasRelatedContacts}.");
                    case ListType.TaskCategory:
                        throw new ArgumentException($"{CRMErrorsResource.BasicCannotBeEdited}. {CRMErrorsResource.TaskCategoryHasRelatedTasks}.");
                    case ListType.HistoryCategory:
                        throw new ArgumentException($"{CRMErrorsResource.BasicCannotBeEdited}. {CRMErrorsResource.HistoryCategoryHasRelatedEvents}.");
                    default:
                        throw new ArgumentException($"{CRMErrorsResource.BasicCannotBeEdited}.");
                }
            }

            var itemToUpdate = Query(CrmDbContext.ListItem).Single(x => x.Id == enumItem.ID);

            itemToUpdate.Description = enumItem.Description;
            itemToUpdate.Title = enumItem.Title;
            itemToUpdate.AdditionalParams = enumItem.AdditionalParams;
            itemToUpdate.Color = enumItem.Color;

            CrmDbContext.SaveChanges();
        }

        public void ChangePicture(int id, String newPicture)
        {
            var itemToUpdate = Query(CrmDbContext.ListItem).Single(x => x.Id == id);

            itemToUpdate.AdditionalParams = newPicture;

            CrmDbContext.Update(itemToUpdate);

            CrmDbContext.SaveChanges();
        }

        private bool HaveRelativeItemsLink(ListType listType, int itemID)
        {
            bool result;

            switch (listType)
            {
                case ListType.ContactStatus:
                    result = Query(CrmDbContext.Contacts).Where(x => x.StatusId == itemID).Any();
                    break;
                case ListType.ContactType:
                    result = Query(CrmDbContext.Contacts).Where(x => x.ContactTypeId == itemID).Any();
                    break;
                case ListType.TaskCategory:
                    result = Query(CrmDbContext.Tasks).Where(x => x.CategoryId == itemID).Any();
                    break;
                case ListType.HistoryCategory:
                    result = Query(CrmDbContext.RelationshipEvent).Where(x => x.CategoryId == itemID).Any();
                    break;
                default:
                    throw new ArgumentException();
            }

            return result;
        }

        public void ChangeRelativeItemsLink(ListType listType, int fromItemID, int toItemID)
        {
            if (!IsExist(fromItemID))
                throw new ArgumentException("", "toItemID");

            if (!HaveRelativeItemsLink(listType, fromItemID)) return;

            if (!IsExist(toItemID))
                throw new ArgumentException("", "toItemID");

            switch (listType)
            {
                case ListType.ContactStatus:
                {
                    var itemToUpdate = Query(CrmDbContext.Contacts).Single(x => x.StatusId == fromItemID);

                    itemToUpdate.StatusId = toItemID;

                    CrmDbContext.Update(itemToUpdate);
                }
                break;
                case ListType.ContactType:
                {
                    var itemToUpdate = Query(CrmDbContext.Contacts).Single(x => x.ContactTypeId == fromItemID);

                    itemToUpdate.ContactTypeId = toItemID;

                    CrmDbContext.Update(itemToUpdate);
                }
                break;
                case ListType.TaskCategory:
                {
                    var itemToUpdate = Query(CrmDbContext.Tasks).Single(x => x.CategoryId == fromItemID);
                    itemToUpdate.CategoryId = toItemID;

                    CrmDbContext.Update(itemToUpdate);
                }
                break;
                case ListType.HistoryCategory:
                {
                    var itemToUpdate = Query(CrmDbContext.RelationshipEvent).Single(x => x.CategoryId == fromItemID);
                    itemToUpdate.CategoryId = toItemID;

                    CrmDbContext.Update(itemToUpdate);
                }
                break;
                default:
                    throw new ArgumentException();
            }

            CrmDbContext.SaveChanges();
        }

        public void DeleteItem(ListType listType, int itemID, int toItemID)
        {
            if (HaveRelativeItemsLink(listType, itemID))
            {
                switch (listType)
                {
                    case ListType.ContactStatus:
                    case ListType.ContactType:
                        throw new ArgumentException($"{CRMErrorsResource.BasicCannotBeDeleted}. {CRMErrorsResource.HasRelatedContacts}.");
                    case ListType.TaskCategory:
                        var exMsg = $"{CRMErrorsResource.BasicCannotBeDeleted}. {CRMErrorsResource.TaskCategoryHasRelatedTasks}.";
                        if (itemID == toItemID) throw new ArgumentException(exMsg);
                        ChangeRelativeItemsLink(listType, itemID, toItemID);
                        break;
                    case ListType.HistoryCategory:
                        throw new ArgumentException($"{CRMErrorsResource.BasicCannotBeDeleted}. {CRMErrorsResource.HistoryCategoryHasRelatedEvents}.");
                    default:
                        throw new ArgumentException($"{CRMErrorsResource.BasicCannotBeDeleted}.");
                }
            }

            var dbEntity = CrmDbContext.ListItem.Find(itemID);

            CrmDbContext.ListItem.Remove(dbEntity);

            CrmDbContext.SaveChanges();

        }

        public void ReorderItems(ListType listType, string[] titles)
        {
            for (int index = 0; index < titles.Length; index++)
            {
                var dbEntity = Query(CrmDbContext.ListItem)
                               .Single(x => string.Compare(x.Title, titles[index]) == 0 && x.ListType == listType);

                dbEntity.SortOrder = index;
            }

            CrmDbContext.SaveChanges();
        }
    }
}