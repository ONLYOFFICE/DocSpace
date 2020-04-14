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


using ASC.Common.Logging;
using ASC.Common.Utils;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Tenants;
using ASC.CRM.Classes;
using ASC.CRM.Core.EF;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.ElasticSearch;
using ASC.Web.Core.ModuleManagement.Common;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.CRM;
using ASC.Web.CRM.Configuration;
using ASC.Web.CRM.Core.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASC.CRM.Core.Dao
{
    public class SearchDao : AbstractDao
    {
        private Dictionary<EntityType, IEnumerable<int>> _findedIDs;
        private bool _fullTextSearchEnable;
        private DaoFactory DaoFactory { get; set; }


        public SearchDao(DbContextManager<CRMDbContext> dbContextManager,
                      TenantManager tenantManager,
                      SecurityContext securityContext,
                      CRMSecurity cRMSecurity,
                      TenantUtil tenantUtil,
                      PathProvider pathProvider,
                      FactoryIndexer<TasksWrapper> tasksWrapperIndexer,
                      FactoryIndexer<InvoicesWrapper> invoicesWrapperIndexer,
                      IOptionsMonitor<ILog> logger,
                      WebImageSupplier webImageSupplier,
                      BundleSearch bundleSearch
                      ) :
           base(dbContextManager,
                tenantManager,
                securityContext,
                logger)
        {
            TasksWrapperIndexer = tasksWrapperIndexer;
            InvoicesWrapperIndexer = invoicesWrapperIndexer;
            CRMSecurity = cRMSecurity;
            TenantUtil = tenantUtil;
            PathProvider = pathProvider;
            WebImageSupplier = webImageSupplier;
            BundleSearch = bundleSearch;
        }

        public BundleSearch BundleSearch { get; }

        public WebImageSupplier WebImageSupplier { get; }

        public TenantUtil TenantUtil { get; }
        public PathProvider PathProvider { get; }
        public FactoryIndexer<TasksWrapper> TasksWrapperIndexer { get; }
        public FactoryIndexer<InvoicesWrapper> InvoicesWrapperIndexer { get; }
        public CRMSecurity CRMSecurity { get; }

        public SearchResultItem[] Search(String searchText)
        {
            var keywords = searchText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
               .ToArray();

            if (keywords.Length == 0) return new List<SearchResultItem>().ToArray();

            _fullTextSearchEnable = BundleSearch.Support(EntityType.Case)
                                    && BundleSearch.Support(EntityType.Contact)
                                    && BundleSearch.Support(EntityType.Opportunity)
                                    && BundleSearch.Support(EntityType.Task)
                                    && BundleSearch.Support(EntityType.Invoice);

            if (_fullTextSearchEnable)
            {
                _findedIDs = new Dictionary<EntityType, IEnumerable<int>>();

                List<int> casesId;
                if (BundleSearch.TrySelectCase(searchText, out casesId))
                {
                    _findedIDs.Add(EntityType.Case, casesId);
                }

                List<int> contactsId;
                if (BundleSearch.TrySelectContact(searchText, out contactsId))
                {
                    _findedIDs.Add(EntityType.Contact, contactsId);
                }

                List<int> dealsId;
                if (BundleSearch.TrySelectOpportunity(searchText, out dealsId))
                {
                    _findedIDs.Add(EntityType.Opportunity, dealsId);
                }

                List<int> tasksId;

                if (TasksWrapperIndexer.TrySelectIds(r => r.MatchAll(searchText), out tasksId))
                {
                    _findedIDs.Add(EntityType.Task, tasksId);
                }

                List<int> invoicesId;

                if (InvoicesWrapperIndexer.TrySelectIds(r => r.MatchAll(searchText), out invoicesId))
                {
                    _findedIDs.Add(EntityType.Invoice, invoicesId);
                }
            }
            else
            {
                _findedIDs = SearchByCustomFields(keywords)
                                     .Union(SearchByRelationshipEvent(keywords))
                                     .Union(SearchByContactInfos(keywords))
                                     .ToLookup(pair => pair.Key, pair => pair.Value)
                                     .ToDictionary(group => group.Key, group => group.First());
            }

            return GetSearchResultItems(keywords);
        }

        private Dictionary<EntityType, IEnumerable<int>> SearchByRelationshipEvent(String[] keywords)
        {
            var sqlQuery = Query(CRMDbContext.RelationshipEvent);

            if (keywords.Length > 0)
            {
                foreach (var k in keywords)
                {
                    sqlQuery = sqlQuery.Where(x => Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Content, k + "%"));
                }
            }

            return sqlQuery.GroupBy(x => x.EntityType)
                            .ToDictionary(x => x.Key, x => x.Select(y => y.EntityId > 0 ? y.EntityId : y.ContactId));
        }

        private Dictionary<EntityType, IEnumerable<int>> SearchByCustomFields(String[] keywords)
        {
            var sqlQuery = Query(CRMDbContext.FieldValue);

            if (keywords.Length > 0)
            {
                foreach (var k in keywords)
                {
                    sqlQuery = sqlQuery.Where(x => Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Value, k + "%"));
                }
            }

            return sqlQuery.GroupBy(x => x.EntityType)
                           .ToDictionary(x => x.Key, x => x.Select(x => x.EntityId));
        }

        private Dictionary<EntityType, IEnumerable<int>> SearchByContactInfos(String[] keywords)
        {
            var sqlQuery = Query(CRMDbContext.ContactsInfo);

            if (keywords.Length > 0)
            {
                foreach (var k in keywords)
                {
                    sqlQuery = sqlQuery.Where(x => Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Data, k + "%"));
                }
            }

            return new Dictionary<EntityType, IEnumerable<int>> { { EntityType.Contact, sqlQuery.Select(x => x.ContactId).Distinct() } };
        }
          
        private bool IncludeToSearch(EntityType entityType)
        {
            return !BundleSearch.Support(entityType) || _findedIDs.ContainsKey(entityType);
        }

        private SearchResultItem[] GetSearchResultItems(String[] keywords)
        {
            var result = new List<SearchResultItem>();

            if (IncludeToSearch(EntityType.Task))
            {
                var sqlQuery = Query(CRMDbContext.Tasks);

                if (_findedIDs.ContainsKey(EntityType.Task))
                {
                    sqlQuery = sqlQuery.Where(x => _findedIDs[EntityType.Task].Contains(x.Id));
                }
                else
                {
                    if (keywords.Length > 0)
                    {
                        foreach (var k in keywords)
                        {
                            sqlQuery = sqlQuery.Where(x => Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Title, k + "%") ||
                                                Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Description, k + "%"));
                        }
                    }
                }

                sqlQuery.ToList().ForEach(x =>
                {
                    if (!CRMSecurity.CanAccessTo(new Task { ID = x.Id })) return;

                    result.Add(new SearchResultItem
                    {
                        Name = x.Title,
                        Description = HtmlUtil.GetText(x.Description, 120),
                        URL = PathProvider.BaseAbsolutePath,
                        Date = TenantUtil.DateTimeFromUtc(x.CreateOn),
                        Additional = new Dictionary<String, Object>
                                            { { "imageRef",  WebImageSupplier.GetAbsoluteWebPath("tasks_widget.png", ProductEntryPoint.ID) },
                                                {"relativeInfo", GetPath(
                                                    x.ContactId,
                                                    x.EntityId,
                                                    x.EntityType)},
                                                {"typeInfo", EntityType.Task.ToLocalizedString()}
                                            }
                    });
                });
            }

            if (IncludeToSearch(EntityType.Opportunity))
            {
                var sqlQuery = Query(CRMDbContext.Deals);

                if (_findedIDs.ContainsKey(EntityType.Opportunity))
                {
                    sqlQuery = sqlQuery.Where(x => _findedIDs[EntityType.Opportunity].Contains(x.Id));
                }
                else
                {
                    if (keywords.Length > 0)
                    {
                        foreach (var k in keywords)
                        {
                            sqlQuery = sqlQuery.Where(x => Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Title, k + "%") ||
                                                Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Description, k + "%"));
                        }
                    }
                }


                sqlQuery.ToList().ForEach(x =>
                {
                    if (!CRMSecurity.CanAccessTo(new Deal { ID = x.Id })) return;

                    result.Add(new SearchResultItem
                    {
                        Name = x.Title,
                        Description = HtmlUtil.GetText(x.Description, 120),
                        URL = string.Concat(PathProvider.BaseAbsolutePath, string.Format("deals.aspx?id={0}", x.Id)),
                        Date = TenantUtil.DateTimeFromUtc(x.CreateOn),
                        Additional = new Dictionary<string, object>
                                                     { { "imageRef",  WebImageSupplier.GetAbsoluteWebPath("deal_widget.png", ProductEntryPoint.ID) },
                                                              {"relativeInfo", GetPath(
                                                                  x.ContactId,
                                                                  0,
                                                                  0)},
                                                              {"typeInfo", EntityType.Opportunity.ToLocalizedString()}
                                                     }
                    });

                });
            }


            if (IncludeToSearch(EntityType.Contact))
            {
                var sqlQuery = Query(CRMDbContext.Contacts);

                if (_findedIDs.ContainsKey(EntityType.Contact))
                {
                    sqlQuery = sqlQuery.Where(x => _findedIDs[EntityType.Contact].Contains(x.Id));
                }
                else
                {
                    if (keywords.Length > 0)
                    {
                        foreach (var k in keywords)
                        {
                            sqlQuery = sqlQuery.Where(x => Microsoft.EntityFrameworkCore.EF.Functions.Like(x.FirstName, k + "%") ||
                                                            Microsoft.EntityFrameworkCore.EF.Functions.Like(x.LastName, k + "%") ||
                                                            Microsoft.EntityFrameworkCore.EF.Functions.Like(x.CompanyName, k + "%") ||
                                                            Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Title, k + "%") ||
                                                            Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Notes, k + "%")
                                                        );
                        }
                    }
                }

                sqlQuery.ToList().ForEach(x =>
                {
                    if (x.IsCompany)
                    {
                        if (!CRMSecurity.CanAccessTo(new Company { ID = x.Id })) return;
                    }
                    else
                    {
                        if (!CRMSecurity.CanAccessTo(new Person { ID = x.Id })) return;
                    }

                    result.Add(new SearchResultItem
                    {
                        Name = x.IsCompany ? x.CompanyName : String.Format("{0} {1}", x.FirstName, x.LastName),
                        Description = HtmlUtil.GetText(x.Notes, 120),
                        URL = String.Concat(PathProvider.BaseAbsolutePath, String.Format("default.aspx?id={0}", x.Id)),
                        Date = TenantUtil.DateTimeFromUtc(x.CreateOn),
                        Additional = new Dictionary<String, Object>
                                                     { { "imageRef",  WebImageSupplier.GetAbsoluteWebPath(x.IsCompany ? "companies_widget.png" : "people_widget.png", ProductEntryPoint.ID) },
                                                              {"relativeInfo", GetPath(
                                                                  0,
                                                                  0,
                                                                  0)},
                                                              {"typeInfo", EntityType.Contact.ToLocalizedString()}
                                                     }
                    });
                });
            }

            if (IncludeToSearch(EntityType.Case))
            {
                var sqlQuery = Query(CRMDbContext.Cases);

                if (_findedIDs.ContainsKey(EntityType.Case))
                {
                    sqlQuery = sqlQuery.Where(x => _findedIDs[EntityType.Case].Contains(x.Id));
                }
                else
                {
                    if (keywords.Length > 0)
                    {
                        foreach (var k in keywords)
                        {
                            sqlQuery = sqlQuery.Where(x => Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Title, k + "%"));
                        }
                    }
                }

                sqlQuery.ToList().ForEach(x =>
                {
                    if (!CRMSecurity.CanAccessTo(new Cases { ID = x.Id })) return;

                    result.Add(new SearchResultItem
                    {
                        Name = x.Title,
                        Description = String.Empty,
                        URL = String.Concat(PathProvider.BaseAbsolutePath, String.Format("cases.aspx?id={0}", x.Id)),
                        Date = TenantUtil.DateTimeFromUtc(x.CreateOn),
                        Additional = new Dictionary<String, Object>
                                                     { { "imageRef",  WebImageSupplier.GetAbsoluteWebPath("cases_widget.png", ProductEntryPoint.ID) },
                                                              {"relativeInfo", GetPath(
                                                                  0,
                                                                  0,
                                                                  0)},
                                                              {"typeInfo", EntityType.Case.ToLocalizedString()}
                                                     }
                    });

                });

            }


            if (IncludeToSearch(EntityType.Invoice))
            {
                var sqlQuery = Query(CRMDbContext.Invoices);

                if (_findedIDs.ContainsKey(EntityType.Invoice))
                {
                    sqlQuery = sqlQuery.Where(x => _findedIDs[EntityType.Invoice].Contains(x.Id));
                }
                else
                {
                    if (keywords.Length > 0)
                    {
                        foreach (var k in keywords)
                        {
                            sqlQuery = sqlQuery.Where(x => Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Number, k + "%") ||
                                                           Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Description, k + "%"));
                        }
                    }
                }

                sqlQuery.ToList().ForEach(x =>
                {
                    if (!CRMSecurity.CanAccessTo(new Invoice { ID = x.Id })) return;

                    result.Add(new SearchResultItem
                    {
                        Name = x.Number,
                        Description = String.Empty,
                        URL = String.Concat(PathProvider.BaseAbsolutePath, String.Format("invoices.aspx?id={0}", x.Id)),
                        Date = TenantUtil.DateTimeFromUtc(x.CreateOn),
                        Additional = new Dictionary<String, Object>
                                                         { { "imageRef",  WebImageSupplier.GetAbsoluteWebPath("invoices_widget.png", ProductEntryPoint.ID) },
                                                              {"relativeInfo", GetPath(
                                                                  x.ContactId,
                                                                  x.EntityId,
                                                                  x.EntityType)},
                                                              {"typeInfo", EntityType.Invoice.ToLocalizedString()}
                                                         }
                    });

                });
            }

            return result.ToArray();
        }

        private String GetPath(int contactID, int entityID, EntityType entityType)
        {

            if (contactID == 0) return String.Empty;

            if (entityID == 0)
                return DaoFactory.GetContactDao().GetByID(contactID).GetTitle();

            switch (entityType)
            {
                case EntityType.Company:
                case EntityType.Person:
                case EntityType.Contact:
                    var contact = DaoFactory.GetContactDao().GetByID(contactID);
                    return contact == null ? string.Empty : contact.GetTitle();
                case EntityType.Opportunity:
                    var opportunity = DaoFactory.GetDealDao().GetByID(entityID);
                    return opportunity == null ? string.Empty : opportunity.Title;
                case EntityType.Case:
                    var @case = DaoFactory.GetCasesDao().GetByID(entityID);
                    return @case == null ? string.Empty : @case.Title;
                default:
                    throw new ArgumentException();
            }
        }
    }
}