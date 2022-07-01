// <auto-generated />
using System;
using ASC.Core.Common.EF.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ASC.Core.Common.Migrations.PostgreSql.AccountLinkContextPostgreSql
{
    [DbContext(typeof(PostgreSqlAccountLinkContext))]
    partial class PostgreSqlAccountLinkContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("ASC.Core.Common.EF.Model.AccountLinks", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(200)")
                        .HasColumnName("id")
                        .UseCollation("utf8_general_ci")
                        .HasAnnotation("MySql:CharSet", "utf8");

                    b.Property<string>("UId")
                        .HasColumnType("varchar(200)")
                        .HasColumnName("uid")
                        .UseCollation("utf8_general_ci")
                        .HasAnnotation("MySql:CharSet", "utf8");

                    b.Property<DateTime>("Linked")
                        .HasColumnType("datetime")
                        .HasColumnName("linked");

                    b.Property<string>("Profile")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("profile")
                        .UseCollation("utf8_general_ci")
                        .HasAnnotation("MySql:CharSet", "utf8");

                    b.Property<string>("Provider")
                        .HasColumnType("char(60)")
                        .HasColumnName("provider")
                        .UseCollation("utf8_general_ci")
                        .HasAnnotation("MySql:CharSet", "utf8");

                    b.HasKey("Id", "UId")
                        .HasName("PRIMARY");

                    b.HasIndex("UId")
                        .HasDatabaseName("uid");

                    b.ToTable("account_links", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
