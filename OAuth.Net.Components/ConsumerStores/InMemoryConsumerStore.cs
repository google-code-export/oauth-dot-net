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
using System.Collections.ObjectModel;
using OAuth.Net.Common;

namespace OAuth.Net.Components
{
    /// <summary>
    /// An thread-safe, in-memory implementation of IConsumerStore. Items are not persisted across
    /// application restarts.
    /// </summary>
    public class InMemoryConsumerStore
        : IConsumerStore, IOutOfBoundConsumerStore
    {
        protected static readonly object SyncRoot = new object();

        /// <summary>
        /// Create a new in-memory consumer store.
        /// </summary>
        public InMemoryConsumerStore()
        {
            this.ConsumerDictionary = new Dictionary<string, IConsumer>();
            this.UriDictionary = new Dictionary<Uri, string>();
        }

        /// <summary>
        /// Gets all the consumers in the store.
        /// </summary>
        public virtual ICollection<IConsumer> Consumers
        {
            get
            {
                lock (SyncRoot)
                {
                    List<IConsumer> consumers = new List<IConsumer>();

                    foreach (IConsumer consumer in this.ConsumerDictionary.Values)
                        consumers.Add(consumer);

                    return new ReadOnlyCollection<IConsumer>(consumers);
                }
            }
        }

        /// <summary>
        /// The number of consumers held in the store
        /// </summary>
        public virtual long Count
        {
            get
            {
                lock (SyncRoot)
                {
                    return this.ConsumerDictionary.Count;
                }
            }
        }

        protected IDictionary<string, IConsumer> ConsumerDictionary
        {
            get;
            private set;
        }

        /// <summary>
        /// The store to map Uri with Consumer keys
        /// </summary>
        protected IDictionary<Uri, string> UriDictionary
        {
            get;
            private set;
        }

        /// <summary>
        /// Adds the supplied consumer to the consumer store. If the consumer conflicts with a 
        /// consumer already in the store, then <c>false</c> is returned.
        /// </summary>
        /// <param name="consumer">The consumer to store</param>
        /// <returns><c>true</c>, iff the consumer was stored</returns>
        public virtual bool Add(IConsumer consumer)
        {
            if (consumer == null)
                throw new ArgumentNullException("consumer");

            lock (SyncRoot)
            {
                if (this.Contains(consumer.Key))
                    return false;
                else
                    this.ConsumerDictionary[consumer.Key] = consumer;
            }

            return true;
        }

        /// <summary>
        /// Adds the supplied consumer to the consumer store. If the consumer conflicts with a 
        /// consumer already in the store, then <c>false</c> is returned.
        /// Associates the provided uri to the consumer
        /// </summary>
        /// <param name="consumer">The consumer to store</param>
        /// <param name="uri">The uri to assocaite the consumer with</param>
        /// <returns><c>true</c>, iff the consumer was stored</returns>
        public virtual bool Add(IConsumer consumer, Uri uri)
        {
            bool wasAdded = Add(consumer);

            lock (SyncRoot)
            {
                if (!this.UriDictionary.Keys.Contains(uri))
                    this.UriDictionary[uri] = consumer.Key;
            }

            return wasAdded;
        }

        /// <summary>
        /// Checks whether the store contains a consumer with the given
        /// consumer key.
        /// 
        /// This will return true even if the consumer has a status other than <c>Valid</c>.
        /// </summary>
        /// <param name="consumerKey">The consumer key</param>
        /// <returns><c>true</c>, iff there is such a consumer 
        /// in the store</returns>        
        public virtual bool Contains(string consumerKey)
        {
            if (string.IsNullOrEmpty(consumerKey))
                throw new ArgumentException("consumerKey argument is mandatory", "consumerKey");

            lock (SyncRoot)
            {
                return this.ConsumerDictionary.ContainsKey(consumerKey);
            }
        }

        /// <summary>
        /// Gets the consumer registered with the given
        /// consumer key.
        /// </summary>
        /// <param name="consumerKey">The consumer key</param>
        /// <returns>The consumer registered with the consumer key if successful, or
        /// <c>null</c> if there is no consumer registered with the supplied key</returns>
        public virtual IConsumer GetByKey(string consumerKey)
        {
            if (string.IsNullOrEmpty(consumerKey))
                throw new ArgumentException("consumerKey argument is mandatory", "consumerKey");

            lock (SyncRoot)
            {
                if (!this.Contains(consumerKey))
                    return null;
                else
                    return this.ConsumerDictionary[consumerKey];
            }
        }


        /// <summary>
        /// Gets the consumer associated to the given Uri
        /// </summary>
        /// <param name="uri">uri</param>
        /// <returns>The consumer registered with the consumer key if successful, or
        /// <c>null</c> if there is no consumer registered with the supplied key</returns>
        public virtual IConsumer GetByUri(Uri uri)
        {
            if (uri == null)
                throw new ArgumentException("uri argument is mandatory", "uri");

            lock (SyncRoot)
            {
                if (!this.UriDictionary.Keys.Contains(uri))
                    return null;
                else
                    return GetByKey(this.UriDictionary[uri]);
            }
        }

        /// <summary>
        /// Updates the given consumer in the store.
        /// </summary>
        /// <param name="consumer">The consumer to update.</param>
        /// <returns><c>true</c>, iff the consumer was successfully updated
        /// in the store</returns>
        public virtual bool Update(IConsumer consumer)
        {
            if (consumer == null)
                throw new ArgumentNullException("consumer");

            lock (SyncRoot)
            {
                if (!this.Contains(consumer.Key))
                    return false;
                else
                    this.ConsumerDictionary[consumer.Key] = consumer;
            }

            return true;
        }

        /// <summary>
        /// Removes the specified consumer from the store.
        /// </summary>
        /// <param name="consumer">The consumer to remove</param>
        /// <returns><c>true</c>, if the consumer was successfully removed
        /// from the store. This will return <c>false</c> if the consumer 
        /// did not exist in the store.</returns>
        public virtual bool Remove(IConsumer consumer)
        {
            if (consumer == null)
                throw new ArgumentNullException("consumer");

            lock (SyncRoot)
            {
                if (!this.Contains(consumer.Key))
                    return false;
                else
                {
                    if (this.UriDictionary.Values.Contains(consumer.Key))
                    {
                        Uri uriToRemove = null;

                        //There must be a better way to do this.
                        foreach (Uri key in this.UriDictionary.Keys)
                        {
                            if (this.UriDictionary[key].Equals(consumer))
                            {
                                uriToRemove = key;
                                break;
                            }
                        }

                        this.UriDictionary.Remove(uriToRemove);
                    }

                    return this.ConsumerDictionary.Remove(consumer.Key);
                }
            }
        }

        /// <summary>
        /// Remove the association between an URI an a consumer in the store
        /// </summary>
        /// <param name="uri">The uri to remove.</param>
        /// <param name="removeConsumer"><c>true</c> if to remove the consumer from the dictionary.</param>
        /// <returns></returns>
        public virtual bool Remove(Uri uri, bool removeConsumer)
        {
            if (uri == null)
                throw new ArgumentNullException("uri is a mandatory argument", "uri");

            lock (SyncRoot)
            {
                if (!this.UriDictionary.Keys.Contains(uri))
                    return false;
                else
                {
                    if (removeConsumer)
                    {
                        IConsumer consumer = this.GetByKey(this.UriDictionary[uri]);
                        return Remove(consumer);
                    }
                    else
                        return this.UriDictionary.Remove(uri);
                }
            }

        }

        /// <summary>
        /// Remove the association between an URI an a consumer in the store, removing the 
        /// consumer completly from the store.
        /// </summary>
        /// <param name="uri">The uri to remove.</param>
        /// <param name="removeConsumer"><c>true</c> if to remove the consumer from the dictionary.</param>
        /// <returns></returns>
        public virtual bool Remove(Uri uri)
        {
            return Remove(uri, true);
        }

    }
}
