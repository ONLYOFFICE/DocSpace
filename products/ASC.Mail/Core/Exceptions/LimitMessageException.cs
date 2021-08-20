using System;

namespace ASC.Mail.Core.Exceptions
{
    public class LimitMessageException : Exception
    {
        public LimitMessageException(string message)
            : base(message) { }
    }
}
