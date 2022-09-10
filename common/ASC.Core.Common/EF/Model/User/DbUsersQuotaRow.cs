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

namespace ASC.Core.Common.EF;

public class DbUsersQuotaRow : BaseEntity, IMapFrom<UserQuotaRow>
{
    public int Tenant { get; set; }
    public Guid UserId { get; set; }
    public string Path { get; set; }
    public long Counter { get; set; }
    public string Tag { get; set; }

    public override object[] GetKeys()
    {
        return new object[] { Tenant, UserId, Path };
    }
}

public static class DbUsersQuotaRowExtension
{
    public static ModelBuilderWrapper AddDbUsersQuotaRow(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder
            .Add(MySqlAddDbUsersQuotaRow, Provider.MySql)
            .Add(PgSqlAddDbUsersQuotaRow, Provider.PostgreSql);

        return modelBuilder;
    }

    public static void MySqlAddDbUsersQuotaRow(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbUsersQuotaRow>(entity =>
        {
            entity.HasKey(e => new { e.Tenant, e.UserId, e.Path })
                .HasName("PRIMARY");

            entity.ToTable("users_quotarow")
                .HasCharSet("utf8");

            entity.Property(e => e.Tenant).HasColumnName("tenant");

            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .HasColumnType("varchar(36)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Path)
                .HasColumnName("path")
                .HasColumnType("varchar(255)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Counter)
                .HasColumnName("counter")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.Tag)
                .HasColumnName("tag")
                .HasColumnType("varchar(36)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");
        });
    }
    public static void PgSqlAddDbUsersQuotaRow(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbUsersQuotaRow>(entity =>
        {
            entity.HasKey(e => new { e.Tenant, e.UserId, e.Path })
                .HasName("tenants_userquotarow_pkey");

            entity.ToTable("users_quotarow", "onlyoffice");

            entity.Property(e => e.Tenant).HasColumnName("tenant");

            entity.Property(e => e.Tag)
                .HasColumnName("user_id")
                .HasMaxLength(36)
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.Path)
                .HasColumnName("path")
                .HasMaxLength(255);

            entity.Property(e => e.Counter)
                .HasColumnName("counter")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.Tag)
                .HasColumnName("tag")
                .HasMaxLength(36)
                .HasDefaultValueSql("'0'");
        });
    }
}
