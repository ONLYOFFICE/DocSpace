using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASC.ActiveDirectory.ComplexOperations.Data
{
    public static class LdapTaskProperty
    {
        public static string OWNER = "LDAPOwner";
        public static string OPERATION_TYPE = "LDAPOperationType";
        public static string SOURCE = "LDAPSource";
        public static string PROGRESS = "LDAPProgress";
        public static string RESULT = "LDAPResult";
        public static string ERROR = "LDAPError";
        public static string WARNING = "LDAPWarning";
        public static string CERT_REQUEST = "LDAPCertRequest";
        public static string FINISHED = "LDAPFinished";
    }
}
