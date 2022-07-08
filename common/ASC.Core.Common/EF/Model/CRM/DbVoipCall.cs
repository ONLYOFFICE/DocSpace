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

namespace ASC.Core.Common.EF.Model;

public class DbVoipCall
{
    public string Id { get; set; }
    public string ParentCallId { get; set; }
    public string NumberFrom { get; set; }
    public string NumberTo { get; set; }
    public int? Status { get; set; }
    public Guid AnsweredBy { get; set; }
    public DateTime? DialDate { get; set; }
    public int? DialDuration { get; set; }
    public string Sid { get; set; }
    public string Uri { get; set; }
    public int? Duration { get; set; }
    public decimal RecordPrice { get; set; }
    public int? ContactId { get; set; }
    public decimal? Price { get; set; }
    public int TenantId { get; set; }

    public CrmContact CrmContact { get; set; }
}
public static class DbVoipCallExtension
{
    public static ModelBuilderWrapper AddDbVoipCall(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder
            .Add(MySqlAddDbVoipCall, Provider.MySql)
            .Add(PgSqlAddDbVoipCall, Provider.PostgreSql);

        return modelBuilder;
    }

    public static void MySqlAddDbVoipCall(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbVoipCall>(entity =>
        {
            entity.ToTable("crm_voip_calls")
                .HasCharSet("utf8");

            entity.HasIndex(e => e.TenantId)
                .HasDatabaseName("tenant_id");

            entity.HasIndex(e => new { e.ParentCallId, e.TenantId })
                .HasDatabaseName("parent_call_id");

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasColumnType("varchar(50)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.AnsweredBy)
                .IsRequired()
                .HasColumnName("answered_by")
                .HasColumnType("varchar(50)")
                .HasDefaultValueSql("'00000000-0000-0000-0000-000000000000'")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.ContactId)
                .HasColumnName("contact_id")
                .IsRequired(false);

            entity.Property(e => e.DialDate)
                .HasColumnName("dial_date")
                .HasColumnType("datetime")
                .IsRequired(false);

            entity.Property(e => e.DialDuration)
                .HasColumnName("dial_duration")
                .IsRequired(false);

            entity.Property(e => e.NumberFrom)
                .IsRequired()
                .HasColumnName("number_from")
                .HasColumnType("varchar(50)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.NumberTo)
                .IsRequired()
                .HasColumnName("number_to")
                .HasColumnType("varchar(50)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.ParentCallId)
                .IsRequired()
                .HasColumnName("parent_call_id")
                .HasColumnType("varchar(50)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Price)
                .HasColumnName("price")
                .HasColumnType("decimal(10,4)")
                .IsRequired(false);

            entity.Property(e => e.Duration)
                .HasColumnName("record_duration")
                .IsRequired(false);

            entity.Property(e => e.RecordPrice)
                .HasColumnName("record_price")
                .HasColumnType("decimal(10,4)");

            entity.Property(e => e.Sid)
                .HasColumnName("record_sid")
                .HasColumnType("varchar(50)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Uri)
                .HasColumnName("record_url")
                .HasColumnType("text")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Status)
                .HasColumnName("status")
                .IsRequired(false);

            entity.Property(e => e.TenantId).HasColumnName("tenant_id");
        });
    }
    public static void PgSqlAddDbVoipCall(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbVoipCall>(entity =>
        {
            entity.ToTable("crm_voip_calls", "onlyoffice");

            entity.HasIndex(e => e.TenantId)
                .HasDatabaseName("tenant_id_crm_voip_calls");

            entity.HasIndex(e => new { e.ParentCallId, e.TenantId })
                .HasDatabaseName("parent_call_id");

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasMaxLength(50);

            entity.Property(e => e.AnsweredBy)
                .IsRequired()
                .HasColumnName("answered_by")
                .HasMaxLength(50)
                .HasDefaultValueSql("'00000000-0000-0000-0000-000000000000'");

            entity.Property(e => e.ContactId).HasColumnName("contact_id");

            entity.Property(e => e.DialDate).HasColumnName("dial_date");

            entity.Property(e => e.DialDuration).HasColumnName("dial_duration");

            entity.Property(e => e.NumberFrom)
                .IsRequired()
                .HasColumnName("number_from")
                .HasMaxLength(50);

            entity.Property(e => e.NumberTo)
                .IsRequired()
                .HasColumnName("number_to")
                .HasMaxLength(50);

            entity.Property(e => e.ParentCallId)
                .IsRequired()
                .HasColumnName("parent_call_id")
                .HasMaxLength(50);

            entity.Property(e => e.Price)
                .HasColumnName("price")
                .HasColumnType("numeric(10,4)")
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.Duration).HasColumnName("record_duration");

            entity.Property(e => e.RecordPrice)
                .HasColumnName("record_price")
                .HasColumnType("numeric(10,4)");

            entity.Property(e => e.Sid)
                .HasColumnName("record_sid")
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.Uri).HasColumnName("record_url");

            entity.Property(e => e.Status).HasColumnName("status");

            entity.Property(e => e.TenantId).HasColumnName("tenant_id");
        });
    }
}
