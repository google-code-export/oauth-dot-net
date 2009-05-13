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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Web.UI;
using OAuth.Net.Examples.TwitterClient.Api;

namespace OAuth.Net.Examples.TwitterClient
{
    public partial class UserPage : Page
    {
        public static readonly string UserInfoKey = "User_ExtendedInfo";
        public static readonly string UserTimelineKey = "User_Timeline";

        /// <summary>
        /// The connected user's extended user information
        /// </summary>
        public ExtendedUser TwitterUser { get; private set; }

        /// <summary>
        /// The connected user's status timeline
        /// </summary>
        public IList<Status> UserTimeline { get; private set; }

        /// <summary>
        /// Load page information
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings",
            Justification = "Spurious warning")]
        protected void Page_Load(object sender, EventArgs e)
        {
            OAuthState state = this.Session[OAuthState.SessionKey] as OAuthState;

            // Create state if necessary
            if (state == null)
                this.Session[OAuthState.SessionKey] = state = new OAuthState();

            // Create Twitter API
            TwitterApi api = new TwitterApi(state);
            ApiCallOptions options = new ApiCallOptions
            {
                AuthorizationCallbackUri = new Uri(
                    this.Request.Url, 
                    new VPathResolver().Resolve("~/User.aspx")),

                RequestTokenVerifier = Request[OAuth.Net.Common.Constants.VerifierParameter]
            };

            try
            {
                // Load current user
                // If the user has not yet connected to Twitter, this will
                // throw an AuthorizationRequiredException, handled below
                CachingContainer<ExtendedUser> extendedUserInfoCache
                    = this.Session[UserInfoKey] as CachingContainer<ExtendedUser>;
                
                if (extendedUserInfoCache == null)
                    this.Session[UserInfoKey] = extendedUserInfoCache 
                        = new CachingContainer<ExtendedUser>();

                this.TwitterUser = extendedUserInfoCache.Load(
                    TimeSpan.FromMinutes(5),
                    () => QueryTwitterForCurrentUser(api, options));

                // Load user timeline
                CachingContainer<IList<Status>> userTimelineCache
                    = this.Session[UserTimelineKey] as CachingContainer<IList<Status>>;

                if (userTimelineCache == null)
                    this.Session[UserTimelineKey] = userTimelineCache
                        = new CachingContainer<IList<Status>>();

                this.UserTimeline = userTimelineCache.Load(
                    TimeSpan.FromSeconds(30),
                    () => api.UserTimeline(options));
            }
            catch (AuthorizationRequiredException ex)
            {
                // The user has not yet connected to Twitter
                // Redirect to Twitter for authorization
                this.Response.Redirect(ex.AuthorizationUri.AbsoluteUri, true);
            }
            catch (BadCredentialsException)
            {
                this.Response.Redirect("~/Disconnect.aspx?error=badcredentials", true);
            }
        }

        /// <summary>
        /// Queries Twitter for extended information about the current user
        /// </summary>
        /// <param name="api">Twitter API</param>
        /// <param name="options">API call options</param>
        /// <returns>Extended user object</returns>
        /// <exception cref="BadCredentialsException">If the credentials are
        /// invalid</exception>
        private static ExtendedUser QueryTwitterForCurrentUser(TwitterApi api, ApiCallOptions options)
        {
            if (api == null)
                throw new ArgumentNullException("api");

            ExtendedUser user;
            if (api.VerifyCredentials(out user, options))
                return user;
            else
                throw new BadCredentialsException();
        }
    }
}
