﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using ASC.Core.Common.EF;
using ASC.ElasticSearch;
using ASC.ElasticSearch.Core;

using Microsoft.EntityFrameworkCore;
using Nest;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace ASC.Mail.Core.Dao.Entities
{
    [ElasticsearchType(RelationName = Tables.ContactInfo)]
    [Table("mail_contact_info")]
    public partial class MailContactInfo : BaseEntity, ISearchItem
    {
        [Key]
        [Column("id", TypeName = "int(10) unsigned")]
        public int Id { get; set; }
        
        [Column("tenant", TypeName = "int(11)")]
        public int TenantId { get; set; }
        
        [Required]
        [Column("id_user", TypeName = "varchar(255)")]
        public string IdUser { get; set; }
        
        [Column("id_contact", TypeName = "int(11) unsigned")]
        public int IdContact { get; set; }
        
        [Required]
        [Column("data", TypeName = "varchar(255)")]
        public string Data { get; set; }
        
        [Column("type", TypeName = "int(11)")]
        public int Type { get; set; }
        
        [Column("is_primary")]
        public bool IsPrimary { get; set; }
        
        [Column("last_modified", TypeName = "timestamp")]
        public DateTime LastModified { get; set; }

        [Nested]
        [NotMapped]
        public MailContact Contact { get; set; }

        [NotMapped]
        [Ignore]
        public string IndexName {
            get { return Tables.ContactInfo; }
        }

        public Expression<Func<ISearchItem, object[]>> SearchContentFields
        {
            get => (a) => new[] { Data };
        }

        public override object[] GetKeys()
        {
            return new object[] { Id };
        }

        public Expression<Func<ISearchItem, object[]>> GetSearchContentFields(SearchSettingsHelper searchSettings)
        {
            throw new NotImplementedException();
        }
    }

    public static class MailContactInfoExtension
    {
        public static ModelBuilder AddMailContactInfo(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MailContactInfo>(entity =>
            {
                entity.HasIndex(e => e.IdContact)
                    .HasDatabaseName("contact_id");

                entity.HasIndex(e => e.LastModified)
                    .HasDatabaseName("last_modified");

                entity.HasIndex(e => new { e.TenantId, e.IdUser, e.Data })
                    .HasDatabaseName("tenant_id_user_data");

                entity.Property(e => e.Data)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.IdUser)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.LastModified)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.HasOne(a => a.Contact)
                    .WithMany(m => m.InfoList)
                    .HasForeignKey(a => a.IdContact);
            });

            return modelBuilder;
        }
    }
}