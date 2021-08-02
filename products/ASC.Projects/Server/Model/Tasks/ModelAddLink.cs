
using ASC.Projects.Core.Domain;

namespace ASC.Projects.Model.Tasks
{
    public class ModelAddLink
    {
        public int DependenceTaskId { get; set; }
        public TaskLinkType LinkType { get; set; }
    }
}
