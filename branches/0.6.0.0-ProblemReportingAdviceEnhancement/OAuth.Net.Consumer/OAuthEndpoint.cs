using System;

using OAuth.Net.Common;

namespace OAuth.Net.Consumer
{
    /// <summary>
    /// Contains details of an Endpoint in the OAuth process needed to be able to connect to this instance.
    /// </summary>
    public struct OAuthEndpoint
    {        
        public OAuthEndpoint(Uri url, string httpMethod, string signatureMethod, OAuthParameterSources acceptedOAuthParameterMethods) : this()
        {            
            Url = url;
            HttpMethod = httpMethod;
            SignatureMethod = signatureMethod;
            AcceptedOAuthParameterMethods = acceptedOAuthParameterMethods;
        }

        public OAuthEndpoint(Uri url, string httpMethod, OAuthParameterSources acceptedOAuthParameterMethods ) : this()
        {
            Url = url;
            HttpMethod = httpMethod;
            SignatureMethod = String.Empty;
            AcceptedOAuthParameterMethods = acceptedOAuthParameterMethods;
        }
        
        /// <summary>
        /// The Url to access the end point.
        /// </summary>
        public Uri Url { get; private set; }

        /// <summary>
        /// The HTTP method that should be used to access this Endpoint
        /// </summary>
        public string HttpMethod { get; private set; }
        
        /// <summary>
        /// The Signature Method that should be used to access this Endpoint
        /// </summary>
        public string SignatureMethod { get; private set; }

        /// <summary>
        /// A Flag of the supported methods that OAuth parametesr can be sent.
        /// </summary>
        public OAuthParameterSources AcceptedOAuthParameterMethods { get; private set; }
    }
}
