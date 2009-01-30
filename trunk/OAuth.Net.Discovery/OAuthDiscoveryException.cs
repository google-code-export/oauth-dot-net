using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OAuth.Net.Discovery
{
    /// <summary>
    /// An explicit DiscoveryException to distinguish from other OAuthExceptions.
    /// </summary>    
    public class OAuthDiscoveryException : OAuth.Net.Common.OAuthException
    {
        public OAuthDiscoveryException(string message) : base(message) { }

        public OAuthDiscoveryException(string message, Exception innerException) : base(message, innerException) { }
    }
}
