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


using ASC.Api.Core;
using ASC.Api.Documents;
using ASC.Common;
using ASC.CRM.Core;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;
using ASC.Web.Api.Models;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ASC.Api.CRM.Wrappers
{
    [DataContract(Name = "entity", Namespace = "")]
    public class EntityWrapper
    {
        [DataMember]
        public String EntityType { get; set; }

        [DataMember]
        public int EntityId { get; set; }

        [DataMember]
        public String EntityTitle { get; set; }

        public static EntityWrapper GetSample()
        {
            return new EntityWrapper
            {
                EntityId = 123445,
                EntityType = "opportunity",
                EntityTitle = "Household appliances internet shop"
            };
        }
    }

    [DataContract(Name = "historyEvent", Namespace = "")]
    public class RelationshipEventWrapper
    {
        public RelationshipEventWrapper()
        {

        }
               
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public EmployeeWraper CreateBy { get; set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public ApiDateTime Created { get; set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public String Content { get; set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public HistoryCategoryBaseWrapper Category { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ContactBaseWrapper Contact { get; set; }

        [DataMember]
        public EntityWrapper Entity { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool CanEdit { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IEnumerable<FileWrapper<int>> Files { get; set; }

        public static RelationshipEventWrapper GetSample()
        {
            return new RelationshipEventWrapper
            {
                CanEdit = true,
                Category = HistoryCategoryBaseWrapper.GetSample(),
                Entity = EntityWrapper.GetSample(),
                Contact = ContactBaseWrapper.GetSample(),
                Created = ApiDateTime.GetSample(),
                CreateBy = EmployeeWraper.GetSample(),
                Files = new[] { FileWrapper<int>.GetSample() },
                Content = @"Agreed to meet at lunch and discuss the client commercial offer"
            };
        }
    }

    public class RelationshipEventWrapperHelper
    {
        public RelationshipEventWrapperHelper(
                           ApiDateTimeHelper apiDateTimeHelper,
                           EmployeeWraperHelper employeeWraperHelper,
                           ContactBaseWrapperHelper contactBaseWrapperHelper,
                           FileWrapperHelper fileWrapperHelper,
                           CRMSecurity cRMSecurity,
                           DaoFactory daoFactory)
        {
            ApiDateTimeHelper = apiDateTimeHelper;
            EmployeeWraperHelper = employeeWraperHelper;
            CRMSecurity = cRMSecurity;
            DaoFactory = daoFactory;
            ContactBaseWrapperHelper = contactBaseWrapperHelper;
            FileWrapperHelper = fileWrapperHelper;
        }

        public FileWrapperHelper FileWrapperHelper { get; }
        public ContactBaseWrapperHelper ContactBaseWrapperHelper { get; }
        public DaoFactory DaoFactory { get; }
        public CRMSecurity CRMSecurity { get; }
        public ApiDateTimeHelper ApiDateTimeHelper { get; }
        public EmployeeWraperHelper EmployeeWraperHelper { get; }

        public RelationshipEventWrapper Get(RelationshipEvent relationshipEvent)
        {
            var result = new RelationshipEventWrapper
            {
                Id = relationshipEvent.ID,
                CreateBy = EmployeeWraperHelper.Get(relationshipEvent.CreateBy),
                Created = ApiDateTimeHelper.Get(relationshipEvent.CreateOn),
                Content = relationshipEvent.Content,
                Files = new List<FileWrapper<int>>(),
                CanEdit = CRMSecurity.CanEdit(relationshipEvent)
            };


            var historyCategory = DaoFactory.GetListItemDao().GetByID(relationshipEvent.CategoryID);

            if (historyCategory != null)
            {
                result.Category = new HistoryCategoryBaseWrapper(historyCategory);
            }

            if (relationshipEvent.EntityID > 0)
            {
                result.Entity = ToEntityWrapper(relationshipEvent.EntityType, relationshipEvent.EntityID);
            }

            result.Files = DaoFactory.GetRelationshipEventDao().GetFiles(relationshipEvent.ID).ConvertAll(file => FileWrapperHelper.Get<int>(file));

            if (relationshipEvent.ContactID > 0)
            {
                var relativeContact = DaoFactory.GetContactDao().GetByID(relationshipEvent.ContactID);
                if (relativeContact != null)
                {
                    result.Contact = ContactBaseWrapperHelper.Get(relativeContact);
                }
            }

            result.CanEdit = CRMSecurity.CanAccessTo(relationshipEvent);

            return result;

        }        
    }

    public static class RelationshipEventWrapperHelperExtension
    {
        public static DIHelper AddRelationshipEventWrapperHelperService(this DIHelper services)
        {
            services.TryAddTransient<RelationshipEventWrapperHelper>();

            return services.AddApiDateTimeHelper()
                           .AddEmployeeWraper()
                           .AddCRMSecurityService()
                           .AddContactBaseWrapperHelperService()
                           .AddFileWrapperHelperService();
        }
    }
}