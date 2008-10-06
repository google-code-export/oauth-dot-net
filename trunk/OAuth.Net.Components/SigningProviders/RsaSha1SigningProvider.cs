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
using System.Security.Cryptography.X509Certificates;
using OAuth.Net.Common;

namespace OAuth.Net.Components
{
    /// <summary>
    /// This implementation of the RsaSha1SigningProvider expects a PFX X.509 certificate file
    /// to be available.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "RSA-SHA1 is a domain term")]
    public class RsaSha1SigningProvider
        : ISigningProvider
    {
        /// <summary>
        /// The default X509Certificate used by a consumer to sign a request,
        /// or where only one X509Certificate is used (but why would they?) 
        /// by the Service Provider to check the signature.
        /// </summary>
        private X509Certificate2 certificate;

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "PFX is a domain term")]
        public virtual string PfxFile
        {
            get;
            set;
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "PFX is a domain term")]
        public virtual string PfxPassword
        {
            get;
            set;
        }

        public virtual X509Certificate2 Certificate
        {
            get
            {
                if (this.certificate == null)
                    if (!string.IsNullOrEmpty(this.PfxFile))
                        if (string.IsNullOrEmpty(this.PfxPassword))
                            this.certificate = new X509Certificate2(
                                PathHelper.ResolveUsingEnvironment(this.PfxFile), string.Empty);
                        else
                            this.certificate = new X509Certificate2(
                                PathHelper.ResolveUsingEnvironment(this.PfxFile), this.PfxPassword);
                    else
                        throw new InvalidOperationException("Pfx file cannot be null or empty");

                return this.certificate;
            }
        }

        public string SignatureMethod
        {
            get { return "RSA-SHA1"; }
        }

        public virtual bool CheckRequest(System.Web.HttpRequest request)
        {            
            return true;
        }

        public virtual string ComputeSignature(string signatureBase, string consumerSecret, string tokenSecret)
        {
            if (this.Certificate != null && this.Certificate.PrivateKey != null)
            {
                using (HashAlgorithm hasher = HashAlgorithm.Create("SHA1"))
                {
                    RSAPKCS1SignatureFormatter signatureFormatter = new RSAPKCS1SignatureFormatter(
                        (RSACryptoServiceProvider)this.Certificate.PrivateKey);
                    signatureFormatter.SetHashAlgorithm("SHA1");

                    byte[] sigBaseBytes = System.Text.Encoding.ASCII.GetBytes(signatureBase);
                    byte[] hash = hasher.ComputeHash(sigBaseBytes);

                    return Convert.ToBase64String(signatureFormatter.CreateSignature(hash));
                }
            }

            throw new InvalidOperationException("Required X509 Certificate containing a private key was not found.");
        }

        public virtual bool CheckSignature(string signatureBase, string signature, string consumerSecret, string tokenSecret)
        {
            throw new NotImplementedException();
        }
    }
}
