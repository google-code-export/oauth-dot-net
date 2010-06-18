// Copyright (c) 2010 Madgex
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
// Authors: Chris Adams
// Website: http://lab.madgex.com/oauth-net/
// Email:   oauth-dot-net@madgex.com


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using OAuth.Net.Consumer;
using OAuth.Net.Components;

namespace OAuth.Net.Examples.YQL2LeggedClient
{        
    public partial class _Default : System.Web.UI.Page
    {                

        private static OAuthConsumer yahooApiConsumer =
            new OAuthConsumer(
                "dj0yJmk9SkR2SWVRUk5uamNVJmQ9WVdrOVVsVk5hRE5FTldjbWNHbzlPVGt6TVRjeE9EWXkmcz1jb25zdW1lcnNlY3JldCZ4PTNj",
                "7d11750639d77d79ac496c7995aee6bcaa77f7dc");

        private static OAuthService yahooService = OAuthService.Create(
            new EndPoint("https://api.login.yahoo.com/oauth/v2/get_request_token"),
            new Uri("https://api.login.yahoo.com/oauth/v2/request_auth"),
            new EndPoint("https://api.login.yahoo.com/oauth/v2/get_token"),
            yahooApiConsumer);


        protected void Page_Load(object sender, EventArgs e)
        {
            OAuthRequest request = OAuthConsumerRequest.Create(
                new EndPoint(@"http://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20search.news%20where%20query%3D%22obama%22&format=xml"),
                _Default.yahooService);

            OAuthResponse response = request.GetResource();
            if (response.HasProtectedResource)
            {
                using( StreamReader reader = new StreamReader( response.ProtectedResource.GetResponseStream() ))
                {
                    YQLDisplay.DocumentContent = reader.ReadToEnd();
                }
            }
        }
    }
}
