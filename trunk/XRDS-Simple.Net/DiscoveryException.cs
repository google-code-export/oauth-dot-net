using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRDS_Simple.Net
{
    public class DiscoveryException : Exception
    {
        public DiscoveryException(string message) : base(message) { }
        public DiscoveryException(string message, Exception innerException) : base(message, innerException) { }

    }
}
