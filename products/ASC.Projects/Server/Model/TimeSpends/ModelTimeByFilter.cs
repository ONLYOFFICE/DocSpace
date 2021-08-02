using System;

using ASC.Api.Core;
using ASC.Projects.Core.Domain;

namespace ASC.Projects.Model.TimeSpends
{
    public class ModelTimeByFilter
    {
        public int Projectid { get; set; }
        public bool MyProjects { get; set; }
        public int? Milestone { get; set; }
        public bool MyMilestones { get; set; }
        public int Tag { get; set; }
        public Guid Departament { get; set; }
        public Guid Participant { get; set; }
        public ApiDateTime CreatedStart { get; set; }
        public ApiDateTime CreatedStop { get; set; }
        public int LastId { get; set; }
        public PaymentStatus? Status { get; set; }
    }
}
