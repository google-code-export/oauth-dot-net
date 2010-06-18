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
using System.Text;

namespace OAuth.Net.Common
{
    /// <summary>
    /// A verification code that is generated when the user has authenticated the Request Token. This code
    /// is provided in the callback Uri if the user is in-bound or must be displayed to the user to enter
    /// at the consumer to inform them that authorization is complete.
    /// </summary>
    public interface IVerificationProvider
    {
        /// <summary>
        /// Generates a verifier code for the provided token.  The provider must ensure
        /// that this token can be re-generated in a non-guessable manner or store
        /// the generated verification in a store.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        string Generate(IRequestToken token);

        /// <summary>
        /// Determines if the verifier is valid for the Request Token provided.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="verifier"></param>
        /// <returns></returns>
        bool IsValid(IRequestToken token, string verifier);
    }
}
