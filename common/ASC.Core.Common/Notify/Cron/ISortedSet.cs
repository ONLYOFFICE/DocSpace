namespace ASC.Notify.Cron;

public interface ISortedSet : ISet
{
    ISortedSet TailSet(object limit);
}
