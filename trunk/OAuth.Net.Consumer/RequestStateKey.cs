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
using System.Web;

namespace OAuth.Net.Consumer
{
    public class RequestStateKey : IEquatable<RequestStateKey>
    {
        /// <summary>
        /// Constructs a request state key from a service realm, consumer key, and
        /// an end user id
        /// </summary>
        /// <param name="serviceRealm">Service realm</param>
        /// <param name="consumerKey">Consumer key</param>
        /// <param name="endUserId">End user identifier</param>
        public RequestStateKey(string serviceRealm, string consumerKey, string endUserId)
        {
            if (string.IsNullOrEmpty(serviceRealm))
                throw new ArgumentException("serviceRealm must not be null or empty", "serviceRealm");

            if (string.IsNullOrEmpty(consumerKey))
                throw new ArgumentException("consumerKey must not be null or empty", "consumerKey");

            this.ServiceRealm = serviceRealm;
            this.ConsumerKey = consumerKey;
            this.EndUserId = endUserId;
        }

        /// <summary>
        /// Constructs a request state key from a service and
        /// an end user id. The service realm is taken to be the OAuthService.Realm
        /// property or else the OAuthService.AuthorizationUri if the realm is null. 
        /// The consumer key is loaded from the service
        /// </summary>
        /// <param name="service">Service</param>
        /// <param name="endUserId">End user identifier</param>
        public RequestStateKey(OAuthService service, string endUserId)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            string serviceRealm = null;
            if (!string.IsNullOrEmpty(service.Realm))
                serviceRealm = service.Realm;
            else if (service.AuthorizationUrl != null)
                serviceRealm = service.AuthorizationUrl.AbsoluteUri;
            else
                throw new ArgumentException("Service does not have realm or authorization URI", "service");

            if (service.Consumer == null || string.IsNullOrEmpty(service.Consumer.Key))
                throw new ArgumentException("Service does not have consumer key", "service");

            this.ServiceRealm = serviceRealm;
            this.ConsumerKey = service.Consumer.Key;
            this.EndUserId = endUserId;
        }

        /// <summary>
        /// The service realm this state applies to. This could be a HTTP Authorization
        /// Realm as per RFC 2616, or some other identifier for a service.
        /// </summary>
        public string ServiceRealm { get; private set; }

        /// <summary>
        /// The consumer key this state applies to.
        /// </summary>
        public string ConsumerKey { get; private set; }

        /// <summary>
        /// An identifier for the end user this state applies to.
        /// </summary>
        public string EndUserId { get; private set; }
       
        public override bool Equals(object obj)
        {
            var other = obj as RequestStateKey;

            if (obj == null)
                return false;

            return this.Equals(other);
        }

        public bool Equals(RequestStateKey other)
        {
            if (other == null)
                return false;

            return string.Equals(this.ServiceRealm, other.ServiceRealm, StringComparison.Ordinal)
                && string.Equals(this.ConsumerKey, other.ConsumerKey, StringComparison.Ordinal)
                && string.Equals(this.EndUserId, other.EndUserId, StringComparison.Ordinal);
        }

        public override int GetHashCode()
        {
            int hashCode = this.ServiceRealm.GetHashCode() ^ this.ConsumerKey.GetHashCode();
            
            if (!string.IsNullOrEmpty(this.EndUserId))
                hashCode ^= this.EndUserId.GetHashCode();

            return hashCode;
        }
    }
}
