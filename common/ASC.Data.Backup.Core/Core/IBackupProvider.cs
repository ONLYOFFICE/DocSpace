namespace ASC.Data.Backup;

public interface IBackupProvider
{
    string Name { get; }
    event EventHandler<ProgressChangedEventArgs> ProgressChanged;

    IEnumerable<XElement> GetElements(int tenant, string[] configs, IDataWriteOperator writer);
    void LoadFrom(IEnumerable<XElement> elements, int tenant, string[] configs, IDataReadOperator reader);
}

public class ProgressChangedEventArgs : EventArgs
{
    public string Status { get; private set; }
    public double Progress { get; private set; }
    public bool Completed { get; private set; }

    public ProgressChangedEventArgs(string status, double progress)
        : this(status, progress, false) { }

    public ProgressChangedEventArgs(string status, double progress, bool completed)
    {
        Status = status;
        Progress = progress;
        Completed = completed;
    }
}
