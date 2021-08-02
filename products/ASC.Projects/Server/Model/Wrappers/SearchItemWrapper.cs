
using ASC.Api.Core;
using ASC.Projects.Core.Domain;

namespace ASC.Api.Projects.Wrappers
{
    public class SearchItemWrapper
    {
        public string Id { get; set; }
        public EntityType EntityType { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ApiDateTime Created { get; set; }

    }
}
