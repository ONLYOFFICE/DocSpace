using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ASC.CRM.Core;
using Microsoft.AspNetCore.Mvc;

namespace ASC.CRM.ApiModels
{
    public class SetCurrencyRatesRequestDto
    {
        public String Currency { get; set; }
        public List<CurrencyRate> Rates { get; set; }
    }
}
