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
// Authors: Bruce Boughton, Chris Adams
// Website: http://lab.madgex.com/oauth-net/
// Email:   oauth-dot-net@madgex.com

using System;

using OAuth.Net.Common;

namespace OAuth.Net.Common.Tokens
{
    /// <summary>
    /// Implements and empty Access token that can be used for Consumer Requests for a protected resource.
    /// </summary>
    public class OAuthConsumerRequestAccessToken : IAccessToken
    {

        public OAuthConsumerRequestAccessToken(string secret, string consumerKey)
        {
            this.Secret = secret;
            this.ConsumerKey = consumerKey;
            this.Status = TokenStatus.Authorized;
        }

        #region IAccessToken Members

        public IRequestToken RequestToken
        {
            get
            {
                return null;
            }
            set
            {
                throw new InvalidOperationException("A Consumer Request does not participate in the Request/Access token process");
            }
        }

        #endregion

        #region IToken Members

        public string Token
        {
            get { return String.Empty; }
        }

        public string Secret
        {
            get;
            private set;
        }

        public string ConsumerKey
        {
            get;
            private set;
        }

        public TokenStatus Status
        {
            get;
            set;
        }

        public TokenType Type
        {
            get{ return TokenType.Access; }            
        }

        #endregion
    }
}
