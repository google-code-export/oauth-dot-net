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

namespace OAuth.Net.Common
{
    /// <summary>
    /// The problem types used by the <a href="http://wiki.oauth.net/ProblemReporting">
    /// Problem Reporting extension</a>. These are the legal values of the 
    /// <see cref="OAuthRequestException.Problem">Problem</see> property of 
    /// <see cref="OAuthRequestException">OAuthRequestException</see>.
    /// </summary>
    public static class OAuthRequestExceptionProblemTypes
    {
        /// <summary>
        /// <c>version_rejected</c>: 
        /// The <see cref="Constants.VersionParameter">oauth_version</see> isn't 
        /// supported by the Service Provider.
        /// </summary>
        /// 
        /// <value>version_rejected</value>
        public const string VersionRejected = "version_rejected";

        /// <summary>
        /// <c>parameter_absent</c>:
        /// A required parameter wasn't received.
        /// </summary>
        /// 
        /// <value>parameter_absent</value>
        public const string ParameterAbsent = "parameter_absent";

        /// <summary>
        /// <c>parameter_rejected</c>:
        /// An unexpected parameter was received. 
        /// </summary>
        /// 
        /// <value>parameter_rejected</value>
        public const string ParameterRejected = "parameter_rejected";

        /// <summary>
        /// <c>timestamp_refused</c>:
        /// The <see cref="Constants.TimestampParameter">oauth_timestamp</see> value is 
        /// unacceptable to the Service Provider.
        /// </summary>
        /// 
        /// <value>timestamp_refused</value>
        public const string TimestampRefused = "timestamp_refused";

        /// <summary>
        /// <c>nonce_used</c>:
        /// The <see cref="Constants.NonceParameter">oauth_nonce</see> value was used in 
        /// a previous request, and consequently can't be used now.
        /// </summary>
        /// 
        /// <value>nonce_used</value>
        public const string NonceUsed = "nonce_used";

        /// <summary>
        /// <c>signature_method_rejected</c>:
        /// The <see cref="Constants.SignatureMethodParameter">oauth_signature_method</see> 
        /// is unacceptable to the Service Provider.
        /// </summary>
        /// 
        /// <value>signature_method_rejected</value>
        public const string SignatureMethodRejected = "signature_method_rejected";

        /// <summary>
        /// <c>signature_invalid</c>:
        /// The <see cref="Constants.SignatureParameter">oauth_signature</see> is invalid. 
        /// That is, it doesn't match the signature computed by the Service Provider.
        /// </summary>
        /// 
        /// <value>signature_invalid</value>
        public const string SignatureInvalid = "signature_invalid";

        /// <summary>
        /// <c>consumer_key_unknown</c>:
        /// The <see cref="Constants.ConsumerKeyParameter">oauth_consumer_key</see> is 
        /// unknown to the Service Provider.
        /// </summary>
        /// 
        /// <value>consumer_key_unknown</value>
        public const string ConsumerKeyUnknown = "consumer_key_unknown";

        /// <summary>
        /// <c>consumer_key_rejected</c>:
        /// The <see cref="Constants.ConsumerKeyParameter">oauth_consumer_key</see> is 
        /// permanently unacceptable to the Service Provider. For example, the 
        /// Consumer may be black listed.
        /// </summary>
        /// 
        /// <value>consumer_key_rejected</value>
        public const string ConsumerKeyRejected = "consumer_key_rejected";

        /// <summary>
        /// <c>consumer_key_refused</c>:
        /// The <see cref="Constants.ConsumerKeyParameter">oauth_consumer_key</see> is 
        /// temporarily unacceptable to the Service Provider. For example, the Service 
        /// Provider may be throttling the Consumer.
        /// </summary>
        /// 
        /// <value>consumer_key_refused</value>
        public const string ConsumerKeyRefused = "consumer_key_refused";

        /// <summary>
        /// <c>token_used</c>:
        /// The <see cref="Constants.TokenParameter">oauth_token</see> has been consumed. 
        /// That is, it can't be used any more because it has already been used in a 
        /// previous request or requests.
        /// </summary>
        /// 
        /// <value>token_used</value>
        public const string TokenUsed = "token_used";

        /// <summary>
        /// <c>token_expired</c>:
        /// The <see cref="Constants.TokenParameter">oauth_token</see> has expired. That 
        /// is, it was issued too long ago to be used now.
        /// </summary>
        /// 
        /// <value>token_expired</value>
        public const string TokenExpired = "token_expired";

        /// <summary>
        /// <c>token_revoked</c>:
        /// The <see cref="Constants.TokenParameter">oauth_token</see> has been revoked. That 
        /// is, the Service Provider has unilaterally decided it will never accept this 
        /// token.
        /// </summary>
        /// 
        /// <value>token_revoked</value>
        public const string TokenRevoked = "token_revoked";

        /// <summary>
        /// <c>token_rejected</c>:
        /// The <see cref="Constants.TokenParameter">oauth_token</see> is unacceptable to the 
        /// Service Provider. The reason is unspecified. It might mean that the token 
        /// was never issued, or consumed or expired and then subsequently forgotten by 
        /// the Service Provider.
        /// </summary>
        /// 
        /// <value>token_rejected</value>
        public const string TokenRejected = "token_rejected";

        /// <summary>
        /// <c>permission_unknown</c>:
        /// The User hasn't decided whether to permit this Consumer to access Protected 
        /// Resources. Usually happens when the Consumer requests Access Token before the 
        /// user completes authorization process.
        /// </summary>
        /// 
        /// <value>permission_unknown</value>
        public const string PermissionUnknown = "permission_unknown";

        /// <summary>
        /// <c>permission_denied</c>:
        /// The User refused to permit this Consumer to access Protected Resources.
        /// </summary>
        /// 
        /// <value>permission_denied</value>
        public const string PermissionDenied = "permission_denied";
    }
}
