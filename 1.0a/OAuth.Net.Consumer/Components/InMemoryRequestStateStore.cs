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

namespace OAuth.Net.Consumer.Components
{
    public sealed class InMemoryRequestStateStore : IRequestStateStore
    {
        private readonly object SyncRoot = new object();

        private readonly IDictionary<RequestStateKey, RequestState> states
            = new Dictionary<RequestStateKey, RequestState>();

        /// <summary>
        /// Stores the specified request <paramref name="state"/>, overriding 
        /// any previous state with the same key
        /// </summary>
        /// <param name="state">Request state</param>
        public void Store(RequestState state)
        {
            if (state == null)
                throw new ArgumentNullException("state");

            lock (this.SyncRoot)
            {
                this.states[state.Key] = state;
            }
        }

        /// <summary>
        /// Gets the request state stored for the specified <paramref name="key"/>,
        /// creating a new state object if none is stored.
        /// </summary>
        /// <param name="key">State key</param>
        /// <returns>State</returns>
        public RequestState Get(RequestStateKey key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            lock (this.SyncRoot)
            {
                if (this.states.ContainsKey(key))
                    return this.states[key];

                var state = new RequestState(key);
                this.Store(state);
                return state;
            }
        }

        /// <summary>
        /// Permanently removes any state stored for the specified <paramref name="key"/>
        /// </summary>
        /// <param name="key">State key</param>
        public void Delete(RequestStateKey key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            lock (this.SyncRoot)
            {
                if (this.states.ContainsKey(key))
                    this.states.Remove(key);
            }
        }
    }
}

