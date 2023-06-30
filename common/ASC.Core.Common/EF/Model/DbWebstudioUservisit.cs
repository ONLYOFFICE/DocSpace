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

public class DbWebstudioUserVisit
{
    public int TenantId { get; set; }
    public DateTime VisitDate { get; set; }
    public Guid ProductId { get; set; }
    public Guid UserId { get; set; }
    public int VisitCount { get; set; }
    public DateTime? FirstVisitTime { get; set; }
    public DateTime? LastVisitTime { get; set; }

    public DbTenant Tenant { get; set; }
}

public static class WebstudioUserVisitExtension
{
    public static ModelBuilderWrapper AddWebstudioUserVisit(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder.Entity<DbWebstudioUserVisit>().Navigation(e => e.Tenant).AutoInclude(false);

        modelBuilder
            .Add(MySqlAddWebstudioUserVisit, Provider.MySql)
            .Add(PgSqlAddWebstudioUserVisit, Provider.PostgreSql);
        //.HasData(
        //new DbWebstudioUserVisit 
        //{ 
        //    TenantId = 1, 
        //    VisitDate = DateTime.UtcNow,
        //    ProductId = Guid.Parse("00000000-0000-0000-0000-000000000000"), 
        //    UserId = Guid.Parse("66faa6e4-f133-11ea-b126-00ffeec8b4ef"), 
        //    VisitCount = 3, 
        //    FirstVisitTime = DateTime.UtcNow, 
        //    LastVisitTime = DateTime.UtcNow
        //},
        //new DbWebstudioUserVisit 
        //{ 
        //    TenantId = 1, 
        //    VisitDate = DateTime.UtcNow, 
        //    ProductId = Guid.Parse("00000000-0000-0000-0000-000000000000"), 
        //    UserId = Guid.Parse("66faa6e4-f133-11ea-b126-00ffeec8b4ef"), 
        //    VisitCount = 2, 
        //    FirstVisitTime = DateTime.UtcNow, 
        //    LastVisitTime = DateTime.UtcNow 
        //},
        //new DbWebstudioUserVisit
        //{ 
        //    TenantId = 1, 
        //    VisitDate = DateTime.UtcNow, 
        //    ProductId = Guid.Parse("e67be73d-f9ae-4ce1-8fec-1880cb518cb4"), 
        //    UserId = Guid.Parse("66faa6e4-f133-11ea-b126-00ffeec8b4ef"), 
        //    VisitCount = 1, 
        //    FirstVisitTime = DateTime.UtcNow, 
        //    LastVisitTime = DateTime.UtcNow 
        //}
        //);

        return modelBuilder;
    }

    public static void MySqlAddWebstudioUserVisit(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbWebstudioUserVisit>(entity =>
        {
            entity.HasKey(e => new { e.TenantId, e.VisitDate, e.ProductId, e.UserId })
                .HasName("PRIMARY");

            entity.ToTable("webstudio_uservisit")
                .HasCharSet("utf8");

            entity.HasIndex(e => e.VisitDate)
                .HasDatabaseName("visitdate");

            entity.Property(e => e.TenantId).HasColumnName("tenantid");

            entity.Property(e => e.VisitDate)
                .HasColumnName("visitdate")
                .HasColumnType("datetime");

            entity.Property(e => e.ProductId)
                .HasColumnName("productid")
                .HasColumnType("varchar(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.UserId)
                .HasColumnName("userid")
                .HasColumnType("varchar(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.FirstVisitTime)
                .HasColumnName("firstvisittime")
                .HasColumnType("datetime")
                .IsRequired(false)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.LastVisitTime)
                .HasColumnName("lastvisittime")
                .HasColumnType("datetime")
                .IsRequired(false)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.VisitCount)
                .HasColumnName("visitcount")
                .HasDefaultValueSql("'0'");
        });
    }
    public static void PgSqlAddWebstudioUserVisit(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbWebstudioUserVisit>(entity =>
        {
            entity.HasKey(e => new { e.TenantId, e.VisitDate, e.ProductId, e.UserId })
                .HasName("webstudio_uservisit_pkey");

            entity.ToTable("webstudio_uservisit", "onlyoffice");

            entity.HasIndex(e => e.VisitDate)
                .HasDatabaseName("visitdate");

            entity.Property(e => e.TenantId).HasColumnName("tenantid");

            entity.Property(e => e.VisitDate).HasColumnName("visitdate");

            entity.Property(e => e.ProductId)
                .HasColumnName("productid")
                .HasMaxLength(38);

            entity.Property(e => e.UserId)
                .HasColumnName("userid")
                .HasMaxLength(38);

            entity.Property(e => e.FirstVisitTime).HasColumnName("firstvisittime");

            entity.Property(e => e.LastVisitTime).HasColumnName("lastvisittime");

            entity.Property(e => e.VisitCount).HasColumnName("visitcount");
        });
    }
}
