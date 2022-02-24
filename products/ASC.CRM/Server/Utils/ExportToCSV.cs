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
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Common.Security.Authentication;
using ASC.Common.Threading;
using ASC.Common.Threading.Progress;
using ASC.Core;
using ASC.Core.Users;
using ASC.CRM.Classes;
using ASC.CRM.Core;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.CRM.Resources;
using ASC.Data.Storage;
using ASC.Files.Core;
using ASC.Web.Core.Files;
using ASC.Web.Core.Users;
using ASC.Web.CRM.Services.NotifyService;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Utils;
using ASC.Web.Studio.Utility;

using ICSharpCode.SharpZipLib.Zip;

using Microsoft.Extensions.Options;

namespace ASC.Web.CRM.Classes
{
    [Transient]
    public class ExportDataOperation : DistributedTaskProgress, IProgressItem
    {

        private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;
        private readonly FilesLinkUtility _filesLinkUtility;
        private readonly FileMarker _fileMarker;
        private readonly IDaoFactory _fileDaoFactory;
        private readonly GlobalFolder _globalFolder;
        private readonly FileUploader _fileUploader;
        private readonly TenantManager _tenantManager;
        private readonly CommonLinkUtility _commonLinkUtility;
        private readonly SecurityContext _securityContext;
        private readonly DaoFactory _daoFactory;
        private readonly UserManager _userManager;
        private readonly FileUtility _fileUtility;
        private readonly int _tenantId;
        private readonly IAccount _author;
        private readonly IDataStore _dataStore;
        private readonly NotifyClient _notifyClient;
        private readonly TempStream _tempStream;
        private FilterObject _filterObject;
        private readonly ILog _log;
        private int _totalCount;

        public ExportDataOperation(UserManager userManager,
                                   FileUtility fileUtility,
                                   SecurityContext securityContext,
                                   IOptionsMonitor<ILog> logger,
                                   TenantManager tenantManager,
                                   Global global,
                                   CommonLinkUtility commonLinkUtility,
                                   NotifyClient notifyClient,
                                   DaoFactory daoFactory,
                                   FileUploader fileUploader,
                                   GlobalFolder globalFolder,
                                   FileMarker fileMarker,
                                   IDaoFactory fileDaoFactory,
                                   FilesLinkUtility filesLinkUtility,
                                   DisplayUserSettingsHelper displayUserSettingsHelper,
                                   TempStream tempStream)
        {
            _daoFactory = daoFactory;
            _userManager = userManager;
            _fileUtility = fileUtility;
            _fileUploader = fileUploader;
            _tenantManager = tenantManager;
            _tenantId = tenantManager.GetCurrentTenant().TenantId;
            _tempStream = tempStream;

            _author = securityContext.CurrentAccount;
            _dataStore = global.GetStore();
            _notifyClient = notifyClient;

            _log = logger.Get("ASC.CRM");

            Error = null;
            Percentage = 0;
            IsCompleted = false;
            FileUrl = null;

            _securityContext = securityContext;

            _commonLinkUtility = commonLinkUtility;
            _globalFolder = globalFolder;
            _fileMarker = fileMarker;
            _fileDaoFactory = fileDaoFactory;
            _filesLinkUtility = filesLinkUtility;
            _displayUserSettingsHelper = displayUserSettingsHelper;
        }

        public void Configure(object id, FilterObject filterObject, string fileName)
        {
            FileName = fileName ?? CRMSettingResource.Export + (filterObject == null ? ".zip" : ".csv");
            _filterObject = filterObject;
            Id = id.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            var exportDataOperation = obj as ExportDataOperation;

            if (exportDataOperation == null) return false;

            return Id == exportDataOperation.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public object Error { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }

        private String WrapDoubleQuote(String value)
        {
            return "\"" + value.Trim().Replace("\"", "\"\"") + "\"";
        }

        private String DataTableToCsv(DataTable dataTable)
        {
            var result = new StringBuilder();

            var columnsCount = dataTable.Columns.Count;

            for (var index = 0; index < columnsCount; index++)
            {
                if (index != columnsCount - 1)
                    result.Append(dataTable.Columns[index].Caption + ",");
                else
                    result.Append(dataTable.Columns[index].Caption);
            }

            result.Append(Environment.NewLine);

            foreach (DataRow row in dataTable.Rows)
            {
                for (var i = 0; i < columnsCount; i++)
                {
                    var itemValue = WrapDoubleQuote(row[i].ToString());

                    if (i != columnsCount - 1)
                        result.Append(itemValue + ",");
                    else
                        result.Append(itemValue);
                }

                result.Append(Environment.NewLine);
            }

            return result.ToString();
        }

        protected override void DoJob()
        {
            try
            {
                _tenantManager.SetCurrentTenant(_tenantId);
                _securityContext.AuthenticateMeWithoutCookie(_author);

                var userCulture = _userManager.GetUsers(_securityContext.CurrentAccount.ID).GetCulture();

                System.Threading.Thread.CurrentThread.CurrentCulture = userCulture;
                System.Threading.Thread.CurrentThread.CurrentUICulture = userCulture;

                _log.Debug("Start Export Data");

                if (_filterObject == null)
                    ExportAllData(_daoFactory);
                else
                    ExportPartData(_daoFactory);

                Complete(100, DistributedTaskStatus.Completed, null);

                _log.Debug("Export is completed");
            }
            catch (OperationCanceledException)
            {
                Complete(0, DistributedTaskStatus.Completed, null);

                _log.Debug("Export is cancel");
            }
            catch (Exception ex)
            {
                Complete(0, DistributedTaskStatus.Failted, ex.Message);

                _log.Error(ex);
            }
        }

        private void Complete(double percentage, DistributedTaskStatus status, object error)
        {
            IsCompleted = true;
            Percentage = percentage;
            Status = status;
            Error = error;
        }

        private void ExportAllData(DaoFactory daoFactory)
        {
            using (var stream = _tempStream.Create())
            {
                var contactDao = daoFactory.GetContactDao();
                var contactInfoDao = daoFactory.GetContactInfoDao();
                var dealDao = daoFactory.GetDealDao();
                var casesDao = daoFactory.GetCasesDao();
                var taskDao = daoFactory.GetTaskDao();
                var historyDao = daoFactory.GetRelationshipEventDao();
                var invoiceItemDao = daoFactory.GetInvoiceItemDao();

                _totalCount += contactDao.GetAllContactsCount();
                _totalCount += dealDao.GetDealsCount();
                _totalCount += casesDao.GetCasesCount();
                _totalCount += taskDao.GetAllTasksCount();
                _totalCount += historyDao.GetAllItemsCount();
                _totalCount += invoiceItemDao.GetInvoiceItemsCount();

                using (var zipStream = new ZipOutputStream(stream))
                {
                    zipStream.PutNextEntry(new ZipEntry(CRMContactResource.Contacts + ".csv"));

                    var contactData = contactDao.GetAllContacts();
                    var contactInfos = new StringDictionary();

                    contactInfoDao.GetAll()
                                  .ForEach(item =>
                                               {
                                                   var contactInfoKey = String.Format("{0}_{1}_{2}", item.ContactID, (int)item.InfoType, item.Category);
                                                   if (contactInfos.ContainsKey(contactInfoKey))
                                                   {
                                                       contactInfos[contactInfoKey] += "," + item.Data;
                                                   }
                                                   else
                                                   {
                                                       contactInfos.Add(contactInfoKey, item.Data);
                                                   }
                                               });

                    using (var zipEntryData = new MemoryStream(Encoding.UTF8.GetBytes(ExportContactsToCsv(contactData, contactInfos, daoFactory))))
                    {
                        zipEntryData.CopyTo(zipStream);
                    }

                    zipStream.PutNextEntry(new ZipEntry(CRMCommonResource.DealModuleName + ".csv"));

                    var dealData = dealDao.GetAllDeals();

                    using (var zipEntryData = new MemoryStream(Encoding.UTF8.GetBytes(ExportDealsToCsv(dealData, daoFactory))))
                    {
                        zipEntryData.CopyTo(zipStream);
                    }

                    zipStream.PutNextEntry(new ZipEntry(CRMCommonResource.CasesModuleName + ".csv"));

                    var casesData = casesDao.GetAllCases();

                    using (var zipEntryData = new MemoryStream(Encoding.UTF8.GetBytes(ExportCasesToCsv(casesData, daoFactory))))
                    {
                        zipEntryData.CopyTo(zipStream);
                    }

                    zipStream.PutNextEntry(new ZipEntry(CRMCommonResource.TaskModuleName + ".csv"));

                    var taskData = taskDao.GetAllTasks();

                    using (var zipEntryData = new MemoryStream(Encoding.UTF8.GetBytes(ExportTasksToCsv(taskData, daoFactory))))
                    {
                        zipEntryData.CopyTo(zipStream);
                    }

                    zipStream.PutNextEntry(new ZipEntry(CRMCommonResource.History + ".csv"));

                    var historyData = historyDao.GetAllItems();

                    using (var zipEntryData = new MemoryStream(Encoding.UTF8.GetBytes(ExportHistoryToCsv(historyData, daoFactory))))
                    {
                        zipEntryData.CopyTo(zipStream);
                    }

                    zipStream.PutNextEntry(new ZipEntry(CRMCommonResource.ProductsAndServices + ".csv"));

                    var invoiceItemData = invoiceItemDao.GetAll();

                    using (var zipEntryData = new MemoryStream(Encoding.UTF8.GetBytes(ExportInvoiceItemsToCsv(invoiceItemData, daoFactory))))
                    {
                        zipEntryData.CopyTo(zipStream);
                    }

                    zipStream.Flush();
                    zipStream.Close();

                    stream.Position = 0;
                }

                FileUrl = _commonLinkUtility.GetFullAbsolutePath(_dataStore.SavePrivateAsync(String.Empty, FileName, stream, DateTime.Now.AddDays(1)).Result);

                _notifyClient.SendAboutExportCompleted(_author.ID, FileName, FileUrl);
            }
        }

        private void ExportPartData(DaoFactory daoFactory)
        {
            var items = _filterObject.GetItemsByFilter(daoFactory);

            string fileContent;

            _totalCount = items.Count;

            if (_totalCount == 0)
                throw new ArgumentException(CRMErrorsResource.ExportToCSVDataEmpty);

            if (items is List<Contact>)
            {
                var contactInfoDao = daoFactory.GetContactInfoDao();

                var contacts = (List<Contact>)items;

                var contactInfos = new StringDictionary();

                contactInfoDao.GetAll(contacts.Select(item => item.ID).ToArray())
                              .ForEach(item =>
                                  {
                                      var contactInfoKey = String.Format("{0}_{1}_{2}", item.ContactID,
                                                                         (int)item.InfoType,
                                                                         item.Category);

                                      if (contactInfos.ContainsKey(contactInfoKey))
                                          contactInfos[contactInfoKey] += "," + item.Data;
                                      else
                                          contactInfos.Add(contactInfoKey, item.Data);
                                  });

                fileContent = ExportContactsToCsv(contacts, contactInfos, daoFactory);
            }
            else if (items is List<Deal>)
            {
                fileContent = ExportDealsToCsv((List<Deal>)items, daoFactory);
            }
            else if (items is List<ASC.CRM.Core.Entities.Cases>)
            {
                fileContent = ExportCasesToCsv((List<ASC.CRM.Core.Entities.Cases>)items, daoFactory);
            }
            else if (items is List<RelationshipEvent>)
            {
                fileContent = ExportHistoryToCsv((List<RelationshipEvent>)items, daoFactory);
            }
            else if (items is List<Task>)
            {
                fileContent = ExportTasksToCsv((List<Task>)items, daoFactory);
            }
            else if (items is List<InvoiceItem>)
            {
                fileContent = ExportInvoiceItemsToCsv((List<InvoiceItem>)items, daoFactory);
            }
            else
                throw new ArgumentException();

            FileUrl = SaveCsvFileInMyDocument(FileName, fileContent);
        }

        private String ExportContactsToCsv(IReadOnlyCollection<Contact> contacts, StringDictionary contactInfos, DaoFactory daoFactory)
        {
            var key = Id;
            var listItemDao = daoFactory.GetListItemDao();
            var tagDao = daoFactory.GetTagDao();
            var customFieldDao = daoFactory.GetCustomFieldDao();
            var contactDao = daoFactory.GetContactDao();

            var dataTable = new DataTable();

            dataTable.Columns.AddRange(new[]
                {
                    new DataColumn
                        {
                            Caption = CRMCommonResource.TypeCompanyOrPerson,
                            ColumnName = "company/person"
                        },
                    new DataColumn
                        {
                            Caption = CRMContactResource.FirstName,
                            ColumnName = "firstname"
                        },
                    new DataColumn
                        {
                            Caption = CRMContactResource.LastName,
                            ColumnName = "lastname"
                        },
                    new DataColumn
                        {
                            Caption = CRMContactResource.CompanyName,
                            ColumnName = "companyname"
                        },
                    new DataColumn
                        {
                            Caption = CRMContactResource.JobTitle,
                            ColumnName = "jobtitle"
                        },
                    new DataColumn
                        {
                            Caption = CRMContactResource.About,
                            ColumnName = "about"
                        },
                    new DataColumn
                        {
                            Caption = CRMContactResource.ContactStage,
                            ColumnName = "contact_stage"
                        },
                    new DataColumn
                        {
                            Caption = CRMContactResource.ContactType,
                            ColumnName = "contact_type"
                        },
                    new DataColumn
                        {
                            Caption = CRMContactResource.ContactTagList,
                            ColumnName = "contact_tag_list"
                        }
                });

            foreach (ContactInfoType infoTypeEnum in Enum.GetValues(typeof(ContactInfoType)))
                foreach (Enum categoryEnum in Enum.GetValues(ContactInfo.GetCategory(infoTypeEnum)))
                {
                    var localTitle = $"{infoTypeEnum.ToLocalizedString()} ({categoryEnum.ToLocalizedString().ToLower()})";

                    if (infoTypeEnum == ContactInfoType.Address)
                        dataTable.Columns.AddRange((from AddressPart addressPartEnum in Enum.GetValues(typeof(AddressPart))
                                                    select new DataColumn
                                                    {
                                                        Caption = String.Format(localTitle + " {0}", addressPartEnum.ToLocalizedString().ToLower()),
                                                        ColumnName = String.Format("contactInfo_{0}_{1}_{2}", (int)infoTypeEnum, categoryEnum, (int)addressPartEnum)
                                                    }).ToArray());

                    else
                        dataTable.Columns.Add(new DataColumn
                        {
                            Caption = localTitle,
                            ColumnName = String.Format("contactInfo_{0}_{1}", (int)infoTypeEnum, categoryEnum)
                        });
                }

            var fieldsDescription = customFieldDao.GetFieldsDescription(EntityType.Company);

            customFieldDao.GetFieldsDescription(EntityType.Person).ForEach(item =>
                                                                               {
                                                                                   var alreadyContains = fieldsDescription.Any(field => field.ID == item.ID);

                                                                                   if (!alreadyContains)
                                                                                       fieldsDescription.Add(item);
                                                                               });

            fieldsDescription.ForEach(
                item =>
                {
                    if (item.Type == CustomFieldType.Heading) return;

                    dataTable.Columns.Add(
                        new DataColumn
                        {
                            Caption = item.Label,
                            ColumnName = "customField_" + item.ID
                        }
                        );
                });

            var customFieldEntity = (contacts.Where(x => x is Company).Any() ? customFieldDao.GetEnityFields(EntityType.Company, contacts.Where(x => x is Company).Select(x => x.ID).ToArray()) : new List<CustomField>())
                        .Union(contacts.Where(x => x is Person).Any() ? customFieldDao.GetEnityFields(EntityType.Person, contacts.Where(x => x is Person).Select(x => x.ID).ToArray()) : new List<CustomField>())
                        .GroupBy(x => x.EntityID)
                        .ToDictionary(x => x.Key, x => x.ToList());


            var tags = tagDao.GetEntitiesTags(EntityType.Contact);

            foreach (var contact in contacts)
            {
                Percentage += 1.0 * 100 / _totalCount;
                PublishChanges();


                var isCompany = contact is Company;

                var compPersType = (isCompany) ? CRMContactResource.Company : CRMContactResource.Person;

                var contactTags = String.Empty;

                if (tags.ContainsKey(contact.ID))
                    contactTags = String.Join(",", tags[contact.ID].OrderBy(x => x));

                String firstName;
                String lastName;

                String companyName;
                String title;

                if (contact is Company)
                {
                    firstName = String.Empty;
                    lastName = String.Empty;
                    title = String.Empty;
                    companyName = ((Company)contact).CompanyName;
                }
                else
                {
                    var people = (Person)contact;

                    firstName = people.FirstName;
                    lastName = people.LastName;
                    title = people.JobTitle;

                    companyName = String.Empty;

                    if (people.CompanyID > 0)
                    {
                        var personCompany = contacts.SingleOrDefault(item => item.ID == people.CompanyID) ??
                                            contactDao.GetByID(people.CompanyID);

                        if (personCompany != null)
                            companyName = personCompany.GetTitle();
                    }
                }

                var contactStatus = String.Empty;

                if (contact.StatusID > 0)
                {

                    var listItem = listItemDao.GetByID(contact.StatusID);

                    if (listItem != null)
                        contactStatus = listItem.Title;
                }

                var contactType = String.Empty;

                if (contact.ContactTypeID > 0)
                {

                    var listItem = listItemDao.GetByID(contact.ContactTypeID);

                    if (listItem != null)
                        contactType = listItem.Title;
                }

                var dataRowItems = new List<String>
                    {
                        compPersType,
                        firstName,
                        lastName,
                        companyName,
                        title,
                        contact.About,
                        contactStatus,
                        contactType,
                        contactTags
                    };

                foreach (ContactInfoType infoTypeEnum in Enum.GetValues(typeof(ContactInfoType)))
                    foreach (Enum categoryEnum in Enum.GetValues(ContactInfo.GetCategory(infoTypeEnum)))
                    {
                        var contactInfoKey = String.Format("{0}_{1}_{2}", contact.ID,
                                                           (int)infoTypeEnum,
                                                           Convert.ToInt32(categoryEnum));

                        var columnValue = "";

                        if (contactInfos.ContainsKey(contactInfoKey))
                            columnValue = contactInfos[contactInfoKey];

                        if (infoTypeEnum == ContactInfoType.Address)
                        {
                            if (!String.IsNullOrEmpty(columnValue))
                            {
                                var addresses = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(String.Concat("[", columnValue, "]"));

                                dataRowItems.AddRange(from AddressPart addressPartEnum in Enum.GetValues(typeof(AddressPart))
                                                      select String.Join(",", addresses.Select(x => x[addressPartEnum.ToString().ToLower()]).ToArray()));
                            }
                            else
                            {
                                dataRowItems.AddRange(new[] { "", "", "", "", "" });
                            }
                        }
                        else
                        {
                            dataRowItems.Add(columnValue);
                        }
                    }

                var dataRow = dataTable.Rows.Add(dataRowItems.ToArray());

                if (customFieldEntity.ContainsKey(contact.ID))
                    customFieldEntity[contact.ID].ForEach(item => dataRow["customField_" + item.ID] = item.Value);
            }

            return DataTableToCsv(dataTable);
        }

        private String ExportDealsToCsv(IEnumerable<Deal> deals, DaoFactory daoFactory)
        {
            var key = Id;
            var tagDao = daoFactory.GetTagDao();
            var customFieldDao = daoFactory.GetCustomFieldDao();
            var dealMilestoneDao = daoFactory.GetDealMilestoneDao();
            var contactDao = daoFactory.GetContactDao();

            var dataTable = new DataTable();

            dataTable.Columns.AddRange(new[]
                {
                    new DataColumn
                        {
                            Caption = CRMDealResource.NameDeal,
                            ColumnName = "title"
                        },
                    new DataColumn
                        {
                            Caption = CRMDealResource.ClientDeal,
                            ColumnName = "client_deal"
                        },
                    new DataColumn
                        {
                            Caption = CRMDealResource.DescriptionDeal,
                            ColumnName = "description"
                        },
                    new DataColumn
                        {
                            Caption = CRMCommonResource.Currency,
                            ColumnName = "currency"
                        },
                    new DataColumn
                        {
                            Caption = CRMDealResource.DealAmount,
                            ColumnName = "amount"
                        },
                    new DataColumn
                        {
                            Caption = CRMDealResource.BidType,
                            ColumnName = "bid_type"
                        },
                    new DataColumn
                        {
                            Caption = CRMDealResource.BidTypePeriod,
                            ColumnName = "bid_type_period"
                        },
                    new DataColumn
                        {
                            Caption = CRMJSResource.ExpectedCloseDate,
                            ColumnName = "expected_close_date"
                        },
                    new DataColumn
                        {
                            Caption = CRMJSResource.ActualCloseDate,
                            ColumnName = "actual_close_date"
                        },
                    new DataColumn
                        {
                            Caption = CRMDealResource.ResponsibleDeal,
                            ColumnName = "responsible_deal"
                        },
                    new DataColumn
                        {
                            Caption = CRMDealResource.CurrentDealMilestone,
                            ColumnName = "current_deal_milestone"
                        },
                    new DataColumn
                        {
                            Caption = CRMDealResource.DealMilestoneType,
                            ColumnName = "deal_milestone_type"
                        },
                    new DataColumn
                        {
                            Caption = (CRMDealResource.ProbabilityOfWinning + " %"),
                            ColumnName = "probability_of_winning"
                        },
                    new DataColumn
                        {
                            Caption = (CRMDealResource.DealTagList),
                            ColumnName = "tag_list"
                        }
                });

            customFieldDao.GetFieldsDescription(EntityType.Opportunity).ForEach(
                item =>
                {
                    if (item.Type == CustomFieldType.Heading) return;

                    dataTable.Columns.Add(new DataColumn
                    {
                        Caption = item.Label,
                        ColumnName = "customField_" + item.ID
                    });
                });

            var customFieldEntity = customFieldDao.GetEnityFields(EntityType.Opportunity, deals.Select(x => x.ID).ToArray())
                                                    .GroupBy(x => x.EntityID)
                                                    .ToDictionary(x => x.Key, x => x.ToList());


            var tags = tagDao.GetEntitiesTags(EntityType.Opportunity);

            foreach (var deal in deals)
            {

                Percentage += 1.0 * 100 / _totalCount;
                PublishChanges();

                var contactTags = String.Empty;

                if (tags.ContainsKey(deal.ID))
                    contactTags = String.Join(",", tags[deal.ID].OrderBy(x => x));

                String bidType;

                switch (deal.BidType)
                {
                    case BidType.FixedBid:
                        bidType = CRMDealResource.BidType_FixedBid;
                        break;
                    case BidType.PerDay:
                        bidType = CRMDealResource.BidType_PerDay;
                        break;
                    case BidType.PerHour:
                        bidType = CRMDealResource.BidType_PerHour;
                        break;
                    case BidType.PerMonth:
                        bidType = CRMDealResource.BidType_PerMonth;
                        break;
                    case BidType.PerWeek:
                        bidType = CRMDealResource.BidType_PerWeek;
                        break;
                    case BidType.PerYear:
                        bidType = CRMDealResource.BidType_PerYear;
                        break;
                    default:
                        throw new ArgumentException();
                }

                var currentDealMilestone = dealMilestoneDao.GetByID(deal.DealMilestoneID);
                var currentDealMilestoneStatus = currentDealMilestone.Status.ToLocalizedString();
                var contactTitle = String.Empty;

                if (deal.ContactID != 0)
                    contactTitle = contactDao.GetByID(deal.ContactID).GetTitle();

                var dataRow = dataTable.Rows.Add(new object[]
                    {
                        deal.Title,
                        contactTitle,
                        deal.Description,
                        deal.BidCurrency,
                        deal.BidValue.ToString(CultureInfo.InvariantCulture),
                        bidType,
                        deal.PerPeriodValue == 0 ? "" : deal.PerPeriodValue.ToString(CultureInfo.InvariantCulture),
                        deal.ExpectedCloseDate.Date == DateTime.MinValue.Date ? "" : deal.ExpectedCloseDate.ToString(),
                        deal.ActualCloseDate.Date == DateTime.MinValue.Date ? "" : deal.ActualCloseDate.ToString(),
                        //deal.ExpectedCloseDate.Date == DateTime.MinValue.Date ? "" : deal.ExpectedCloseDate.ToString(DateTimeExtension.DateFormatPattern),
                        //deal.ActualCloseDate.Date == DateTime.MinValue.Date ? "" : deal.ActualCloseDate.ToString(DateTimeExtension.DateFormatPattern),
                        _userManager.GetUsers(deal.ResponsibleID).DisplayUserName(_displayUserSettingsHelper),
                        currentDealMilestone.Title,
                        currentDealMilestoneStatus,
                        deal.DealMilestoneProbability.ToString(CultureInfo.InvariantCulture),
                        contactTags
                    });

                if (customFieldEntity.ContainsKey(deal.ID))
                    customFieldEntity[deal.ID].ForEach(item => dataRow["customField_" + item.ID] = item.Value);
            }

            return DataTableToCsv(dataTable);
        }

        private String ExportCasesToCsv(IEnumerable<ASC.CRM.Core.Entities.Cases> cases, DaoFactory daoFactory)
        {
            var key = Id;
            var tagDao = daoFactory.GetTagDao();
            var customFieldDao = daoFactory.GetCustomFieldDao();

            var dataTable = new DataTable();

            dataTable.Columns.AddRange(new[]
                {
                    new DataColumn
                        {
                            Caption = CRMCasesResource.CaseTitle,
                            ColumnName = "title"
                        },
                    new DataColumn(CRMCasesResource.CasesTagList)
                        {
                            Caption = CRMCasesResource.CasesTagList,
                            ColumnName = "tag_list"
                        }
                });

            customFieldDao.GetFieldsDescription(EntityType.Case).ForEach(
                item =>
                {
                    if (item.Type == CustomFieldType.Heading) return;

                    dataTable.Columns.Add(new DataColumn
                    {
                        Caption = item.Label,
                        ColumnName = "customField_" + item.ID
                    });
                });

            var customFieldEntity = customFieldDao.GetEnityFields(EntityType.Case, cases.Select(x => x.ID).ToArray())
               .GroupBy(x => x.EntityID)
               .ToDictionary(x => x.Key, x => x.ToList());

            var tags = tagDao.GetEntitiesTags(EntityType.Case);

            foreach (var item in cases)
            {
                Percentage += 1.0 * 100 / _totalCount;
                PublishChanges();

                var contactTags = String.Empty;

                if (tags.ContainsKey(item.ID))
                    contactTags = String.Join(",", tags[item.ID].OrderBy(x => x));

                var dataRow = dataTable.Rows.Add(new object[]
                    {
                        item.Title,
                        contactTags
                    });

                if (customFieldEntity.ContainsKey(item.ID))
                    customFieldEntity[item.ID].ForEach(row => dataRow["customField_" + row.ID] = row.Value);
            }

            return DataTableToCsv(dataTable);
        }

        private String ExportHistoryToCsv(IEnumerable<RelationshipEvent> events, DaoFactory daoFactory)
        {
            var key = Id;
            var listItemDao = daoFactory.GetListItemDao();
            var dealDao = daoFactory.GetDealDao();
            var casesDao = daoFactory.GetCasesDao();
            var contactDao = daoFactory.GetContactDao();

            var dataTable = new DataTable();

            dataTable.Columns.AddRange(new[]
                {
                    new DataColumn
                        {
                            Caption = (CRMContactResource.Content),
                            ColumnName = "content"
                        },
                    new DataColumn
                        {
                            Caption = (CRMCommonResource.Category),
                            ColumnName = "category"
                        },
                    new DataColumn
                        {
                            Caption = (CRMContactResource.ContactTitle),
                            ColumnName = "contact_title"
                        },
                    new DataColumn
                        {
                            Caption = (CRMContactResource.RelativeEntity),
                            ColumnName = "relative_entity"
                        },
                    new DataColumn
                        {
                            Caption = (CRMCommonResource.Author),
                            ColumnName = "author"
                        },
                    new DataColumn
                        {
                            Caption = (CRMCommonResource.CreateDate),
                            ColumnName = "create_date"
                        }
                });

            foreach (var item in events)
            {
                Percentage += 1.0 * 100 / _totalCount;
                PublishChanges();

                var entityTitle = String.Empty;

                if (item.EntityID > 0)
                    switch (item.EntityType)
                    {
                        case EntityType.Case:
                            var casesObj = casesDao.GetByID(item.EntityID);

                            if (casesObj != null)
                                entityTitle = $"{CRMCasesResource.Case}: {casesObj.Title}";
                            break;
                        case EntityType.Opportunity:
                            var dealObj = dealDao.GetByID(item.EntityID);

                            if (dealObj != null)
                                entityTitle = $"{CRMDealResource.Deal}: {dealObj.Title}";
                            break;
                    }

                var contactTitle = String.Empty;

                if (item.ContactID > 0)
                {
                    var contactObj = contactDao.GetByID(item.ContactID);

                    if (contactObj != null)
                        contactTitle = contactObj.GetTitle();
                }

                var categoryTitle = String.Empty;

                if (item.CategoryID > 0)
                {
                    var categoryObj = listItemDao.GetByID(item.CategoryID);

                    if (categoryObj != null)
                        categoryTitle = categoryObj.Title;

                }
                else if (item.CategoryID == (int)HistoryCategorySystem.TaskClosed)
                    categoryTitle = HistoryCategorySystem.TaskClosed.ToLocalizedString();
                else if (item.CategoryID == (int)HistoryCategorySystem.FilesUpload)
                    categoryTitle = HistoryCategorySystem.FilesUpload.ToLocalizedString();
                else if (item.CategoryID == (int)HistoryCategorySystem.MailMessage)
                    categoryTitle = HistoryCategorySystem.MailMessage.ToLocalizedString();

                dataTable.Rows.Add(new object[]
                    {
                        item.Content,
                        categoryTitle,
                        contactTitle,
                        entityTitle,
                        _userManager.GetUsers(item.CreateBy).DisplayUserName(_displayUserSettingsHelper),
               //         item.CreateOn.ToShortString()
                        item.CreateOn
                    });
            }

            return DataTableToCsv(dataTable);
        }

        private String ExportTasksToCsv(IEnumerable<Task> tasks, DaoFactory daoFactory)
        {
            var key = Id;
            var listItemDao = daoFactory.GetListItemDao();
            var dealDao = daoFactory.GetDealDao();
            var casesDao = daoFactory.GetCasesDao();
            var contactDao = daoFactory.GetContactDao();

            var dataTable = new DataTable();

            dataTable.Columns.AddRange(new[]
                {
                    new DataColumn
                        {
                            Caption = (CRMTaskResource.TaskTitle),
                            ColumnName = "title"
                        },
                    new DataColumn
                        {
                            Caption = (CRMTaskResource.Description),
                            ColumnName = "description"
                        },
                    new DataColumn
                        {
                            Caption = (CRMTaskResource.DueDate),
                            ColumnName = "due_date"
                        },
                    new DataColumn
                        {
                            Caption = (CRMTaskResource.Responsible),
                            ColumnName = "responsible"
                        },
                    new DataColumn
                        {
                            Caption = (CRMContactResource.ContactTitle),
                            ColumnName = "contact_title"
                        },
                    new DataColumn
                        {
                            Caption = (CRMTaskResource.TaskStatus),
                            ColumnName = "task_status"
                        },
                    new DataColumn
                        {
                            Caption = (CRMTaskResource.TaskCategory),
                            ColumnName = "task_category"
                        },
                    new DataColumn
                        {
                            Caption = (CRMContactResource.RelativeEntity),
                            ColumnName = "relative_entity"
                        },
                    new DataColumn
                        {
                            Caption = (CRMCommonResource.Alert),
                            ColumnName = "alert_value"
                        }
                });

            foreach (var item in tasks)
            {
                Percentage += 1.0 * 100 / _totalCount;
                PublishChanges();

                var entityTitle = String.Empty;

                if (item.EntityID > 0)
                    switch (item.EntityType)
                    {
                        case EntityType.Case:
                            var caseObj = casesDao.GetByID(item.EntityID);

                            if (caseObj != null)
                                entityTitle = $"{CRMCasesResource.Case}: {caseObj.Title}";
                            break;
                        case EntityType.Opportunity:
                            var dealObj = dealDao.GetByID(item.EntityID);

                            if (dealObj != null)
                                entityTitle = $"{CRMDealResource.Deal}: {dealObj.Title}";
                            break;
                    }

                var contactTitle = String.Empty;

                if (item.ContactID > 0)
                {
                    var contact = contactDao.GetByID(item.ContactID);

                    if (contact != null)
                        contactTitle = contact.GetTitle();
                }

                dataTable.Rows.Add(new object[]
                    {
                        item.Title,
                        item.Description,
                        item.DeadLine == DateTime.MinValue
                            ? ""
//                            : item.DeadLine.ToShortString(),
                            : item.DeadLine.ToString(),
                        _userManager.GetUsers(item.ResponsibleID).DisplayUserName(_displayUserSettingsHelper),
                        contactTitle,
                        item.IsClosed
                            ? CRMTaskResource.TaskStatus_Closed
                            : CRMTaskResource.TaskStatus_Open,
                        listItemDao.GetByID(item.CategoryID).Title,
                        entityTitle,
                        item.AlertValue.ToString(CultureInfo.InvariantCulture)
                    });
            }

            return DataTableToCsv(dataTable);
        }

        private String ExportInvoiceItemsToCsv(IEnumerable<InvoiceItem> invoiceItems, DaoFactory daoFactory)
        {
            var key = Id;
            var taxes = daoFactory.GetInvoiceTaxDao().GetAll();
            var dataTable = new DataTable();

            dataTable.Columns.AddRange(new[]
                {
                    new DataColumn
                        {
                            Caption = (CRMInvoiceResource.InvoiceItemName),
                            ColumnName = "title"
                        },
                    new DataColumn
                        {
                            Caption = (CRMSettingResource.Description),
                            ColumnName = "description"
                        },
                    new DataColumn
                        {
                            Caption = (CRMInvoiceResource.StockKeepingUnit),
                            ColumnName = "sku"
                        },
                    new DataColumn
                        {
                            Caption = (CRMInvoiceResource.InvoiceItemPrice),
                            ColumnName = "price"
                        },
                    new DataColumn
                        {
                            Caption = (CRMInvoiceResource.FormInvoiceItemStockQuantity),
                            ColumnName = "stock_quantity"
                        },
                    new DataColumn
                        {
                            Caption = (CRMInvoiceResource.TrackInventory),
                            ColumnName = "track_inventory"
                        },
                    new DataColumn
                        {
                            Caption = (CRMInvoiceResource.Currency),
                            ColumnName = "currency"
                        },

                    new DataColumn
                        {
                            Caption = (CRMInvoiceResource.InvoiceTax1Name),
                            ColumnName = "tax1_name"
                        },
                    new DataColumn
                        {
                            Caption = (CRMInvoiceResource.InvoiceTax1Rate),
                            ColumnName = "tax1_rate"
                        },
                    new DataColumn
                        {
                            Caption = (CRMInvoiceResource.InvoiceTax2Name),
                            ColumnName = "tax2_name"
                        },
                    new DataColumn
                        {
                            Caption = (CRMInvoiceResource.InvoiceTax2Rate),
                            ColumnName = "tax2_rate"
                        }

                });


            foreach (var item in invoiceItems)
            {
                Percentage += 1.0 * 100 / _totalCount;
                PublishChanges();

                var tax1 = item.InvoiceTax1ID != 0 ? taxes.Find(t => t.ID == item.InvoiceTax1ID) : null;
                var tax2 = item.InvoiceTax2ID != 0 ? taxes.Find(t => t.ID == item.InvoiceTax2ID) : null;

                dataTable.Rows.Add(new object[]
                    {
                        item.Title,
                        item.Description,
                        item.StockKeepingUnit,
                        item.Price.ToString(CultureInfo.InvariantCulture),
                        item.StockQuantity.ToString(CultureInfo.InvariantCulture),
                        item.TrackInventory.ToString(),
                        item.Currency,
                        tax1 != null ? tax1.Name : "",
                        tax1 != null ? tax1.Rate.ToString(CultureInfo.InvariantCulture) : "",
                        tax2 != null ? tax2.Name : "",
                        tax2 != null ? tax2.Rate.ToString(CultureInfo.InvariantCulture) : ""
                    });
            }

            return DataTableToCsv(dataTable);
        }

        private String SaveCsvFileInMyDocument(String title, String data)
        {
            string fileUrl;

            using (var memStream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
            {
                var file = _fileUploader.ExecAsync(_globalFolder.GetFolderMy(_fileMarker, _fileDaoFactory).ToString(), title, memStream.Length, memStream, true).Result;

                if (_fileUtility.CanWebView(title) || _fileUtility.CanWebEdit(title))
                {
                    fileUrl = _filesLinkUtility.GetFileWebEditorUrl(file.ID);
                    fileUrl += string.Format("&options={{\"delimiter\":{0},\"codePage\":{1}}}",
                                     (int)FileUtility.CsvDelimiter.Comma,
                                     Encoding.UTF8.CodePage);
                }
                else
                {
                    fileUrl = _filesLinkUtility.GetFileDownloadUrl(file.ID);
                }
            }

            return fileUrl;
        }
    }

    public class ExportToCsv
    {
        private readonly object Locker = new object();
        private readonly DistributedTaskQueue _queue;
        private readonly ExportDataOperation _exportDataOperation;
        private readonly SecurityContext _securityContext;
        protected readonly int _tenantID;

        public ExportToCsv(SecurityContext securityContext,
                           TenantManager tenantManager,
                           DistributedTaskQueueOptionsManager queueOptions,
                           ExportDataOperation exportDataOperation)
        {
            _securityContext = securityContext;
            _tenantID = tenantManager.GetCurrentTenant().TenantId;
            _queue = queueOptions.Get<ExportDataOperation>();
            _exportDataOperation = exportDataOperation;
        }

        public IProgressItem GetStatus(bool partialDataExport)
        {
            var key = GetKey(partialDataExport);

            var operation = _queue.GetTasks<ExportDataOperation>().FirstOrDefault(x => x.Id == key);

            return operation;
        }

        public IProgressItem Start(FilterObject filterObject, string fileName)
        {
            lock (Locker)
            {
                var key = GetKey(filterObject != null);

                var operation = _queue.GetTasks<ExportDataOperation>().FirstOrDefault(x => x.Id == key);

                if (operation != null && operation.IsCompleted)
                {
                    _queue.RemoveTask(operation.Id);
                    operation = null;
                }

                if (operation == null)
                {
                    _exportDataOperation.Configure(key, filterObject, fileName);

                    _queue.QueueTask(_exportDataOperation);
                }

                return operation;
            }
        }

        public void Cancel(bool partialDataExport)
        {
            lock (Locker)
            {
                var key = GetKey(partialDataExport);

                var findedItem = _queue.GetTasks<ExportDataOperation>().FirstOrDefault(x => x.Id == key);

                if (findedItem != null)
                {
                    _queue.RemoveTask(findedItem.Id);
                }
            }
        }

        public string GetKey(bool partialDataExport)
        {
            return string.Format("{0}_{1}", _tenantID,
                                 partialDataExport ? _securityContext.CurrentAccount.ID : Guid.Empty);
        }
    }
}