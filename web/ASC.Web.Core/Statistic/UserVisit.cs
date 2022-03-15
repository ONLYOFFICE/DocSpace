namespace ASC.Web.Studio.Core.Statistic
{
    public class UserVisit
    {
        public virtual int TenantID { get; set; }

        public virtual DateTime VisitDate { get; set; }

        public virtual DateTime? FirstVisitTime { get; set; }

        public virtual DateTime? LastVisitTime { get; set; }

        public virtual Guid UserID { get; set; }

        public virtual Guid ProductID { get; set; }

        public virtual int VisitCount { get; set; }
    }
}