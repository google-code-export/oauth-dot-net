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
using System.Security.Principal;
using OAuth.Net.Common;

namespace OAuth.Net.Components
{
    /// <summary>
    /// An thread-safe, in-memory implementation of ITokenStore. Items are not persisted across
    /// application restarts.
    /// </summary>
    public class InMemoryTokenStore
        : ITokenStore
    {
        protected static readonly object SyncRoot = new object();

        /// <summary>
        /// Create a new in-memory token store.
        /// </summary>
        public InMemoryTokenStore()
        {
            this.RequestTokenDictionary = new Dictionary<string, IRequestToken>();
            this.AccessTokenDictionary = new Dictionary<string, IAccessToken>();
        }

        /// <summary>
        /// Gets all the tokens in the store as an array.
        /// </summary>
        public virtual ICollection<IIssuedToken> Tokens
        {
            get
            {
                lock (SyncRoot)
                {
                    var tokens = new List<IIssuedToken>();

                    foreach (IRequestToken token in this.RequestTokenDictionary.Values)
                        tokens.Add(token);

                    foreach (IAccessToken token in this.AccessTokenDictionary.Values)
                        tokens.Add(token);

                    return new ReadOnlyCollection<IIssuedToken>(tokens);
                }
            }
        }

        /// <summary>
        /// The total number of tokens held in the store
        /// </summary>
        public virtual long Count
        {
            get
            {
                lock (SyncRoot)
                {
                    return this.RequestTokenDictionary.Count + this.AccessTokenDictionary.Count;
                }
            }
        }

        /// <summary>
        /// Gets all the request tokens in the store as an array.
        /// </summary>
        public virtual ICollection<IRequestToken> RequestTokens
        {
            get
            {
                lock (SyncRoot)
                {
                    return new ReadOnlyCollection<IRequestToken>(
                        new List<IRequestToken>(this.RequestTokenDictionary.Values));
                }
            }
        }

        /// <summary>
        /// The number of request tokens held in the store
        /// </summary>
        public virtual long CountRequestTokens
        {
            get
            {
                lock (SyncRoot)
                {
                    return this.RequestTokenDictionary.Count;
                }
            }
        }

        /// <summary>
        /// Gets all the access tokens in the store as an array.
        /// </summary>
        public virtual ICollection<IAccessToken> AccessTokens
        {
            get
            {
                lock (SyncRoot)
                {
                    return new ReadOnlyCollection<IAccessToken>(
                        new List<IAccessToken>(this.AccessTokenDictionary.Values));
                }
            }
        }

        /// <summary>
        /// The number of access tokens held in the store
        /// </summary>
        public virtual long CountAccessTokens
        {
            get
            {
                lock (SyncRoot)
                {
                    return this.AccessTokenDictionary.Count;
                }
            }
        }

        protected IDictionary<string, IRequestToken> RequestTokenDictionary
        {
            get;
            private set;
        }

        protected IDictionary<string, IAccessToken> AccessTokenDictionary
        {
            get;
            private set;
        }

        /// <summary>
        /// Adds the supplied request token to the token store. If the token conflicts 
        /// with a token already in the store, then <c>false</c> is
        /// returned.
        /// </summary>
        /// <param name="token">The token to store</param>
        /// <returns><c>true</c>, iff the token was stored</returns>
        public virtual bool Add(IRequestToken token)
        {
            if (token == null)
                throw new ArgumentNullException("token");

            lock (SyncRoot)
            {
                if (this.Contains(token.Token))
                    return false;
                else
                    this.RequestTokenDictionary[token.Token] = token;
            }

            return true;
        }

        /// <summary>
        /// Adds the supplied access token to the token store. If the token conflicts 
        /// with a token already in the store, then <c>false</c> is
        /// returned.
        /// </summary>
        /// <param name="token">The token to store</param>
        /// <returns><c>true</c>, iff the token was stored</returns>
        public virtual bool Add(IAccessToken token)
        {
            if (token == null)
                throw new ArgumentNullException("token");

            lock (SyncRoot)
            {
                if (this.Contains(token.Token))
                    return false;
                else
                    this.AccessTokenDictionary[token.Token] = token;
            }

            return true;
        }

        /// <summary>
        /// Checks whether the store contains ANY token with the given token 
        /// string.
        /// 
        /// This will return true even if the token has a status other than <c>Valid</c>.
        /// </summary>
        /// <param name="token">The token string</param>
        /// <returns><c>true</c>, iff there is such a token in the store</returns>
        public virtual bool Contains(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("token argument is mandatory", "token");

            return this.ContainsRequestToken(token) || this.ContainsAccessToken(token);
        }

        /// <summary>
        /// Checks whether the store contains a request token with the given token 
        /// string.
        /// 
        /// This will return true even if the token has a status other than <c>Valid</c>.
        /// </summary>
        /// <param name="token">The token string</param>
        /// <returns><c>true</c>, iff there is such a token
        /// in the store</returns>
        public virtual bool ContainsRequestToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("token argument is mandatory", "token");

            lock (SyncRoot)
            {
                return this.RequestTokenDictionary.ContainsKey(token);
            }
        }

        /// <summary>
        /// Checks whether the store contains an access token with the given token 
        /// string.
        /// 
        /// This will return true even if the token has a status other than <c>Valid</c>.
        /// </summary>
        /// <param name="token">The token string</param>
        /// <returns><c>true</c>, iff there is such a token
        /// in the store</returns>
        public virtual bool ContainsAccessToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("token argument cannot be null or empty", "token");

            lock (SyncRoot)
            {
                return this.AccessTokenDictionary.ContainsKey(token);
            }
        }

        /// <summary>
        /// Gets the request token with the given token from the store. Returns
        /// <c>null</c> if there is no such token in the store.
        /// </summary>
        /// <param name="token">The token string</param>
        /// <returns>The token matching the inputs, or <c>null</c> if
        /// no such token exists in the store</returns>
        public virtual IRequestToken GetRequestToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("token argument is mandatory", "token");

            lock (SyncRoot)
            {
                if (!this.ContainsRequestToken(token))
                    return null;
                else
                    return this.RequestTokenDictionary[token];
            }
        }

        /// <summary>
        /// Gets the request token with the given token from the store. Returns
        /// <c>null</c> if there is no such token in the store.
        /// </summary>
        /// <param name="token">The token string</param>
        /// <returns>The token matching the inputs, or <c>null</c> if
        /// no such token exists in the store</returns>
        public virtual IAccessToken GetAccessToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("token argument is mandatory", "token");

            lock (SyncRoot)
            {
                if (!this.ContainsAccessToken(token))
                    return null;
                else
                    return this.AccessTokenDictionary[token];
            }
        }

        /// <summary>
        /// Gets an array of request AND access tokens associated with a given
        /// user.
        /// 
        /// NB: In this implementation, this is an inefficient lookup as it requires
        /// a scan of all the tokens in the store.
        /// </summary>
        /// <param name="user">The user</param>
        /// <returns>A collection of request AND access tokens, or <c>null</c>
        /// if no tokens are associated with the user</returns>
        public virtual ICollection<IIssuedToken> GetTokensByUser(IIdentity user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return this.GetByPredicate(
                (IRequestToken token) => token.AuthenticatedUser.Equals(user),
                (IAccessToken token) => token.RequestToken.AuthenticatedUser.Equals(user));
        }

        /// <summary>
        /// Gets an array of request AND access tokens associated with a given
        /// user for a given consumer.
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="consumerKey">The consumer key</param>
        /// <returns>A collection of request AND access tokens, or <c>null</c>
        /// if no tokens are associated with the user</returns>
        public virtual ICollection<IIssuedToken> GetTokensByUser(IIdentity user, string consumerKey)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (string.IsNullOrEmpty(consumerKey))
                throw new ArgumentException("consumerKey argument is mandatory", "consumerKey");

            return this.GetByPredicate(
                (IRequestToken token) => token.ConsumerKey.Equals(consumerKey)
                                            && token.AuthenticatedUser.Equals(user),
                (IAccessToken token) => token.ConsumerKey.Equals(consumerKey)
                                            && token.RequestToken.AuthenticatedUser.Equals(user));
        }

        /// <summary>
        /// Gets an array of request AND access tokens associated with a given
        /// consumer.
        /// </summary>
        /// <param name="consumerKey">The consumer key</param>
        /// <returns>A collection of request AND access tokens, or <c>null</c>
        /// if no tokens are associated with the consumer</returns>
        public virtual ICollection<IIssuedToken> GetTokensByConsumer(string consumerKey)
        {
            if (string.IsNullOrEmpty(consumerKey))
                throw new ArgumentException("consumerKey argument is mandatory", "consumerKey");

            return this.GetByPredicate(
                (IRequestToken token) => token.ConsumerKey.Equals(consumerKey),
                (IAccessToken token) => token.ConsumerKey.Equals(consumerKey));
        }

        /// <summary>
        /// Updates the the given request token in the store. 
        /// </summary>
        /// <param name="token">The token to update</param>
        /// <returns><c>true</c>, iff the token was successfully updated
        /// in the store.</returns>
        public virtual bool Update(IRequestToken token)
        {
            if (token == null)
                throw new ArgumentNullException("token");

            lock (SyncRoot)
            {
                if (!this.ContainsRequestToken(token.Token))
                    return false;
                else
                    this.RequestTokenDictionary[token.Token] = token;
            }

            return true;
        }

        /// <summary>
        /// Updates the the given access token in the store. 
        /// </summary>
        /// <param name="token">The token to update</param>
        /// <returns><c>true</c>, iff the token was successfully updated
        /// in the store.</returns>
        public virtual bool Update(IAccessToken token)
        {
            if (token == null)
                throw new ArgumentNullException("token");

            lock (SyncRoot)
            {
                if (!this.ContainsAccessToken(token.Token))
                    return false;
                else
                    this.AccessTokenDictionary[token.Token] = token;
            }

            return true;
        }

        /// <summary>
        /// Removes the specified request token from the store.
        /// </summary>
        /// <param name="token">The token to remove</param>
        /// <returns><c>true</c>, iff the token was successfully removed
        /// from the store. This will return <c>false</c> if the token
        /// did not exist in the store.</returns>
        public virtual bool Remove(IRequestToken token)
        {
            if (token == null)
                throw new ArgumentNullException("token");

            lock (SyncRoot)
            {
                if (!this.ContainsRequestToken(token.Token))
                    return false;
                else
                    return this.RequestTokenDictionary.Remove(token.Token);
            }
        }

        /// <summary>
        /// Removes the specified access token from the store.
        /// </summary>
        /// <param name="token">The token to remove</param>
        /// <returns><c>true</c>, iff the token was successfully removed
        /// from the store. This will return <c>false</c> if the token
        /// did not exist in the store.</returns>
        public virtual bool Remove(IAccessToken token)
        {
            if (token == null)
                throw new ArgumentNullException("token");

            lock (SyncRoot)
            {
                if (!this.ContainsAccessToken(token.Token))
                    return false;
                else
                    return this.AccessTokenDictionary.Remove(token.Token);
            }
        }

        /// <summary>
        /// Gets an array of request AND access tokens that match the supplied predicates.
        /// </summary>
        /// <param name="requestPredicate">The predicate for request tokens</param>
        /// <param name="accessPredicate">The predicate for access tokens</param>
        /// <returns>An array of request AND access tokens, or <c>null</c>
        /// if no tokens match the predicates</returns>
        protected virtual ICollection<IIssuedToken> GetByPredicate(
            Predicate<IRequestToken> requestPredicate,
            Predicate<IAccessToken> accessPredicate)
        {
            var matches = new List<IIssuedToken>();

            lock (SyncRoot)
            {
                foreach (IRequestToken token in this.RequestTokenDictionary.Values)
                    if (requestPredicate(token))
                        matches.Add(token);

                foreach (IAccessToken token in this.AccessTokenDictionary.Values)
                    if (accessPredicate(token))
                        matches.Add(token);
            }

            if (matches.Count == 0)
                return null;
            else
                return new ReadOnlyCollection<IIssuedToken>(matches);
        }
    }
}
