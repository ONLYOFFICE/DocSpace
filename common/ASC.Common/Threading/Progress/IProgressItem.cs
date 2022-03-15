namespace ASC.Common.Threading.Progress;

public interface IProgressItem : ICloneable
{
    string Id { get; }
    DistributedTaskStatus Status { get; set; }
    object Error { get; set; }
    double Percentage { get; set; }
    bool IsCompleted { get; set; }

    void RunJob();
}
