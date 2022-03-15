namespace ASC.Data.Backup;

public static class ActionInvoker
{
    public static void Try(
        Action action,
        int maxAttempts,
        Action<Exception> onFailure = null,
        Action<Exception> onAttemptFailure = null,
        int sleepMs = 1000,
        bool isSleepExponential = true)
    {
        Try(state => action(), null, maxAttempts, onFailure, onAttemptFailure, sleepMs, isSleepExponential);
    }

    public static void Try(
        Action<object> action,
        object state,
        int maxAttempts,
        Action<Exception> onFailure = null,
        Action<Exception> onAttemptFailure = null,
        int sleepMs = 1000,
        bool isSleepExponential = true)
    {
        ArgumentNullException.ThrowIfNull(action);

        var countAttempts = 0;
        while (countAttempts++ < maxAttempts)
        {
            try
            {
                action(state);
                return;
            }
            catch (Exception error)
            {
                if (countAttempts < maxAttempts)
                {
                    onAttemptFailure?.Invoke(error);

                    if (sleepMs > 0)
                    {
                        Thread.Sleep(isSleepExponential ? sleepMs * countAttempts : sleepMs);
                    }
                }
                else
                {
                    onFailure?.Invoke(error);
                }
            }
        }
    }
}
