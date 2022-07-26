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

public class DbipLocation
{
    public int Id { get; set; }
    public string AddrType { get; set; }
    public string IPStart { get; set; }
    public string IPEnd { get; set; }
    public string Country { get; set; }
    public string StateProv { get; set; }
    public string District { get; set; }
    public string City { get; set; }
    public string ZipCode { get; set; }
    public long? Latitude { get; set; }
    public long? Longitude { get; set; }
    public int? GeonameId { get; set; }
    public double? TimezoneOffset { get; set; }
    public string TimezoneName { get; set; }
    public int Processed { get; set; }
}

public static class DbipLocationExtension
{
    public static ModelBuilderWrapper AddDbipLocation(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder
            .Add(MySqlAddDbipLocation, Provider.MySql)
            .Add(PgSqlAddDbipLocation, Provider.PostgreSql);

        return modelBuilder;
    }

    public static void MySqlAddDbipLocation(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbipLocation>(entity =>
        {
            entity.ToTable("dbip_location")
                .HasCharSet("utf8");

            entity.HasIndex(e => e.IPStart)
                .HasDatabaseName("ip_start");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.AddrType)
                .IsRequired()
                .HasColumnName("addr_type")
                .HasColumnType("enum('ipv4','ipv6')")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.City)
                .IsRequired()
                .HasColumnName("city")
                .HasColumnType("varchar(255)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Country)
                .IsRequired()
                .HasColumnName("country")
                .HasColumnType("varchar(2)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.District)
                .HasColumnName("district")
                .HasColumnType("varchar(255)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.GeonameId)
                .HasColumnName("geoname_id")
                .IsRequired(false)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.IPEnd)
                .IsRequired()
                .HasColumnName("ip_end")
                .HasColumnType("varchar(39)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.IPStart)
                .IsRequired()
                .HasColumnName("ip_start")
                .HasColumnType("varchar(39)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Latitude)
                .HasColumnName("latitude")
                .HasColumnType("float")
                .IsRequired(false)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.Longitude)
                .HasColumnName("longitude")
                .HasColumnType("float")
                .IsRequired(false)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.Processed)
                .HasColumnName("processed")
                .HasDefaultValueSql("'1'");

            entity.Property(e => e.StateProv)
                .IsRequired()
                .HasColumnName("stateprov")
                .HasColumnType("varchar(255)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.TimezoneName)
                .HasColumnName("timezone_name")
                .HasColumnType("varchar(255)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.TimezoneOffset)
                .HasColumnType("int")
                .HasColumnName("timezone_offset")
                .IsRequired(false)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.ZipCode)
                .HasColumnName("zipcode")
                .HasColumnType("varchar(255)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");
        });

    }
    public static void PgSqlAddDbipLocation(this ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum("onlyoffice", "enum_dbip_location", new[] { "ipv4", "ipv6" });
        modelBuilder.Entity<DbipLocation>(entity =>
        {
            entity.ToTable("dbip_location", "onlyoffice");

            entity.HasIndex(e => e.IPStart)
                .HasDatabaseName("ip_start");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.City)
                .IsRequired()
                .HasColumnName("city")
                .HasMaxLength(255);

            entity.Property(e => e.Country)
                .IsRequired()
                .HasColumnName("country")
                .HasMaxLength(2);

            entity.Property(e => e.District)
                .HasColumnName("district")
                .HasMaxLength(255)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.GeonameId).HasColumnName("geoname_id");

            entity.Property(e => e.IPEnd)
                .IsRequired()
                .HasColumnName("ip_end")
                .HasMaxLength(39);

            entity.Property(e => e.IPStart)
                .IsRequired()
                .HasColumnName("ip_start")
                .HasMaxLength(39);

            entity.Property(e => e.Latitude).HasColumnName("latitude");

            entity.Property(e => e.Longitude).HasColumnName("longitude");

            entity.Property(e => e.Processed)
                .HasColumnName("processed")
                .HasDefaultValueSql("1");

            entity.Property(e => e.StateProv)
                .IsRequired()
                .HasColumnName("stateprov")
                .HasMaxLength(255);

            entity.Property(e => e.TimezoneName)
                .HasColumnName("timezone_name")
                .HasMaxLength(255)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.TimezoneOffset).HasColumnName("timezone_offset");

            entity.Property(e => e.ZipCode)
                .HasColumnName("zipcode")
                .HasMaxLength(255)
                .HasDefaultValueSql("NULL");
        });
    }
}
