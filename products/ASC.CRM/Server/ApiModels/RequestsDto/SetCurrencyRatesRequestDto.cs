using System;
using System.Collections.Generic;

using ASC.CRM.Core;

namespace ASC.CRM.ApiModels
{
    public class SetCurrencyRatesRequestDto
    {
        public String Currency { get; set; }
        public List<CurrencyRate> Rates { get; set; }
    }
}
