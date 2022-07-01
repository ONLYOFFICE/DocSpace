// <auto-generated />
using System;
using ASC.Core.Common.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ASC.Core.Common.Migrations.PostgreSql.IntegrationEventLogContextPostgreSql
{
    [DbContext(typeof(PostgreSqlIntegrationEventLogContext))]
    partial class PostgreSqlIntegrationEventLogContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("ASC.Core.Common.Hosting.InstanceRegistration", b =>
                {
                    b.Property<string>("InstanceRegistrationId")
                        .HasColumnType("varchar(255)")
                        .HasColumnName("instance_registration_id")
                        .UseCollation("utf8_general_ci")
                        .HasAnnotation("MySql:CharSet", "utf8");

                    b.Property<sbyte>("IsActive")
                        .HasColumnType("tinyint(4)")
                        .HasColumnName("is_active");

                    b.Property<DateTime?>("LastUpdated")
                        .HasColumnType("datetime")
                        .HasColumnName("last_updated");

                    b.Property<string>("WorkerTypeName")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasColumnName("worker_type_name")
                        .UseCollation("utf8_general_ci")
                        .HasAnnotation("MySql:CharSet", "utf8");

                    b.HasKey("InstanceRegistrationId")
                        .HasName("PRIMARY");

                    b.HasIndex("WorkerTypeName")
                        .HasDatabaseName("worker_type_name");

                    b.ToTable("hosting_instance_registration", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
