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


#region Import

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Enums;
using ASC.Web.CRM.Classes;

using EnumExtension = ASC.CRM.Classes.EnumExtension;

#endregion

namespace ASC.CRM.Core.Entities
{
    public abstract class FilterObject
    {
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
        public string FilterValue { get; set; }

        public bool IsAsc
        {
            get
            {
                return !String.IsNullOrEmpty(SortOrder) && SortOrder != "descending";
            }
        }

        public abstract ICollection GetItemsByFilter(DaoFactory daofactory);
    }

    public class CasesFilterObject : FilterObject
    {
        public bool? IsClosed { get; set; }
        public List<string> Tags { get; set; }

        public CasesFilterObject()
        {
            SortBy = "title";
            SortOrder = "ascending";
        }

        public CasesFilterObject(string base64String)
        {
            if (string.IsNullOrEmpty(base64String)) return;

            var json = Encoding.UTF8.GetString(Convert.FromBase64String(base64String));

            var jsonArray = json.Split(';');

            foreach (var filterItem in jsonArray)
            {
                var filterObj = JsonDocument.Parse(filterItem).RootElement;

                var paramString = filterObj.GetProperty("params").GetString();

                if (string.IsNullOrEmpty(paramString)) continue;

                var filterParam = JsonDocument.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(paramString))).RootElement;

                switch (filterObj.GetProperty("id").GetString())
                {
                    case "sorter":
                        SortBy = filterParam.GetProperty("id").GetString();
                        SortOrder = filterParam.GetProperty("sortOrder").GetString();
                        break;

                    case "text":
                        FilterValue = filterParam.GetProperty("value").GetString();
                        break;

                    case "closed":
                    case "opened":
                        IsClosed = filterParam.GetProperty("value").GetBoolean();
                        break;

                    case "tags":
                        Tags = filterParam.GetProperty("value").EnumerateArray().Select(x => x.GetString()).ToList();
                        break;
                }
            }
        }

        public override ICollection GetItemsByFilter(DaoFactory daofactory)
        {
            SortedByType sortBy;
            if (!EnumExtension.TryParse(SortBy, true, out sortBy))
            {
                sortBy = SortedByType.Title;
            }

            return daofactory.GetCasesDao().GetCases(
                FilterValue,
                0,
                IsClosed,
                Tags,
                0, 0,
                new OrderBy(sortBy, IsAsc));
        }
    }

    public class TaskFilterObject : FilterObject
    {
        public int CategoryId { get; set; }
        public int ContactId { get; set; }
        public Guid ResponsibleId { get; set; }
        public bool? IsClosed { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public TaskFilterObject()
        {
            IsClosed = null;
            FromDate = DateTime.MinValue;
            ToDate = DateTime.MinValue;
            SortBy = "deadline";
            SortOrder = "ascending";
        }

        public TaskFilterObject(string base64String)
        {
            if (string.IsNullOrEmpty(base64String)) return;

            var json = Encoding.UTF8.GetString(Convert.FromBase64String(base64String));

            var jsonArray = json.Split(';');

            foreach (var filterItem in jsonArray)
            {
                var filterObj = JsonDocument.Parse(filterItem).RootElement;

                var paramString = filterObj.GetProperty("params").GetString();

                if (string.IsNullOrEmpty(paramString)) continue;

                var filterParam = Global.JObjectParseWithDateAsString(Encoding.UTF8.GetString(Convert.FromBase64String(paramString))).RootElement;

                switch (filterObj.GetProperty("id").GetString())
                {
                    case "sorter":
                        SortBy = filterParam.GetProperty("id").GetString();
                        SortOrder = filterParam.GetProperty("sortOrder").GetString();
                        break;

                    case "text":
                        FilterValue = filterParam.GetProperty("value").GetString();
                        break;

                    case "my":
                    case "responsibleID":
                        ResponsibleId = filterParam.GetProperty("value").GetGuid();
                        break;

                    case "overdue":
                    case "today":
                    case "theNext":
                        var valueString = filterParam.GetProperty("value").GetString();
                        var fromToArray = JsonDocument.Parse(valueString)
                                         .RootElement
                                         .EnumerateArray()
                                         .Select(x => x.GetString())
                                         .ToList();

                        if (fromToArray.Count != 2) continue;

                        FromDate = !String.IsNullOrEmpty(fromToArray[0])
                                              ? Global.ApiDateTimeParse(fromToArray[0]) : DateTime.MinValue;
                        ToDate = !String.IsNullOrEmpty(fromToArray[1])
                                            ? Global.ApiDateTimeParse(fromToArray[1]) : DateTime.MinValue;
                        break;

                    case "fromToDate":
                        FromDate = filterParam.GetProperty("from").GetDateTime();
                        ToDate = (filterParam.GetProperty("to").GetDateTime()).AddDays(1).AddSeconds(-1);
                        break;

                    case "categoryID":
                        CategoryId = filterParam.GetProperty("value").GetInt32();
                        break;

                    case "openTask":
                    case "closedTask":
                        IsClosed = filterParam.GetProperty("value").GetBoolean();
                        break;

                    case "contactID":
                        ContactId = filterParam.GetProperty("id").GetInt32();
                        break;
                }
            }
        }

        public override ICollection GetItemsByFilter(DaoFactory daofactory)
        {
            TaskSortedByType sortBy;
            if (!EnumExtension.TryParse(SortBy, true, out sortBy))
            {
                sortBy = TaskSortedByType.DeadLine;
            }

            return daofactory.GetTaskDao().GetTasks(
                FilterValue,
                ResponsibleId,
                CategoryId,
                IsClosed,
                FromDate,
                ToDate,
                ContactId > 0 ? EntityType.Contact : EntityType.Any,
                ContactId,
                0, 0,
                new OrderBy(sortBy, IsAsc));
        }
    }

    public class DealFilterObject : FilterObject
    {
        public Guid ResponsibleId { get; set; }
        public String StageType { get; set; }
        public int OpportunityStageId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int ContactId { get; set; }
        public bool? ContactAlsoIsParticipant { get; set; }
        public List<string> Tags { get; set; }

        public DealFilterObject()
        {
            ContactAlsoIsParticipant = null;
            FromDate = DateTime.MinValue;
            ToDate = DateTime.MinValue;
            SortBy = "stage";
            SortOrder = "ascending";
        }

        public DealFilterObject(string base64String)
        {
            if (string.IsNullOrEmpty(base64String)) return;

            var json = Encoding.UTF8.GetString(Convert.FromBase64String(base64String));

            var jsonArray = json.Split(';');

            foreach (var filterItem in jsonArray)
            {
                var filterObj = JsonDocument.Parse(filterItem).RootElement;

                var paramString = filterObj.GetProperty("params").GetString();

                if (string.IsNullOrEmpty(paramString)) continue;

                var filterParam = Global.JObjectParseWithDateAsString(Encoding.UTF8.GetString(Convert.FromBase64String(paramString))).RootElement;

                switch (filterObj.GetProperty("id").GetString())
                {
                    case "sorter":
                        SortBy = filterParam.GetProperty("id").GetString();
                        SortOrder = filterParam.GetProperty("sortOrder").GetString();
                        break;

                    case "text":
                        FilterValue = filterParam.GetProperty("value").GetString();
                        break;

                    case "my":
                    case "responsibleID":
                        ResponsibleId = filterParam.GetProperty("value").GetGuid();

                        break;
                    case "stageTypeOpen":
                    case "stageTypeClosedAndWon":
                    case "stageTypeClosedAndLost":
                        StageType = filterParam.GetProperty("value").GetString();
                        break;
                    case "opportunityStagesID":
                        OpportunityStageId = filterParam.GetProperty("value").GetInt32();
                        break;
                    case "lastMonth":
                    case "yesterday":
                    case "today":
                    case "thisMonth":
                        var valueString = filterParam.GetProperty("value").GetString();
                        var fromToArray = JsonDocument.Parse(valueString)
                                                      .RootElement
                                                      .EnumerateArray()
                                                      .Select(x => x.GetString())
                                                      .ToList();

                        if (fromToArray.Count != 2) continue;

                        FromDate = Global.ApiDateTimeParse(fromToArray[0]);
                        ToDate = Global.ApiDateTimeParse(fromToArray[1]);
                        break;

                    case "fromToDate":
                        FromDate = Global.ApiDateTimeParse(filterParam.GetProperty("from").GetString());
                        ToDate = Global.ApiDateTimeParse(filterParam.GetProperty("to").GetString());
                        break;

                    case "participantID":
                        ContactId = filterParam.GetProperty("id").GetInt32();
                        ContactAlsoIsParticipant = true;
                        break;

                    case "contactID":
                        ContactId = filterParam.GetProperty("id").GetInt32();
                        ContactAlsoIsParticipant = false;
                        break;

                    case "tags":
                        Tags = filterParam.GetProperty("value").EnumerateArray().Select(x => x.GetString()).ToList();
                        break;
                }
            }

        }

        public override ICollection GetItemsByFilter(DaoFactory daofactory)
        {
            DealSortedByType sortBy;
            EnumExtension.TryParse(SortBy, true, out sortBy);

            DealMilestoneStatus? stageType = null;
            DealMilestoneStatus stage;
            if (EnumExtension.TryParse(StageType, true, out stage))
            {
                stageType = stage;
            }

            return daofactory.GetDealDao().GetDeals(
                FilterValue,
                ResponsibleId,
                OpportunityStageId,
                Tags,
                ContactId,
                stageType,
                ContactAlsoIsParticipant,
                FromDate,
                ToDate,
                0, 0,
                new OrderBy(sortBy, IsAsc));
        }
    }

    public class ContactFilterObject : FilterObject
    {
        public List<string> Tags { get; set; }
        public string ContactListView { get; set; }
        public int ContactStage { get; set; }
        public int ContactType { get; set; }
        public Guid? ResponsibleId { get; set; }
        public bool? IsShared { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public ContactFilterObject()
        {
            FromDate = DateTime.MinValue;
            ToDate = DateTime.MinValue;
            ResponsibleId = null;
            ContactStage = -1;
            ContactType = -1;
            SortBy = "created";
            SortOrder = "descending";
        }

        public ContactFilterObject(string base64String)
        {
            ContactStage = -1;
            ContactType = -1;

            if (string.IsNullOrEmpty(base64String)) return;

            var json = Encoding.UTF8.GetString(Convert.FromBase64String(base64String));

            var jsonArray = json.Split(';');

            foreach (var filterItem in jsonArray)
            {
                var filterObj = JsonDocument.Parse(filterItem).RootElement;

                var paramString = filterObj.GetProperty("params").GetString();

                if (string.IsNullOrEmpty(paramString)) continue;

                var filterParam = Global.JObjectParseWithDateAsString(Encoding.UTF8.GetString(Convert.FromBase64String(paramString))).RootElement;

                switch (filterObj.GetProperty("id").GetString())
                {
                    case "sorter":
                        SortBy = filterParam.GetProperty("id").GetString();
                        SortOrder = filterParam.GetProperty("sortOrder").GetString();
                        break;

                    case "text":
                        FilterValue = filterParam.GetProperty("value").GetString();
                        break;

                    case "my":
                    case "responsibleID":
                    case "noresponsible":
                        ResponsibleId = filterParam.GetProperty("value").GetGuid();
                        break;

                    case "tags":
                        Tags = filterParam.GetProperty("value").EnumerateArray().Select(x => x.GetString()).ToList();
                        break;

                    case "withopportunity":
                    case "person":
                    case "company":
                        ContactListView = filterParam.GetProperty("value").GetString();
                        break;

                    case "contactType":
                        ContactType = filterParam.GetProperty("value").GetInt32();
                        break;

                    case "contactStage":
                        ContactStage = filterParam.GetProperty("value").GetInt32();
                        break;

                    case "lastMonth":
                    case "yesterday":
                    case "today":
                    case "thisMonth":
                        var valueString = filterParam.GetProperty("value").GetString();
                        var fromToArray = JsonDocument.Parse(valueString)
                                                      .RootElement
                                                      .EnumerateArray()
                                                      .Select(x => x.GetString())
                                                      .ToList();

                        if (fromToArray.Count != 2) continue;

                        FromDate = Global.ApiDateTimeParse(fromToArray[0]);
                        ToDate = Global.ApiDateTimeParse(fromToArray[1]);
                        break;

                    case "fromToDate":
                        FromDate = Global.ApiDateTimeParse(filterParam.GetProperty("from").GetString());
                        ToDate = Global.ApiDateTimeParse(filterParam.GetProperty("to").GetString());
                        break;

                    case "restricted":
                    case "shared":
                        IsShared = filterParam.GetProperty("value").GetBoolean();
                        break;
                }
            }
        }

        public override ICollection GetItemsByFilter(DaoFactory daofactory)
        {
            ContactSortedByType sortBy;
            if (!EnumExtension.TryParse(SortBy, true, out sortBy))
            {
                sortBy = ContactSortedByType.Created;
            }

            ContactListViewType contactListViewType;
            EnumExtension.TryParse(ContactListView, true, out contactListViewType);

            return daofactory.GetContactDao().GetContacts(
                FilterValue,
                Tags,
                ContactStage,
                ContactType,
                contactListViewType,
                FromDate,
                ToDate,
                0,
                0,
                new OrderBy(sortBy, IsAsc),
                ResponsibleId,
                IsShared);
        }
    };

    public class InvoiceItemFilterObject : FilterObject
    {
        public bool? InventoryStock { get; set; }

        public InvoiceItemFilterObject()
        {
            InventoryStock = null;
            SortBy = "name";
            SortOrder = "ascending";
        }

        public InvoiceItemFilterObject(string base64String)
        {
            if (string.IsNullOrEmpty(base64String)) return;

            var json = Encoding.UTF8.GetString(Convert.FromBase64String(base64String));

            var jsonArray = json.Split(';');

            foreach (var filterItem in jsonArray)
            {
                var filterObj = JsonDocument.Parse(filterItem).RootElement;

                var paramString = filterObj.GetProperty("params").GetString();

                if (string.IsNullOrEmpty(paramString)) continue;

                var filterParam = JsonDocument.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(paramString))).RootElement;

                switch (filterObj.GetProperty("id").GetString())
                {
                    case "sorter":
                        SortBy = filterParam.GetProperty("id").GetString();
                        SortOrder = filterParam.GetProperty("sortOrder").GetString();
                        break;

                    case "text":
                        FilterValue = filterParam.GetProperty("value").GetString();
                        break;

                    case "withInventoryStock":
                    case "withoutInventoryStock":
                        InventoryStock = filterParam.GetProperty("value").GetBoolean();
                        break;
                }
            }

        }

        public override ICollection GetItemsByFilter(DaoFactory daofactory)
        {
            InvoiceItemSortedByType sortBy;
            EnumExtension.TryParse(SortBy, true, out sortBy);

            return daofactory.GetInvoiceItemDao().GetInvoiceItems(
                FilterValue,
                0,
                InventoryStock,
                0, 0,
                new OrderBy(sortBy, IsAsc));
        }
    }
}