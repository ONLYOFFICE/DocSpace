using System;
using System.ComponentModel.DataAnnotations.Schema;

using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;
using Microsoft.EntityFrameworkCore;

namespace ASC.Files.Core.EF
{
    [Table("encrypted_data")]
    public class DbEncryptedData : BaseEntity, IDbFile
    {
        [Column("public_key")]
        public string PublicKey { get; set; }

        [Column("file_hash")]
        public string FileHash { get; set; }

        public string Data { get; set; }

        [Column("create_on")]
        public DateTime CreateOn { get; set; }

        [Column("tenant_id")]
        public int TenantId { get; set; }

        public override object[] GetKeys() => new object[] { PublicKey, FileHash };
    }

    public static class DbEncryptedDataExtension
    {
        public static ModelBuilderWrapper AddDbEncryptedData(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddDbEncryptedData, Provider.MySql)
                .Add(PgSqlAddDbEncryptedData, Provider.Postrge);
            return modelBuilder;
        }
        public static void MySqlAddDbEncryptedData(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbEncryptedData>(entity =>
            {
                entity.HasKey(e => new { e.PublicKey, e.FileHash })
                    .HasName("PRIMARY");

                entity.ToTable("encrypted_data");

                entity.HasIndex(e => e.TenantId)
                    .HasName("tenant_id");

                entity.Property(e => e.PublicKey)
                    .HasColumnName("public_key")
                    .HasColumnType("char(64)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.FileHash)
                    .HasColumnName("file_hash")
                    .HasColumnType("char(66)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CreateOn)
                    .HasColumnName("create_on")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.Data)
                    .IsRequired()
                    .HasColumnName("data")
                    .HasColumnType("char(112)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            });
        }
        public static void PgSqlAddDbEncryptedData(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbEncryptedData>(entity =>
            {
                entity.HasKey(e => new { e.PublicKey, e.FileHash })
                    .HasName("encrypted_data_pkey");

                entity.ToTable("encrypted_data", "onlyoffice");

                entity.HasIndex(e => e.TenantId)
                    .HasName("tenant_id_encrypted_data");

                entity.Property(e => e.PublicKey)
                    .HasColumnName("public_key")
                    .HasMaxLength(64)
                    .IsFixedLength();

                entity.Property(e => e.FileHash)
                    .HasColumnName("file_hash")
                    .HasMaxLength(66)
                    .IsFixedLength();

                entity.Property(e => e.CreateOn)
                    .HasColumnName("create_on")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Data)
                    .IsRequired()
                    .HasColumnName("data")
                    .HasMaxLength(112)
                    .IsFixedLength();

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            });
        }
    }
}
