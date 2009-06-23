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
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using OAuth.Net.Common;

namespace OAuth.Net.Components
{
    /// <summary>
    /// An IRequestIdValidator which requires that timestamps fall within a time window of
    /// the server's time. Requests with timestamps outside this window are rejected. Request
    /// within this window must have nonces that are unique to that consumer key in the time
    /// window. This cuts down the length of time that nonce have to be cached.
    /// 
    /// The consumer key should be validated before the nonce and timestamp
    /// to avoid potential resource attacks involving large numbers of requests
    /// with large numbers of different consumer keys making the cache very large.
    /// </summary>
    public class InMemoryWindowedRequestIdValidator
        : IRequestIdValidator, IDisposable
    {
        protected static readonly object SyncRoot = new object();

        public InMemoryWindowedRequestIdValidator(long windowSize)
        {
            this.HalfWindow = windowSize / 2;
            this.RequestCache = new Dictionary<long, IList<RequestId>>();

            // Spawn a thread to prune the request cache every (1.5 x window) seconds
            this.TidyUpTimer = new Timer(
                this.TidyUp, 
                null, 
                new TimeSpan(0, 0, (int)(1.5 * windowSize)),
                new TimeSpan(0, 0, (int)(1.5 * windowSize)));            
        }

        /// <summary>
        /// The number of seconds +/- server time that a timestamp must 
        /// fall within to be valid
        /// </summary>
        public long HalfWindow
        {
            get;
            protected set;
        }

        // Dictionary of timestamps to lists of request ids
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Nested generic types are necessary here")]
        protected IDictionary<long, IList<RequestId>> RequestCache
        {
            get;
            private set;
        }

        protected Timer TidyUpTimer
        {
            get;
            set;
        }

        public virtual RequestId CheckRequest(string nonce, string timestampValue, string consumerKey, string requestToken)
        {
            // Compute the server time
            long now = UnixTime.ToUnixTime(DateTime.Now);

            // Get and validate the timestamp
            long timestamp = this.ValidateTimestamp(timestampValue, now);

            return this.ValidateNonce(nonce, timestamp, consumerKey, requestToken);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                if (this.TidyUpTimer != null)
                {
                    this.TidyUpTimer.Dispose();
                    this.TidyUpTimer = null;
                }
            }
        }

        // Checks whether the supplied timestamp falls within the window of valid timestamps. The
        // window of valid timestamps is +/- (window / 2) seconds from the current 
        // server time.
        //
        // Returns the timestamp as an integer if it is ok, or throw an OAuthRequestException if it
        // not ok
        protected virtual long ValidateTimestamp(string timestampValue, long now)
        {
            long timestamp;

            // Parse the timestamp (it must be a positive integer) and check 
            // the timestamp is within +/ HalfWindow from the current time  
            if (!long.TryParse(timestampValue, out timestamp)
                    || timestamp <= 0
                    || (now - timestamp) > this.HalfWindow)
            {
                OAuthRequestException.ThrowTimestampRefused(now - this.HalfWindow, now + this.HalfWindow, null);
            }
            else
            {
                return timestamp;
            }

            // We won't get here but the compiler can't know that
            throw new InvalidOperationException("Should be unreachable code");
        }

        // Checks whether the supplied nonce, timestamp and consumer key combination is unique within
        // the current window.
        protected virtual RequestId ValidateNonce(string nonce, long timestamp, string consumerKey, string token)
        {
            RequestId currentId = new RequestId(timestamp, nonce, consumerKey, token);

            bool foundClash = false;

            // Lock the request cache while we look for the current id
            lock (SyncRoot)
            {
                if (this.RequestCache.ContainsKey(currentId.Timestamp))
                {
                    // Get all requests with the same timestamp
                    IList<RequestId> requests = this.RequestCache[currentId.Timestamp];
                                        
                    foreach (RequestId request in requests)
                        if (request == currentId)
                        {
                            foundClash = true;
                            break;
                        }
                }

                // If we didn't find a clash, store the current id in the cache
                if (!foundClash)
                {
                    if (!this.RequestCache.ContainsKey(currentId.Timestamp))
                        this.RequestCache[currentId.Timestamp] = new List<RequestId>();

                    this.RequestCache[currentId.Timestamp].Add(currentId);
                }
            }

            // If we did find a clash, throw a nonce used OAuthRequestException
            if (foundClash)
                OAuthRequestException.ThrowNonceUsed(null);

            return currentId;
        }

        protected virtual void TidyUp(object sender)
        {
            long now = UnixTime.ToUnixTime(DateTime.Now);

            long[] timestamps;

            // Lock to get keys
            lock (SyncRoot)
            {
                timestamps = new long[this.RequestCache.Count];
                this.RequestCache.Keys.CopyTo(timestamps, 0);
            }

            foreach (long timestamp in timestamps)
                if ((timestamp + this.HalfWindow) < now)
                    this.RequestCache.Remove(timestamp);
        }
    }
}
