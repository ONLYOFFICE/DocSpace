using System;

using ASC.Api.Core;

namespace ASC.CRM.ApiModels
{
    public class CreateOrUpdateTaskRequestDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public ApiDateTime Deadline { get; set; }
        public Guid ResponsibleId { get; set; }
        public int CategoryId { get; set; }
        public int ContactId { get; set; }
        public String EntityType { get; set; }
        public int EntityId { get; set; }
        public bool isNotify { get; set; }
        public int AlertValue { get; set; }
    }
}
