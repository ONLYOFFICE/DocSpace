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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.CRM.Core.EF
{
    [Table("crm_deal")]
    public partial class DbDeal : IDbCrm
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        
        [Required]
        [Column("title", TypeName = "varchar(255)")]
        public string Title { get; set; }
        
        [Column("description", TypeName = "text")]
        public string Description { get; set; }
        
        [Required]
        [Column("responsible_id", TypeName = "char(38)")]
        public Guid ResponsibleId { get; set; }
        
        [Column("contact_id", TypeName = "int(11)")]
        public int ContactId { get; set; }
        
        [Column("create_on", TypeName = "datetime")]
        public DateTime CreateOn { get; set; }
        
        [Required]        
        [Column("create_by", TypeName = "char(38)")]
        public Guid CreateBy { get; set; }
        
        [Column("bid_currency", TypeName = "varchar(255)")]
        public string BidCurrency { get; set; }
        
        [Column("bid_value", TypeName = "decimal(50,9)")]
        public decimal BidValue { get; set; }
        
        [Column("bid_type", TypeName = "int(11)")]
        public int BidType { get; set; }
        
        [Column("deal_milestone_id", TypeName = "int(11)")]
        public int DealMilestoneId { get; set; }
                
        [Column("tenant_id", TypeName = "int(11)")]
        public int TenantId { get; set; }
        
        [Column("expected_close_date", TypeName = "datetime")]
        public DateTime ExpectedCloseDate { get; set; }
        
        [Column("per_period_value", TypeName = "int(11)")]
        public int PerPeriodValue { get; set; }
       
        [Column("deal_milestone_probability", TypeName = "int(11)")]
        public int? DealMilestoneProbability { get; set; }
       
        [Column("last_modifed_on", TypeName = "datetime")]
        public DateTime? LastModifedOn { get; set; }

        [Column("last_modifed_by", TypeName = "char(38)")]
        public Guid LastModifedBy { get; set; }

        [Column("actual_close_date", TypeName = "datetime")]
        public DateTime? ActualCloseDate { get; set; }
    }
}