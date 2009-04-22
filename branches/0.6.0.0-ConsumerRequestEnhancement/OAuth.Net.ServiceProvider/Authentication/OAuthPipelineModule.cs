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
using System.Diagnostics.CodeAnalysis;
using System.Web;
using OAuth.Net.Common;

namespace OAuth.Net.ServiceProvider
{
    public class OAuthPipelineModule
        : IHttpModule
    {
        /// <summary>
        /// Initialise the module in the context of the given application.
        /// </summary>
        /// <param name="context">Application</param>
        public virtual void Init(HttpApplication context)
        {
            context.AuthenticateRequest += this.HandleAuthenticateRequest;
            ////context.PreSendRequestHeaders += this.HandlePreSendRequestHeaders;
        }

        public virtual void Dispose()
        {
        }
        
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers", Justification = "This is an extension point")]
        public virtual void HandleAuthenticateRequest(object sender, EventArgs args)
        {
            HttpApplication application = (HttpApplication)sender;

            // Don't do anything if another authentication module has set the user already
            if (application.Context.User != null)
                return;

            OAuthRequestContext context = new OAuthRequestContext();
            WorkflowHelper.StoreOAuthContext(application.Context, context);

            try
            {
                this.ParseParameters(application, context);
                this.SetConsumer(application, context);
                this.SetAccessToken(application, context);
            }
            catch (OAuthRequestException ex)
            {
                // The request may not be an OAuth request so don't pass the exception to the consumer
                context.AddError(ex);
                return;
            }

            try
            {
                this.SetSigningProvider(application, context);
                this.SetRequestId(application, context);
                this.SetSignature(application, context);
            }
            catch (OAuthRequestException ex)
            {
                context.AddError(ex);
                WorkflowHelper.SendBadRequest(application.Context, ex, null);
            }

            this.UpdateAccessToken(application, context);
            this.SetUser(application, context);
        }

        protected virtual void ParseParameters(HttpApplication application, OAuthRequestContext context)
        {
            // Try to parse the parameters
            OAuthParameters parameters = OAuthParameters.Parse(application.Request);

            /*
             * Check for missing required parameters:
             * 
             * The consumer key, token, signature method, signature, timestamp and nonce parameters
             * are all required
             */
            parameters.RequireAllOf(
                    Constants.ConsumerKeyParameter,
                    Constants.TokenParameter,
                    Constants.SignatureMethodParameter,
                    Constants.SignatureParameter,
                    Constants.TimestampParameter,
                    Constants.NonceParameter);
            /*
             * The version parameter is optional, but it if is present its value must be 1.0
             */
            if (parameters.Version != null)
                parameters.RequireVersion(Constants.Version1_0);

            context.Parameters = parameters;
        }

        protected virtual void SetSigningProvider(HttpApplication application, OAuthRequestContext context)
        {
            WorkflowHelper.SetSigningProvider(application.Context, context);
        }

        protected virtual void SetConsumer(HttpApplication application, OAuthRequestContext context)
        {
            WorkflowHelper.SetConsumer(context);
        }

        protected virtual void SetAccessToken(HttpApplication application, OAuthRequestContext context)
        {
            /*
             * Check the token
             */
            if (ServiceProviderContext.TokenStore.ContainsAccessToken(context.Parameters.Token))
            {
                IAccessToken accessToken = ServiceProviderContext.TokenStore.GetAccessToken(context.Parameters.Token);

                if (accessToken == null)
                    OAuthRequestException.ThrowTokenRejected(null);
                else
                {
                    /*
                     * Ensure the token was issued to the same consumer as this request purports
                     * to be from.
                     */
                    if (!accessToken.ConsumerKey.Equals(context.Parameters.ConsumerKey))
                        OAuthRequestException.ThrowTokenRejected(null);

                    switch (accessToken.Status)
                    {
                        case TokenStatus.Authorized:
                            context.AccessToken = accessToken;
                            break;

                        case TokenStatus.Expired:
                            OAuthRequestException.ThrowTokenExpired(null);
                            break;

                        case TokenStatus.Used:
                            OAuthRequestException.ThrowTokenUsed(null);
                            break;

                        case TokenStatus.Revoked:
                            OAuthRequestException.ThrowTokenRevoked(null);
                            break;

                        case TokenStatus.Unauthorized:
                        case TokenStatus.Unknown:
                        default:
                            OAuthRequestException.ThrowTokenRejected(null);
                            break;
                    }
                }
            }
            else
                OAuthRequestException.ThrowTokenRejected(null);
        }

        protected virtual void SetRequestId(HttpApplication application, OAuthRequestContext context)
        {
            WorkflowHelper.SetRequestId(context);
        }

        protected virtual void SetSignature(HttpApplication application, OAuthRequestContext context)
        {
            WorkflowHelper.SetSignature(application.Context, context);
        }

        protected virtual void UpdateAccessToken(HttpApplication application, OAuthRequestContext context)
        {
            // TODO: Update access token according to its usage policy...
        }

        protected virtual void SetUser(HttpApplication application, OAuthRequestContext context)
        {
            // Create the principal
            context.Principal = new OAuthPrincipal(context.AccessToken);

            application.Context.User = context.Principal;
        }

        ////#region Advertising and error reporting

        ////[SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers", Justification = "This is an extension point")]
        ////public virtual void HandlePreSendRequestHeaders(object sender, EventArgs args)
        ////{
        ////    HttpApplication application = (HttpApplication)sender;
        ////    HttpResponse response = application.Response;

        ////    OAuthRequestContext context = Utilities.RetrieveOAuthContext(application.Context);

        ////    // TODO: check for any errors that need to be reported

        ////    // If advertising is on, advertise the protection realm
        ////    if (ServiceProviderContext.Settings.AdvertiseOAuthSupport)
        ////    {
        ////        if (response.StatusCode == Constants.UnauthorizedStatusCode)
        ////        {
        ////            #warning TODO: Need to check this is a protected resource before we advertise

        ////            //// WWW-Authenticate: OAuth realm=".."
        ////            //response.AddHeader(Constants.WwwAuthenticateHeaderParam,
        ////            //    Constants.OAuthAuthScheme + " " + Constants.RealmParam
        ////            //        + "=\"" + ServiceProviderContext.Settings.AuthenticationRealm + "\"");
        ////        }
        ////    }
        ////}

        ////#endregion
    }
}
