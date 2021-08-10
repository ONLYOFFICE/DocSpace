using System;

using ASC.Api.Core;

namespace ASC.Projects.Model.TimeSpends
{
    public class ModelAddTime
    {
        public string Note { get; set; }
        public DateTime Date { get; set; }
        public Guid PersonId { get; set; }
        public float Hours { get; set; }
        public int ProjectId { get; set; }
    }
}
