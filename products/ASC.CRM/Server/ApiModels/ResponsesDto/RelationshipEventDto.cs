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


using ASC.Api.Core;
using ASC.Common;
using ASC.Common.Mapping;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.CRM.Mapping;
using ASC.Files.Core.ApiModels.ResponseDto;
using ASC.Web.Api.Models;

using AutoMapper;

namespace ASC.CRM.ApiModels
{
    public class EntityDto
    {
        public String EntityType { get; set; }
        public int EntityId { get; set; }
        public String EntityTitle { get; set; }
        public static EntityDto GetSample()
        {
            return new EntityDto
            {
                EntityId = 123445,
                EntityType = "opportunity",
                EntityTitle = "Household appliances internet shop"
            };
        }
    }

    [Scope]
    public class EntityDtoHelper
    {
        public EntityDtoHelper(DaoFactory daoFactory)
        {
            DaoFactory = daoFactory;
        }

        public DaoFactory DaoFactory { get; }

        public EntityDto Get(EntityType entityType, int entityID)
        {
            if (entityID == 0) return null;

            var result = new EntityDto
            {
                EntityId = entityID
            };

            switch (entityType)
            {
                case EntityType.Case:
                    var caseObj = DaoFactory.GetCasesDao().GetByID(entityID);
                    if (caseObj == null)
                        return null;

                    result.EntityType = "case";
                    result.EntityTitle = caseObj.Title;

                    break;
                case EntityType.Opportunity:
                    var dealObj = DaoFactory.GetDealDao().GetByID(entityID);
                    if (dealObj == null)
                        return null;

                    result.EntityType = "opportunity";
                    result.EntityTitle = dealObj.Title;

                    break;
                default:
                    return null;
            }

            return result;
        }
    }

    public class RelationshipEventDto : IMapFrom<RelationshipEvent>
    {
        public RelationshipEventDto()
        {

        }

        public int Id { get; set; }
        public EmployeeDto CreateBy { get; set; }

        public ApiDateTime Created { get; set; }
        public String Content { get; set; }
        public HistoryCategoryBaseDto Category { get; set; }
        public ContactBaseDto Contact { get; set; }
        public EntityDto Entity { get; set; }
        public bool CanEdit { get; set; }
        public IEnumerable<FileDto<int>> Files { get; set; }
        public static RelationshipEventDto GetSample()
        {
            return new RelationshipEventDto
            {
                CanEdit = true,
                Category = HistoryCategoryBaseDto.GetSample(),
                Entity = EntityDto.GetSample(),
                Contact = ContactBaseDto.GetSample(),
                Created = ApiDateTime.GetSample(),
                CreateBy = EmployeeDto.GetSample(),
                Files = new[] { FileDto<int>.GetSample() },
                Content = @"Agreed to meet at lunch and discuss the client commercial offer"
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<RelationshipEvent, RelationshipEventDto>()
                   .ConvertUsing<RelationshipEventDtoTypeConverter>();
        }
    }

}