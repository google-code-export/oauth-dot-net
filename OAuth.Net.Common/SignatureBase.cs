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
using System.Text;

namespace OAuth.Net.Common
{
    public static class SignatureBase
    {
        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "OAuth Core 1.0 mandates lower case")]
        public static string Create(string httpMethod, Uri requestUrl, OAuthParameters parameters)
        {
            StringBuilder sigbase = new StringBuilder();

            // Http header
            sigbase.Append(Rfc3986.Encode(httpMethod)).Append("&");

            // Normalized request URL
            sigbase.Append(Rfc3986.Encode(requestUrl.Scheme));
            sigbase.Append(Rfc3986.Encode("://"));
            sigbase.Append(Rfc3986.Encode(requestUrl.Authority.ToLowerInvariant()));
            sigbase.Append(Rfc3986.Encode(requestUrl.AbsolutePath));
            sigbase.Append("&");
            
            // Normalized parameters
            sigbase.Append(Rfc3986.Encode(parameters.ToNormalizedString(
                    Constants.RealmParameter,
                    Constants.SignatureParameter,
                    Constants.TokenSecretParameter)));

            return sigbase.ToString();
        }
    }
}
