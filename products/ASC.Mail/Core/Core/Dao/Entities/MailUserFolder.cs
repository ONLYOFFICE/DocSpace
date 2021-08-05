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
    [ElasticsearchType(RelationName = Tables.UserFolder)]
    [Table("mail_user_folder")]
    public partial class MailUserFolder : BaseEntity, ISearchItem
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }

        [Column("parent_id", TypeName = "int(11)")]
        public int ParentId { get; set; }
        
        [Column("tenant", TypeName = "int(11)")]
        public int TenantId { get; set; }
        
        [Required]
        [Column("id_user", TypeName = "varchar(38)")]
        public string IdUser { get; set; }
        
        [Required]
        [Column("name", TypeName = "varchar(400)")]
        public string Name { get; set; }
        
        [Column("folders_count", TypeName = "int(11) unsigned")]
        public uint FoldersCount { get; set; }
        
        [Column("unread_messages_count", TypeName = "int(11) unsigned")]
        public uint UnreadMessagesCount { get; set; }
        
        [Column("total_messages_count", TypeName = "int(11) unsigned")]
        public uint TotalMessagesCount { get; set; }
        
        [Column("unread_conversations_count", TypeName = "int(11) unsigned")]
        public uint UnreadConversationsCount { get; set; }
        
        [Column("total_conversations_count", TypeName = "int(11) unsigned")]
        public uint TotalConversationsCount { get; set; }
        
        [Column("modified_on", TypeName = "timestamp")]
        public DateTime ModifiedOn { get; set; }

        [NotMapped]
        [Ignore]
        public string IndexName
        {
            get => Tables.UserFolder;
        }

        public Expression<Func<ISearchItem, object[]>> SearchContentFields
        {
            get => (a) => new[] { Name };
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

    public static class MailUserFolderExtension
    {
        public static ModelBuilder AddMailUserFolder(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MailUserFolder>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.IdUser, e.ParentId })
                    .HasDatabaseName("tenant_user_parent");

                entity.Property(e => e.IdUser)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ModifiedOn)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.Name)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });

            return modelBuilder;
        }
    }
}