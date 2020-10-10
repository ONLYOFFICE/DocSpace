using System.ComponentModel.DataAnnotations.Schema;

using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;
using Microsoft.EntityFrameworkCore;

namespace ASC.Files.Core.EF
{
    [Table("files_thirdparty_id_mapping")]
    public class DbFilesThirdpartyIdMapping : BaseEntity, IDbFile
    {
        [Column("tenant_id")]
        public int TenantId { get; set; }

        [Column("hash_id")]
        public string HashId { get; set; }
        public string Id { get; set; }

        public override object[] GetKeys()
        {
            return new object[] { Id };
        }
    }

    public static class DbFilesThirdpartyIdMappingExtension
    {
        public static ModelBuilderWrapper AddDbFilesThirdpartyIdMapping(this ModelBuilderWrapper modelBuilder)
        {
            _ = modelBuilder
                .Add(MySqlAddDbFilesThirdpartyIdMapping, Provider.MySql)
                .Add(PgSqlAddDbFilesThirdpartyIdMapping, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddDbFilesThirdpartyIdMapping(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<DbFilesThirdpartyIdMapping>(entity =>
            {
                _ = entity.HasKey(e => e.HashId)
                    .HasName("PRIMARY");

                _ = entity.ToTable("files_thirdparty_id_mapping");

                _ = entity.HasIndex(e => new { e.TenantId, e.HashId })
                    .HasName("index_1");

                _ = entity.Property(e => e.HashId)
                    .HasColumnName("hash_id")
                    .HasColumnType("char(32)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.Id)
                    .IsRequired()
                    .HasColumnName("id")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            });
    }
        public static void PgSqlAddDbFilesThirdpartyIdMapping(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<DbFilesThirdpartyIdMapping>(entity =>
            {
                _ = entity.HasKey(e => e.HashId)
                    .HasName("files_thirdparty_id_mapping_pkey");

                _ = entity.ToTable("files_thirdparty_id_mapping", "onlyoffice");

                _ = entity.HasIndex(e => new { e.TenantId, e.HashId })
                    .HasName("index_1");

                _ = entity.Property(e => e.HashId)
                    .HasColumnName("hash_id")
                    .HasMaxLength(32)
                    .IsFixedLength();

                _ = entity.Property(e => e.Id)
                    .IsRequired()
                    .HasColumnName("id");

                _ = entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            });
        }
    }
}
