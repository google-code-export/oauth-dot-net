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

#if DEBUG
using System;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using OAuth.Net.Common;

namespace OAuth.Net.TestCases.Common
{
    [TestFixture]
    public class SignatureBaseTests
    {
        ////[Test]
        ////[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Unit test")]
        ////[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Unit test methods cannot be static")]
        ////public void Test_AuthCore1_0_AppendixA_5_1_Example()
        ////{
        ////    NameValueCollection nvc = new NameValueCollection();
        ////    nvc.Add(Constants.ConsumerKeyParameter, "dpf43f3p2l4k3l03");
        ////    nvc.Add(Constants.TokenParameter, "nnch734d00sl2jdk");
        ////    nvc.Add(Constants.SignatureMethodParameter, "HMAC-SHA1");
        ////    nvc.Add(Constants.SignatureParameter, "kd94hf93k423kf44%26hdhd0244k9j7ao03");
        ////    nvc.Add(Constants.TimestampParameter, "1191242096");
        ////    nvc.Add(Constants.NonceParameter, "kllo9940pd9333jh");
        ////    nvc.Add(Constants.VersionParameter, Constants.Version1_0);
        ////    nvc.Add("file", "vacation.jpg");
        ////    nvc.Add("size", "original");

        ////    OAuthParameters @params = OAuthParameters.DoParse(string.Empty, string.Empty, null, nvc, OAuthParameterSources.HttpQueryString, true);

        ////    string basesig = SignatureBase.Create(
        ////        "GET",
        ////        new Uri("http://photos.example.net:80/photos"), // Put port :80 in to test whether default ports are removed
        ////        @params);

        ////    Assert.That(basesig, Is.EqualTo("GET&http%3A%2F%2Fphotos.example.net%2Fphotos&file%3Dvacation.jpg%26oauth_consumer_key%3Ddpf43f3p2l4k3l03%26oauth_nonce%3Dkllo9940pd9333jh%26oauth_signature_method%3DHMAC-SHA1%26oauth_timestamp%3D1191242096%26oauth_token%3Dnnch734d00sl2jdk%26oauth_version%3D1.0%26size%3Doriginal"));
        ////}

        ////[Test]
        ////[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Unit test")]
        ////[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Unit test methods cannot be static")]
        ////public void Test_AuthCore1_0_AppendixA_5_1_Example_WithOddPort()
        ////{
        ////    NameValueCollection nvc = new NameValueCollection();
        ////    nvc.Add(Constants.ConsumerKeyParameter, "dpf43f3p2l4k3l03");
        ////    nvc.Add(Constants.TokenParameter, "nnch734d00sl2jdk");
        ////    nvc.Add(Constants.SignatureParameter, "kd94hf93k423kf44%26hdhd0244k9j7ao03");
        ////    nvc.Add(Constants.SignatureMethodParameter, "HMAC-SHA1");
        ////    nvc.Add(Constants.TimestampParameter, "1191242096");
        ////    nvc.Add(Constants.NonceParameter, "kllo9940pd9333jh");
        ////    nvc.Add(Constants.VersionParameter, Constants.Version1_0);
        ////    nvc.Add("file", "vacation.jpg");
        ////    nvc.Add("size", "original");

        ////    OAuthParameters @params = OAuthParameters.DoParse(string.Empty, string.Empty, null, nvc, OAuthParameterSources.HttpQueryString, true);

        ////    string basesig = SignatureBase.Create(
        ////        "GET",
        ////        new Uri("http://photos.example.net:8888/photos"), // Non-default ports should be present in the base string
        ////        @params);

        ////    Assert.That(basesig, Is.EqualTo("GET&http%3A%2F%2Fphotos.example.net%3A8888%2Fphotos&file%3Dvacation.jpg%26oauth_consumer_key%3Ddpf43f3p2l4k3l03%26oauth_nonce%3Dkllo9940pd9333jh%26oauth_signature_method%3DHMAC-SHA1%26oauth_timestamp%3D1191242096%26oauth_token%3Dnnch734d00sl2jdk%26oauth_version%3D1.0%26size%3Doriginal"));
        ////}

        [Test]
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Unit test")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Unit test methods cannot be static")]
        public void Test_FunnyCharacters()
        {
            OAuthParameters parameters = new OAuthParameters()
            {
                ConsumerKey = "weitu.googlepages.com",
                Nonce = "face868c-04a9-4e75-9534-0b58616c351c",
                SignatureMethod = "RSA-SHA1",
                Timestamp = "1213351382",
                Token = "1/rTf4q3P05rP2xv2xP1ls8mATiaQZnWPB51nTvo8n9Sw",
                Version = "1.0"
            };

            string basesig = SignatureBase.Create(
                "GET",
                new Uri("http://www.google.com/m8/feeds/contacts/default/base"),
                parameters);

            Assert.That(basesig, Is.EqualTo("GET&http%3A%2F%2Fwww.google.com%2Fm8%2Ffeeds%2Fcontacts%2Fdefault%2Fbase&oauth_consumer_key%3Dweitu.googlepages.com%26oauth_nonce%3Dface868c-04a9-4e75-9534-0b58616c351c%26oauth_signature_method%3DRSA-SHA1%26oauth_timestamp%3D1213351382%26oauth_token%3D1%252FrTf4q3P05rP2xv2xP1ls8mATiaQZnWPB51nTvo8n9Sw%26oauth_version%3D1.0"));
        }
    }
}
#endif
