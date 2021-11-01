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

using ASC.Api.Core.Convention;
using ASC.Common;
using ASC.Common.Web;
using ASC.CRM.Core;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.Web.Api.Routing;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;

namespace ASC.Api.CRM
{
    [Scope]
    [DefaultRoute]
    [ApiController]
    [ControllerName("crm")]
    public abstract class BaseApiController : ControllerBase
    {
        protected IMapper _mapper;
        protected DaoFactory _daoFactory;
        protected CrmSecurity _crmSecurity;

        public BaseApiController(DaoFactory daoFactory,
                                 CrmSecurity crmSecurity,
                                 IMapper mapper)
        {
            _daoFactory = daoFactory;
            _crmSecurity = crmSecurity;
            _mapper = mapper;
        }


        protected static EntityType ToEntityType(string entityTypeStr)
        {
            EntityType entityType;

            if (string.IsNullOrEmpty(entityTypeStr)) return EntityType.Any;

            switch (entityTypeStr.ToLower())
            {
                case "person":
                    entityType = EntityType.Person;
                    break;
                case "company":
                    entityType = EntityType.Company;
                    break;
                case "contact":
                    entityType = EntityType.Contact;
                    break;
                case "opportunity":
                    entityType = EntityType.Opportunity;
                    break;
                case "case":
                    entityType = EntityType.Case;
                    break;
                default:
                    entityType = EntityType.Any;
                    break;
            }

            return entityType;
        }

        protected string GetEntityTitle(EntityType entityType, int entityId, bool checkAccess, out DomainObject entity)
        {
            switch (entityType)
            {
                case EntityType.Contact:
                case EntityType.Company:
                case EntityType.Person:
                    var contact = (entity = _daoFactory.GetContactDao().GetByID(entityId)) as ASC.CRM.Core.Entities.Contact;
                    if (contact == null || (checkAccess && !_crmSecurity.CanAccessTo(contact)))
                        throw new ItemNotFoundException();
                    return contact.GetTitle();
                case EntityType.Opportunity:
                    var deal = (entity = _daoFactory.GetDealDao().GetByID(entityId)) as Deal;
                    if (deal == null || (checkAccess && !_crmSecurity.CanAccessTo(deal)))
                        throw new ItemNotFoundException();
                    return deal.Title;
                case EntityType.Case:
                    var cases = (entity = _daoFactory.GetCasesDao().GetByID(entityId)) as Cases;
                    if (cases == null || (checkAccess && !_crmSecurity.CanAccessTo(cases)))
                        throw new ItemNotFoundException();
                    return cases.Title;
                default:
                    throw new ArgumentException("Invalid entityType: " + entityType);
            }
        }

    }
}
