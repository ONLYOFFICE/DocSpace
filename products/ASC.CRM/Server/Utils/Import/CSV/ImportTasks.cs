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

using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.CRM.Resources;
using ASC.Web.Core.Users;

using LumenWorks.Framework.IO.Csv;

#endregion

namespace ASC.Web.CRM.Classes
{
    public partial class ImportDataOperation
    {
        private ImportFromCSV _importFromCSV;
        private TenantUtil _tenantUtil;
        private DisplayUserSettingsHelper _displayUserSettingsHelper;

        public ImportDataOperation(TenantUtil tenantUtil,
                                   ImportFromCSV importFromCSV,
                                   DisplayUserSettingsHelper displayUserSettingsHelper)
        {
            _tenantUtil = tenantUtil;
            _importFromCSV = importFromCSV;
            _displayUserSettingsHelper = displayUserSettingsHelper;
        }

        private void ImportTaskData(DaoFactory _daoFactory)
        {
            using (var CSVFileStream = _dataStore.GetReadStreamAsync("temp", _csvFileURI).Result)
            using (CsvReader csv = _importFromCSV.CreateCsvReaderInstance(CSVFileStream, _importSettings))
            {
                int currentIndex = 0;

                var contactDao = _daoFactory.GetContactDao();
                var listItemDao = _daoFactory.GetListItemDao();
                var taskDao = _daoFactory.GetTaskDao();

                var findedTasks = new List<Task>();
                var taskCategories = listItemDao.GetItems(ListType.TaskCategory);

                var allUsers = _userManager.GetUsers(EmployeeStatus.All).ToList();

                while (csv.ReadNextRecord())
                {
                    _columns = csv.GetCurrentRowFields(false);

                    var obj = new Task();

                    obj.ID = currentIndex;

                    obj.Title = GetPropertyValue("title");

                    if (String.IsNullOrEmpty(obj.Title)) continue;

                    obj.Description = GetPropertyValue("description");

                    DateTime deadline;

                    if (DateTime.TryParse(GetPropertyValue("due_date"), out deadline))
                        obj.DeadLine = deadline;
                    else
                        obj.DeadLine = _tenantUtil.DateTimeNow();

                    var csvResponsibleValue = GetPropertyValue("responsible");
                    var responsible = allUsers.Where(n => n.DisplayUserName(_displayUserSettingsHelper).Equals(csvResponsibleValue)).FirstOrDefault();

                    if (responsible != null)
                        obj.ResponsibleID = responsible.ID;
                    else
                        obj.ResponsibleID = Constants.LostUser.ID;

                    var categoryTitle = GetPropertyValue("taskCategory");

                    if (!String.IsNullOrEmpty(categoryTitle))
                    {
                        var findedCategory = taskCategories.Find(item => string.Equals(item.Title, categoryTitle));

                        if (findedCategory == null)
                        {
                            obj.CategoryID = taskCategories[0].ID;
                        }
                        else
                            obj.CategoryID = findedCategory.ID;
                    }
                    else
                        obj.CategoryID = taskCategories[0].ID;

                    var contactName = GetPropertyValue("contact");

                    if (!String.IsNullOrEmpty(contactName))
                    {
                        var contacts = contactDao.GetContactsByName(contactName, true);

                        if (contacts.Count > 0)
                        {
                            obj.ContactID = contacts[0].ID;
                        }
                        else
                        {
                            contacts = contactDao.GetContactsByName(contactName, false);
                            if (contacts.Count > 0)
                            {
                                obj.ContactID = contacts[0].ID;
                            }
                        }
                    }

                    obj.IsClosed = false;

                    var taskStatus = GetPropertyValue("status");

                    if (!string.IsNullOrEmpty(taskStatus))
                    {
                        if (string.Equals(taskStatus, CRMTaskResource.TaskStatus_Closed, StringComparison.OrdinalIgnoreCase))
                            obj.IsClosed = true;

                    }

                    var alertValue = GetPropertyValue("alertValue");

                    if (Int32.TryParse(alertValue, out var alertIntVal))
                        obj.AlertValue = alertIntVal;
                    else
                        obj.AlertValue = 0;


                    findedTasks.Add(obj);

                    if ((currentIndex + 1) > _importFromCSV.MaxRoxCount) break;

                    currentIndex++;

                }

                Percentage = 50;
                PublishChanges();

                taskDao.SaveTaskList(findedTasks);

                Percentage += 12.5;
                PublishChanges();

                Complete();

            }

        }

    }
}