using System;

using ASC.Api.Core;

namespace ASC.CRM.ApiModels
{
    public class CreateTaskGroupRequestDto
    {
        public String Title { get; set; }
        public String Description { get; set; }
        public ApiDateTime Deadline { get; set; }
        public Guid ResponsibleId { get; set; }
        public int CategoryId { get; set; }
        public int[] ContactIds { get; set; }
        public string EntityType { get; set; }
        public int EntityId { get; set; }
        public bool isNotify { get; set; }
        public int AlertValue { get; set; }

    }
}
