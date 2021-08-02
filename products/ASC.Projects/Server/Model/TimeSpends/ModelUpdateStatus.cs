
using ASC.Projects.Core.Domain;

namespace ASC.Projects.Model.TimeSpends
{
    public class ModelUpdateStatus
    {
        public int[] TimeIds { get; set; }
        public PaymentStatus Status { get; set; }
    }
}
