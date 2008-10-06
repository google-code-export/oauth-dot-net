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

using System;
using System.Diagnostics;
using System.Security.Principal;
using OAuth.Net.Common;

namespace OAuth.Net.Components
{
    public class GuidTokenGenerator
        : ITokenGenerator
    {
        /// <summary>
        /// Create a new request token
        /// </summary>
        /// <param name="consumer">The consumer for whom the token is to be created</param>
        /// <param name="parameters">The parameters that were sent in the request that
        /// created this token (both OAuth and additional parameters).</param>
        /// <returns>A request token</returns>
        public virtual IRequestToken CreateRequestToken(IConsumer consumer, OAuthParameters parameters)
        {
            return new OAuthRequestToken(
                GuidHelper.CreateGuid().ToString("D"),
                GuidHelper.CreateGuid().ToString("D"),
                consumer,
                TokenStatus.Unauthorized,
                parameters,
                null,
                new string[] { });
        }

        /// <summary>
        /// Create a new access token
        /// </summary>
        /// <param name="consumer">The consumer for whom the token is to be created</param>
        /// <param name="requestToken">The request token being swapped for this access token</param>
        /// <returns>An access token</returns>
        public virtual IAccessToken CreateAccessToken(IConsumer consumer, IRequestToken requestToken)
        {
            return new OAuthAccessToken(
                GuidHelper.CreateGuid().ToString("D"),
                GuidHelper.CreateGuid().ToString("D"),
                consumer,
                TokenStatus.Unauthorized,
                requestToken);
        }
    }
}
