using System;
using System.Collections.Generic;

using ASC.Api.Collections;
using ASC.CRM.Core.Enums;

using Microsoft.AspNetCore.Http;

namespace ASC.CRM.ApiModels
{
    public class CreateOrUpdateContactRequestDto
    {
        public string About { get; set; }
        public ShareType ShareType { get; set; }
        public IEnumerable<Guid> ManagerList { get; set; }
        public IEnumerable<ItemKeyValuePair<int, string>> CustomFieldList { get; set; }
        public IEnumerable<IFormFile> Photos { get; set; }
    }

    public class CreateOrUpdateCompanyRequestDto : CreateOrUpdateContactRequestDto
    {
        public String CompanyName { get; set; }

        public IEnumerable<int> PersonList { get; set; }

    }

    public class CreateOrUpdatePersonRequestDto : CreateOrUpdateContactRequestDto
    {
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String JobTitle { get; set; }
        public int CompanyId { get; set; }
    }
}


