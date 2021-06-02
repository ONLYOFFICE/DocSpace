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
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;
using ASC.CRM.Core.Enums;
using ASC.ElasticSearch;
using ASC.ElasticSearch.Core;

using Microsoft.EntityFrameworkCore;

using Nest;

namespace ASC.CRM.Core.EF
{
    [ElasticsearchType(RelationName = "crm_deal")]
    [Table("crm_deal")]
    public class DbDeal : IDbCrm, ISearchItem
    {
        public int Id { get; set; }

        [Text(Analyzer = "whitespacecustom")]
        public string Title { get; set; }

        [Text(Analyzer = "whitespacecustom")]
        public string Description { get; set; }

        [Column("responsible_id")]
        public Guid ResponsibleId { get; set; }

        [Column("contact_id")]
        public int ContactId { get; set; }

        [Column("create_on")]
        public DateTime CreateOn { get; set; }

        [Column("create_by")]
        public Guid CreateBy { get; set; }

        [Column("bid_currency")]
        public string BidCurrency { get; set; }

        [Column("bid_value")]
        public decimal BidValue { get; set; }

        [Column("bid_type")]
        public BidType BidType { get; set; }

        [Column("deal_milestone_id")]
        public int DealMilestoneId { get; set; }

        [Column("tenant_id")]
        public int TenantId { get; set; }

        [Column("expected_close_date")]
        public DateTime ExpectedCloseDate { get; set; }

        [Column("per_period_value")]
        public int PerPeriodValue { get; set; }

        [Column("deal_milestone_probability")]
        public int DealMilestoneProbability { get; set; }

        [Column("last_modifed_on")]
        public DateTime? LastModifedOn { get; set; }

        [Column("last_modifed_by")]
        public Guid LastModifedBy { get; set; }

        [Column("actual_close_date")]
        public DateTime? ActualCloseDate { get; set; }

        [NotMapped]
        [Ignore]
        public string IndexName
        {
            get
            {
                return "crm_deal";
            }
        }

        public Expression<Func<ISearchItem, object[]>> GetSearchContentFields(SearchSettingsHelper searchSettings)
        {
            return (a) => new[] { Title, Description };
        }
    }

    public static class DbDealExtension
    {
        public static ModelBuilderWrapper AddDbDeal(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddDbDeal, Provider.MySql)
                .Add(PgSqlAddDbDeal, Provider.Postgre);

            return modelBuilder;
        }
        private static void MySqlAddDbDeal(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbDeal>(entity =>
            {
                entity.Property(x => x.Id)
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.Title)
                     .HasCharSet("utf8")
                     .UseCollation("utf8_general_ci");

                entity.Property(e => e.Description)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ResponsibleId)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasIndex(e => new { e.TenantId, e.ContactId })
                    .HasDatabaseName("contact_id");

                entity.HasIndex(e => e.CreateOn)
                    .HasDatabaseName("create_on");

                entity.Property(e => e.CreateBy)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.BidCurrency)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasIndex(e => e.LastModifedOn)
                    .HasDatabaseName("last_modifed_on");

                entity.HasIndex(e => e.DealMilestoneId)
                    .HasDatabaseName("deal_milestone_id");

                entity.Property(e => e.LastModifedBy)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });
        }

        private static void PgSqlAddDbDeal(this ModelBuilder modelBuilder)
        {
            throw new NotImplementedException();
        }

    }
}