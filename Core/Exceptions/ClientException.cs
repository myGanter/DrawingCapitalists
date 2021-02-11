using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Exceptions
{
    public class ClientException : Exception
    {
        public ClientException(string exStr) : base(exStr) { }

        public ClientException(string exStr, Exception innerException) : base(exStr, innerException) { }
    }
}
