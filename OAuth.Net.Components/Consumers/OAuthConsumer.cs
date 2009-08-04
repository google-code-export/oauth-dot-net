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

using System.Diagnostics;
using OAuth.Net.Common;

namespace OAuth.Net.Components
{
    /// <summary>
    /// A simple consumer which minimally implements IConsumer.
    /// </summary>
    [DebuggerDisplay("Key: {Key} Secret: {Secret} Status: {Status}")]
    public class OAuthConsumer
        : IConsumer
    {
        /// <summary>
        /// Creates a consumer with the given key and secret. The status will
        /// be Unknown.
        /// </summary>
        /// <param name="key">The consumer key</param>
        /// <param name="secret">The consumer secret</param>
        public OAuthConsumer(string key, string secret)
            : this(key, secret, "Unnamed consumer", ConsumerStatus.Unknown)
        {
        }

        /// <summary>
        /// Creates a consumer with the given key and secret. The status will
        /// be Unknown.
        /// </summary>
        /// <param name="key">The consumer key</param>
        /// <param name="secret">The consumer secret</param>
        /// <param name="friendlyName">The consumer's friendly name</param>
        public OAuthConsumer(string key, string secret, string friendlyName)
            : this(key, secret, friendlyName, ConsumerStatus.Unknown)
        {
        }

        /// <summary>
        /// Creates a consumer with the given key, secret and status.
        /// </summary>
        /// <param name="key">The consumer key</param>
        /// <param name="secret">The consumer secret</param>
        /// <param name="friendlyName">The consumer's friendly name</param>
        /// <param name="status">The status of the consumer</param>
        public OAuthConsumer(string key, string secret, string friendlyName, ConsumerStatus status)
        {
            this.Key = key;
            this.Secret = secret;
            this.FriendlyName = friendlyName;
            this.Status = status;
        }

        /// <summary>
        /// The consumer key (this MUST be unique across all consumers)
        /// </summary>
        public string Key
        {
            get;
            protected set;
        }

        /// <summary>
        /// The consumer secret
        /// </summary>
        public string Secret
        {
            get;
            protected set;
        }

        /// <summary>
        /// The status of the consumer
        /// </summary>
        public ConsumerStatus Status
        {
            get;
            set;
        }

        /// <summary>
        /// The name of the consumer. Service providers may choose to expose
        /// this to users.
        /// </summary>
        public string FriendlyName
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (System.Object.ReferenceEquals(this, obj))
                return true;

            if (this.GetType() != obj.GetType())
                return false;

            return this.Equals(obj as OAuthConsumer);
        }

        public bool Equals(OAuthConsumer other)
        {
            return other != null
                && string.Equals(this.Key, other.Key)
                && string.Equals(this.Secret, other.Secret)                
                && this.Status == other.Status;
        }

        public override int GetHashCode()
        {
            return this.Key.GetHashCode() ^ this.Secret.GetHashCode() ^ this.Status.GetHashCode();
        }
    }
}
