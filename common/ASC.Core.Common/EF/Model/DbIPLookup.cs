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

public class DbIPLookup
{
    public string AddrType { get; set; } //ipv4, ipv6
    public byte[] IPStart { get; set; }
    public byte[] IPEnd { get; set; }
    public string Continent { get; set; }
    public string Country { get; set; }
    public string StateProvCode { get; set; }
    public string StateProv { get; set; }
    public string District { get; set; }
    public string City { get; set; }
    public string ZipCode { get; set; }
    public float Latitude { get; set; }
    public float Longitude { get; set; }
    public int? GeonameId { get; set; }
    public float TimezoneOffset { get; set; }
    public string TimezoneName { get; set; }
    public string WeatherCode { get; set; }

}

public static class DbIPLookupExtension
{
    public static ModelBuilderWrapper AddDbIPLookup(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder
            .Add(MySqlAddDbIPLookup, Provider.MySql)
            .Add(PgSqlAddDbIPLookup, Provider.PostgreSql);

        return modelBuilder;
    }

    public static void MySqlAddDbIPLookup(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbIPLookup>(entity =>
        {
            entity.ToTable("dbip_lookup")
                 .HasCharSet("utf8mb4");

            entity.HasKey(nameof(DbIPLookup.AddrType), nameof(DbIPLookup.IPStart));

            entity.Property(e => e.AddrType)
                .IsRequired()
                .HasColumnName("addr_type")
                .HasColumnType("enum('ipv4','ipv6')");
       
            entity.Property(e => e.IPStart)
                .IsRequired()
                .HasColumnName("ip_start")
                .HasColumnType("varbinary(16)");

            entity.Property(e => e.IPEnd)
                .IsRequired()
                .HasColumnName("ip_end")
                .HasColumnType("varbinary(16)");

            entity.Property(e => e.Continent)
                .IsRequired()
                .HasColumnName("continent")
                .HasColumnType("char(2)");

            entity.Property(e => e.Country)
                .IsRequired()
                .HasColumnName("country")
                .HasColumnType("char(2)");

            entity.Property(e => e.StateProvCode)
                .HasColumnName("stateprov_code")
                .HasColumnType("varchar(15)");

            entity.Property(e => e.StateProv)
                .IsRequired()
                .HasColumnName("stateprov")
                .HasColumnType("varchar(80)");

            entity.Property(e => e.District)
                .IsRequired()
                .HasColumnName("district")
                .HasColumnType("varchar(80)");


            entity.Property(e => e.City)
                .IsRequired()
                .HasColumnName("city")
                .HasColumnType("varchar(80)");

            entity.Property(e => e.ZipCode)
                .HasColumnName("zipcode")
                .HasColumnType("varchar(20)");

            entity.Property(e => e.Latitude)
                .IsRequired()
                .HasColumnName("latitude")
                .HasColumnType("float");

            entity.Property(e => e.Longitude)
                .IsRequired()
                .HasColumnName("longitude")
                .HasColumnType("float");

            entity.Property(e => e.GeonameId)
               .IsRequired(false)
               .HasColumnName("geoname_id")
               .HasColumnType("int(10)");

            entity.Property(e => e.TimezoneOffset)
                .IsRequired()
                .HasColumnType("float")
                .HasColumnName("timezone_offset");

            entity.Property(e => e.TimezoneName)
                .IsRequired()
                .HasColumnName("timezone_name")
                .HasColumnType("varchar(64)");

            entity.Property(e => e.WeatherCode)
                .IsRequired()
                .HasColumnName("weather_code")
                .HasColumnType("varchar(10)");
        });

    }
    public static void PgSqlAddDbIPLookup(this ModelBuilder modelBuilder)
    {
        throw new NotImplementedException();
    }
}
