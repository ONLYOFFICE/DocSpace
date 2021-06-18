using System;
using System.Collections.Generic;

using ASC.Api.Core;

namespace ASC.CRM.ApiModels
{
    public class AddHistoryToRequestDto
    {
        public String EntityType { get; set; }
        public int EntityId { get; set; }
        public int ContactId { get; set; }
        public string Content { get; set; }
        public int CategoryId { get; set; }
        public ApiDateTime Created { get; set; }
        public IEnumerable<int> FileId { get; set; }
        public IEnumerable<Guid> NotifyUserList { get; set; }
    }
}
