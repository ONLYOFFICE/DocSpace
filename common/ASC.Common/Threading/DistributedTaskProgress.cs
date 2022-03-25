namespace ASC.Common.Threading;

[Transient]
[ProtoContract(IgnoreUnknownSubTypes = true)]
public class DistributedTaskProgress : DistributedTask
{
    [ProtoMember(1)]
    private double _percentage;

    public double Percentage
    {
        get => Math.Min(100.0, Math.Max(0, _percentage));
        set => _percentage = value;
    }

    [ProtoMember(2)]
    public bool IsCompleted { get; set; }

    [ProtoMember(3)]
    protected int StepCount { get; set; }

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