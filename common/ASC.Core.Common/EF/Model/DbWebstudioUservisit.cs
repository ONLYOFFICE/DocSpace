using System;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Model
{
    [Table("webstudio_uservisit")]
    public class DbWebstudioUserVisit
    {
        public int TenantId { get; set; }
        public DateTime VisitDate { get; set; }
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }
        public int VisitCount { get; set; }
        public DateTime FirstVisitTime { get; set; }
        public DateTime LastVisitTime { get; set; }
    }

    public static class WebstudioUserVisitExtension
    {
        public static ModelBuilder AddWebstudioUserVisit(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbWebstudioUserVisit>()
                .HasKey(c => new { c.TenantId, c.VisitDate, c.ProductId, c.UserId });

            return modelBuilder;
        }
    }
}
