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
using Castle.Core;
using OAuth.Net.Common;

namespace OAuth.Net.Components
{
    [PerThread]
    public class PlaintextSigningProvider
        : ISigningProvider
    {
        /// <summary>
        /// Creates a PlaintextSigningProvider, which may require secure connections.
        /// </summary>
        /// <param name="doRequireSecureConnection">Whether the signing provider
        /// requires secure connections</param>
        public PlaintextSigningProvider(bool doRequireSecureConnection)
        {
            this.DoRequireSecureConnection = doRequireSecureConnection;
        }

        /// <summary>
        /// This signing provider provides PLAINTEXT signing
        /// </summary>
        public string SignatureMethod
        {
            get { return "PLAINTEXT"; }
        }

        protected bool DoRequireSecureConnection
        {
            get;
            private set;
        }

        /// <summary>
        /// The PLAINTEXT signature method may require a secure connection. This
        /// is controlled by the requireSecureConnection configuration parameter.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual bool CheckRequest(HttpRequest request)
        {
            return !this.DoRequireSecureConnection || request.IsSecureConnection;
        }

        /// <summary>
        /// Computes the PLAINTEXT signature as 
        /// <paramref name="consumerSecret"/>&amp;<paramref name="tokenSecret"/> parameter 
        /// encoded.
        /// </summary>
        /// <param name="signatureBase">The signature base string (see OAuth Core 1.0 §9.1)</param>
        /// <param name="consumerSecret">The consumer secret</param>
        /// <param name="tokenSecret">The token secret, if present</param>
        /// <returns>The computed signature</returns>
        public virtual string ComputeSignature(string signatureBase, string consumerSecret, string tokenSecret)
        {
            string signature = consumerSecret + "&" + tokenSecret;

            return Rfc3986.Encode(signature);
        }

        /// <summary>
        /// Checks the signature provided is the same as that which would be generated
        /// for the consumerSecret, tokenSecret and signatureBase.
        /// </summary>
        /// <param name="signatureBase">The signature base string (see OAuth Core 1.0 §9.1)</param>
        /// <param name="signature">The signature to check.</param>
        /// <param name="consumerSecret">The consumer secret</param>
        /// <param name="tokenSecret">The token secret, if present</param>
        /// <returns></returns>
        public virtual bool CheckSignature(string signatureBase, string signature, string consumerSecret, string tokenSecret)
        {
            string expectedSignature = this.ComputeSignature(
                signatureBase,
                consumerSecret,
                tokenSecret);

            return expectedSignature == signature;
        }
    }
}
