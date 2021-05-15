using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Model.Resource
{
    public class ResAuthorsLang
    {
        public string AuthorLogin { get; set; }
        public string CultureTitle { get; set; }
    }

    public static class ResAuthorsLangExtension
    {
        public static ModelBuilderWrapper AddResAuthorsLang(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddResAuthorsLang, Provider.MySql)
                .Add(PgSqlAddResAuthorsLang, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddResAuthorsLang(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ResAuthorsLang>(entity =>
            {
                entity.HasKey(e => new { e.AuthorLogin, e.CultureTitle })
                    .HasName("PRIMARY");

                entity.ToTable("res_authorslang");

                entity.HasIndex(e => e.CultureTitle)
                    .HasDatabaseName("res_authorslang_FK2");

                entity.Property(e => e.AuthorLogin)
                    .HasColumnName("authorLogin")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.CultureTitle)
                    .HasColumnName("cultureTitle")
                    .HasColumnType("varchar(20)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });
        }
        public static void PgSqlAddResAuthorsLang(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ResAuthorsLang>(entity =>
            {
                entity.HasKey(e => new { e.AuthorLogin, e.CultureTitle })
                    .HasName("res_authorslang_pkey");

                entity.ToTable("res_authorslang", "onlyoffice");

                entity.HasIndex(e => e.CultureTitle)
                    .HasDatabaseName("res_authorslang_FK2");

                entity.Property(e => e.AuthorLogin)
                    .HasColumnName("authorLogin")
                    .HasMaxLength(50);

                entity.Property(e => e.CultureTitle)
                    .HasColumnName("cultureTitle")
                    .HasMaxLength(50);
            });
        }
    }
}
