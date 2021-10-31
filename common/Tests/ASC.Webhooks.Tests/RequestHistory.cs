using ASC.Common;

namespace ASC.Webhooks.Tests
{
    [Singletone]
    public class RequestHistory
    {
        public int FailedCounter { get; set; }
        public int SuccessCounter { get; set; }
        public bool СorrectSignature { get; set; }
    }
}
