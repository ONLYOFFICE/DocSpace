using System;

using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.Hosting.Extensions;
internal static class InstanceRegistrationExtension
{
    public static ModelBuilderWrapper AddInstanceRegistration(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder
            .Add(MySqlAddInstanceRegistration, Provider.MySql)
            .Add(PgSqlAddInstanceRegistration, Provider.PostgreSql);

        return modelBuilder;
    }
    public static void MySqlAddInstanceRegistration(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InstanceRegistration>(entity =>
        {
            entity.ToTable("hosting_instance_registration");

            entity.HasKey(e => e.InstanceRegistrationId)
                  .HasName("PRIMARY");

            entity.HasIndex(e => e.WorkerTypeName)
                  .HasDatabaseName("worker_type_name");

            entity.Property(e => e.WorkerTypeName)
                  .HasColumnName("worker_type_name")
                  .HasColumnType("varchar(255)")
                  .HasCharSet("utf8")
                  .UseCollation("utf8_general_ci")
                  .IsRequired();

            entity.Property(e => e.IsActive)
                  .HasColumnName("is_active")
                  .HasColumnType("tinyint(4)")
                  .IsRequired();

            entity.Property(e => e.InstanceRegistrationId)
                  .HasColumnName("instance_registration_id")
                  .HasColumnType("varchar(255)")
                  .HasCharSet("utf8")
                  .UseCollation("utf8_general_ci")
                  .IsRequired();

            entity.Property(e => e.LastUpdated)
                  .HasColumnName("last_updated")
                  .HasColumnType("datetime");        
        });
    }

    public static void PgSqlAddInstanceRegistration(this ModelBuilder modelBuilder)
    {
        throw new NotImplementedException();
    }
}