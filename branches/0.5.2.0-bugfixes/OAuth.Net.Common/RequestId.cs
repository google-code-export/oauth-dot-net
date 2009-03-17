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

using System.Diagnostics;

namespace OAuth.Net.Common
{
    [DebuggerDisplay("Timestamp: {Timestamp} Nonce: {Nonce} Consumer Key: {ConsumerKey}")]
    public struct RequestId
    {
        public RequestId(long timestamp, string nonce, string consumerKey)
            : this()
        {
            this.Timestamp = timestamp;
            this.Nonce = nonce;
            this.ConsumerKey = consumerKey;
        }

        /// <summary>
        /// The unix timestamp (number of seconds since Unix epoch)
        /// </summary>
        public long Timestamp
        {
            get;
            private set;
        }

        /// <summary>
        /// The nonce
        /// </summary>
        public string Nonce
        {
            get;
            private set;
        }

        /// <summary>
        /// The consumer key
        /// </summary>
        public string ConsumerKey
        {
            get;
            private set;
        }

        public static bool operator ==(RequestId left, RequestId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RequestId left, RequestId right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (!(obj is RequestId))
                return false;

            return this.Equals((RequestId)obj);
        }

        public bool Equals(RequestId other)
        {
            return this.Timestamp == other.Timestamp
                && this.Nonce.Equals(other.Nonce)
                && this.ConsumerKey.Equals(other.ConsumerKey);
        }

        public override int GetHashCode()
        {
            return this.Timestamp.GetHashCode() ^ this.Nonce.GetHashCode() ^ this.ConsumerKey.GetHashCode();
        }
    }
}
