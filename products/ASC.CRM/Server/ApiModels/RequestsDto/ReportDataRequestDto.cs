using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ASC.CRM.Core.Enums;
using Microsoft.AspNetCore.Mvc;

namespace ASC.CRM.ApiModels
{
    public class ReportDataRequestDto
    {
       public ReportType Type { get; set; }
       public ReportTimePeriod TimePeriod { get; set; }
       public Guid[] Managers { get; set; }

    }
}
