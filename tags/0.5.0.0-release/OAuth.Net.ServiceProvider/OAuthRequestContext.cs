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
using System.Collections.Specialized;
using System.Diagnostics;
using OAuth.Net.Common;

namespace OAuth.Net.ServiceProvider
{
    [DebuggerDisplay("Parameters: {Parameters} Signing Provider: {SigningProvider} Consumer: {Consumer} Request Token: {RequestToken} Access Token: {AccessToken} Request Id: {RequestId} Is Signature Valid: {IsSignatureValid} Signature: {Signature} Principal: {Principal} Response Parameters: {ResponseParameters}")]
    public class OAuthRequestContext
    {
        private List<OAuthRequestException> errors = new List<OAuthRequestException>();

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
            get;
            set;
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
            return left.Equals(right);
        }

        public static bool operator !=(OAuthRequestContext left, OAuthRequestContext right)
        {
            return !left.Equals(right);
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

            return this.Equals(obj as OAuthRequestContext);
        }

        public bool Equals(OAuthRequestContext other)
        {
            return other != null
                && this.Parameters.Equals(other.Parameters)
                && this.SigningProvider.Equals(other.SigningProvider)
                && this.Consumer.Equals(other.Consumer)
                && this.RequestToken.Equals(other.RequestToken)
                && this.AccessToken.Equals(other.AccessToken)
                && this.RequestId.Equals(other.RequestId)
                && this.IsSignatureValid == other.IsSignatureValid
                && this.Signature.Equals(other.Signature)
                && this.Principal.Equals(other.Principal)
                && this.ResponseParameters.Equals(other.ResponseParameters);
        }

        public override int GetHashCode()
        {
            return this.Parameters.GetHashCode() ^ this.SigningProvider.GetHashCode() ^ this.Consumer.GetHashCode()
                ^ this.RequestToken.GetHashCode() ^ this.AccessToken.GetHashCode() ^ this.RequestId.GetHashCode()
                ^ this.IsSignatureValid.GetHashCode() ^ this.Signature.GetHashCode() ^ this.Principal.GetHashCode()
                ^ this.ResponseParameters.GetHashCode();
        }
    }
}
