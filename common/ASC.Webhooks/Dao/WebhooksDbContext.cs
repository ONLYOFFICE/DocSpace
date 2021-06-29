using ASC.Core.Common.EF;
using ASC.Webhooks.Dao.Models;

using Microsoft.EntityFrameworkCore;

#nullable disable

namespace ASC.Webhooks.Dao
{
    public partial class WebhooksDbContext : BaseDbContext
    {
        public WebhooksDbContext()
        {
        }

        public WebhooksDbContext(DbContextOptions<WebhooksDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<WebhooksConfig> WebhooksConfigs { get; set; }
        public virtual DbSet<WebhooksPayload> WebhooksPayloads { get; set; }

//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            if (!optionsBuilder.IsConfigured)
//            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
//                optionsBuilder.UseMySQL("server=localhost;port=3306;database=onlyoffice;uid=root;password=onlyoffice");
//            }
//        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WebhooksConfig>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.Uri })
                    .HasName("PRIMARY");

                entity.ToTable("webhooks_config");

                entity.Property(e => e.TenantId).HasColumnType("int unsigned");

                entity.Property(e => e.Uri)
                    .HasMaxLength(50)
                    .HasColumnName("URI")
                    .HasDefaultValueSql("''");
            });

            modelBuilder.Entity<WebhooksPayload>(entity =>
            {
                entity.HasKey(e => new { e.Id })
                   .HasName("PRIMARY");

                entity.ToTable("webhooks_payload");

                entity.Property(e => e.Id)
                    .HasColumnType("int")
                    .HasColumnName("ID")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Data)
                    .IsRequired()
                    .HasColumnType("json");

                entity.Property(e => e.Event)
                    .HasColumnType("varchar")
                    .HasColumnName("EventID")
                    .HasMaxLength(50);

                entity.Property(e => e.TenantId)
                    .HasColumnType("int unsigned")
                    .HasColumnName("TenantID");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
