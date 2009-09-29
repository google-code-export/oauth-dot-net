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
// OAuth.net uses the Common Service Locator interface, released under the MS-PL
// license. See "CommonServiceLocator License.txt" in the Licenses folder.
// 
// The examples and test cases use the Windsor Container from the Castle Project
// and Common Service Locator Windsor adaptor, released under the Apache License,
// Version 2.0. See "Castle Project License.txt" in the Licenses folder.
// 
// XRDS-Simple.net uses the HTMLAgility Pack. See "HTML Agility Pack License.txt"
// in the Licenses folder.
//
// Authors: Chris Adams, Bruce Boughton
// Website: http://lab.madgex.com/oauth-net/
// Email:   oauth-dot-net@madgex.com


using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using OAuth.Net.Common;

namespace OAuth.Net.Consumer
{
    /// <summary>
    /// Makes an OAuth Consumer request for a resource without an access token.
    /// <see href="http://oauth.googlecode.com/svn/spec/ext/consumer_request/1.0/drafts/1/spec.html">
    /// 
    /// A method for using the OAuth flow to sign a request made between a Consumer and Service Provider where no User is involved. 
    /// This enables OAuth to be used as a replacement to HTTP Basic authentication, which is not as complex as HTTP Digest authentication, 
    /// and extends the capabilities of the OAuth Consumer credentials. 
    /// 
    /// </summary>
    public static class OAuthConsumerRequest 
    {
        /// <summary>
        /// Creates a new OAuth protected requests.
        /// </summary>
        /// <remarks>
        /// Since neither a request token nor an access token is supplied,
        /// the user will have to authorize this request.
        /// </remarks>
        /// <param name="resourceEndPoint">Protected resource End Point</param>
        /// <param name="settings">Service settings</param>
        /// <returns>An OAuth protected request for the protected resource</returns>
        public static OAuthRequest Create(EndPoint resourceEndPoint, OAuthService settings)
        {
            return OAuthRequest.Create(
                resourceEndPoint,
                settings,
                new EmptyToken(settings.Consumer.Key, TokenType.Access),
                new EmptyToken(settings.Consumer.Key, TokenType.Request)
            );
        }


        private class EmptyToken : IToken
        {
            public EmptyToken(string consumerKey, TokenType tokenType)
            {
                this.ConsumerKey = consumerKey;
                this.Type = tokenType;
            }

            #region IToken Members

            public string Token
            {
                get { return String.Empty; }
            }

            public string Secret
            {
                get { return null; }
            }

            public string ConsumerKey
            {
                get;
                private set;
            }

            public TokenStatus Status
            {
                get
                {
                    return TokenStatus.Authorized;
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public TokenType Type
            {
                get;
                private set;
            }

            #endregion
        }

    }
}
