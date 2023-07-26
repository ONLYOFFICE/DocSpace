// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.Feed.Model;

public class FeedAggregate : BaseEntity, IMapFrom<FeedRow>
{
    public string Id { get; set; }
    public int TenantId { get; set; }
    public string Product { get; set; }
    public string Module { get; set; }
    public Guid Author { get; set; }
    public Guid ModifiedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public string GroupId { get; set; }
    public DateTime AggregateDate { get; set; }
    public string Json { get; set; }
    public string Keywords { get; set; }
    public string ContextId { get; set; }

    public DbTenant Tenant { get; set; }

    public override object[] GetKeys()
    {
        return new object[] { Id };
    }
}
public static class FeedAggregateExtension
{
    public static ModelBuilderWrapper AddFeedAggregate(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder.Entity<FeedAggregate>().Navigation(e => e.Tenant).AutoInclude(false);

        modelBuilder
            .Add(MySqlAddFeedAggregate, Provider.MySql)
            .Add(PgSqlAddFeedAggregate, Provider.PostgreSql);
        return modelBuilder;
    }
    public static void MySqlAddFeedAggregate(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FeedAggregate>(entity =>
        {
            entity.ToTable("feed_aggregate")
                .HasCharSet("utf8");

            entity.HasIndex(e => new { e.TenantId, e.AggregateDate })
                .HasDatabaseName("aggregated_date");

            entity.HasIndex(e => new { e.TenantId, e.ModifiedDate })
                .HasDatabaseName("modified_date");

            entity.HasIndex(e => new { e.TenantId, e.Product })
                .HasDatabaseName("product");

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasColumnType("varchar(88)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.AggregateDate)
                .HasColumnName("aggregated_date")
                .HasColumnType("datetime");

            entity.Property(e => e.Author)
                .IsRequired()
                .HasColumnName("author")
                .HasColumnType("char(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.CreatedDate)
                .HasColumnName("created_date")
                .HasColumnType("datetime");

            entity.Property(e => e.GroupId)
                .HasColumnName("group_id")
                .HasColumnType("varchar(70)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Json)
                .IsRequired()
                .HasColumnName("json")
                .HasColumnType("mediumtext")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Keywords)
                .HasColumnName("keywords")
                .HasColumnType("text")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.ModifiedBy)
                .IsRequired()
                .HasColumnName("modified_by")
                .HasColumnType("char(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.ModifiedDate)
                .HasColumnName("modified_date")
                .HasColumnType("datetime");

            entity.Property(e => e.Module)
                .IsRequired()
                .HasColumnName("module")
                .HasColumnType("varchar(50)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Product)
                .IsRequired()
                .HasColumnName("product")
                .HasColumnType("varchar(50)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.TenantId).HasColumnName("tenant");

            entity.Property(e => e.ContextId)
                .HasColumnName("context_id")
                .HasColumnType("text")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");
        });
    }
    public static void PgSqlAddFeedAggregate(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FeedAggregate>(entity =>
        {
            entity.ToTable("feed_aggregate", "onlyoffice");

            entity.HasIndex(e => new { e.TenantId, e.AggregateDate })
                .HasDatabaseName("aggregated_date");

            entity.HasIndex(e => new { e.TenantId, e.ModifiedDate })
                .HasDatabaseName("modified_date");

            entity.HasIndex(e => new { e.TenantId, e.Product })
                .HasDatabaseName("product");

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasMaxLength(88);

            entity.Property(e => e.AggregateDate).HasColumnName("aggregated_date");

            entity.Property(e => e.Author)
                .IsRequired()
                .HasColumnName("author")
                .HasMaxLength(38)
                .IsFixedLength();

            entity.Property(e => e.CreatedDate).HasColumnName("created_date");

            entity.Property(e => e.GroupId)
                .HasColumnName("group_id")
                .HasMaxLength(70)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.Json)
                .IsRequired()
                .HasColumnName("json");

            entity.Property(e => e.Keywords).HasColumnName("keywords");

            entity.Property(e => e.ModifiedBy)
                .IsRequired()
                .HasColumnName("modified_by")
                .HasMaxLength(38)
                .IsFixedLength();

            entity.Property(e => e.ModifiedDate).HasColumnName("modified_date");

            entity.Property(e => e.Module)
                .IsRequired()
                .HasColumnName("module")
                .HasMaxLength(50);

            entity.Property(e => e.Product)
                .IsRequired()
                .HasColumnName("product")
                .HasMaxLength(50);

            entity.Property(e => e.TenantId).HasColumnName("tenant");

            entity.Property(e => e.ContextId).HasColumnName("context_id");
        });
    }
}
