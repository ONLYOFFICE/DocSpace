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

namespace ASC.MessagingSystem.EF.Model;

public class AuditEvent : MessageEvent, IMapFrom<EventMessage>
{
    public string Initiator { get; set; }
    public string Target { get; set; }

    public DbTenant Tenant { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<MessageEvent, AuditEvent>();
        profile.CreateMap<EventMessage, AuditEvent>()
            .ConvertUsing<EventTypeConverter>();
    }
}

public static class AuditEventExtension
{
    public static ModelBuilderWrapper AddAuditEvent(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder.Entity<AuditEvent>().Navigation(e => e.Tenant).AutoInclude(false);

        modelBuilder
            .Add(MySqlAddAuditEvent, Provider.MySql)
            .Add(PgSqlAddAuditEvent, Provider.PostgreSql);

        return modelBuilder;
    }

    public static void MySqlAddAuditEvent(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditEvent>(entity =>
        {
            entity.ToTable("audit_events")
                .HasCharSet("utf8");

            entity.HasIndex(e => new { e.TenantId, e.Date })
                .HasDatabaseName("date");

            entity
                .Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Action)
                .HasColumnName("action")
                .IsRequired(false);

            entity.Property(e => e.Browser)
                .HasColumnName("browser")
                .HasColumnType("varchar(200)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Date)
                .HasColumnName("date")
                .HasColumnType("datetime");

            entity.Property(e => e.DescriptionRaw)
                .HasColumnName("description")
                .HasColumnType("varchar(20000)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Initiator)
                .HasColumnName("initiator")
                .HasColumnType("varchar(200)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Ip)
                .HasColumnName("ip")
                .HasColumnType("varchar(50)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Page)
                .HasColumnName("page")
                .HasColumnType("varchar(300)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Platform)
                .HasColumnName("platform")
                .HasColumnType("varchar(200)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Target)
                .HasColumnName("target")
                .HasColumnType("text")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.TenantId).HasColumnName("tenant_id");

            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .IsRequired(false)
                .HasColumnType("char(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");
        });
    }
    public static void PgSqlAddAuditEvent(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditEvent>(entity =>
        {
            entity.ToTable("audit_events", "onlyoffice");

            entity.HasIndex(e => new { e.TenantId, e.Date })
                .HasDatabaseName("date");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Action).HasColumnName("action");

            entity.Property(e => e.Browser)
                .HasColumnName("browser")
                .HasMaxLength(200)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.Date).HasColumnName("date");

            entity.Property(e => e.DescriptionRaw)
                .HasColumnName("description")
                .HasMaxLength(20000)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.Initiator)
                .HasColumnName("initiator")
                .HasMaxLength(200)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.Ip)
                .HasColumnName("ip")
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.Page)
                .HasColumnName("page")
                .HasMaxLength(300)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.Platform)
                .HasColumnName("platform")
                .HasMaxLength(200)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.Target).HasColumnName("target");

            entity.Property(e => e.TenantId).HasColumnName("tenant_id");

            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .HasMaxLength(38)
                .IsFixedLength()
                .HasDefaultValueSql("NULL");
        });
    }
}
