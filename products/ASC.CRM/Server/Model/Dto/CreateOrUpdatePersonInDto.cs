using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

using ASC.Api.Collections;
using ASC.CRM.Core.Enums;

using Microsoft.AspNetCore.Http;

namespace ASC.CRM.Model
{   
    public class CreateOrUpdateContactInDto
    {
        public string About { get; set; }

        public ShareType ShareType { get; set; }

        public IEnumerable<Guid> ManagerList { get; set; }

        public IEnumerable<ItemKeyValuePair<int, string>> CustomFieldList { get; set; }

        public IEnumerable<IFormFile> Photos { get; set; }
    }

    public class CreateOrUpdateCompanyInDto : CreateOrUpdateContactInDto
    {
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public String CompanyName { get; set; }

        public IEnumerable<int> PersonList { get; set; }

    }


    public class CreateOrUpdatePersonInDto : CreateOrUpdateContactInDto
    {
        public String FirstName { get; set; }

        public String LastName { get; set; }

        public String JobTitle { get; set; }

        public int CompanyId { get; set; }
    }
}


