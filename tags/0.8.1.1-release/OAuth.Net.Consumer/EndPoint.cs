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
using System.Collections.Generic;
using System.Text;

namespace OAuth.Net.Consumer
{
    /// <summary>
    /// Represent an Uri and HttpMethod that define the EndPoint in the OAuth dance.  This could either
    /// be the protected resource, or the oauth request and token end points.
    /// </summary>
    public struct EndPoint
    {        
        /// <summary>
        /// Constructs and EndPoint instance using the HTTP GET method for the 
        /// supplied uri
        /// </summary>
        /// <param name="uri">An absolute Uri</param>
        public EndPoint(string uri) : this(new Uri(uri))
        {            
        }

        /// <summary>
        /// Constructs and EndPoint instance using the HTTP GET method for the 
        /// supplied uri
        /// </summary>
        /// <param name="uri">An absolute Uri</param>
        public EndPoint(Uri uri) : this()
        {            
            this.Uri = uri;
            if (!this.Uri.IsAbsoluteUri)
                throw new ArgumentException("The uri of this EndPoint must be absolute.");
            this.HttpMethod = "GET";
        }

         /// <summary>
        /// Constructs and EndPoint instance.
        /// </summary>
        /// <param name="uri">An absolute Uri</param>
        /// <param name="httpMethod">GET, POST, DELETE or PUT</param>
        public EndPoint(string uri, string httpMethod) : this(new Uri(uri), httpMethod)
        {
        }

        /// <summary>
        /// Constructs and EndPoint instance.
        /// </summary>
        /// <param name="uri">An absolute Uri</param>
        /// <param name="httpMethod">GET, POST, DELETE or PUT</param>
        public EndPoint(Uri uri, string httpMethod) : this(uri)
        {            
            switch (httpMethod)
            {
                case "GET":
                case "POST":
                case "DELETE":
                case "PUT":
                    this.HttpMethod = httpMethod;
                    break;
                default:
                    throw new ArgumentException("Only GET, POST, DELETE AND PUT are supported HttpMethods", "httpMethod");
            }            
        }

        /// <summary>
        /// The absolute Uri of the OAuth Endpoint.
        /// </summary>
        public Uri Uri
        {
            get;
            private set;
        }

        /// <summary>
        /// The HttpMethod to access this end point.
        /// </summary>
        public string HttpMethod
        {
            get;
            private set;
        }

        /// <summary>
        /// Checks if two EndPoints are equal
        /// </summary>
        /// <param name="left">Left-hand-side EndPoint</param>
        /// <param name="right">Right-hand-side EndPoint</param>
        /// <returns><c>true</c>, if they are equal; otherwise <c>false</c></returns>
        public static bool operator ==(EndPoint left, EndPoint right)
        {            
            return left.Equals(right);
        }

        /// <summary>
        /// Checks if two OAuthServices are not equal
        /// </summary>
        /// <param name="left">Left-hand-side EndPoint</param>
        /// <param name="right">Right-hand-side EndPoint</param>
        /// <returns><c>true</c>, if they are not equal; otherwise <c>false</c></returns>
        public static bool operator !=(EndPoint left, EndPoint right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return this.HttpMethod.GetHashCode() ^ this.Uri.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (System.Object.ReferenceEquals(this, obj))
                return true;

            if (this.GetType() != obj.GetType())
                return false;

            return this.Equals((EndPoint)obj);
        }

        private bool Equals(EndPoint other)
        {
            return string.Equals(this.HttpMethod, other.HttpMethod)
                 && ((this.Uri == null && other.Uri == null) ||
                     (this.Uri != null && this.Uri.Equals(other.Uri)));
        }        
    }
}
