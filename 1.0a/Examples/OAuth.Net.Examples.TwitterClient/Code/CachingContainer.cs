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

namespace OAuth.Net.Examples.TwitterClient
{
    /// <summary>
    /// Poor man's time-limited caching container
    /// </summary>
    /// <typeparam name="TType">Type of item to be cached in container</typeparam>
    public class CachingContainer<TType>
    {
        private TType item;
        private DateTime populatedDate;

        /// <summary>
        /// Loads the cached item. If it has not been generated or has expired,
        /// it will be regenerated.
        /// </summary>
        /// <param name="expiryAge">
        /// Age at which to expire the item, or <c>0</c> to never expire the 
        /// item
        /// </param>
        /// <param name="itemGenerator">Function which generates the item</param>
        /// <returns>The item</returns>
        public TType Load(TimeSpan expiryAge, Func<TType> itemGenerator)
        {
            if (expiryAge < new TimeSpan(0))
                throw new ArgumentException("expiryAge must be non-negative", "expiryAge");

            if (itemGenerator == null)
                throw new ArgumentNullException("itemGenerator");

            if ((this.populatedDate + expiryAge) > DateTime.Now
                || (this.populatedDate != DateTime.MinValue && expiryAge == new TimeSpan(0)))
            {
                // Item is populated and has not expired
                return this.item;
            }

            // Item needs to be re-generated
            this.item = itemGenerator();
            this.populatedDate = DateTime.Now;

            return this.item;
        }
    }
}
