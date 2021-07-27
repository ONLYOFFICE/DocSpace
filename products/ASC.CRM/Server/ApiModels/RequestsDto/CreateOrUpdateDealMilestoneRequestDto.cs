using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ASC.CRM.Core.Enums;

using Microsoft.AspNetCore.Mvc;

namespace ASC.CRM.ApiModels
{
    public class CreateOrUpdateDealMilestoneRequestDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public int SuccessProbability { get; set; }
        public DealMilestoneStatus StageType { get; set; }
    }
}
