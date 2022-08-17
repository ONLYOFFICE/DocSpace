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

namespace ASC.Webhooks.Core.EF.Model;

public partial class WebhooksLog
{
    public int ConfigId { get; set; }
    public DateTime CreationTime { get; set; }
    public int Id { get; set; }
    public string Method { get; set; }
    public string Route { get; set; }
    public string RequestHeaders { get; set; }
    public string RequestPayload { get; set; }
    public string ResponseHeaders { get; set; }
    public string ResponsePayload { get; set; }
    public ProcessStatus Status { get; set; }
    public int TenantId { get; set; }
    public Guid Uid { get; set; }
}

public static class WebhooksPayloadExtension
{
    public static ModelBuilderWrapper AddWebhooksLog(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder
            .Add(MySqlAddWebhooksLog, Provider.MySql)
            .Add(PgSqlAddWebhooksLog, Provider.PostgreSql);

        return modelBuilder;
    }

    private static void MySqlAddWebhooksLog(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WebhooksLog>(entity =>
        {
            entity.HasKey(e => new { e.Id })
                .HasName("PRIMARY");

            entity.ToTable("webhooks_logs")
                .HasCharSet("utf8");

            entity.Property(e => e.Id)
                .HasColumnType("int")
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.ConfigId)
                .HasColumnType("int")
                .HasColumnName("config_id");

            entity.Property(e => e.Uid)
                .HasColumnName("uid")
                .HasColumnType("varchar(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.TenantId)
                .HasColumnName("tenant_id")
                .HasColumnType("int unsigned");

            entity.Property(e => e.RequestPayload)
                .IsRequired()
                .HasColumnName("request_payload")
                .HasColumnType("text")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.RequestHeaders)
                .HasColumnName("request_headers")
                .HasColumnType("json");

            entity.Property(e => e.ResponsePayload)
                .HasColumnName("response_payload")
                .HasColumnType("text")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.ResponseHeaders)
                .HasColumnName("response_headers")
                .HasColumnType("json");

            entity.Property(e => e.Method)
                .HasColumnType("varchar")
                .HasColumnName("method")
                .HasMaxLength(100);

            entity.Property(e => e.Route)
                .HasColumnType("varchar")
                .HasColumnName("route")
                .HasMaxLength(100);

            entity.Property(e => e.CreationTime)
                .HasColumnType("datetime")
                .HasColumnName("creation_time");

            entity.Property(e => e.Status)
                .HasColumnType("varchar")
                .HasColumnName("status")
                .HasMaxLength(50);
        });
    }

    private static void PgSqlAddWebhooksLog(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WebhooksLog>(entity =>
        {
            entity.HasKey(e => new { e.Id })
                .HasName("PRIMARY");

            entity.ToTable("webhooks_logs");

            entity.Property(e => e.Id)
                .HasColumnType("int")
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.ConfigId)
                .HasColumnType("int")
                .HasColumnName("config_id");

            entity.Property(e => e.Uid)
                .HasColumnType("varchar")
                .HasColumnName("uid")
                .HasMaxLength(50);

            entity.Property(e => e.TenantId)
                .HasColumnName("tenant_id")
                .HasColumnType("int unsigned");

            entity.Property(e => e.RequestPayload)
                .IsRequired()
                .HasColumnName("request_payload");

            entity.Property(e => e.RequestHeaders)
                .HasColumnName("request_headers")
                .HasColumnType("json");

            entity.Property(e => e.ResponsePayload)
                .HasColumnName("response_payload");

            entity.Property(e => e.ResponseHeaders)
                .HasColumnName("response_headers")
                .HasColumnType("json");

            entity.Property(e => e.Method)
                .HasColumnType("varchar")
                .HasColumnName("method")
                .HasMaxLength(100);

            entity.Property(e => e.Route)
                .HasColumnType("varchar")
                .HasColumnName("route")
                .HasMaxLength(100);

            entity.Property(e => e.CreationTime)
                .HasColumnType("datetime")
                .HasColumnName("creation_time");

            entity.Property(e => e.Status)
                .HasColumnType("varchar")
                .HasColumnName("status")
                .HasMaxLength(50);
        });
    }
}