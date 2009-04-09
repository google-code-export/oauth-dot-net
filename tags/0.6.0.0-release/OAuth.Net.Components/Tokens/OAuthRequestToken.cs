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
using System.Security.Principal;
using OAuth.Net.Common;

namespace OAuth.Net.Components
{
    [DebuggerDisplay("Token: {Token} Secret: {Secret} Consumer Key: {ConsumerKey} Status: {Status} Authenticated User: {AuthenticatedUser} Associated Parameters: {AssociatedParameters} Roles: {Roles}")]
    public struct OAuthRequestToken
        : IRequestToken
    {
        public OAuthRequestToken(
            string token,
            string secret,
            IConsumer consumer,
            TokenStatus status,
            OAuthParameters associatedParameters,
            IIdentity authenticatedUser,
            string[] roles)
            : this()
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("token must not be null or empty", "token");

            if (secret == null)
                throw new ArgumentNullException("secret", "secret must not be null");

            if (consumer == null)
                throw new ArgumentNullException("consumer", "consumer must not be null");

            if (roles == null)
                throw new ArgumentNullException("roles", "roles must not be null");

            this.Token = token;
            this.Secret = secret;
            this.Status = status;
            this.ConsumerKey = consumer.Key;
            this.AssociatedParameters = associatedParameters;
            this.AuthenticatedUser = authenticatedUser;
            this.Roles = roles;
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
        /// Returns the type of token represented
        /// </summary>
        public TokenType Type
        {
            get { return TokenType.Request; }
        }

        /// <summary>
        /// The parameters that were sent in the request that
        /// created this token (both OAuth and additional parameters).
        /// </summary>
        public OAuthParameters AssociatedParameters
        {
            get;
            private set;
        }

        /// <summary>
        /// The identity of the user who has authenticated against this token
        /// </summary>
        public IIdentity AuthenticatedUser
        {
            get;
            set;
        }

        /// <summary>
        /// The roles of the user who has authenticated against this token
        /// </summary>
        public string[] Roles
        {
            get;
            set;
        }

        public static bool operator ==(OAuthRequestToken left, IRequestToken right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(OAuthRequestToken left, IRequestToken right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is OAuthRequestToken)
                return false;

            return this.Equals((IRequestToken)obj);
        }

        public bool Equals(IRequestToken other)
        {
            return this.Token.Equals(other.Token)
                && this.Secret.Equals(other.Secret)
                && this.Status == other.Status
                && this.ConsumerKey.Equals(other.ConsumerKey)
                && this.AssociatedParameters.Equals(other.AssociatedParameters)
                && this.AuthenticatedUser.Equals(other.AuthenticatedUser)
                && Array.Equals(this.Roles, other.Roles);
        }

        public override int GetHashCode()
        {
            return this.Token.GetHashCode() ^ this.Secret.GetHashCode() ^ this.Status.GetHashCode()
                ^ this.ConsumerKey.GetHashCode() ^ this.AssociatedParameters.GetHashCode()
                ^ this.AuthenticatedUser.GetHashCode() ^ this.Roles.GetHashCode();
        }
    }
}
