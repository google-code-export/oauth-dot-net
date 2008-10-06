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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace OAuth.Net.Common
{
    /// <summary>
    /// An OAuthProtectedResponse is a facade for HttpWebResponse. Its response stream
    /// is read into a MemoryStream which can be replayed.
    /// </summary>
    [Serializable]
    public class OAuthResource : IDisposable, ISerializable
    {
        private bool alreadyDisposed;
        private HttpWebResponse response;
        private MemoryStream responseStream;

        /// <summary>
        /// Creates an OAuthProtectedResponse from a HttpWebResponse.
        /// </summary>
        /// <param name="response">HttpWebResponse</param>
        public OAuthResource(HttpWebResponse response)
        {
            this.response = response;

            // Read and store response stream
            byte[] buffer;
            int totalRead;
            using (Stream stream = response.GetResponseStream())
            {
                buffer = new byte[(this.ContentLength > 0) ? this.ContentLength : 4096];
                totalRead = 0;

                int thisRead = 0;
                while ((thisRead = stream.Read(buffer, totalRead, buffer.Length - totalRead)) != 0)
                {
                    totalRead += thisRead;

                    if (totalRead == buffer.Length)
                    {
                        // Increase by 4096 bytes at a time
                        byte[] newBuffer = new byte[buffer.Length + 4096];
                        buffer.CopyTo(newBuffer, 0);
                        buffer = newBuffer;
                    }
                }
            }

            this.responseStream = new MemoryStream(buffer, 0, totalRead, false, false);
            this.responseStream.Position = 0;
        }

        protected OAuthResource(SerializationInfo info, StreamingContext context)
        {
            this.response = (HttpWebResponse)info.GetValue("response", typeof(HttpWebResponse));
            this.responseStream = (MemoryStream)info.GetValue("responseStream", typeof(MemoryStream));
        }

        public string CharacterSet
        {
            get { return this.response.CharacterSet; }
        }

        public string ContentEncoding
        {
            get { return this.response.ContentEncoding; }
        }

        public long ContentLength
        {
            get { return this.response.ContentLength; }
        }

        public string ContentType
        {
            get { return this.response.ContentType; }
        }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Intentionally mimics HttpWebResponse")]
        public CookieCollection Cookies
        {
            get { return this.response.Cookies; }
            set { this.response.Cookies = value; }
        }

        public WebHeaderCollection Headers
        {
            get { return this.response.Headers; }
        }

        public bool IsFromCache
        {
            get { return this.response.IsFromCache; }
        }

        public bool IsMutuallyAuthenticated
        {
            get { return this.response.IsMutuallyAuthenticated; }
        }

        public DateTime LastModified
        {
            get { return this.response.LastModified; }
        }

        public string Method
        {
            get { return this.response.Method; }
        }

        public Version ProtocolVersion
        {
            get { return this.response.ProtocolVersion; }
        }

        public Uri ResponseUri
        {
            get { return this.response.ResponseUri; }
        }

        public string Server
        {
            get { return this.response.Server; }
        }

        public HttpStatusCode StatusCode
        {
            get { return this.response.StatusCode; }
        }

        public string StatusDescription
        {
            get { return this.response.StatusDescription; }
        }

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Intentionally mimics HttpWebRequest")]
        public Stream GetResponseStream()
        {
            return this.responseStream;
        }

        public void Close()
        {
            this.response.Close();
        }

        public string GetResponseHeader(string headerName)
        {
            return this.response.GetResponseHeader(headerName);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("response", this.response, typeof(HttpWebResponse));
            info.AddValue("responseStream", this.responseStream, typeof(MemoryStream));
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (this.alreadyDisposed)
                return;

            if (isDisposing)
            {
                this.response = null;

                if (this.responseStream != null)
                {
                    this.responseStream.Dispose();
                    this.responseStream = null;
                }
            }

            this.alreadyDisposed = true;
        }
    }
}
