

using ASC.Projects.Core.Domain;

namespace ASC.Projects.Model.Tasks
{
    public class ModelUpdateTasks
    {
        public int[] TaskIds { get; set; }
        public TaskStatus Status { get; set; }
        public int StatusId { get; set; }
    }
}
