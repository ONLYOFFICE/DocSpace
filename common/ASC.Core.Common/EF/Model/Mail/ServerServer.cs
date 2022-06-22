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

namespace ASC.Core.Common.EF.Model.Mail;

public class ServerServer
{
    public int Id { get; set; }
    public string MxRecord { get; set; }
    public string ConnectionString { get; set; }
    public int ServerType { get; set; }
    public int SmtpSettingsId { get; set; }
    public int ImapSettingsId { get; set; }
}

public static class ServerServerExtension
{
    public static ModelBuilderWrapper AddServerServer(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder
            .Add(MySqlAddServerServer, Provider.MySql)
            .Add(PgSqlAddServerServer, Provider.PostgreSql);

        return modelBuilder;
    }
    public static void MySqlAddServerServer(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ServerServer>(entity =>
        {
            entity.ToTable("mail_server_server");

            entity.HasIndex(e => e.ServerType)
                .HasDatabaseName("mail_server_server_type_server_type_fk_id");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.ConnectionString)
                .IsRequired()
                .HasColumnName("connection_string")
                .HasColumnType("text")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.ImapSettingsId).HasColumnName("imap_settings_id");

            entity.Property(e => e.MxRecord)
                .IsRequired()
                .HasColumnName("mx_record")
                .HasColumnType("varchar(128)")
                .HasDefaultValueSql("''")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.ServerType).HasColumnName("server_type");

            entity.Property(e => e.SmtpSettingsId).HasColumnName("smtp_settings_id");
        });
    }
    public static void PgSqlAddServerServer(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ServerServer>(entity =>
        {
            entity.ToTable("mail_server_server", "onlyoffice");

            entity.HasIndex(e => e.ServerType)
                .HasDatabaseName("mail_server_server_type_server_type_fk_id");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.ConnectionString)
                .IsRequired()
                .HasColumnName("connection_string");

            entity.Property(e => e.ImapSettingsId).HasColumnName("imap_settings_id");

            entity.Property(e => e.MxRecord)
                .IsRequired()
                .HasColumnName("mx_record")
                .HasMaxLength(128)
                .HasDefaultValueSql("' '");

            entity.Property(e => e.ServerType).HasColumnName("server_type");

            entity.Property(e => e.SmtpSettingsId).HasColumnName("smtp_settings_id");
        });
    }
}
