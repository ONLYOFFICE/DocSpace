using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Model.Resource
{
    [Table("res_authorsfile")]
    public class ResAuthorsFile
    {
        public string AuthorLogin { get; set; }
        public int FileId { get; set; }
        public bool WriteAccess { get; set; }
    }

    public static class ResAuthorsFileExtension
    {
        public static ModelBuilder MySqlAddResAuthorsFile(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ResAuthorsFile>(entity =>
            {
                entity.HasKey(e => new { e.AuthorLogin, e.FileId })
                    .HasName("PRIMARY");

                entity.ToTable("res_authorsfile");

                entity.HasIndex(e => e.FileId)
                    .HasName("res_authorsfile_FK2");

                entity.Property(e => e.AuthorLogin)
                    .HasColumnName("authorLogin")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.FileId).HasColumnName("fileid");

                entity.Property(e => e.WriteAccess).HasColumnName("writeAccess");
            });

            return modelBuilder;
        }
        public static ModelBuilder PgSqlAddResAuthorsFile(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ResAuthorsFile>(entity =>
            {
                entity.HasKey(e => new { e.AuthorLogin, e.FileId })
                    .HasName("res_authorsfile_pkey");

                entity.ToTable("res_authorsfile", "onlyoffice");

                entity.HasIndex(e => e.FileId)
                    .HasName("res_authorsfile_FK2");

                entity.Property(e => e.AuthorLogin)
                    .HasColumnName("authorLogin")
                    .HasMaxLength(50);

                entity.Property(e => e.FileId).HasColumnName("fileid");

                entity.Property(e => e.WriteAccess).HasColumnName("writeAccess");
            });

            return modelBuilder;
        }
    }
}
