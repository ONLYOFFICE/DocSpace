using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Files.Core.EF
{
    public class DbFilesThirdpartyIdMapping : BaseEntity, IDbFile
    {
        public int TenantId { get; set; }
        public string HashId { get; set; }
        public string Id { get; set; }

        public override object[] GetKeys()
        {
            return new object[] { HashId };
        }
    }

    public static class DbFilesThirdpartyIdMappingExtension
    {
        public static ModelBuilderWrapper AddDbFilesThirdpartyIdMapping(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddDbFilesThirdpartyIdMapping, Provider.MySql)
                .Add(PgSqlAddDbFilesThirdpartyIdMapping, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddDbFilesThirdpartyIdMapping(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbFilesThirdpartyIdMapping>(entity =>
            {
                entity.HasKey(e => e.HashId)
                    .HasName("PRIMARY");

                entity.ToTable("files_thirdparty_id_mapping");

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
}
