namespace ASC.Common.Threading;
using System.Threading.Tasks;

[Transient]
public class DistributedTaskProgress : DistributedTask
{
    public double Percentage
    {
        get => Math.Min(100.0, Math.Max(0, DistributedTaskCache.Percentage));
        set => DistributedTaskCache.Percentage = value;
    }
    public bool IsCompleted
    {
        get => DistributedTaskCache.IsCompleted;
        set => DistributedTaskCache.IsCompleted = value;
    }
    protected int StepCount
    {
        get => DistributedTaskCache.StepCount;
        set => DistributedTaskCache.StepCount = value;
    }

    public void RunJob()
    {
        Percentage = 0;
        Status = DistributedTaskStatus.Running;

        DoJob();
    }

    protected virtual void DoJob() { }

    protected void StepDone()
    {
        if (StepCount > 0)
        {
            Percentage += 100.0 / StepCount;
        }

        PublishChanges();
    }
}