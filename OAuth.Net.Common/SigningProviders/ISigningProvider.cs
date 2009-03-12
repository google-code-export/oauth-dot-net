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

using System.Web;

namespace OAuth.Net.Common
{
    /// <summary>
    /// A signing provider provides signature computation for a given
    /// algorithm (the signature method).
    /// </summary>
    public interface ISigningProvider
    {
        /// <summary>
        /// The signature method that this signing provider provides
        /// </summary>
        string SignatureMethod
        {
            get;
        }

        /// <summary>
        /// Checks the HTTP request satisfies any pre-conditions for the signing
        /// provider. For example, the signing provider may require a secure HTTP
        /// connection.
        /// </summary>
        /// <param name="request">The HTTP request</param>
        /// <returns>True iff the request satisfies all pre-conditions</returns>
        bool CheckRequest(HttpRequest request);

        /// <summary>
        /// Computes a signature. The implementation will vary from provider to 
        /// provider.
        /// </summary>
        /// <param name="signatureBase">The signature base string (see OAuth Core 1.0 §9.1)</param>
        /// <param name="consumerSecret">The consumer secret</param>
        /// <param name="tokenSecret">The token secret, if present</param>
        /// <returns>A signature for the given signature base, consumer secret and token secret</returns>
        string ComputeSignature(string signatureBase, string consumerSecret, string tokenSecret);

        /// <summary>
        /// Checks the signature is a valid for the provided signatureBase.  The implementation will vary from provider to
        /// provider.
        /// </summary>
        /// <param name="signatureBase">The signature base string (see OAuth Core 1.0 §9.1)</param>
        /// <param name="signature">The signature to compare</param>
        /// <param name="consumerSecret">The consumer secret</param>
        /// <param name="tokenSecret">he token secret, if present</param>
        /// <returns>true if the signature provided is valid.</returns>
        bool CheckSignature(string signatureBase, string signature, string consumerSecret, string tokenSecret);  
    }
}
