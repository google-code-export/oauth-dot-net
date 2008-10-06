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

namespace OAuth.Net.Common
{
    /// <summary>
    /// Stores information about previous requests to validate future timestamps
    /// and nonces.
    /// </summary>
    public interface IRequestIdValidator
    {
        /// <summary>
        /// Checks that the timestamp is valid and that the nonce has not been used 
        /// before by the same consumer, according to the timestamp/nonce policies 
        /// of the implementation.
        /// 
        /// Throws an OAuthRequestException if there is a problem with the nonce or timestamp.
        /// </summary>
        /// <param name="nonce">The nonce</param>
        /// <param name="timestampValue">The timestamp</param>
        /// <param name="consumerKey">The consumer key</param>
        /// <returns>A RequestId (formed of the timestamp, nonce and consumer key)
        /// if the request is ok</returns>
        /// <exception cref="OAuth.Net.Common.OAuthRequestException" />
        RequestId CheckRequest(string nonce, string timestampValue, string consumerKey);
    }
}
