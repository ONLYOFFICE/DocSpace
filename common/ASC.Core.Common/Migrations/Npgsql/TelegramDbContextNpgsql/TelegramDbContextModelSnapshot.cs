﻿// <auto-generated />
using System;
using ASC.Core.Common.EF.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ASC.Core.Common.Migrations.Npgsql.TelegramDbContextNpgsql
{
    [DbContext(typeof(PostgreSqlTelegramDbContext))]
    partial class TelegramDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("ASC.Core.Common.EF.Model.TelegramUser", b =>
                {
                    b.Property<int>("TenantId")
                        .HasColumnName("tenant_id")
                        .HasColumnType("integer");

                    b.Property<Guid>("PortalUserId")
                        .HasColumnName("portal_user_id")
                        .HasColumnType("uuid")
                        .HasMaxLength(38);

                    b.Property<int>("TelegramUserId")
                        .HasColumnName("telegram_user_id")
                        .HasColumnType("integer");

                    b.HasKey("TenantId", "PortalUserId")
                        .HasName("telegram_users_pkey");

                    b.HasIndex("TelegramUserId")
                        .HasName("tgId");

                    b.ToTable("telegram_users","onlyoffice");
                });
#pragma warning restore 612, 618
        }
    }
}
