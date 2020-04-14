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
using ASC.Common;
using ASC.Core;
using ASC.CRM.Core.Entities;
using ASC.ElasticSearch;

using Microsoft.Extensions.DependencyInjection;

namespace ASC.Web.CRM.Core.Search
{
    public sealed class FieldsWrapper : Wrapper
    {
        [ColumnLastModified("last_modifed_on")]
        public override DateTime LastModifiedOn { get; set; }

        [ColumnCondition("field_id", 1)]
        public int FieldId { get; set; }

        [ColumnCondition("entity_id", 2)]
        public int EntityId { get; set; }

        [ColumnCondition("entity_type", 3)]
        public int EntityType { get; set; }

        [Column("value", 4)]
        public string Value { get; set; }

        protected override string Table { get { return "crm_field_value"; } }

        public static FieldsWrapper GetEventsWrapper(IServiceProvider serviceProvider, CustomField cf)
        {
            var tenantManager = serviceProvider.GetService<TenantManager>();

            return new FieldsWrapper
            {
                Id = cf.ID,
                EntityId = cf.EntityID,
                EntityType = (int)cf.EntityType,
                Value = cf.Value,
                TenantId = tenantManager.GetCurrentTenant().TenantId
            };
        }
    }

    public static class FieldsWrapperExtention
    {
        public static DIHelper AddFieldsWrapperService(this DIHelper services)
        {
            services.TryAddTransient<FieldsWrapper>();

            return services
                .AddFactoryIndexerService<FieldsWrapper>();
        }
    }
}