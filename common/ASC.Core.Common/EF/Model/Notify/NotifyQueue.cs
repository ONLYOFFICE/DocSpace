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

public class NotifyQueue
{
    public int NotifyId { get; set; }
    public int TenantId { get; set; }
    public string Sender { get; set; }
    public string Reciever { get; set; }
    public string Subject { get; set; }
    public string ContentType { get; set; }
    public string Content { get; set; }
    public string SenderType { get; set; }
    public string ReplyTo { get; set; }
    public DateTime CreationDate { get; set; }
    public string Attachments { get; set; }
    public string AutoSubmitted { get; set; }

    public DbTenant Tenant { get; set; }
}
public static class NotifyQueueExtension
{
    public static ModelBuilderWrapper AddNotifyQueue(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder.Entity<NotifyQueue>().Navigation(e => e.Tenant).AutoInclude(false);

        modelBuilder
            .Add(MySqlAddNotifyQueue, Provider.MySql)
            .Add(PgSqlAddNotifyQueue, Provider.PostgreSql);

        return modelBuilder;
    }
    public static void MySqlAddNotifyQueue(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NotifyQueue>(entity =>
        {
            entity.HasKey(e => e.NotifyId)
                .HasName("PRIMARY");

            entity.ToTable("notify_queue")
                .HasCharSet("utf8");

            entity.Property(e => e.NotifyId).HasColumnName("notify_id");

            entity.Property(e => e.Attachments)
                .HasColumnName("attachments")
                .HasColumnType("text")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.AutoSubmitted)
                .HasColumnName("auto_submitted")
                .HasColumnType("varchar(64)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Content)
                .HasColumnName("content")
                .HasColumnType("text")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.ContentType)
                .HasColumnName("content_type")
                .HasColumnType("varchar(64)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.CreationDate)
                .HasColumnName("creation_date")
                .HasColumnType("datetime");

            entity.HasIndex(e => e.CreationDate)
                .HasDatabaseName("creation_date");

            entity.Property(e => e.Reciever)
                .HasColumnName("reciever")
                .HasColumnType("varchar(255)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.ReplyTo)
                .HasColumnName("reply_to")
                .HasColumnType("varchar(1024)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Sender)
                .HasColumnName("sender")
                .HasColumnType("varchar(255)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.SenderType)
                .HasColumnName("sender_type")
                .HasColumnType("varchar(64)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Subject)
                .HasColumnName("subject")
                .HasColumnType("varchar(1024)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.TenantId).HasColumnName("tenant_id");
        });
    }
    public static void PgSqlAddNotifyQueue(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NotifyQueue>(entity =>
        {
            entity.HasKey(e => e.NotifyId)
                .HasName("notify_queue_pkey");

            entity.ToTable("notify_queue", "onlyoffice");

            entity.Property(e => e.NotifyId).HasColumnName("notify_id");

            entity.Property(e => e.Attachments).HasColumnName("attachments");

            entity.Property(e => e.AutoSubmitted)
                .HasColumnName("auto_submitted")
                .HasMaxLength(64)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.Content).HasColumnName("content");

            entity.Property(e => e.ContentType)
                .HasColumnName("content_type")
                .HasMaxLength(64)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.CreationDate).HasColumnName("creation_date");

            entity.HasIndex(e => e.CreationDate)
                    .HasDatabaseName("creation_date_notify_queue");

            entity.Property(e => e.Reciever)
                .HasColumnName("reciever")
                .HasMaxLength(255)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.ReplyTo)
                .HasColumnName("reply_to")
                .HasMaxLength(1024)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.Sender)
                .HasColumnName("sender")
                .HasMaxLength(255)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.SenderType)
                .HasColumnName("sender_type")
                .HasMaxLength(64)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.Subject)
                .HasColumnName("subject")
                .HasMaxLength(1024)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.TenantId).HasColumnName("tenant_id");
        });
    }
}
