namespace ASC.Data.Backup;

public interface IDataWriteOperator : IDisposable
{
    void WriteEntry(string key, Stream stream);
}

public interface IDataReadOperator : IDisposable
{
    Stream GetEntry(string key);
    IEnumerable<string> GetEntries(string key);
}
