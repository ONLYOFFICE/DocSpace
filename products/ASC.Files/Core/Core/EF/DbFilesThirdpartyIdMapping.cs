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

namespace ASC.Files.Core.EF;

public class DbFilesThirdpartyIdMapping : BaseEntity, IDbFile
{
    public int TenantId { get; set; }
    public string HashId { get; set; }
    public string Id { get; set; }

    public DbTenant Tenant { get; set; }

    public override object[] GetKeys()
    {
        return new object[] { HashId };
    }
}

public static class DbFilesThirdpartyIdMappingExtension
{
    public static ModelBuilderWrapper AddDbFilesThirdpartyIdMapping(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder.Entity<DbFilesThirdpartyIdMapping>().Navigation(e => e.Tenant).AutoInclude(false);

        modelBuilder
            .Add(MySqlAddDbFilesThirdpartyIdMapping, Provider.MySql)
            .Add(PgSqlAddDbFilesThirdpartyIdMapping, Provider.PostgreSql);

        return modelBuilder;
    }

    public static void MySqlAddDbFilesThirdpartyIdMapping(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbFilesThirdpartyIdMapping>(entity =>
        {
            entity.HasKey(e => e.HashId)
                .HasName("PRIMARY");

            entity.ToTable("files_thirdparty_id_mapping")
                .HasCharSet("utf8");

            entity.HasIndex(e => new { e.TenantId, e.HashId })
                .HasDatabaseName("index_1");

            entity.Property(e => e.HashId)
                .HasColumnName("hash_id")
                .HasColumnType("char(32)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Id)
                .IsRequired()
                .HasColumnName("id")
                .HasColumnType("text")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.TenantId).HasColumnName("tenant_id");
        });
    }
    public static void PgSqlAddDbFilesThirdpartyIdMapping(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbFilesThirdpartyIdMapping>(entity =>
        {
            entity.HasKey(e => e.HashId)
                .HasName("files_thirdparty_id_mapping_pkey");

            entity.ToTable("files_thirdparty_id_mapping", "onlyoffice");

            entity.HasIndex(e => new { e.TenantId, e.HashId })
                .HasDatabaseName("index_1");

            entity.Property(e => e.HashId)
                .HasColumnName("hash_id")
                .HasMaxLength(32)
                .IsFixedLength();

            entity.Property(e => e.Id)
                .IsRequired()
                .HasColumnName("id");

            entity.Property(e => e.TenantId).HasColumnName("tenant_id");
        });
    }
}
