using System;
using System.Threading.Tasks;

namespace ASC.Common.Threading
{
    [Transient]
    public class DistributedTaskProgress : DistributedTask
    {
        public double Percentage
        {
            get
            {
                return Math.Min(100.0, Math.Max(0, DistributedTaskCache.Percentage));
            }
            set
            {
                DistributedTaskCache.Percentage = value;
            }
        }

        public bool IsCompleted
        {
            get
            {
                return DistributedTaskCache.IsCompleted;
            }
            set
            {
                DistributedTaskCache.IsCompleted = value;
            }
        }

        protected int StepCount
        {
            get
            {
                return DistributedTaskCache.StepCount;
            }
            set
            {
                DistributedTaskCache.StepCount = value;
            }
        }

        protected void StepDone()
        {
            if (StepCount > 0)
            {
                Percentage += 100.0 / StepCount;
            }

            PublishChanges();
        }

        public void RunJob()
        {
            Percentage = 0;
            Status = DistributedTaskStatus.Running;
            DoJob();
        }

        protected virtual void DoJob()
        {

        }
    }
}
