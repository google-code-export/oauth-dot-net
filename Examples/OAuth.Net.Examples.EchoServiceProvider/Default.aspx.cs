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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Web;
using System.Web.UI;
using OAuth.Net.Common;
using OAuth.Net.Components;
using OAuth.Net.ServiceProvider;

namespace OAuth.Net.Examples.EchoServiceProvider
{
    public class DemoPage : Page
    {
        [SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings", Justification = "VirtualPathUtility returns a string")]
        public Uri RequestTokenBaseUri
        {
            get
            {
                return new Uri(
                    this.Request.Url, 
                    VirtualPathUtility.ToAbsolute("~/request-token.ashx"));
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings", Justification = "VirtualPathUtility returns a string")]
        public Uri AccessTokenBaseUri 
        {
            get
            {
                return new Uri(
                    this.Request.Url, 
                    VirtualPathUtility.ToAbsolute("~/access-token.ashx"));
            }
        }
        
        [SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings", Justification = "VirtualPathUtility returns a string")]
        public Uri EchoApiBaseUri
        {
            get
            {
                return new Uri(
                    this.Request.Url,
                    VirtualPathUtility.ToAbsolute("~/echo.ashx"));
            }
        }

        public Uri BuildRequestTokenUri()
        {
            int timestamp = UnixTime.ToUnixTime(DateTime.Now);

            OAuthParameters parameters = new OAuthParameters();
            parameters.ConsumerKey = ConsumerStore.FixedConsumer.Key;
            parameters.Nonce = new GuidNonceProvider().GenerateNonce(timestamp);
            parameters.SignatureMethod = "HMAC-SHA1";
            parameters.Timestamp = timestamp.ToString(CultureInfo.InvariantCulture);
            parameters.Version = "1.0";

            parameters.Signature = ServiceProviderContext.GetSigningProvider("HMAC-SHA1").ComputeSignature(
                SignatureBase.Create("GET", this.RequestTokenBaseUri, parameters),
                ConsumerStore.FixedConsumer.Secret,
                null);

            UriBuilder builder = new UriBuilder(this.RequestTokenBaseUri) 
            { 
                Query = parameters.ToQueryStringFormat() 
            };
                
            return builder.Uri;
        }

        public Uri BuildAccessTokenUri()
        {
            int timestamp = UnixTime.ToUnixTime(DateTime.Now);

            OAuthParameters parameters = new OAuthParameters();
            parameters.ConsumerKey = ConsumerStore.FixedConsumer.Key;
            parameters.Nonce = new GuidNonceProvider().GenerateNonce(timestamp);
            parameters.SignatureMethod = "HMAC-SHA1";
            parameters.Timestamp = timestamp.ToString(CultureInfo.InvariantCulture);
            parameters.Version = "1.0";
            parameters.Token = TokenGenerator.FixedRequestToken.Token;

            parameters.Signature = ServiceProviderContext.GetSigningProvider("HMAC-SHA1").ComputeSignature(
                SignatureBase.Create("GET", this.AccessTokenBaseUri, parameters),
                ConsumerStore.FixedConsumer.Secret,
                TokenGenerator.FixedRequestToken.Secret);

            UriBuilder builder = new UriBuilder(this.AccessTokenBaseUri)
            {
                Query = parameters.ToQueryStringFormat()
            };

            return builder.Uri;
        }

        public Uri BuildEchoCallUri(params KeyValuePair<string, string>[] pairs)
        {
            int timestamp = UnixTime.ToUnixTime(DateTime.Now);

            OAuthParameters parameters = new OAuthParameters();
            parameters.ConsumerKey = ConsumerStore.FixedConsumer.Key;
            parameters.Nonce = new GuidNonceProvider().GenerateNonce(timestamp);
            parameters.SignatureMethod = "HMAC-SHA1";
            parameters.Timestamp = timestamp.ToString(CultureInfo.InvariantCulture);
            parameters.Version = "1.0";
            parameters.Token = TokenGenerator.FixedAccessToken.Token;

            foreach (KeyValuePair<string, string> pair in pairs)
                parameters.AdditionalParameters.Add(pair.Key, pair.Value);

            parameters.Signature = ServiceProviderContext.GetSigningProvider("HMAC-SHA1").ComputeSignature(
                SignatureBase.Create("GET", this.EchoApiBaseUri, parameters),
                ConsumerStore.FixedConsumer.Secret,
                TokenGenerator.FixedAccessToken.Secret);

            UriBuilder builder = new UriBuilder(this.EchoApiBaseUri)
            {
                Query = parameters.ToQueryStringFormat()
            };

            return builder.Uri;
        }
    }
}