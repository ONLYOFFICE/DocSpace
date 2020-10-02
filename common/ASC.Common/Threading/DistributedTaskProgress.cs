using System;

namespace ASC.Common.Threading
{
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
            try
            {
                Percentage = 0;
                Status = DistributedTaskStatus.Running;
                DoJob();
            }
            catch (AggregateException e)
            {
                Status = DistributedTaskStatus.Failted;
                Exception = e;
            }
            finally
            {
                Percentage = 100;
                IsCompleted = true;
                Status = DistributedTaskStatus.Completed;
                PublishChanges();
            }
        }

        protected virtual void DoJob()
        {

        }
    }
}
