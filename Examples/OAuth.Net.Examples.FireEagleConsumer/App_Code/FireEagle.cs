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

using System;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Web;
using System.Xml;
using OAuth.Net.Common;
using OAuth.Net.Components;
using OAuth.Net.Consumer;

namespace OAuth.Net.Examples.FireEagleConsumer
{
    public static class FireEagle
    {
        /// <summary>
        /// The Fire Eagle OAuth service definition for GET requests
        /// </summary>
        private static readonly OAuthService GetService = OAuthService.Create(
            new Uri("https://fireeagle.yahooapis.com/oauth/request_token"),
            new Uri("https://fireeagle.yahoo.net/oauth/authorize"),
            new Uri("https://fireeagle.yahooapis.com/oauth/access_token"),
            "GET", 
            false, 
            string.Empty, 
            "HMAC-SHA1", 
            "1.0",
            new OAuthConsumer("rYdWCYtnpoWM", "VjCvuxJbcDWwyFst4btSyv3BYET2KJYw"));

        /// <summary>
        /// The Fire Eagle OAuth service definition for POST requests
        /// </summary>
        private static readonly OAuthService PostService = OAuthService.Create(
            FireEagle.GetService.RequestTokenUrl,
            FireEagle.GetService.AuthorizationUrl,
            FireEagle.GetService.AccessTokenUrl,
            "POST",
            FireEagle.GetService.UseAuthorizationHeader,
            FireEagle.GetService.Realm,
            FireEagle.GetService.SignatureMethod,
            FireEagle.GetService.OAuthVersion,
            FireEagle.GetService.Consumer);

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "callback", Justification = "Callback is a domain term")]
        public static Location GetLocation(HttpContext context, Uri callback)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (callback == null)
                throw new ArgumentNullException("callback");

            // Find the user's location
            var request = OAuthRequest.Create(
                new Uri("https://fireeagle.yahooapis.com/api/0.1/user"), 
                FireEagle.GetService,
                context.Session["request_token"] as IToken,
                context.Session["access_token"] as IToken);

            request.OnBeforeGetRequestToken += new EventHandler<PreRequestEventArgs>((sender,args)=>{
                args.CallbackUrl= callback;   
            });

            request.OnBeforeGetAccessToken += new EventHandler<PreAccessTokenRequestEventArgs>((sender, args) =>
            {
                args.Verifier = context.Request[OAuth.Net.Common.Constants.VerifierParameter];
            });

            OAuthResponse response = request.GetResource();

            if (response.HasProtectedResource)
            {
                // Store the access token
                context.Session["access_token"] = response.Token;

                // Load the response XML
                XmlDocument responseXml = new XmlDocument();
                responseXml.Load(response.ProtectedResource.GetResponseStream());

                // Check the response status
                if (responseXml.SelectSingleNode("rsp/@stat").Value == "ok")
                    return Location.Parse(responseXml.SelectSingleNode("rsp/user/location-hierarchy/location[@best-guess='true']"));
                else
                    return null;
            }
            else
            {
                context.Session["request_token"] = response.Token;

                throw new AuthorizationRequiredException()
                {
                    AuthorizationUri = FireEagle.GetService.BuildAuthorizationUrl(response.Token)
                };
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "callback", Justification = "Callback is a domain term")]
        public static bool UpdateLocation(HttpContext context, Location location, Uri callback)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (location == null)
                throw new ArgumentNullException("location");

            if (callback == null)
                throw new ArgumentNullException("callback");

            // Update the user's location
            var request = OAuthRequest.Create(
                new Uri("https://fireeagle.yahooapis.com/api/0.1/update"),
                FireEagle.PostService,
                context.Session["request_token"] as IToken,
                context.Session["access_token"] as IToken);

            request.OnBeforeGetRequestToken += new EventHandler<PreRequestEventArgs>((sender, args) =>
            {
                args.CallbackUrl = callback;
            });

            request.OnBeforeGetAccessToken += new EventHandler<PreAccessTokenRequestEventArgs>((sender, args) =>
            {
                args.Verifier = context.Request[OAuth.Net.Common.Constants.VerifierParameter];
            });

            // Send the location latitude and longitude with the request
            OAuthResponse response = request.GetResource(
                new NameValueCollection()
                {
                    { "lat", location.Point.Latitude.ToString(CultureInfo.InvariantCulture) },
                    { "lon", location.Point.Longitude.ToString(CultureInfo.InvariantCulture) }
                });

            if (response.HasProtectedResource)
            {
                // Store the access token
                context.Session["access_token"] = response.Token;

                // Load the response XML
                XmlDocument responseXml = new XmlDocument();
                responseXml.Load(response.ProtectedResource.GetResponseStream());

                // Check the response status
                if (responseXml.SelectSingleNode("rsp/@stat").Value == "ok")
                    return true;
                else
                    return false;
            }
            else
            {
                context.Session["request_token"] = response.Token;

                throw new AuthorizationRequiredException()
                {
                    AuthorizationUri = FireEagle.PostService.BuildAuthorizationUrl(response.Token)
                };
            }
        }
    }
}
