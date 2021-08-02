using System;

namespace ASC.Projects.Model.TimeSpends
{
    public class ModelUpdateTime
    {
        public string Note { get; set; }
        public DateTime Date { get; set; }
        public Guid PersonId { get; set; }
        public float Hours { get; set; }
    }
}
