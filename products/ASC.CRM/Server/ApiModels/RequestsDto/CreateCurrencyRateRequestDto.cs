using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace ASC.CRM.ApiModels
{
    public class CreateCurrencyRateRequestDto
    {
       public string FromCurrency { get; set; }
       public string ToCurrency { get; set; }
       public decimal Rate { get; set; }
    }
}
