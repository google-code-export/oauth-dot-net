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
using System.Web;
using OAuth.Net.Common;

namespace OAuth.Net.ServiceProvider
{
    /// <summary>
    /// AccessTokenHandler handles request to issue access tokens.
    /// </summary>
    public class AccessTokenHandler
        : IHttpHandler
    {
        /// <summary>
        /// Gets a value indicating whether another request can use the 
        /// System.Web.IHttpHandler instance.
        /// </summary>
        public bool IsReusable
        {
            get { return true; }
        }

        /// <summary>
        /// Processes the HTTP web request.
        /// </summary>
        /// <param name="context">HTTP context</param>
        public void ProcessRequest(HttpContext context)
        {
            OAuthRequestContext requestContext = new OAuthRequestContext(new NameValueCollection());         

            // Check request parameters
            try
            {
                // TODO: Should we ensure the realm parameter, if present, matches the configured realm?
                this.ParseParameters(context, requestContext);
                this.SetSigningProvider(context, requestContext);
                this.SetConsumer(context, requestContext);
                this.SetRequestId(context, requestContext);
                this.SetRequestToken(context, requestContext);                                                
                this.SetSignature(context, requestContext);
                this.CheckVerifier(context, requestContext);
            }
            catch (OAuthRequestException ex)
            {
                requestContext.AddError(ex);
                WorkflowHelper.SendBadRequest(context, ex, requestContext.ResponseParameters);
                return;
            }

            // Allow the application to decide whether to issue the access token
            bool isRequestAllowed = this.AllowRequest(context, requestContext);  

            if (isRequestAllowed)
            {   
                // Allow the application to add additional response parameters
                WorkflowHelper.AddApplicationResponseParameters(
                    requestContext,
                    this.GetAdditionalResponseParameters(
                        context, 
                        requestContext));

                // Issue the token
                this.IssueAccessToken(context, requestContext);
                WorkflowHelper.SendOk(context, requestContext.ResponseParameters);
            }
            else
            {   
                // Send an unauthorized response
                WorkflowHelper.SendUnauthorized(context, requestContext.ResponseParameters);
            }
        }

        protected virtual void CheckVerifier(HttpContext httpContext, OAuthRequestContext requestContext)
        {
            if (!ServiceProviderContext.VerificationProvider.IsValid(requestContext.RequestToken, requestContext.Parameters.Verifier))
            {
                OAuthRequestException.ThrowParametersRejected(new string[] { Constants.VerifierParameter }, "Invalid verifier for request token.");
            }
        }

        protected virtual void ParseParameters(HttpContext httpContext, OAuthRequestContext requestContext)
        {
            // Try to parse the parameters
            OAuthParameters parameters = OAuthParameters.Parse(httpContext.Request, ServiceProviderContext.Settings.ParameterSources);

            /*
             * Check for missing required parameters:
             * 
             * The consumer key, signature method, signature, timestamp and nonce parameters
             * are all required
             */
            parameters.RequireAllOf(
                    Constants.ConsumerKeyParameter,
                    Constants.TokenParameter,
                    Constants.SignatureMethodParameter,
                    Constants.SignatureParameter,
                    Constants.TimestampParameter,
                    Constants.NonceParameter,
                    Constants.VerifierParameter);

            /*
             * The version parameter is optional, but it if is present its value must be 1.0
             */
            if (parameters.Version != null)
                parameters.RequireVersion(Constants.Version1_0);

            /*
             * Check that there are no other parameters except for realm, version and
             * the required parameters
             */
            parameters.AllowOnly(
                    Constants.ConsumerKeyParameter,
                    Constants.TokenParameter,
                    Constants.SignatureMethodParameter,
                    Constants.SignatureParameter,
                    Constants.TimestampParameter,
                    Constants.NonceParameter,
                    Constants.VerifierParameter,
                    Constants.VersionParameter, // (optional)
                    Constants.RealmParameter); // (optional)

            requestContext.Parameters = parameters;
        }

        protected virtual void SetSigningProvider(HttpContext httpContext, OAuthRequestContext requestContext)
        {
            WorkflowHelper.SetSigningProvider(httpContext, requestContext);
        }

        protected virtual void SetConsumer(HttpContext httpContext, OAuthRequestContext requestContext)
        {
            WorkflowHelper.SetConsumer(requestContext);
        }

        protected virtual void SetRequestId(HttpContext httpContext, OAuthRequestContext requestContext)
        {
            WorkflowHelper.SetRequestId(requestContext);
        }

        protected virtual void SetRequestToken(HttpContext httpContext, OAuthRequestContext requestContext)
        {
            /*
             * Check the token
             */
            IRequestToken token;
            if (ServiceProviderContext.TokenStore.ContainsRequestToken(requestContext.Parameters.Token))
            {
                token = ServiceProviderContext.TokenStore.GetRequestToken(requestContext.Parameters.Token);

                /*
                 * Ensure the token was issued to the same consumer as this request purports
                 * to be from.
                 */
                if (!token.ConsumerKey.Equals(requestContext.Parameters.ConsumerKey))
                    OAuthRequestException.ThrowTokenRejected(null);

                /*
                 * Ensure the token is valid
                 */
                if (token == null)
                    OAuthRequestException.ThrowTokenRejected(null);
                else
                    switch (token.Status)
                    {
                        case TokenStatus.Authorized:
                            requestContext.RequestToken = token;
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
            else
                OAuthRequestException.ThrowTokenRejected(null);
        }

        protected virtual void SetSignature(HttpContext httpContext, OAuthRequestContext requestContext)
        {
            WorkflowHelper.SetSignature(httpContext, requestContext);
        }

        protected virtual void IssueAccessToken(HttpContext httpContext, OAuthRequestContext requestContext)
        {
            // Generate an access token
            IAccessToken accessToken = this.GenerateAccessToken(httpContext, requestContext);

            // Mark the token as authorized
            accessToken.Status = TokenStatus.Authorized;

            // Store the token
            ServiceProviderContext.TokenStore.Add(accessToken);

            // Mark the request token as used
            requestContext.RequestToken.Status = TokenStatus.Used;
            ServiceProviderContext.TokenStore.Update(requestContext.RequestToken);

            // Add to the response
            requestContext.ResponseParameters[Constants.TokenParameter] = accessToken.Token;
            requestContext.ResponseParameters[Constants.TokenSecretParameter] = accessToken.Secret;
        }

        protected virtual IAccessToken GenerateAccessToken(HttpContext httpContext, OAuthRequestContext requestContext)
        {
            IAccessToken accessToken;
            do
            {
                accessToken = ServiceProviderContext.TokenGenerator.CreateAccessToken(requestContext.Consumer, requestContext.RequestToken);
            }
            while (ServiceProviderContext.TokenStore.Contains(accessToken.Token));

            return accessToken;
        }

        /// <summary>
        /// Determines whether an access token is issued for this request.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// If you need to apply extra rules before allowing an access
        /// token to be issued, you should create an AccessTokenHandler class
        /// which inherits from this &amp; overrides the AllowRequest method
        /// and add it as a HTTP handler for the access token URI.
        /// </para>
        /// 
        /// <para>
        /// If AllowRequest returns <c>true</c> the access token will
        /// be issued. Additional application-specific response 
        /// parameters can be added by overriding the 
        /// <see cref="GetAdditionalResponseParameters"/> method.
        /// </para>
        /// 
        /// <para>
        /// If AllowRequest returns <c>false</c> then a 401 Unauthorized
        /// response will be returned.
        /// </para>
        /// 
        /// <para>
        /// The <paramref name="requestContext"/> parameter contains
        /// the OAuth information parsed from the request.
        /// </para>
        /// </remarks>
        /// 
        /// <param name="httpContext">HTTP context</param>
        /// <param name="requestContext">OAuth request context</param>
        /// 
        /// <returns>
        /// <c>true</c>, if an access token should be issued; <c>false</c>
        /// otherwise
        /// </returns>
        protected virtual bool AllowRequest(HttpContext httpContext, OAuthRequestContext requestContext)
        {
            // By default, requests for access tokens are allowed
            return true;
        }

        /// <summary>
        /// Supplies additional parameters to send in the response.
        /// </summary>
        /// 
        /// <remarks>
        /// Parameter names prefixed with <c>"oauth_"</c> will be stripped.
        /// </remarks>
        /// 
        /// <param name="httpContext">HTTP context</param>
        /// <param name="requestContext">OAuth request context</param>
        /// 
        /// <returns>
        /// A collection of parameters to add to the response, or 
        /// <c>null</c> if no additional parameters should be sent
        /// </returns>
        protected virtual NameValueCollection GetAdditionalResponseParameters(HttpContext httpContext, OAuthRequestContext requestContext)
        {
            // By default, there are no extra parameters
            return null;
        }
    }
}
