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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using OAuth.Net.Common;

namespace OAuth.Net.Components
{
    [DebuggerDisplay("Token: {Token} Secret: {Secret} Consumer Key: {ConsumerKey} Status: {Status} Request Token: {RequestToken}")]
    public struct OAuthAccessToken
        : IAccessToken
    {
        public OAuthAccessToken(
            string token,
            string secret,
            IConsumer consumer,
            TokenStatus status,
            IRequestToken requestToken)
            : this()
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("token must not be null or empty", "token");

            if (secret == null)
                throw new ArgumentNullException("secret", "secret must not be null");

            if (consumer == null)
                throw new ArgumentNullException("consumer", "consumer must not be null");

            if (requestToken == null)
                throw new ArgumentNullException("requestToken", "requestToken must not be null");

            this.Token = token;
            this.Secret = secret;
            this.Status = status;
            this.ConsumerKey = consumer.Key;
            this.RequestToken = requestToken;
        }

        /// <summary>
        /// The token
        /// </summary>
        public string Token
        {
            get;
            private set;
        }

        /// <summary>
        /// The token secret
        /// </summary>
        public string Secret
        {
            get;
            private set;
        }

        /// <summary>
        /// The key of the consumer this token was issued to
        /// </summary>
        public string ConsumerKey
        {
            get;
            private set;
        }

        /// <summary>
        /// The status of the token
        /// </summary>
        public TokenStatus Status
        {
            get;
            set;
        }

        /// <summary>
        /// The type of token represented
        /// </summary>
        public TokenType Type
        {
            get { return TokenType.Access; }
        }

        /// <summary>
        /// The request token exchanged for this access token.
        /// </summary>
        public IRequestToken RequestToken
        {
            get;
            set;
        }

        public static bool operator ==(OAuthAccessToken left, IAccessToken right)
        {
            if (System.Object.ReferenceEquals(left, right))
                return true;

            if (((object)left == null) || ((object)right == null))
                return false;

            return left.Equals(right);
        }

        public static bool operator !=(OAuthAccessToken left, IAccessToken right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            // If the token is set then all parameters must have a value as they would have been required in the constructor.            
            if (this.Token != null)
                return this.Token.GetHashCode() ^ this.Secret.GetHashCode() ^ this.ConsumerKey.GetHashCode();
            else
                return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (System.Object.ReferenceEquals(this, obj))
                return true;

            if (this.GetType() != obj.GetType())
                return false;

            return this.Equals(obj as IAccessToken);    
        }
                        
        [SuppressMessage("Microsoft.StyleCop.CSharp.SpacingRules", "SA1003:SymbolsMustBeSpaceCorrectly", Justification = "Improves readability")]
        [SuppressMessage("Microsoft.StyleCop.CSharp.SpacingRules", "SA1009:ClosingParenthesisMustBeSpacedCorrectly", Justification = "Improves readability")]
        private bool Equals(IAccessToken other)
        {
            if (other == null)
                return false;

            return string.Equals(this.Token, other.Token)
                && string.Equals(this.Secret, other.Secret)
                && this.Status == other.Status
                && string.Equals(this.ConsumerKey, other.ConsumerKey)
                && ((this.RequestToken == null && other.RequestToken == null ) || (this.RequestToken != null && this.RequestToken.Equals(other.RequestToken)));
        }        
    }
}
