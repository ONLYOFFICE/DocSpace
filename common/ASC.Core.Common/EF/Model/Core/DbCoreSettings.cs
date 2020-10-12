using System;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Model
{
    [Table("core_settings")]
    public class DbCoreSettings : BaseEntity
    {
        public int Tenant { get; set; }
        public string Id { get; set; }
        public byte[] Value { get; set; }

        [Column("last_modified")]
        public DateTime LastModified { get; set; }

        public override object[] GetKeys()
        {
            return new object[] { Tenant, Id };
        }
    }

    public static class CoreSettingsExtension
    {
        public static ModelBuilderWrapper AddCoreSettings(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddCoreSettings, Provider.MySql)
                .Add(PgSqlAddCoreSettings, Provider.Postgre)
                .HasData(
                    new DbCoreSettings { Tenant = -1, Id = "CompanyWhiteLabelSettings", Value = System.Text.Encoding.ASCII.GetBytes("0xF547048A4865171587D9CEBC8A496C601D96031F2C1C3E9160353942EE765DACD316F4B5F42892436FC4A21B9A6DF8FFD3BC4036B47E3A5A1B4C881B26609869FEBB6848BD88C02EEAC6A4CCB3E8F404290812F0E6E124A552BE81A58C64BB8BD3C9A8C0EDE1F9421281DE0C7AF82733C0B754E97EFFFA5A75607A91957896CBECF9563FC831300DC8E7C930A55B298EB82D6F69E0ED6E4D8752607F1881F61B032306E0F069A5F69F086A177EB41AC06F889EB0B39CBFD4B5CDB763E996554DEADB9C71CF3EF86F4A0354A864A10639DFD29B5C6D5DCDA9D4B0988EE406948BCB54C6A70ADC6C00577174285CEBCD76"), LastModified = DateTime.UtcNow },
                    new DbCoreSettings { Tenant = -1, Id = "FullTextSearchSettings", Value = System.Text.Encoding.ASCII.GetBytes("0x0878CF0599B517CAA2D3DAED9D064C3EDCEEAF431F35A6F642DCADA04817E3513227BBB1DE6E2BABEB9E1077B2CF318C489814545E877501F633FBBE94022CFCDD025B5395973AF510943408BB56962EE35DA35F2F8374CF5FD12695359449D7CEFBC2C7BD112AE58752179AA2A59E5E17801E580CCC60FAEC8EBDD3D612C4886666D96D6CF060605E64C90A1FAA80C0"), LastModified = DateTime.UtcNow },
                    new DbCoreSettings { Tenant = -1, Id = "SmtpSettings", Value = System.Text.Encoding.ASCII.GetBytes("0xF052E090A1A3750DADCD4E9961DA04AA51EF0197E2C0623CF12C5838BFA40A9B48BAEFCBE371587731D7E3DC9E7C6009742F9E415D56DB0F0AE08E32F8904B2C441CC657C64543EAEE262044A28B4335DCB0F0C4E9401D891FA06369F984CA2D475C86C237917961C5827769831585230A66AC7787E6FB56FD3E37389267A46A"), LastModified = DateTime.UtcNow }
                );

            return modelBuilder;
        }

        public static void MySqlAddCoreSettings(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbCoreSettings>(entity =>
            {
                entity.HasKey(e => new { e.Tenant, e.Id })
                    .HasName("PRIMARY");

                entity.ToTable("core_settings");

                entity.Property(e => e.Tenant).HasColumnName("tenant");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("varchar(128)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.LastModified)
                    .HasColumnName("last_modified")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasColumnName("value")
                    .HasColumnType("mediumblob");
            });
        }
        public static void PgSqlAddCoreSettings(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbCoreSettings>(entity =>
            {
                entity.HasKey(e => new { e.Tenant, e.Id })
                    .HasName("core_settings_pkey");

                entity.ToTable("core_settings", "onlyoffice");

                entity.Property(e => e.Tenant).HasColumnName("tenant");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasMaxLength(128);

                entity.Property(e => e.LastModified)
                    .HasColumnName("last_modified")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasColumnName("value");
            });
        }
    }
}
