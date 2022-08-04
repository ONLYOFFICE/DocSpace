// <auto-generated />
using System;
using ASC.Core.Common.EF.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ASC.Migrations.PostgreSql.Migrations
{
    [DbContext(typeof(TelegramDbContext))]
    partial class TelegramDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "6.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("ASC.Core.Common.EF.Model.TelegramUser", b =>
                {
                    b.Property<int>("TenantId")
                        .HasColumnType("integer")
                        .HasColumnName("tenant_id");

                    b.Property<Guid>("PortalUserId")
                        .HasMaxLength(38)
                        .HasColumnType("uuid")
                        .HasColumnName("portal_user_id");

                    b.Property<long>("TelegramUserId")
                        .HasColumnType("bigint")
                        .HasColumnName("telegram_user_id");

                    b.HasKey("TenantId", "PortalUserId")
                        .HasName("telegram_users_pkey");

                    b.HasIndex("TelegramUserId")
                        .HasDatabaseName("tgId");

                    b.ToTable("telegram_users", "onlyoffice");
                });
#pragma warning restore 612, 618
        }
    }
}
