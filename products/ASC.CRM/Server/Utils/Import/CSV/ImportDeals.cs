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
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

using ASC.Core.Users;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.CRM.Resources;

using LumenWorks.Framework.IO.Csv;

#endregion

namespace ASC.Web.CRM.Classes
{
    public partial class ImportDataOperation
    {
        private void ImportOpportunityData(DaoFactory _daoFactory)
        {
            var allUsers = _userManager.GetUsers(EmployeeStatus.All).ToList();

            using (var CSVFileStream = _dataStore.GetReadStreamAsync("temp", _csvFileURI).Result)
            using (CsvReader csv = _importFromCSV.CreateCsvReaderInstance(CSVFileStream, _importSettings))
            {
                int currentIndex = 0;

                var customFieldDao = _daoFactory.GetCustomFieldDao();
                var contactDao = _daoFactory.GetContactDao();
                var tagDao = _daoFactory.GetTagDao();
                var dealDao = _daoFactory.GetDealDao();
                var dealMilestoneDao = _daoFactory.GetDealMilestoneDao();

                var findedTags = new Dictionary<int, List<String>>();
                var findedCustomField = new List<CustomField>();
                var findedDeals = new List<Deal>();
                var findedDealMembers = new Dictionary<int, List<int>>();

                var dealMilestones = dealMilestoneDao.GetAll();

                while (csv.ReadNextRecord())
                {
                    _columns = csv.GetCurrentRowFields(false);

                    var obj = new Deal();

                    obj.ID = currentIndex;

                    obj.Title = GetPropertyValue("title");

                    if (String.IsNullOrEmpty(obj.Title)) continue;

                    obj.Description = GetPropertyValue("description");

                    var csvResponsibleValue = GetPropertyValue("responsible");
                    var responsible = allUsers.Where(n => n.DisplayUserName(_displayUserSettingsHelper).Equals(csvResponsibleValue)).FirstOrDefault();

                    if (responsible != null)
                        obj.ResponsibleID = responsible.ID;
                    else
                        obj.ResponsibleID = Constants.LostUser.ID;

                    DateTime actualCloseDate;

                    DateTime expectedCloseDate;

                    if (DateTime.TryParse(GetPropertyValue("actual_close_date"), out actualCloseDate))
                        obj.ActualCloseDate = actualCloseDate;

                    if (DateTime.TryParse(GetPropertyValue("expected_close_date"), out expectedCloseDate))
                        obj.ExpectedCloseDate = expectedCloseDate;

                    var currency = _currencyProvider.Get(GetPropertyValue("bid_currency"));

                    var crmSettings = _settingsManager.Load<CrmSettings>();
                    var defaultCurrency = _currencyProvider.Get(crmSettings.DefaultCurrency);

                    if (currency != null)
                        obj.BidCurrency = currency.Abbreviation;
                    else
                        obj.BidCurrency = defaultCurrency.Abbreviation;

                    decimal bidValue;

                    var bidValueStr = GetPropertyValue("bid_amount");

                    if (Decimal.TryParse(bidValueStr, out bidValue))
                        obj.BidValue = bidValue;
                    else
                        obj.BidValue = 0;


                    var bidTypeStr = GetPropertyValue("bid_type");

                    BidType bidType = BidType.FixedBid;

                    if (!string.IsNullOrEmpty(bidTypeStr))
                    {
                        if (string.Equals(CRMDealResource.BidType_FixedBid, bidTypeStr, StringComparison.OrdinalIgnoreCase))
                            bidType = BidType.FixedBid;
                        else if (string.Equals(CRMDealResource.BidType_PerDay, bidTypeStr, StringComparison.OrdinalIgnoreCase))
                            bidType = BidType.PerDay;
                        else if (string.Equals(CRMDealResource.BidType_PerHour, bidTypeStr, StringComparison.OrdinalIgnoreCase))
                            bidType = BidType.PerHour;
                        else if (string.Equals(CRMDealResource.BidType_PerMonth, bidTypeStr, StringComparison.OrdinalIgnoreCase))
                            bidType = BidType.PerMonth;
                        else if (string.Equals(CRMDealResource.BidType_PerWeek, bidTypeStr, StringComparison.OrdinalIgnoreCase))
                            bidType = BidType.PerWeek;
                        else if (string.Equals(CRMDealResource.BidType_PerYear, bidTypeStr, StringComparison.OrdinalIgnoreCase))
                            bidType = BidType.PerYear;
                    }

                    obj.BidType = bidType;

                    if (obj.BidType != BidType.FixedBid)
                    {
                        int perPeriodValue;

                        if (int.TryParse(GetPropertyValue("per_period_value"), out perPeriodValue))
                            obj.PerPeriodValue = perPeriodValue;
                    }

                    int probabilityOfWinning;

                    if (int.TryParse(GetPropertyValue("probability_of_winning"), out probabilityOfWinning))
                        obj.DealMilestoneProbability = probabilityOfWinning;

                    var dealMilestoneTitle = GetPropertyValue("deal_milestone");

                    var tag = GetPropertyValue("tag");


                    if (!String.IsNullOrEmpty(tag))
                    {
                        var tagList = tag.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        tagList.AddRange(_importSettings.Tags);
                        tagList = tagList.Distinct().ToList();
                        findedTags.Add(obj.ID, tagList);
                    }
                    else if (_importSettings.Tags.Count != 0)
                    {
                        findedTags.Add(obj.ID, _importSettings.Tags);
                    }


                    if (String.IsNullOrEmpty(dealMilestoneTitle))
                        obj.DealMilestoneID = dealMilestones[0].ID;
                    else
                    {
                        var dealMilestone = dealMilestones.Find(item => string.Equals(item.Title, dealMilestoneTitle, StringComparison.OrdinalIgnoreCase));

                        if (dealMilestone == null)
                            obj.DealMilestoneID = dealMilestones[0].ID;
                        else
                            obj.DealMilestoneID = dealMilestone.ID;
                    }

                    var contactName = GetPropertyValue("client");

                    var localMembersDeal = new List<int>();

                    if (!String.IsNullOrEmpty(contactName))
                    {
                        var contacts = contactDao.GetContactsByName(contactName, true);

                        if (contacts.Count > 0)
                        {
                            obj.ContactID = contacts[0].ID;
                            localMembersDeal.Add(obj.ContactID);
                        }
                        else
                        {
                            contacts = contactDao.GetContactsByName(contactName, false);
                            if (contacts.Count > 0)
                            {
                                obj.ContactID = contacts[0].ID;
                                localMembersDeal.Add(obj.ContactID);
                            }
                        }
                    }

                    var members = GetPropertyValue("member");

                    if (!String.IsNullOrEmpty(members))
                    {
                        var membersList = members.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (var item in membersList)
                        {
                            var findedMember = contactDao.GetContactsByName(item, true);

                            if (findedMember.Count > 0)
                            {
                                localMembersDeal.Add(findedMember[0].ID);
                            }
                            else
                            {
                                findedMember = _daoFactory.GetContactDao().GetContactsByName(item, false);
                                if (findedMember.Count > 0)
                                {
                                    localMembersDeal.Add(findedMember[0].ID);
                                }
                            }
                        }
                    }

                    if (localMembersDeal.Count > 0)
                        findedDealMembers.Add(obj.ID, localMembersDeal);


                    foreach (JsonProperty jToken in _importSettings.ColumnMapping.EnumerateObject())
                    {
                        var propertyValue = GetPropertyValue(jToken.Name);

                        if (String.IsNullOrEmpty(propertyValue)) continue;

                        if (!jToken.Name.StartsWith("customField_")) continue;

                        var fieldID = Convert.ToInt32(jToken.Name.Split(new[] { '_' })[1]);
                        var field = customFieldDao.GetFieldDescription(fieldID);

                        if (field != null)
                        {
                            findedCustomField.Add(new CustomField
                            {
                                EntityID = obj.ID,
                                EntityType = EntityType.Opportunity,
                                ID = fieldID,
                                Value = field.Type == CustomFieldType.CheckBox ? (propertyValue == "on" || propertyValue == "true" ? "true" : "false") : propertyValue
                            });
                        }
                    }

                    Percentage += 1.0 * 100 / (_importFromCSV.MaxRoxCount * 2);
                    PublishChanges();

                    findedDeals.Add(obj);

                    if (currentIndex + 1 > _importFromCSV.MaxRoxCount) break;

                    currentIndex++;

                }

                Percentage = 50;
                PublishChanges();


                var newDealIDs = dealDao.SaveDealList(findedDeals);
                findedDeals.ForEach(d => d.ID = newDealIDs[d.ID]);

                Percentage += 12.5;
                PublishChanges();

                findedCustomField.ForEach(item => item.EntityID = newDealIDs[item.EntityID]);

                customFieldDao.SaveList(findedCustomField);

                Percentage += 12.5;
                PublishChanges();

                foreach (var findedDealMemberKey in findedDealMembers.Keys)
                {
                    dealDao.SetMembers(newDealIDs[findedDealMemberKey], findedDealMembers[findedDealMemberKey].ToArray());
                }

                Percentage += 12.5;
                PublishChanges();

                foreach (var findedTagKey in findedTags.Keys)
                {
                    tagDao.SetTagToEntity(EntityType.Opportunity, newDealIDs[findedTagKey], findedTags[findedTagKey].ToArray());
                }

                if (_importSettings.IsPrivate)
                    findedDeals.ForEach(dealItem => _crmSecurity.SetAccessTo(dealItem, _importSettings.AccessList));

                Percentage += 12.5;
                PublishChanges();
            }

            Complete();
        }

    }
}