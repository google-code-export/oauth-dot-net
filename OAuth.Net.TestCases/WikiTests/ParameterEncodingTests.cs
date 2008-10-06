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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using OAuth.Net.Common;

namespace OAuth.Net.TestCases.WikiTests
{
    [TestFixture]
    public class ParameterEncodingTests
    {
        [Test]        
        public void TestAlphanumerics()
        {
            Assert.That(Rfc3986.Encode("abcABC123"), Is.EqualTo("abcABC123"));
        }

        [Test]
        public void TestDashDotUnderscoreTilde()
        {
            Assert.That(Rfc3986.Encode("-._~"), Is.EqualTo("-._~"));
        }

        [Test]
        public void TestPercent()
        {
            Assert.That(Rfc3986.Encode("%"), Is.EqualTo("%25"));
        }

        [Test]
        public void TestPlus()
        {
            Assert.That(Rfc3986.Encode("+"), Is.EqualTo("%2B"));
        }

        [Test]
        public void TestAmpersandEqualsAsterisk()
        {
            Assert.That(Rfc3986.Encode("&=*"), Is.EqualTo("%26%3D%2A"));
        }

        [Test]
        public void TestUnicodeU000A()
        {
            Assert.That(Rfc3986.Encode("\u000A"), Is.EqualTo("%0A"));
        }

        [Test]
        public void TestUnicodeU0020()
        {
            Assert.That(Rfc3986.Encode("\u0020"), Is.EqualTo("%20"));
        }

        [Test]
        public void TestUnicodeU007F()
        {
            Assert.That(Rfc3986.Encode("\u007F"), Is.EqualTo("%7F"));
        }

        [Test]
        public void TestUnicodeU0080()
        {
            Assert.That(Rfc3986.Encode("\u0080"), Is.EqualTo("%C2%80"));
        }

        [Test]
        public void TestUnicodeU3001()
        {
            Assert.That(Rfc3986.Encode("\u3001"), Is.EqualTo("%E3%80%81"));
        }
    }
}
#endif