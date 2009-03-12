// Copyright (c) 2008 Madgex
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
// OAuth.net uses the Windsor Container from the Castle Project. See "Castle 
// Project License.txt" in the Licenses folder.
// 
// Authors: Bruce Boughton, Chris Adams
// Website: http://lab.madgex.com/oauth-net/
// Email:   oauth-dot-net@madgex.com

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace OAuth.Net.Common
{
    /// <summary>
    /// Constants used by OAuth.Net libraries
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// The HTTP content-type for url encoded parameters in the POST body
        /// </summary>
        public const string HttpPostUrlEncodedContentType = "application/x-www-form-urlencoded";

        /// <summary>
        /// Test for HTTP content-type for url encoded parameters in the POST 
        /// body, which may be followed by a semicolon and an encoding.
        /// </summary>
        public static readonly Regex HttpPostUrlEncodedContentTypeRegex 
            = new Regex(@"^application/x-www-form-urlencoded;?", RegexOptions.Compiled);

        /// <summary>
        /// The Authorization HTTP header parameter
        /// </summary>
        public const string AuthorizationHeaderParameter = "Authorization";

        /// <summary>
        /// The WWW-Authenticate HTTP header parameter
        /// </summary>
        public const string WwwAuthenticateHeaderParameter = "WWW-Authenticate";

        /// <summary>
        /// The Authorization realm parameter
        /// </summary>
        public const string RealmParameter = "realm";

        /// <summary>
        /// The OAuth RFC-2617 auth-scheme
        /// </summary>
        public const string OAuthAuthScheme = "OAuth";

        /// <summary>
        /// The OAuth credentials prefix for the Authorization HTTP header
        /// </summary>
        public static readonly Regex OAuthCredentialsRegex = new Regex(@"^" + OAuthAuthScheme + @"\s+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// String escape sequences
        /// </summary>
        public static readonly Regex StringEscapeSequence = new Regex(@"\\([""'\0abfnrtv]|U[0-9a-fA-F]{8}|u[0-9a-fA-F]{4}|x[0-9a-fA-F]+)", RegexOptions.Compiled);

        /// <summary>
        /// RFC 3986 percent encoding escape sequence
        /// </summary>
        public static readonly Regex Rfc3986EscapeSequence =
            new Regex("%([0-9A-Fa-f]{2})", RegexOptions.Compiled);

        /// <summary>
        /// The oauth_ prefix for parameters
        /// </summary>
        public const string OAuthParameterPrefix = "oauth_";

        /// <summary>
        /// The parameter for the OAuth consumer key
        /// </summary>
        public const string ConsumerKeyParameter = "oauth_consumer_key";

        /// <summary>
        /// The parameter for the OAuth signature method
        /// </summary>
        public const string SignatureMethodParameter = "oauth_signature_method";

        /// <summary>
        /// The parameter for the OAuth signature
        /// </summary>
        public const string SignatureParameter = "oauth_signature";

        /// <summary>
        /// The parameter for the OAuth timestamp
        /// </summary>
        public const string TimestampParameter = "oauth_timestamp";

        /// <summary>
        /// The parameter for the OAuth nonce
        /// </summary>
        public const string NonceParameter = "oauth_nonce";

        /// <summary>
        /// The parameter for the OAuth version
        /// </summary>
        public const string VersionParameter = "oauth_version";

        /// <summary>
        /// The parameter for the OAuth token
        /// </summary>
        public const string TokenParameter = "oauth_token";

        /// <summary>
        /// The parameter for the OAuth token secret
        /// </summary>
        public const string TokenSecretParameter = "oauth_token_secret";

        /// <summary>
        /// The parameter for the OAuth callback
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Callback", Justification = "Callback is a domain term")]
        public const string CallbackParameter = "oauth_callback";

        /// <summary>
        /// The OAuth version parameter value for OAuth Core 1.0
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "No other sensible punctuation")]
        public const string Version1_0 = "1.0";

        /// <summary>
        /// The Windsor configuration ID prefix for signing providers
        /// </summary>
        public const string SigningProviderIdPrefix = "signing.provider:";

        /// <summary>
        /// Parameter names that are reserved by OAuth
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "ReadOnlyCollection<T> is immutable")]
        public static readonly ReadOnlyCollection<string> ReservedParameterNames
            = new ReadOnlyCollection<string>(new List<string>()
                {
                    Constants.CallbackParameter,
                    Constants.ConsumerKeyParameter,
                    Constants.NonceParameter,
                    Constants.RealmParameter,
                    Constants.SignatureParameter,                
                    Constants.SignatureMethodParameter,
                    Constants.TimestampParameter,
                    Constants.TokenParameter,
                    Constants.TokenSecretParameter,
                    Constants.VersionParameter
                });
    }
}
