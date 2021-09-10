using System;

namespace ASC.Mail.Core.Exceptions
{
    public class ProcessedBoxesException : Exception
    {
        public ProcessedBoxesException(string message)
            : base(message)
        {
        }
    }
}
