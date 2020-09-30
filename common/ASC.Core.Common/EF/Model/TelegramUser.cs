using System;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Model
{
    [Table("telegram_users")]
    public class TelegramUser : BaseEntity
    {
        [Column("portal_user_id")]
        public Guid PortalUserId { get; set; }

        [Column("tenant_id")]
        public int TenantId { get; set; }

        [Column("telegram_user_id")]
        public int TelegramUserId { get; set; }

        public override object[] GetKeys()
        {
            return new object[] { TenantId, PortalUserId };
        }
    }

    public static class TelegramUsersExtension
    {
        public static ModelBuilder AddTelegramUsers(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<TelegramUser>()
                .HasKey(c => new { c.TenantId, c.PortalUserId });

            return modelBuilder;
        }
    }
}
