
using ASC.Common;
using ASC.Common.Logging;

namespace ASC.Mail.Aggregator.CollectionService.Service
{
    [Singletone]
    public class AggregatorLogger
    {
        public ILog Log { get; private set; }

        public AggregatorLogger()
        {

        }

        internal void SetLog(ILog log)
        {
            Log = log;
        }

        public void Info(string message, int id)
        {
            Log.MboxInfo(message, id);
        }

    }

    public static class NlogAggregatorExtension
    {
        /// <summary>
        /// Info level for specifical message
        /// </summary>
        /// <param name="message">hm...</param>
        /// <param name="id">mailbox id</param>
        public static void MboxInfo(this ILog log, string message, int id)
        {
            log.Info($"Mbox_{id} - {message}");
        }
    }
}
