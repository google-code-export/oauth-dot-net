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

using System.Collections.Generic;
using System.Security.Principal;

namespace OAuth.Net.Common
{
    public interface ITokenStore
    {
        /// <summary>
        /// Gets all the tokens in the store as an array.
        /// </summary>
        ICollection<IToken> Tokens
        {
            get;
        }

        /// <summary>
        /// The total number of tokens held in the store
        /// </summary>
        long Count
        {
            get;
        }

        /// <summary>
        /// Gets all the request tokens in the store as an array.
        /// </summary>
        ICollection<IRequestToken> RequestTokens
        {
            get;
        }

        /// <summary>
        /// The number of request tokens held in the store
        /// </summary>
        long CountRequestTokens
        {
            get;
        }

        /// <summary>
        /// Gets all the access tokens in the store as an array.
        /// </summary>
        ICollection<IAccessToken> AccessTokens
        {
            get;
        }

        /// <summary>
        /// The number of access tokens held in the store
        /// </summary>
        long CountAccessTokens
        {
            get;
        }

        /// <summary>
        /// Adds the supplied request token to the token store. If the token conflicts 
        /// with a token already in the store, then <c>false</c> is
        /// returned.
        /// </summary>
        /// <param name="token">The token to store</param>
        /// <returns><c>true</c>, iff the token was stored</returns>
        bool Add(IRequestToken token);

        /// <summary>
        /// Adds the supplied access token to the token store. If the token conflicts 
        /// with a token already in the store, then <c>false</c> is
        /// returned.
        /// </summary>
        /// <param name="token">The token to store</param>
        /// <returns><c>true</c>, iff the token was stored</returns>
        bool Add(IAccessToken token);

        /// <summary>
        /// Checks whether the store contains ANY token with the given token 
        /// string.
        /// 
        /// This will return true even if the token has a status other than <c>Valid</c>.
        /// </summary>
        /// <param name="token">The token string</param>
        /// <returns><c>true</c>, iff there is such a token in the store</returns>
        bool Contains(string token);

        /// <summary>
        /// Checks whether the store contains a request token with the given token 
        /// string.
        /// 
        /// This will return true even if the token has a status other than <c>Valid</c>.
        /// </summary>
        /// <param name="token">The token string</param>
        /// <returns><c>true</c>, iff there is such a token
        /// in the store</returns>
        bool ContainsRequestToken(string token);

        /// <summary>
        /// Checks whether the store contains an access token with the given token 
        /// string.
        /// 
        /// This will return true even if the token has a status other than <c>Valid</c>.
        /// </summary>
        /// <param name="token">The token string</param>
        /// <returns><c>true</c>, iff there is such a token
        /// in the store</returns>
        bool ContainsAccessToken(string token);

        /// <summary>
        /// Gets the request token with the given token from the store. Returns
        /// <c>null</c> if there is no such token in the store.
        /// </summary>
        /// <param name="token">The token string</param>
        /// <returns>The token matching the inputs, or <c>null</c> if
        /// no such token exists in the store</returns>
        IRequestToken GetRequestToken(string token);

        /// <summary>
        /// Gets the request token with the given token from the store. Returns
        /// <c>null</c> if there is no such token in the store.
        /// </summary>
        /// <param name="token">The token string</param>
        /// <returns>The token matching the inputs, or <c>null</c> if
        /// no such token exists in the store</returns>
        IAccessToken GetAccessToken(string token);

        /// <summary>
        /// Gets an array of request AND access tokens associated with a given
        /// user.
        /// </summary>
        /// <param name="user">The user</param>
        /// <returns>An array of request AND access tokens, or <c>null</c>
        /// if no tokens are associated with the user</returns>
        ICollection<IToken> GetTokensByUser(IIdentity user);

        /// <summary>
        /// Gets an array of request AND access tokens associated with a given
        /// user for a given consumer.
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="consumerKey">The consumer key</param>
        /// <returns>An array of request AND access tokens, or <c>null</c>
        /// if no tokens are associated with the user</returns>
        ICollection<IToken> GetTokensByUser(IIdentity user, string consumerKey);

        /// <summary>
        /// Gets an array of request AND access tokens associated with a given
        /// consumer.
        /// </summary>
        /// <param name="consumerKey">The consumer key</param>
        /// <returns>An array of request AND access tokens, or <c>null</c>
        /// if no tokens are associated with the consumer</returns>
        ICollection<IToken> GetTokensByConsumer(string consumerKey);

        /// <summary>
        /// Updates the the given request token in the store. 
        /// </summary>
        /// <param name="token">The token to update</param>
        /// <returns><c>true</c>, iff the token was successfully updated
        /// in the store.</returns>
        bool Update(IRequestToken token);

        /// <summary>
        /// Updates the the given access token in the store. 
        /// </summary>
        /// <param name="token">The token to update</param>
        /// <returns><c>true</c>, iff the token was successfully updated
        /// in the store.</returns>
        bool Update(IAccessToken token);

        /// <summary>
        /// Removes the specified request token from the store.
        /// </summary>
        /// <param name="token">The token to remove</param>
        /// <returns><c>true</c>, iff the token was successfully removed
        /// from the store. This will return <c>false</c> if the token
        /// did not exist in the store.</returns>
        bool Remove(IRequestToken token);

        /// <summary>
        /// Removes the specified access token from the store.
        /// </summary>
        /// <param name="token">The token to remove</param>
        /// <returns><c>true</c>, iff the token was successfully removed
        /// from the store. This will return <c>false</c> if the token
        /// did not exist in the store.</returns>
        bool Remove(IAccessToken token);
    }
}
