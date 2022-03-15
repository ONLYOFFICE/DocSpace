namespace ASC.Notify.Cron;

public interface ISet : IList
{
    bool AddAll(ICollection c);
    new bool Add(object obj);
    object First();
}
