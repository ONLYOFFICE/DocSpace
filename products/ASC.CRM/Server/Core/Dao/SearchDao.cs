
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

using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Utils;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Tenants;
using ASC.CRM.Classes;
using ASC.CRM.Core.EF;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.Web.Core.ModuleManagement.Common;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.CRM;
using ASC.Web.CRM.Configuration;
using ASC.Web.CRM.Core.Search;

using AutoMapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ASC.CRM.Core.Dao
{
    [Scope]
    public class SearchDao : AbstractDao
    {
        private Dictionary<EntityType, IEnumerable<int>> _findedIDs;

        //TODO: setting _fullTextSearchEnable field
        private bool _fullTextSearchEnable;

        private readonly DaoFactory _daoFactory;
        private readonly CrmSecurity _crmSecurity;
        private readonly BundleSearch _bundleSearch;
        private readonly WebImageSupplier _webImageSupplier;
        private readonly TenantUtil _tenantUtil;
        private readonly PathProvider _pathProvider;
        private readonly FactoryIndexerTask _factoryIndexerTask;
        private readonly FactoryIndexerInvoice _factoryIndexerInvoice;


        public SearchDao(DbContextManager<CrmDbContext> dbContextManager,
                      TenantManager tenantManager,
                      DaoFactory daoFactory,
                      SecurityContext securityContext,
                      CrmSecurity crmSecurity,
                      TenantUtil tenantUtil,
                      PathProvider pathProvider,
                      FactoryIndexerTask tasksDtoIndexer,
                      FactoryIndexerInvoice invoicesDtoIndexer,
                      ILogger logger,
                      ICache ascCache,
                      WebImageSupplier webImageSupplier,
                      BundleSearch bundleSearch,
                      IMapper mapper
                      ) :
           base(dbContextManager,
                tenantManager,
                securityContext,
                logger,
                ascCache,
                mapper)
        {
            _daoFactory = daoFactory;
            _factoryIndexerTask = tasksDtoIndexer;
            _factoryIndexerInvoice = invoicesDtoIndexer;
            _crmSecurity = crmSecurity;
            _tenantUtil = tenantUtil;
            _pathProvider = pathProvider;
            _webImageSupplier = webImageSupplier;
            _bundleSearch = bundleSearch;
        }



        public SearchResultItem[] Search(String searchText)
        {
            var keywords = searchText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
               .ToArray();

            if (keywords.Length == 0) return new List<SearchResultItem>().ToArray();

            _fullTextSearchEnable = _bundleSearch.CheckFullTextSearchEnable();

            if (_fullTextSearchEnable)
            {
                _findedIDs = new Dictionary<EntityType, IEnumerable<int>>();

                List<int> casesId;
                if (_bundleSearch.TrySelectCase(searchText, out casesId))
                {
                    _findedIDs.Add(EntityType.Case, casesId);
                }

                List<int> contactsId;
                if (_bundleSearch.TrySelectContact(searchText, out contactsId))
                {
                    _findedIDs.Add(EntityType.Contact, contactsId);
                }

                List<int> dealsId;
                if (_bundleSearch.TrySelectOpportunity(searchText, out dealsId))
                {
                    _findedIDs.Add(EntityType.Opportunity, dealsId);
                }

                List<int> tasksId;

                if (_factoryIndexerTask.TrySelectIds(r => r.MatchAll(searchText), out tasksId))
                {
                    _findedIDs.Add(EntityType.Task, tasksId);
                }

                List<int> invoicesId;

                if (_factoryIndexerInvoice.TrySelectIds(r => r.MatchAll(searchText), out invoicesId))
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
            var sqlQuery = Query(CrmDbContext.RelationshipEvent);

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
            var sqlQuery = Query(CrmDbContext.FieldValue);

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
            var sqlQuery = Query(CrmDbContext.ContactsInfo);

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
            return _findedIDs.ContainsKey(entityType);
        }

        private SearchResultItem[] GetSearchResultItems(String[] keywords)
        {
            var result = new List<SearchResultItem>();

            if (IncludeToSearch(EntityType.Task))
            {
                var sqlQuery = Query(CrmDbContext.Tasks);

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
                    if (!_crmSecurity.CanAccessTo(new Task { ID = x.Id })) return;

                    result.Add(new SearchResultItem
                    {
                        Name = x.Title,
                        Description = HtmlUtil.GetText(x.Description, 120),
                        URL = _pathProvider.BaseAbsolutePath,
                        Date = _tenantUtil.DateTimeFromUtc(x.CreateOn),
                        Additional = new Dictionary<String, Object>
                                            { { "imageRef",  _webImageSupplier.GetAbsoluteWebPath("tasks_widget.png", ProductEntryPoint.ID) },
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
                var sqlQuery = Query(CrmDbContext.Deals);

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
                    if (!_crmSecurity.CanAccessTo(new Deal { ID = x.Id })) return;

                    result.Add(new SearchResultItem
                    {
                        Name = x.Title,
                        Description = HtmlUtil.GetText(x.Description, 120),
                        URL = string.Concat(_pathProvider.BaseAbsolutePath, string.Format("deals.aspx?id={0}", x.Id)),
                        Date = _tenantUtil.DateTimeFromUtc(x.CreateOn),
                        Additional = new Dictionary<string, object>
                                                     { { "imageRef",  _webImageSupplier.GetAbsoluteWebPath("deal_widget.png", ProductEntryPoint.ID) },
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
                var sqlQuery = Query(CrmDbContext.Contacts);

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
                        if (!_crmSecurity.CanAccessTo(new Company { ID = x.Id })) return;
                    }
                    else
                    {
                        if (!_crmSecurity.CanAccessTo(new Person { ID = x.Id })) return;
                    }

                    result.Add(new SearchResultItem
                    {
                        Name = x.IsCompany ? x.CompanyName : $"{x.FirstName} {x.LastName}",
                        Description = HtmlUtil.GetText(x.Notes, 120),
                        URL = String.Concat(_pathProvider.BaseAbsolutePath, String.Format("default.aspx?id={0}", x.Id)),
                        Date = _tenantUtil.DateTimeFromUtc(x.CreateOn),
                        Additional = new Dictionary<String, Object>
                                                     { { "imageRef",  _webImageSupplier.GetAbsoluteWebPath(x.IsCompany ? "companies_widget.png" : "people_widget.png", ProductEntryPoint.ID) },
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
                var sqlQuery = Query(CrmDbContext.Cases);

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
                    if (!_crmSecurity.CanAccessTo(new Cases { ID = x.Id })) return;

                    result.Add(new SearchResultItem
                    {
                        Name = x.Title,
                        Description = String.Empty,
                        URL = String.Concat(_pathProvider.BaseAbsolutePath, String.Format("cases.aspx?id={0}", x.Id)),
                        Date = _tenantUtil.DateTimeFromUtc(x.CreateOn),
                        Additional = new Dictionary<String, Object>
                                                     { { "imageRef",  _webImageSupplier.GetAbsoluteWebPath("cases_widget.png", ProductEntryPoint.ID) },
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
                var sqlQuery = Query(CrmDbContext.Invoices);

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
                    if (!_crmSecurity.CanAccessTo(new Invoice { ID = x.Id })) return;

                    result.Add(new SearchResultItem
                    {
                        Name = x.Number,
                        Description = String.Empty,
                        URL = String.Concat(_pathProvider.BaseAbsolutePath, String.Format("invoices.aspx?id={0}", x.Id)),
                        Date = _tenantUtil.DateTimeFromUtc(x.CreateOn),
                        Additional = new Dictionary<String, Object>
                                                         { { "imageRef",  _webImageSupplier.GetAbsoluteWebPath("invoices_widget.png", ProductEntryPoint.ID) },
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
                return _daoFactory.GetContactDao().GetByID(contactID).GetTitle();

            switch (entityType)
            {
                case EntityType.Company:
                case EntityType.Person:
                case EntityType.Contact:
                    var contact = _daoFactory.GetContactDao().GetByID(contactID);
                    return contact == null ? string.Empty : contact.GetTitle();
                case EntityType.Opportunity:
                    var opportunity = _daoFactory.GetDealDao().GetByID(entityID);
                    return opportunity == null ? string.Empty : opportunity.Title;
                case EntityType.Case:
                    var @case = _daoFactory.GetCasesDao().GetByID(entityID);
                    return @case == null ? string.Empty : @case.Title;
                default:
                    throw new ArgumentException();
            }
        }
    }

}