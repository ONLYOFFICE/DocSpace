using System.Security.Authentication;

namespace ASC.Core.Common.Security
{
    public class BruteForceCredentialException : InvalidCredentialException
    {
        public BruteForceCredentialException() { }

        public BruteForceCredentialException(string message) : base(message) { }
    }
}
