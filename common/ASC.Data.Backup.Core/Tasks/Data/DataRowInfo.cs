namespace ASC.Data.Backup.Tasks.Data;

public class DataRowInfo
{
    public string TableName { get; private set; }
    public IReadOnlyCollection<string> ColumnNames => _columnNames.AsReadOnly();

    public object this[int index] => _values[index];
    public object this[string columnName] => _values[GetIndex(columnName)];

    private readonly List<string> _columnNames = new List<string>();
    private readonly List<object> _values = new List<object>();

    public DataRowInfo(string tableName)
    {
        TableName = tableName;
    }

    public void SetValue(string columnName, object item)
    {
        var index = GetIndex(columnName);
        if (index == -1)
        {
            _columnNames.Add(columnName);
            _values.Add(item);
        }
        else
        {
            _values[index] = item;
        }
    }

    public override string ToString()
    {
        const int maxStrLength = 150;

        var sb = new StringBuilder(maxStrLength);

        var i = 0;
        while (i < _values.Count && sb.Length <= maxStrLength)
        {
            var strVal = Convert.ToString(_values[i]);
                sb.Append($"\"{strVal}\", ");
            i++;
        }

        if (sb.Length > maxStrLength + 2)
        {
            sb.Length = maxStrLength - 3;
            sb.Append("...");
        }
        else if (sb.Length > 0)
        {
            sb.Length -= 2;
        }

        return sb.ToString();
    }

    private int GetIndex(string columnName)
    {
        return _columnNames.FindIndex(name => name.Equals(columnName, StringComparison.OrdinalIgnoreCase));
    }
}
