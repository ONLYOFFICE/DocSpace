using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace ASC.CRM.ApiModels
{
    public class CreateListItemCategoryRequestDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageName { get; set; }
        public int SortOrder { get; set; }
    }
}
