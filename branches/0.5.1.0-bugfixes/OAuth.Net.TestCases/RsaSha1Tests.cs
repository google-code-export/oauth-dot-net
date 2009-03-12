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

#if DEBUG
using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using OAuth.Net.Common;
using OAuth.Net.Components;

namespace OAuth.Net.TestCases
{
    [TestFixture]
    public class RsaSha1Tests
    {
        [Test]
        public void TestCase()
        {
            ISigningProvider rsa = new RsaSha1SigningProvider()
            {
                // PFX file is copied to output directory
                PfxFile = "testcase.rsa.pfx"
            };

            OAuthParameters parameters = new OAuthParameters()
            {
                SignatureMethod = "RSA-SHA1",
                Version = Constants.Version1_0,
                ConsumerKey = "dpf43f3p2l4k3l03",
                Timestamp = "1196666512",
                Nonce = "13917289812797014437"
            };
            parameters.AdditionalParameters.Add("file", "vacaction.jpg");
            parameters.AdditionalParameters.Add("size", "original");

            Uri baseUri = new Uri("http://photos.example.net/photos");

            string sigbase = SignatureBase.Create(
                "GET", 
                baseUri, 
                parameters);

            Assert.That(sigbase, Is.EqualTo("GET&http%3A%2F%2Fphotos.example.net%2Fphotos&file%3Dvacaction.jpg%26oauth_consumer_key%3Ddpf43f3p2l4k3l03%26oauth_nonce%3D13917289812797014437%26oauth_signature_method%3DRSA-SHA1%26oauth_timestamp%3D1196666512%26oauth_version%3D1.0%26size%3Doriginal"));

            parameters.Signature = rsa.ComputeSignature(sigbase, "kd94hf93k423kf44", null);

            Assert.That(parameters.Signature, Is.EqualTo("jvTp/wX1TYtByB1m+Pbyo0lnCOLIsyGCH7wke8AUs3BpnwZJtAuEJkvQL2/9n4s5wUmUl4aCI4BwpraNx4RtEXMe5qg5T1LVTGliMRpKasKsW//e+RinhejgCuzoH26dyF8iY2ZZ/5D1ilgeijhV/vBka5twt399mXwaYdCwFYE="));

            // There is no point comparing the URLs because order is not query string parameter important in URLs
        }
    }
}
#endif