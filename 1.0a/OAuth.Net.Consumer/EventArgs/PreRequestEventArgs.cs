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
using System.Collections.Specialized;

namespace OAuth.Net.Consumer
{
    /// <summary>
    /// The event arguments used for events that fire before a request is sent.
    /// </summary>
    public class PreRequestEventArgs
        : EventArgs
    {
        /// <summary>
        /// Creates event arguments with the corresponding request Uri, HTTP method and parameter collection.
        /// </summary>
        /// <param name="requestUri">Request Uri</param>
        /// <param name="httpMethod">HTTP method</param>
        /// <param name="parameters">Additional parameters</param>
        public PreRequestEventArgs(Uri requestUri, string httpMethod, Uri callbackUrl, NameValueCollection parameters)
        {
            this.RequestUri = requestUri;
            this.HttpMethod = httpMethod;
            this.AdditionalParameters = parameters;
            this.CallbackUrl = callbackUrl;
        }

        /// <summary>
        /// The Uri of the request to be sent.
        /// </summary>
        public Uri RequestUri
        {
            get;
            set;
        }

        /// <summary>
        /// The HTTP method of the request to be sent
        /// </summary>
        public string HttpMethod
        {
            get;
            set;
        }

        /// <summary>
        /// The URL that the service provider will return the user to after 
        /// </summary>
        public Uri CallbackUrl
        {
            get;
            set;
        }

        /// <summary>
        /// A name value collection of parameters that will be applied to the
        /// request before it is sent.
        /// </summary>
        public NameValueCollection AdditionalParameters
        {
            get;
            private set;
        }
    }
}
