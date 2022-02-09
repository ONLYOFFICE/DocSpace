﻿// <auto-generated />

namespace ASC.Core.Common.Migrations.MySql.TelegramDbContextMySql;

[DbContext(typeof(MySqlTelegramDbContext))]
partial class MySqlTelegramDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("Relational:MaxIdentifierLength", 64)
            .HasAnnotation("ProductVersion", "5.0.10");

        modelBuilder.Entity("ASC.Core.Common.EF.Model.TelegramUser", b =>
            {
                b.Property<int>("TenantId")
                    .HasColumnType("int")
                    .HasColumnName("tenant_id");

                b.Property<string>("PortalUserId")
                    .HasColumnType("varchar(38)")
                    .HasColumnName("portal_user_id")
                    .UseCollation("utf8_general_ci")
                    .HasAnnotation("MySql:CharSet", "utf8");

                b.Property<int>("TelegramUserId")
                    .HasColumnType("int")
                    .HasColumnName("telegram_user_id");

                b.HasKey("TenantId", "PortalUserId")
                    .HasName("PRIMARY");

                b.HasIndex("TelegramUserId")
                    .HasDatabaseName("tgId");

                b.ToTable("telegram_users");
            });
#pragma warning restore 612, 618
    }
}
