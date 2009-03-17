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
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace OAuth.Net.Common
{
    [TestFixture]
    public class OAuthParametersTests
    {
        [Test]
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Unit test")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Unit test methods cannot be static")]
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Term makes sense in context")]
        public void Test_EnumParsing_Flags()
        {
            OAuthParameterSources sources = (OAuthParameterSources) Enum.Parse(typeof(OAuthParameterSources), "HttpAuthorizationHeader, HttpPostBody");

            Assert.That(sources & OAuthParameterSources.HttpAuthorizationHeader, Is.EqualTo(OAuthParameterSources.HttpAuthorizationHeader));

            Assert.That(sources & OAuthParameterSources.HttpPostBody, Is.EqualTo(OAuthParameterSources.HttpPostBody));
        }

        [Test]
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Unit test")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Unit test methods cannot be static")]
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Term makes sense in context")]
        public void Test_EnumFormatting_Flags()
        {
            OAuthParameterSources sources = OAuthParameterSources.HttpAuthorizationHeader | OAuthParameterSources.HttpPostBody;

            Assert.That(Enum.Format(typeof(OAuthParameterSources), sources, "G"), Is.EqualTo("HttpAuthorizationHeader, HttpPostBody"));
        }

        [Test]
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Unit test")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Unit test methods cannot be static")]
        public void Test_ParseParameters()
        {
            NameValueCollection queryString = new NameValueCollection();
            NameValueCollection form = new NameValueCollection();
            string authHeader;

            queryString.Add("file", "vacation.jpg");

            form.Add("size", "original");

            authHeader = @"OAuth realm=""http://sp.example.com/"",
            oauth_consumer_key=""0685bd9184jfhq22"",
            oauth_token=""ad180jjd733klru7"",
            oauth_signature_method=""HMAC-SHA1"",
            oauth_signature=""wOJIO9A2W5mFwDgiDvZbTSMK%2FPY%3D"",
            oauth_timestamp=""137131200"",
            oauth_nonce=""4572616e48616d6d65724c61686176"",
            oauth_version=""1.0""";

            OAuthParameters parameters = OAuthParameters.DoParse(authHeader, null, form, queryString, OAuthParameterSources.ServiceProviderDefault, true);

            Assert.That(parameters.AdditionalParameters["file"], Is.EqualTo("vacation.jpg"));
            Assert.That(parameters.AdditionalParameters["size"], Is.EqualTo("original"));
            Assert.That(parameters.Realm, Is.EqualTo("http://sp.example.com/"));
            Assert.That(parameters.ConsumerKey, Is.EqualTo("0685bd9184jfhq22"));
            Assert.That(parameters.Token, Is.EqualTo("ad180jjd733klru7"));
            Assert.That(parameters.SignatureMethod, Is.EqualTo("HMAC-SHA1"));
            Assert.That(parameters.Signature, Is.EqualTo("wOJIO9A2W5mFwDgiDvZbTSMK%2FPY%3D"));
            Assert.That(parameters.Timestamp, Is.EqualTo("137131200"));
            Assert.That(parameters.Nonce, Is.EqualTo("4572616e48616d6d65724c61686176"));
            Assert.That(parameters.Version, Is.EqualTo(Constants.Version1_0));
        }
    }
}
#endif