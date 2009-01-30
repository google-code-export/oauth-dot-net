using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OAuth.Net.Discovery
{
    public static class Constants
    {
        public const string OAuthDiscoveryServiceTypeURI = "http://oauth.net/discovery/1.0";

        public const string OAuthEndPointRequestServiceTypeURI = "http://oauth.net/core/1.0/endpoint/request";

        public const string OAuthEndPointAuthorizeServiceTypeURI = "http://oauth.net/core/1.0/endpoint/authorize";

        public const string OAuthEndPointAccessServiceTypeURI = "http://oauth.net/core/1.0/endpoint/access";

        public const string OAuthEndPointResourceServiceTypeURI = "http://oauth.net/core/1.0/endpoint/resource";

        public const string OAuthConsumerIdentityStaticServiceTypeURI = "http://oauth.net/core/1.0/consumer-identity/static";

        public const string OAuthConsumerIdentityOutOfBandServiceTypeURI = "http://oauth.net/core/1.0/consumer-identity/oob";

        public const string OAuthRequestParameterAuthHeaderServiceTypeURI = "http://oauth.net/core/1.0/parameters/auth-header";

        public const string OAuthRequestParameterPostBodyServiceTypeURI = "http://oauth.net/core/1.0/parameters/post-body";

        public const string OAuthRequestParameterURIQueryServiceTypeURI = "http://oauth.net/core/1.0/parameters/uri-query";

        public const string OAuthSignatureServiceTypeURI = "http://oauth.net/core/1.0/signature/";

        public const string OAuthSignatureHMACSHA1ServiceTypeURI = "http://oauth.net/core/1.0/signature/HMAC-SHA1";

        public const string OAuthSignatureRSASHA1ServiceTypeURI = "http://oauth.net/core/1.0/signature/RSA-SHA1";

        public const string OAuthSignaturePlaintextServiceTypeURI = "http://oauth.net/core/1.0/signature/PLAINTEXT";


    }
}
