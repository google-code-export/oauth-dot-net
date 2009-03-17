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
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("OAuth.Net.Example.FireEagleConsumer")]
[assembly: AssemblyDescription("Example OAuth.net consumer: Fire Eagle demo")]
[assembly: Guid("3399DAF4-3E6C-4e41-901E-1B5707783783")]

#if DEBUG
[assembly: AssemblyConfiguration(OAuth.Net.ProductInfo.DebugConfiguration)]
#elif RELEASE
[assembly: AssemblyConfiguration(OAuth.Net.ProductInfo.ReleaseConfiguration)]
#endif

[assembly: AssemblyCompany(OAuth.Net.ProductInfo.Company)]
[assembly: AssemblyProduct(OAuth.Net.ProductInfo.Product)]
[assembly: AssemblyCopyright(OAuth.Net.ProductInfo.Copyright)]
[assembly: AssemblyTrademark(OAuth.Net.ProductInfo.Trademark)]
[assembly: ComVisible(OAuth.Net.ProductInfo.IsComVisible)]
[assembly: CLSCompliant(OAuth.Net.ProductInfo.IsClsCompliant)]

[assembly: AssemblyVersion("1.0.0.0")]