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

using ASC.Collections;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.CRM.Classes;
using ASC.CRM.Core.EF;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.CRM.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace ASC.CRM.Core.Dao
{

    public class CachedListItem : ListItemDao
    {
        private readonly HttpRequestDictionary<ListItem> _listItemCache;

        public CachedListItem(
            DbContextManager<CRMDbContext> dbContextManager,
            TenantManager tenantManager,
            SecurityContext securityContext,
            IHttpContextAccessor httpContextAccessor,
            IOptionsMonitor<ILog> logger)
            : base(dbContextManager,
                  tenantManager,
                  securityContext,
                  logger)
        {
            _listItemCache = new HttpRequestDictionary<ListItem>(httpContextAccessor?.HttpContext, "crm_list_item");
        }

        public override void ChangeColor(int id, string newColor)
        {
            ResetCache(id);

            base.ChangeColor(id, newColor);
        }

        public override void DeleteItem(ListType listType, int itemID, int toItemID)
        {
            ResetCache(itemID);

            base.DeleteItem(listType, itemID, toItemID);
        }

        public override void ChangePicture(int id, string newPicture)
        {
            ResetCache(id);

            base.ChangePicture(id, newPicture);
        }

        public override void EditItem(ListType listType, ListItem enumItem)
        {
            ResetCache(enumItem.ID);

            base.EditItem(listType, enumItem);
        }

        public override void ReorderItems(ListType listType, string[] titles)
        {
            _listItemCache.Clear();

            base.ReorderItems(listType, titles);
        }

        public override ListItem GetByID(int id)
        {
            return _listItemCache.Get(id.ToString(), () => GetByIDBase(id));
        }

        private ListItem GetByIDBase(int id)
        {
            return base.GetByID(id);
        }

        private void ResetCache(int id)
        {
            _listItemCache.Reset(id.ToString());
        }
    }

    public class ListItemDao : AbstractDao
    {
        public ListItemDao(
            DbContextManager<CRMDbContext> dbContextManager,
            TenantManager tenantManager,
            SecurityContext securityContext,
            IOptionsMonitor<ILog> logger)
            : base(dbContextManager,
                  tenantManager,
                  securityContext,
                  logger)
        {


        }

        public bool IsExist(ListType listType, String title)
        {
            return CRMDbContext.ListItem
                    .Where(x => x.TenantId == TenantID && x.ListType == listType && String.Compare(x.Title, title, true) == 0)
                    .Any();
        }

        public bool IsExist(int id)
        {
            return CRMDbContext.ListItem.Where(x => x.Id == id).Any();
        }

        public List<ListItem> GetItems()
        {
            return Query(CRMDbContext.ListItem)
            .OrderBy(x => x.SortOrder)
            .ToList()
            .ConvertAll(ToListItem);
        }

        public List<ListItem> GetItems(ListType listType)
        {
            return Query(CRMDbContext.ListItem)
                        .Where(x => x.ListType == listType)
                        .OrderBy(x => x.SortOrder)
                        .ToList()
                        .ConvertAll(ToListItem);
        }

        public int GetItemsCount(ListType listType)
        {
            return Query(CRMDbContext.ListItem)
                .Where(x => x.ListType == listType)
                .OrderBy(x => x.SortOrder).Count();
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

        public virtual ListItem GetByID(int id)
        {
            if (id < 0) return GetSystemListItem(id);

            return ToListItem(Query(CRMDbContext.ListItem).FirstOrDefault(x => x.Id == id));
        }

        public virtual List<ListItem> GetItems(int[] id)
        {
            var sqlResult = CRMDbContext.ListItem.Where(x => id.Contains(x.Id)).ToList().ConvertAll(ToListItem);

            var systemItem = id.Where(item => item < 0).Select(GetSystemListItem);

            return systemItem.Any() ? sqlResult.Union(systemItem).ToList() : sqlResult;
        }

        public virtual List<ListItem> GetAll()
        {
            return CRMDbContext
                        .ListItem
                        .ToList()
                        .ConvertAll(ToListItem);
        }

        public virtual void ChangeColor(int id, string newColor)
        {
            var listItem = new DbListItem
            {
                Id = id,
                Color = newColor
            };


            CRMDbContext.Attach(listItem);
            CRMDbContext.Entry(listItem).Property("Color").IsModified = true;
            CRMDbContext.SaveChanges();
        }

        public NameValueCollection GetColors(ListType listType)
        {
            var result = new NameValueCollection();

            Query(CRMDbContext.ListItem)
                            .Where(x => x.ListType == listType)
                            .Select(x => new { x.Id, x.Color })
                            .ToList()
                            .ForEach(x => result.Add(x.Id.ToString(), x.Color.ToString()));

            return result;

        }

        public ListItem GetByTitle(ListType listType, string title)
        {
            return ToListItem(Query(CRMDbContext.ListItem)
                                .FirstOrDefault(x => String.Compare(x.Title, title, true) == 0 && x.ListType == listType));
        }

        public int GetRelativeItemsCount(ListType listType, int id)
        {
            int result;

            switch (listType)
            {
                case ListType.ContactStatus:
                    result = Query(CRMDbContext.Contacts).Where(x => x.StatusId == id).Count();
                    break;
                case ListType.ContactType:
                    result = Query(CRMDbContext.Contacts).Where(x => x.ContactTypeId == id).Count();
                    break;
                case ListType.TaskCategory:
                    result = Query(CRMDbContext.Tasks).Where(x => x.CategoryId == id).Count();
                    break;
                case ListType.HistoryCategory:
                    result = Query(CRMDbContext.RelationshipEvent).Where(x => x.CategoryId == id).Count();
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
                        result = Query(CRMDbContext.ListItem)
                                .GroupJoin(Query(CRMDbContext.Contacts),
                                           x => x.Id,
                                           y => y.StatusId,
                                           (x, y) => new { Column1 = x, Column2 = y.Count() })
                                .Where(x => x.Column1.ListType == listType)
                                .OrderBy(x => x.Column1.SortOrder)
                                .ToDictionary(x => x.Column1.Id, x => x.Column2);

                        break;
                    }
                case ListType.ContactType:
                    {
                        result = Query(CRMDbContext.ListItem)
                                .GroupJoin(Query(CRMDbContext.Contacts),
                                           x => x.Id,
                                           y => y.ContactTypeId,
                                           (x, y) => new { Column1 = x, Column2 = y.Count() })
                                .Where(x => x.Column1.ListType == listType)
                                .OrderBy(x => x.Column1.SortOrder)
                                .ToDictionary(x => x.Column1.Id, x => x.Column2);
                        break;
                    }
                case ListType.TaskCategory:
                    {
                        result = Query(CRMDbContext.ListItem)
                                .GroupJoin(Query(CRMDbContext.Tasks),
                                           x => x.Id,
                                           y => y.CategoryId,
                                           (x, y) => new { Column1 = x, Column2 = y.Count() })
                                .Where(x => x.Column1.ListType == listType)
                                .OrderBy(x => x.Column1.SortOrder)
                                .ToDictionary(x => x.Column1.Id, x => x.Column2);

                        break;
                    }
                case ListType.HistoryCategory:
                    {
                        result = Query(CRMDbContext.ListItem)
                            .GroupJoin(Query(CRMDbContext.RelationshipEvent),
                                       x => x.Id,
                                       y => y.CategoryId,
                                       (x, y) => new { Column1 = x, Column2 = y.Count() })
                            .Where(x => x.Column1.ListType == listType)
                            .OrderBy(x => x.Column1.SortOrder)
                            .ToDictionary(x => x.Column1.Id, x => x.Column2);

                        break;
                    }
                default:
                    throw new ArgumentException();
            }

            return result;
        }

        public virtual int CreateItem(ListType listType, ListItem enumItem)
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
                sortOrder = Query(CRMDbContext.ListItem)
                    .Where(x => x.ListType == listType)
                    .Max(x => x.SortOrder) + 1;

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

            CRMDbContext.Add(listItem);

            CRMDbContext.SaveChanges();

            return listItem.Id;
        }

        public virtual void EditItem(ListType listType, ListItem enumItem)
        {
            if (HaveRelativeItemsLink(listType, enumItem.ID))
            {
                switch (listType)
                {
                    case ListType.ContactStatus:
                    case ListType.ContactType:
                        throw new ArgumentException(string.Format("{0}. {1}.", CRMErrorsResource.BasicCannotBeEdited, CRMErrorsResource.HasRelatedContacts));
                    case ListType.TaskCategory:
                        throw new ArgumentException(string.Format("{0}. {1}.", CRMErrorsResource.BasicCannotBeEdited, CRMErrorsResource.TaskCategoryHasRelatedTasks));
                    case ListType.HistoryCategory:
                        throw new ArgumentException(string.Format("{0}. {1}.", CRMErrorsResource.BasicCannotBeEdited, CRMErrorsResource.HistoryCategoryHasRelatedEvents));
                    default:
                        throw new ArgumentException(string.Format("{0}.", CRMErrorsResource.BasicCannotBeEdited));
                }
            }

            var itemToUpdate = Query(CRMDbContext.ListItem).Single(x => x.Id == enumItem.ID);

            itemToUpdate.Description = enumItem.Description;
            itemToUpdate.Title = enumItem.Title;
            itemToUpdate.AdditionalParams = enumItem.AdditionalParams;
            itemToUpdate.Color = enumItem.Color;

            CRMDbContext.SaveChanges();
        }

        public virtual void ChangePicture(int id, String newPicture)
        {
            var itemToUpdate = Query(CRMDbContext.ListItem).Single(x => x.Id == id);

            itemToUpdate.AdditionalParams = newPicture;

            CRMDbContext.Update(itemToUpdate);

            CRMDbContext.SaveChanges();
        }

        private bool HaveRelativeItemsLink(ListType listType, int itemID)
        {
            bool result;

            switch (listType)
            {
                case ListType.ContactStatus:
                    result = Query(CRMDbContext.Contacts).Where(x => x.StatusId == itemID).Any();
                    break;
                case ListType.ContactType:
                    result = Query(CRMDbContext.Contacts).Where(x => x.ContactTypeId == itemID).Any();
                    break;
                case ListType.TaskCategory:
                    result = Query(CRMDbContext.Tasks).Where(x => x.CategoryId == itemID).Any();
                    break;
                case ListType.HistoryCategory:
                    result = Query(CRMDbContext.RelationshipEvent).Where(x => x.CategoryId == itemID).Any();
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
                        var itemToUpdate = Query(CRMDbContext.Contacts).Single(x => x.StatusId == fromItemID);

                        itemToUpdate.StatusId = toItemID;

                        CRMDbContext.Update(itemToUpdate);
                    }
                    break;
                case ListType.ContactType:
                    {
                        var itemToUpdate = Query(CRMDbContext.Contacts).Single(x => x.ContactTypeId == fromItemID);

                        itemToUpdate.ContactTypeId = toItemID;

                        CRMDbContext.Update(itemToUpdate);
                    }
                    break;
                case ListType.TaskCategory:
                    {
                        var itemToUpdate = Query(CRMDbContext.Tasks).Single(x => x.CategoryId == fromItemID);
                        itemToUpdate.CategoryId = toItemID;

                        CRMDbContext.Update(itemToUpdate);
                    }
                    break;
                case ListType.HistoryCategory:
                    {
                        var itemToUpdate = Query(CRMDbContext.RelationshipEvent).Single(x => x.CategoryId == fromItemID);
                        itemToUpdate.CategoryId = toItemID;

                        CRMDbContext.Update(itemToUpdate);
                    }
                    break;
                default:
                    throw new ArgumentException();
            }

            CRMDbContext.SaveChanges();
        }

        public virtual void DeleteItem(ListType listType, int itemID, int toItemID)
        {
            if (HaveRelativeItemsLink(listType, itemID))
            {
                switch (listType)
                {
                    case ListType.ContactStatus:
                    case ListType.ContactType:
                        throw new ArgumentException(string.Format("{0}. {1}.", CRMErrorsResource.BasicCannotBeDeleted, CRMErrorsResource.HasRelatedContacts));
                    case ListType.TaskCategory:
                        var exMsg = string.Format("{0}. {1}.", CRMErrorsResource.BasicCannotBeDeleted, CRMErrorsResource.TaskCategoryHasRelatedTasks);
                        if (itemID == toItemID) throw new ArgumentException(exMsg);
                        ChangeRelativeItemsLink(listType, itemID, toItemID);
                        break;
                    case ListType.HistoryCategory:
                        throw new ArgumentException(string.Format("{0}. {1}.", CRMErrorsResource.BasicCannotBeDeleted, CRMErrorsResource.HistoryCategoryHasRelatedEvents));
                    default:
                        throw new ArgumentException(string.Format("{0}.", CRMErrorsResource.BasicCannotBeDeleted));
                }
            }

            var itemToRemove = new DbListItem { Id = itemID };

            CRMDbContext.Entry(itemToRemove).State = EntityState.Deleted;

            CRMDbContext.SaveChanges();

        }

        public virtual void ReorderItems(ListType listType, String[] titles)
        {
            using var tx = CRMDbContext.Database.BeginTransaction();

            for (int index = 0; index < titles.Length; index++)
            {
                var itemToUpdate = Query(CRMDbContext.ListItem)
                                       .Single(x => String.Compare(x.Title, titles[index]) == 0 && x.ListType == listType);

                itemToUpdate.SortOrder = index;

                CRMDbContext.Update(itemToUpdate);
            }


            CRMDbContext.SaveChanges();

            tx.Commit();
        }

        public static ListItem ToListItem(DbListItem dbListItem)
        {
            if (dbListItem == null) return null;

            var result = new ListItem
            {
                ID = dbListItem.Id,
                Title = dbListItem.Title,
                Description = dbListItem.Description,
                Color = dbListItem.Color,
                SortOrder = dbListItem.SortOrder,
                AdditionalParams = dbListItem.AdditionalParams,
                ListType = dbListItem.ListType
            };

            return result;
        }
    }
}