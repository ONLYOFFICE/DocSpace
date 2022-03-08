using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ASC.Security.Cryptography;

namespace ASC.ActiveDirectory.Base
{
    public abstract class testClass
    {
        private InstanceCrypto InstanceCrypto { get; }

        protected testClass(
            InstanceCrypto instanceCrypto)
        {
            InstanceCrypto = instanceCrypto;
        }

        public static string Hello()
        {
            return InstanceCrypto.Encrypt("sdad");
        }

    }
}
