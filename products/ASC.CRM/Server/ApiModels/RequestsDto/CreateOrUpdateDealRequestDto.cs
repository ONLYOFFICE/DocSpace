using System;
using System.Collections.Generic;

using ASC.Api.Collections;
using ASC.Api.Core;
using ASC.CRM.Core.Enums;

namespace ASC.CRM.ApiModels
{
    public class CreateOrUpdateDealRequestDto
    {
        public int Contactid { get; set; }
        public IEnumerable<int> Members { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid Responsibleid { get; set; }
        public BidType BidType { get; set; }
        public decimal BidValue { get; set; }
        public string BidCurrencyAbbr { get; set; }
        public int PerPeriodValue { get; set; }
        public int Stageid { get; set; }
        public int SuccessProbability { get; set; }
        public ApiDateTime ActualCloseDate { get; set; }
        public ApiDateTime ExpectedCloseDate { get; set; }
        public IEnumerable<ItemKeyValuePair<int, string>> CustomFieldList { get; set; }
        public bool isPrivate { get; set; }
        public IEnumerable<Guid> AccessList { get; set; }
        public bool isNotify { get; set; }
    }
}
