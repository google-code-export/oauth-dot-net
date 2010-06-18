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

using OAuth.Net.Common;

namespace OAuth.Net.Consumer
{
    /// <summary>
    /// An OAuthResponse is returned when making a request for an OAuthResource
    /// from an OAuthService provider.  The response will also contain a Token 
    /// which will either by a RequestToken or AccessToken.  If the service
    /// has returned the resource in the response then the ProtectedResource
    /// will contain the OAuthResource requested.
    /// </summary>
    public class OAuthResponse
    {
        internal OAuthResponse(IToken token) : this(token, null)
        {            
        }

        internal OAuthResponse(IToken token, OAuthResource resource)
        {
            this.Token = token;
            this.ProtectedResource = resource;
            this.HasProtectedResource = resource != null;
        }

        /// <summary>
        /// The token associated with the resource.  In the first stage 
        /// of the request this will return a RequestToken.  When an OAuthResource
        /// is succesfully return it will be the AccessToken which can be used for
        /// further requests.
        /// </summary>
        public IToken Token
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns true if the Response contains the requested OAuthResource        
        /// </summary>
        public bool HasProtectedResource
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns the OAuthResource requested from the OAuthService 
        /// if the request was succesfull or null.
        /// </summary>
        public OAuthResource ProtectedResource
        {
            get;
            private set;
        }
    }
}
