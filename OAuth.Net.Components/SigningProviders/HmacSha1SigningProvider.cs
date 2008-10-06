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
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using OAuth.Net.Common;

namespace OAuth.Net.Components
{
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "HMAC-SHA1 is a domain term")]
    public class HmacSha1SigningProvider
        : ISigningProvider
    {
        /// <summary>
        /// This signing provider provides HMAC-SHA1 signing
        /// </summary>        
        public string SignatureMethod
        {
            get { return "HMAC-SHA1"; }
        }

        /// <summary>
        /// The HMAC-SHA1 signature method has no pre-conditions
        /// </summary>
        /// <param name="request">The HTTP request</param>
        /// <returns>True</returns>
        public virtual bool CheckRequest(HttpRequest request)
        {
            return true;
        }

        /// <summary>
        /// Computes the HMAC-SHA1 signature using <paramref name="signatureBase"/> as the text
        /// and <paramref name="consumerSecret"/>&amp;<paramref name="tokenSecret"/> parameter 
        /// encoded as the key.
        /// </summary>
        /// <param name="signatureBase">The signature base string (see OAuth Core 1.0 §9.1)</param>
        /// <param name="consumerSecret">The consumer secret</param>
        /// <param name="tokenSecret">The token secret, if present</param>
        /// <returns>The computed signature</returns>
        public virtual string ComputeSignature(string signatureBase, string consumerSecret, string tokenSecret)
        {
            using (HMACSHA1 crypto = new HMACSHA1())
            {
                string key = Rfc3986.Encode(consumerSecret) + "&" 
                                + Rfc3986.Encode(tokenSecret);

                crypto.Key = Encoding.ASCII.GetBytes(key);

                string hash = Convert.ToBase64String(crypto.ComputeHash(Encoding.ASCII.GetBytes(signatureBase)));

                crypto.Clear();

                return hash;
            }
        }

        public virtual bool CheckSignature(string signatureBase, string signature, string consumerSecret, string tokenSecret)
        {                        
            string expectedSignature = this.ComputeSignature(
                signatureBase,
                consumerSecret,
                tokenSecret);

            string actualSignature = Rfc3986.Decode(signature);

            return expectedSignature == actualSignature;
        }
    }
}
