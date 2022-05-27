using System;

using ASC.CRM.Core.Enums;

namespace ASC.CRM.ApiModels
{
    public class ReportDataRequestDto
    {
       public ReportType Type { get; set; }
       public ReportTimePeriod TimePeriod { get; set; }
       public Guid[] Managers { get; set; }

    }
}
