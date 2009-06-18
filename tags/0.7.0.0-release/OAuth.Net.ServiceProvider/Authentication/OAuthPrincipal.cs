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
using System.Security.Principal;
using OAuth.Net.Common;

namespace OAuth.Net.ServiceProvider
{
    public class OAuthPrincipal
        : IPrincipal
    {
        /// <summary>
        /// Creates an OAuthPrincipal from an access token
        /// </summary>
        /// <param name="accessToken">Access token</param>
        public OAuthPrincipal(IAccessToken accessToken)
        {
            if (accessToken == null)
                throw new ArgumentNullException("accessToken");

            if (accessToken.RequestToken == null)
                throw new ArgumentException("Access token must have a request token", "accessToken");

            if (accessToken.RequestToken.AuthenticatedUser == null)
                throw new ArgumentException("Request token must have an authenticated user", "accessToken");

            this.AccessToken = accessToken;
            this.RequestToken = accessToken.RequestToken;

            if (ServiceProviderContext.Settings.ImpersonateUserForProtectedResources)
                this.Identity = this.RequestToken.AuthenticatedUser;
            else
                this.Identity = ServiceProviderContext.DummyIdentity;
        }

        /// <summary>
        /// The identity of the current principal.
        /// </summary>
        public IIdentity Identity
        {
            get;
            private set;
        }

        /// <summary>
        /// The access token
        /// </summary>
        public IAccessToken AccessToken
        {
            get;
            private set;
        }

        /// <summary>
        /// The request token
        /// </summary>
        public IRequestToken RequestToken
        {
            get;
            private set;
        }

        /// <summary>
        /// Determines whether the current principal belongs to the specified role.
        /// </summary>
        /// <param name="role">The name of the role for which to check membership.</param>
        /// <returns><c>true</c> if the current principal is a member of the 
        /// specified role; otherwise, <c>false</c>.</returns>
        public bool IsInRole(string role)
        {
            if (this.RequestToken.Roles == null)
                return false;

            return Array.IndexOf<string>(this.RequestToken.Roles, role) >= 0;
        }
    }
}
