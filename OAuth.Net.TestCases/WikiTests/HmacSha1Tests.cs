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
using System.Collections.Specialized;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using OAuth.Net.Common;
using OAuth.Net.Components;

namespace OAuth.Net.TestCases.WikiTests
{
    [TestFixture]
    public class HmacSha1Tests
    {
        [Test]
        public void Test1()
        {
            Assert.That(
                new HmacSha1SigningProvider().ComputeSignature(
                    "bs",
                    "cs",
                    null),
                Is.EqualTo("egQqG5AJep5sJ7anhXju1unge2I="));
        }

        [Test]
        public void Test2()
        {
            Assert.That(
                   new HmacSha1SigningProvider().ComputeSignature(
                    "bs",
                    "cs",
                    "ts"),
                Is.EqualTo("VZVjXceV7JgPq/dOTnNmEfO0Fv8="));
        }

        [Test]
        public void Test3()
        {
            Assert.That(
                   new HmacSha1SigningProvider().ComputeSignature(
                    "GET&http%3A%2F%2Fphotos.example.net%2Fphotos&file%3Dvacation.jpg%26oauth_consumer_key%3Ddpf43f3p2l4k3l03%26oauth_nonce%3Dkllo9940pd9333jh%26oauth_signature_method%3DHMAC-SHA1%26oauth_timestamp%3D1191242096%26oauth_token%3Dnnch734d00sl2jdk%26oauth_version%3D1.0%26size%3Doriginal",
                    "kd94hf93k423kf44",
                    "pfkkdhi9sl3r4s00"),
                Is.EqualTo("tR3+Ty81lMeYAr/Fid0kMTYa/WM="));
        }
    }
}
#endif