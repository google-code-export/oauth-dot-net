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
using System.Web;
using OAuth.Net.Common;

namespace OAuth.Net.Consumer
{
    public static class AspNetOAuthRequest
    {
        /// <summary>
        /// Creates a new OAuth protected request configured for an ASP.NET context, 
        /// with the current URL as the callback URL and the current user or session id
        /// used as a state key.
        /// </summary>
        /// <param name="resourceEndPoint">Protected resource End Point</param>
        /// <param name="settings">Service settings</param>
        /// <returns>An OAuth protected request for the protected resource,
        /// configured for an ASP.NET context</returns>
        public static OAuthRequest Create(EndPoint resourceEndPoint, OAuthService settings)
        {
            HttpContext context = HttpContext.Current;

            return AspNetOAuthRequest.Create(
                resourceEndPoint,
                settings,
                context.Request.Url,
                GetEndUserIdFromHttpContextUser() ?? GetEndUserIdFromHttpSession());
        }

        /// <summary>
        /// Creates a new OAuth protected request configured for an ASP.NET context, 
        /// with the the current user or session id used as a state key.
        /// </summary>
        /// <param name="resourceEndPoint">Protected resource End Point</param>
        /// <param name="settings">Service settings</param>
        /// <param name="callbackUri">Callback URI</param>
        /// <returns>An OAuth protected request for the protected resource,
        /// configured for an ASP.NET context</returns>
        public static OAuthRequest Create(EndPoint resourceEndPoint, OAuthService settings, Uri callbackUri)
        {
            return AspNetOAuthRequest.Create(
                resourceEndPoint,
                settings,
                callbackUri,
                GetEndUserIdFromHttpContextUser() ?? GetEndUserIdFromHttpSession());
        }

        /// <summary>
        /// Creates a new OAuth protected request configured for an ASP.NET context.
        /// </summary>
        /// <param name="resourceEndPoint">Protected resource End Point</param>
        /// <param name="settings">Service settings</param>
        /// <param name="callbackUri">Callback URI</param>
        /// <param name="endUserId">End user ID</param>
        /// <returns>An OAuth protected request for the protected resource,
        /// configured for an ASP.NET context</returns>
        public static OAuthRequest Create(EndPoint resourceEndPoint, OAuthService settings, Uri callbackUri, string endUserId)
        {
            OAuthRequest request = OAuthRequest.Create(resourceEndPoint, settings, callbackUri, endUserId);
            request.AuthorizationHandler = AspNetOAuthRequest.HandleAuthorization;
            request.VerificationHandler = AspNetOAuthRequest.HandleVerification;

            return request;
        }

        /// <summary>
        /// Handles verification for an OAuthRequest in an ASP.NET scenario.
        /// </summary>
        /// <param name="sender">OAuthRequest</param>
        /// <param name="args">Authorization args</param>
        public static void HandleVerification(object sender, AuthorizationVerificationEventArgs args)
        {
            HttpContext httpContext = HttpContext.Current;

            // Look for verifier
            string verifier = httpContext.Request.QueryString[Constants.VerifierParameter];
            if (verifier != null)
                args.Verifier = verifier; // Found verifier
        }

        /// <summary>
        /// Handles authorization for an OAuthRequest in an ASP.NET scenario.
        /// </summary>
        /// <param name="sender">OAuthRequest</param>
        /// <param name="args">Authorization args</param>
        public static void HandleAuthorization(object sender, AuthorizationEventArgs args)
        {
            OAuthRequest request = (OAuthRequest)sender;
            HttpContext httpContext = HttpContext.Current;

            // No verifier; redirect to authorization uri
            httpContext.Response.Redirect(
                    request.Service.BuildAuthorizationUrl(args.RequestToken).AbsoluteUri,
                    true);
        }

        /// <summary>
        /// Gets the end user id for the current user stored in the 
        /// HTTP context, if present.
        /// </summary>
        /// <returns>End user id, or <c>null</c></returns>
        public static string GetEndUserIdFromHttpContextUser()
        {
            HttpContext context = HttpContext.Current;
            if (context.User != null && context.User.Identity != null
                    && context.User.Identity.IsAuthenticated)
                return context.User.Identity.Name;

            return null;
        }

        /// <summary>
        /// Gets the current session id as the end user id, if a session is 
        /// present.
        /// </summary>
        /// <returns>End user id, or <c>null</c></returns>
        public static string GetEndUserIdFromHttpSession()
        {
            HttpContext context = HttpContext.Current;
            if (context.Session != null && !string.IsNullOrEmpty(context.Session.SessionID))
                return context.Session.SessionID;

            return null;
        }
    }
}
