// <auto-generated />
using System;
using ASC.Core.Common.EF.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ASC.Core.Common.Migrations.PostgreSql.NotifyDbContextPostgreSql
{
    [DbContext(typeof(PostgreSqlNotifyDbContext))]
    [Migration("20211012145337_NotifyDbContextPostgreSql")]
    partial class NotifyDbContextPostgreSql
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.10");

            modelBuilder.Entity("ASC.Core.Common.EF.Model.NotifyInfo", b =>
                {
                    b.Property<int>("NotifyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("notify_id");

                    b.Property<int>("Attempts")
                        .HasColumnType("int")
                        .HasColumnName("attempts");

                    b.Property<DateTime>("ModifyDate")
                        .HasColumnType("datetime")
                        .HasColumnName("modify_date");

                    b.Property<int>("Priority")
                        .HasColumnType("int")
                        .HasColumnName("priority");

                    b.Property<int>("State")
                        .HasColumnType("int")
                        .HasColumnName("state");

                    b.HasKey("NotifyId")
                        .HasName("PRIMARY");

                    b.HasIndex("State")
                        .HasDatabaseName("state");

                    b.ToTable("notify_info");
                });

            modelBuilder.Entity("ASC.Core.Common.EF.Model.NotifyQueue", b =>
                {
                    b.Property<int>("NotifyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("notify_id");

                    b.Property<string>("Attachments")
                        .HasColumnType("text")
                        .HasColumnName("attachments")
                        .UseCollation("utf8_general_ci")
                        .HasAnnotation("MySql:CharSet", "utf8");

                    b.Property<string>("AutoSubmitted")
                        .HasColumnType("varchar(64)")
                        .HasColumnName("auto_submitted")
                        .UseCollation("utf8_general_ci")
                        .HasAnnotation("MySql:CharSet", "utf8");

                    b.Property<string>("Content")
                        .HasColumnType("text")
                        .HasColumnName("content")
                        .UseCollation("utf8_general_ci")
                        .HasAnnotation("MySql:CharSet", "utf8");

                    b.Property<string>("ContentType")
                        .HasColumnType("varchar(64)")
                        .HasColumnName("content_type")
                        .UseCollation("utf8_general_ci")
                        .HasAnnotation("MySql:CharSet", "utf8");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime")
                        .HasColumnName("creation_date");

                    b.Property<string>("Reciever")
                        .HasColumnType("varchar(255)")
                        .HasColumnName("reciever")
                        .UseCollation("utf8_general_ci")
                        .HasAnnotation("MySql:CharSet", "utf8");

                    b.Property<string>("ReplyTo")
                        .HasColumnType("varchar(1024)")
                        .HasColumnName("reply_to")
                        .UseCollation("utf8_general_ci")
                        .HasAnnotation("MySql:CharSet", "utf8");

                    b.Property<string>("Sender")
                        .HasColumnType("varchar(255)")
                        .HasColumnName("sender")
                        .UseCollation("utf8_general_ci")
                        .HasAnnotation("MySql:CharSet", "utf8");

                    b.Property<string>("SenderType")
                        .HasColumnType("varchar(64)")
                        .HasColumnName("sender_type")
                        .UseCollation("utf8_general_ci")
                        .HasAnnotation("MySql:CharSet", "utf8");

                    b.Property<string>("Subject")
                        .HasColumnType("varchar(1024)")
                        .HasColumnName("subject")
                        .UseCollation("utf8_general_ci")
                        .HasAnnotation("MySql:CharSet", "utf8");

                    b.Property<int>("TenantId")
                        .HasColumnType("int")
                        .HasColumnName("tenant_id");

                    b.HasKey("NotifyId")
                        .HasName("PRIMARY");

                    b.ToTable("notify_queue");
                });
#pragma warning restore 612, 618
        }
    }
}
