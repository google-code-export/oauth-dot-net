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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using OAuth.Net.Common;
using OAuth.Net.Components;
using OAuth.Net.Consumer;

namespace OAuth.Net.Examples.TwitterClient.Api
{
    public class TwitterApi : ITwitterApi
    {
        public static readonly string DateFormat = "ddd MMM dd HH:mm:ss zzz yyyy";

        private static readonly object staticLock = new object();
        private readonly OAuthState state;
        private static OAuthService readServiceDefinition;
        private static OAuthService writeServiceDefinition;

        public TwitterApi(OAuthState state)
        {
            if (state == null)
                throw new ArgumentNullException("state");

            this.state = state;
        }

        /// <summary>
        /// The OAuth service definition for the Twitter client for read 
        /// operations
        /// </summary>
        private static OAuthService ReadServiceDefinition
        {
            get
            {
                if (readServiceDefinition == null)
                    lock (staticLock)
                        if (readServiceDefinition == null)
                            readServiceDefinition = OAuthService.Create(
                                new Uri("http://twitter.com/oauth/request_token"),
                                new Uri("http://twitter.com/oauth/authorize"),
                                new Uri("http://twitter.com/oauth/access_token"),
                                "HMAC-SHA1",
                                new OAuthConsumer(
                                    "S9NZ9AlPtRAJO7O2fHWrg",
                                    "sSHY3q2obL6BKbPyB3QwnuCruWbFH53qocbGaCDf7U"));

                return readServiceDefinition;
            }
        }

        /// <summary>
        /// The OAuth service definition for the Twitter client for write
        /// operations
        /// </summary>
        private static OAuthService WriteServiceDefinition
        {
            get
            {
                if (writeServiceDefinition == null)
                    lock (staticLock)
                        if (writeServiceDefinition == null)
                            writeServiceDefinition = OAuthService.Create(
                                new Uri("http://twitter.com/oauth/request_token"),
                                new Uri("http://twitter.com/oauth/authorize"),
                                new Uri("http://twitter.com/oauth/access_token"),
                                "POST",
                                true,
                                string.Empty,
                                "HMAC-SHA1",
                                Constants.Version1_0,
                                new OAuthConsumer(
                                    "S9NZ9AlPtRAJO7O2fHWrg",
                                    "sSHY3q2obL6BKbPyB3QwnuCruWbFH53qocbGaCDf7U"));

                return writeServiceDefinition;
            }
        }

        /// <summary>
        /// Verifies the user credentials and fetches extended user information
        /// for the current user.
        /// </summary>
        /// <param name="user">(Out) extended user information</param>
        /// <param name="options">API options</param>
        /// <returns>
        /// <para>
        /// Returns <c>true</c> and sets <paramref name="user"/> to a 
        /// representation of the requesting user if authentication was 
        /// successful.
        /// </para>
        /// <para>
        /// Returns <c>false</c> and sets <paramref name="user"/> to null
        /// if authentication was not successful.
        /// </para>
        /// </returns>
        public bool VerifyCredentials(out ExtendedUser user, ApiCallOptions options)
        {
            OAuthResource resource = this.ExecuteRequest(
                TwitterApi.ReadServiceDefinition,
                options,
                null,
                "http://twitter.com/account/verify_credentials.xml");

            switch (resource.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    user = null;
                    return false;

                case HttpStatusCode.OK:
                    user = ExtendedUser.Deserialize(resource.GetResponseStream());
                    return true;

                default:
                    throw new InvalidOperationException(
                        string.Format(
                            CultureInfo.InvariantCulture, 
                            "Unexpected response from Twitter account/verify_credentials.xml: {0}",
                            resource.StatusCode));
            }
        }

        /// <summary>
        /// Retrieves the 20 most recent statuses posted from the authenticated 
        /// user.
        /// </summary>
        /// <param name="options">API options</param>
        /// <returns>List of statuses</returns>
        public ReadOnlyCollection<Status> UserTimeline(ApiCallOptions options)
        {
            OAuthResource resource = this.ExecuteRequest(
                TwitterApi.ReadServiceDefinition,
                options,
                null,
                "http://twitter.com/statuses/user_timeline.xml");

            return new ReadOnlyCollection<Status>(
                new ListSerializationHelper().DeserializeStatusList(
                    resource.GetResponseStream()));
        }

        public Status UpdateStatus(string status, ApiCallOptions options)
        {
            if (status == null)
                throw new ArgumentNullException("status");

            if (string.IsNullOrEmpty(status))
                throw new ArgumentException(
                    "status must not be empty", 
                    "status");

            if (status.Length > 140)
                throw new ArgumentException(
                    "status must not be more than 140 characters", 
                    "status");

            OAuthResource resource = this.ExecuteRequest(
                TwitterApi.WriteServiceDefinition,
                options,
                new NameValueCollection
                {
                    { "status", status }
                },
                "http://twitter.com/statuses/update.xml");

            return Status.Deserialize(resource.GetResponseStream());
        }

        /// <summary>
        /// Executes an <see cref="OAuthRequest"/> for the specified resource 
        /// and returns the resource.
        /// </summary>
        /// <param name="service">The OAuth service to use</param>
        /// <param name="options">API options</param>
        /// <param name="parameters">Additional parameters to send</param>
        /// <param name="uriFormat">Resource URI (optionally with format
        /// placeholders)</param>
        /// <param name="args">Arguments to format with</param>
        /// <returns>
        /// <see cref="OAuthResource"/> representing the response for the 
        /// specified resource
        /// </returns>
        [SuppressMessage("Microsoft.Performance", "CA1820:TestForEmptyStringsUsingStringLength",
            Justification = "Need to distinguish between null and empty")]
        private OAuthResource ExecuteRequest(
            OAuthService service,
            ApiCallOptions options, 
            NameValueCollection parameters,
            string uriFormat, 
            params string[] args)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            if (options.AuthorizationCallbackUri == null)
                throw new ArgumentException("AuthorizationCallbackUri option must not be null", "options");

            if (uriFormat == null)
                throw new ArgumentNullException("uriFormat");

            if (string.IsNullOrEmpty(uriFormat))
                throw new ArgumentException("uriFormat must not be empty", "uriFormat");

            var request = this.CreateRequest(service, uriFormat, args);
            request.OnBeforeGetRequestToken += new EventHandler<PreRequestEventArgs>((sender, preRequestEventArgs) =>
            {
                preRequestEventArgs.CallbackUrl = options.AuthorizationCallbackUri;
            });

            request.OnBeforeGetAccessToken += new EventHandler<PreAccessTokenRequestEventArgs>((sender, preAccesstokenRequestEventArgs) =>
            {
                preAccesstokenRequestEventArgs.Verifier = options.RequestTokenVerifier;
            });

            var response = request.GetResource(parameters);

            if (response.HasProtectedResource)
            {
                // Store access token if present
                IToken accessToken = response.Token as IToken;
                if (accessToken != null)
                    this.state.AccessToken = accessToken;

                return response.ProtectedResource;
            }
            else
            {
                // Store request token if present
                IToken requestToken = response.Token as IToken;
                if (requestToken != null)
                    this.state.RequestToken = requestToken;

                // Throw to indicate authorization is required
                throw new AuthorizationRequiredException(
                    "Authorization required to request " + 
                        string.Format(CultureInfo.InvariantCulture, uriFormat, args))
                {
                    AuthorizationUri = TwitterApi.ReadServiceDefinition.BuildAuthorizationUrl(
                        requestToken,
                        options.AuthorizationArguments)
                };
            }
        }

        /// <summary>
        /// Creates an <see cref="OAuthRequest"/> for the specified resource
        /// </summary>
        /// <param name="service">OAuth service</param>
        /// <param name="uriFormat">Resource URI (optionally with format 
        /// placeholders)</param>
        /// <param name="args">Arguments to format with</param>
        /// <returns>
        /// <see cref="OAuthRequest"/> for the specified resource
        /// </returns>
        private OAuthRequest CreateRequest(
            OAuthService service,
            string uriFormat, 
            params string[] args)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            if (uriFormat == null)
                throw new ArgumentNullException("uriFormat");

            if (string.IsNullOrEmpty(uriFormat))
                throw new ArgumentException("uriFormat must not be empty", "uriFormat");

            return OAuthRequest.Create(
                new Uri(
                    string.Format(
                        CultureInfo.InvariantCulture, 
                        uriFormat, 
                        args)),
                service,
                this.state.RequestToken,
                this.state.AccessToken);
        }
    }
}
