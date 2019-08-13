using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Text;

namespace ASC.Core.Common.Security
{
    public class BruteForceCredentialException : InvalidCredentialException
    {
        public BruteForceCredentialException()
        {
        }

        public BruteForceCredentialException(string message) : base(message)
        {
        }
    }
}
