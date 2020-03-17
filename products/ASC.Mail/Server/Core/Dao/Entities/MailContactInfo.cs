﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using ASC.Core.Common.EF;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Mail.Core.Dao.Entities
{
    [Table("mail_contact_info")]
    public partial class MailContactInfo : BaseEntity
    {
        [Key]
        [Column("id", TypeName = "int(10) unsigned")]
        public uint Id { get; set; }
        [Column("tenant", TypeName = "int(11)")]
        public int Tenant { get; set; }
        [Required]
        [Column("id_user", TypeName = "varchar(255)")]
        public string IdUser { get; set; }
        [Column("id_contact", TypeName = "int(11) unsigned")]
        public uint IdContact { get; set; }
        [Required]
        [Column("data", TypeName = "varchar(255)")]
        public string Data { get; set; }
        [Column("type", TypeName = "int(11)")]
        public int Type { get; set; }
        [Column("is_primary")]
        public bool IsPrimary { get; set; }
        [Column("last_modified", TypeName = "timestamp")]
        public DateTime LastModified { get; set; }

        public override object[] GetKeys()
        {
            return new object[] { Id };
        }
    }
}