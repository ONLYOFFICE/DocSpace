using System;
using System.Collections.Generic;

using ASC.Api.Core;
using ASC.CRM.Core.Enums;

namespace ASC.CRM.ApiModels
{
    public class SetAccessToBatchDealByFilterRequestDto
    {
        public Guid Responsibleid { get; set; }
        public int OpportunityStagesid { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public int Contactid { get; set; }
        public DealMilestoneStatus? StageType { get; set; }
        public bool? ContactAlsoIsParticipant { get; set; }
        public ApiDateTime FromDate { get; set; }
        public ApiDateTime ToDate { get; set; }
        public bool isPrivate { get; set; }
        public IEnumerable<Guid> AccessList { get; set; }
    }

    public class SetAccessToBatchDealRequestDto
    {
        public IEnumerable<int> Opportunityid { get; set; }
        public bool isPrivate { get; set; }
        public IEnumerable<Guid> AccessList { get; set; }
    }
}
