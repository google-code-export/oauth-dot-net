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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using OAuth.Net.Common;

namespace OAuth.Net.ServiceProvider
{
    [DebuggerDisplay("Parameters: {Parameters} Signing Provider: {SigningProvider} Consumer: {Consumer} Request Token: {RequestToken} Access Token: {AccessToken} Request Id: {RequestId} Is Signature Valid: {IsSignatureValid} Signature: {Signature} Principal: {Principal} Response Parameters: {ResponseParameters}")]
    public class OAuthRequestContext
    {
        private List<OAuthRequestException> errors = new List<OAuthRequestException>();
        private IRequestToken requestToken;

        public OAuthRequestContext()
            : this(null)
        {
        }

        public OAuthRequestContext(NameValueCollection responseParameters)
        {
            this.ResponseParameters = responseParameters;
        }

        public ICollection<OAuthRequestException> Errors
        {
            get { return new ReadOnlyCollection<OAuthRequestException>(this.errors); }
        }

        public OAuthParameters Parameters
        {
            get;
            set;
        }

        public ISigningProvider SigningProvider
        {
            get;
            set;
        }

        public IConsumer Consumer
        {
            get;
            set;
        }
       
        public IRequestToken RequestToken
        {
            get 
            {
                if (this.requestToken == null && this.AccessToken != null)
                    this.requestToken = this.AccessToken.RequestToken;

                return this.requestToken;
            }

            set 
            {
                this.requestToken = value; 
            }
        }

        public IAccessToken AccessToken
        {
            get;
            set;
        }

        public RequestId RequestId
        {
            get;
            set;
        }

        public bool IsSignatureValid
        {
            get;
            set;
        }

        public string Signature
        {
            get;
            set;
        }

        public bool IsOAuthRequest
        {
            get;
            set;
        }

        public OAuthPrincipal Principal
        {
            get;
            set;
        }

        public NameValueCollection ResponseParameters
        {
            get;
            private set;
        }

        public static bool operator ==(OAuthRequestContext left, OAuthRequestContext right)
        {
            if (System.Object.ReferenceEquals(left, right))
                return true;

            if (((object)left == null) || ((object)right == null))
                return false;

            return left.Equals(right);
        }

        public static bool operator !=(OAuthRequestContext left, OAuthRequestContext right)
        {
            return !(left == right);
        }

        public void AddError(OAuthRequestException error)
        {
            this.errors.Add(error);
        }

        public void RemoveError(OAuthRequestException error)
        {
            this.errors.Remove(error);
        }

        public void ClearErrors()
        {
            this.errors = new List<OAuthRequestException>();
        }
        
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (System.Object.ReferenceEquals(this, obj))
                return true;

            if (this.GetType() != obj.GetType())
                return false;

            return this.Equals(obj as OAuthRequestContext);
        }

        public override int GetHashCode()
        {
            int hash = 1;

            if (this.ResponseParameters != null)
                hash ^= this.ResponseParameters.GetHashCode();

            return hash;
        }

        private bool Equals(OAuthRequestContext other)
        {
            if (other == null)
                return false;

            return ((this.Parameters == null && other.Parameters == null) ||
                    (this.Parameters != null && this.Parameters.Equals(other.Parameters))) &&
                   ((this.SigningProvider == null && other.SigningProvider == null) ||
                    (this.SigningProvider != null && this.SigningProvider.Equals(other.SigningProvider))) &&
                   ((this.Consumer == null && other.Consumer == null) ||
                    (this.Consumer != null && this.Consumer.Equals(other.Consumer))) &&
                   ((this.RequestToken == null && other.RequestToken == null) ||
                    (this.RequestToken != null && this.RequestToken.Equals(other.RequestToken))) &&
                   ((this.AccessToken == null && other.AccessToken == null) ||
                    (this.AccessToken != null && this.AccessToken.Equals(other.AccessToken))) &&                   
                   (this.RequestId != null && this.RequestId.Equals(other.RequestId)) &&
                   (this.IsSignatureValid == other.IsSignatureValid) &&
                   ((this.Signature == null && other.Signature == null) ||
                    (this.Signature != null && this.Signature.Equals(other.Signature))) &&
                   ((this.Principal == null && other.Principal == null) ||
                    (this.Principal != null && this.Principal.Equals(other.Principal))) &&
                   ((this.ResponseParameters == null && other.ResponseParameters == null) ||
                    (this.ResponseParameters != null && this.ResponseParameters.Equals(other.ResponseParameters))) &&
                   (this.IsOAuthRequest == other.IsOAuthRequest);
        }
    }
}