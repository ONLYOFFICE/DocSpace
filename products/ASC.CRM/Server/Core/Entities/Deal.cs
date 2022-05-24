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
using System.Text.Json.Serialization;

using ASC.Common.Mapping;
using ASC.Common.Security;
using ASC.Common.Security.Authorizing;
using ASC.CRM.Core.EF;
using ASC.CRM.Core.Enums;

namespace ASC.CRM.Core.Entities
{
    public class Deal : DomainObject, ISecurityObjectId, IMapFrom<DbDeal>
    {
        public Guid CreateBy { get; set; }
        public DateTime CreateOn { get; set; }
        public Guid? LastModifedBy { get; set; }
        public DateTime? LastModifedOn { get; set; }

        [JsonPropertyName("contact_id")]
        public int ContactID { get; set; }
        public Contact Contact { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        [JsonPropertyName("responsible_id")]
        public Guid ResponsibleID { get; set; }

        [JsonPropertyName("bid_type")]
        public BidType BidType { get; set; }

        [JsonPropertyName("bid_value")]
        public decimal BidValue { get; set; }

        [JsonPropertyName("bid_currency")]
        public string BidCurrency { get; set; }

        [JsonPropertyName("per_period_value")]
        public int PerPeriodValue { get; set; }

        [JsonPropertyName("deal_milestone")]
        public int DealMilestoneID { get; set; }

        [JsonPropertyName("deal_milestone_probability")]
        public int DealMilestoneProbability { get; set; }

        [JsonPropertyName("actual_close_date")]
        public DateTime ActualCloseDate { get; set; }

        //[DataMember(Name = "actual_close_date")]
        //private String ActualCloseDateStr
        //{
        //    get
        //    {
        //        return ActualCloseDate.Date == DateTime.MinValue.Date
        //                   ? string.Empty : ActualCloseDate.ToString(DateTimeExtension.DateFormatPattern);
        //    }
        //    set { ; }
        //}

        [JsonPropertyName("expected_close_date")]
        public DateTime ExpectedCloseDate { get; set; }

        //private String ExpectedCloseDateStr
        //{
        //    get
        //    {
        //        return ExpectedCloseDate.Date == DateTime.MinValue.Date
        //                   ? string.Empty : ExpectedCloseDate.ToString(DateTimeExtension.DateFormatPattern);
        //    }
        //    set { ; }
        //}

        public object SecurityId
        {
            get { return ID; }
        }
        public string FullId => AzObjectIdHelper.GetFullObjectId(this);

        public Type ObjectType
        {
            get { return GetType(); }
        }
    }
}
