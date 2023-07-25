// <auto-generated />
using System;
using ASC.Feed.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ASC.Migrations.PostgreSql.Migrations
{
    [DbContext(typeof(FeedDbContext))]
    partial class FeedDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "6.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("ASC.Feed.Model.FeedAggregate", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(88)
                        .HasColumnType("character varying(88)")
                        .HasColumnName("id");

                    b.Property<DateTime>("AggregateDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("aggregated_date");

                    b.Property<Guid>("Author")
                        .HasMaxLength(38)
                        .HasColumnType("uuid")
                        .HasColumnName("author")
                        .IsFixedLength();

                    b.Property<string>("ContextId")
                        .HasColumnType("text")
                        .HasColumnName("context_id");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_date");

                    b.Property<string>("GroupId")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(70)
                        .HasColumnType("character varying(70)")
                        .HasColumnName("group_id")
                        .HasDefaultValueSql("NULL");

                    b.Property<string>("Json")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("json");

                    b.Property<string>("Keywords")
                        .HasColumnType("text")
                        .HasColumnName("keywords");

                    b.Property<Guid>("ModifiedBy")
                        .HasMaxLength(38)
                        .HasColumnType("uuid")
                        .HasColumnName("modified_by")
                        .IsFixedLength();

                    b.Property<DateTime>("ModifiedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("modified_date");

                    b.Property<string>("Module")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("module");

                    b.Property<string>("Product")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("product");

                    b.Property<int>("Tenant")
                        .HasColumnType("integer")
                        .HasColumnName("tenant");

                    b.HasKey("Id");

                    b.HasIndex("Tenant", "AggregateDate")
                        .HasDatabaseName("aggregated_date");

                    b.HasIndex("Tenant", "ModifiedDate")
                        .HasDatabaseName("modified_date");

                    b.HasIndex("Tenant", "Product")
                        .HasDatabaseName("product");

                    b.ToTable("feed_aggregate", "onlyoffice");
                });

            modelBuilder.Entity("ASC.Feed.Model.FeedLast", b =>
                {
                    b.Property<string>("LastKey")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)")
                        .HasColumnName("last_key");

                    b.Property<DateTime>("LastDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_date");

                    b.HasKey("LastKey")
                        .HasName("feed_last_pkey");

                    b.ToTable("feed_last", "onlyoffice");
                });

            modelBuilder.Entity("ASC.Feed.Model.FeedReaded", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasMaxLength(38)
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.Property<int>("Tenant")
                        .HasColumnType("integer")
                        .HasColumnName("tenant_id");

                    b.Property<string>("Module")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("module");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("timestamp");

                    b.HasKey("UserId", "Tenant", "Module")
                        .HasName("feed_readed_pkey");

                    b.ToTable("feed_readed", "onlyoffice");
                });

            modelBuilder.Entity("ASC.Feed.Model.FeedUsers", b =>
                {
                    b.Property<string>("FeedId")
                        .HasMaxLength(88)
                        .HasColumnType("character varying(88)")
                        .HasColumnName("feed_id");

                    b.Property<Guid>("UserId")
                        .HasMaxLength(38)
                        .HasColumnType("uuid")
                        .HasColumnName("user_id")
                        .IsFixedLength();

                    b.HasKey("FeedId", "UserId")
                        .HasName("feed_users_pkey");

                    b.HasIndex("UserId")
                        .HasDatabaseName("user_id_feed_users");

                    b.ToTable("feed_users", "onlyoffice");
                });
#pragma warning restore 612, 618
        }
    }
}
