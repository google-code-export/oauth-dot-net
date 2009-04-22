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
using System.Net;
using System.Web;
using OAuth.Net.Common;

namespace OAuth.Net.ServiceProvider
{
    public static class WorkflowHelper
    {
        /// <summary>
        /// Stores the OAuth request context in the HTTP context items dictionary.
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <param name="oauthContext">OAuth request context</param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "OAuth is a domain term")]
        public static void StoreOAuthContext(HttpContext httpContext, OAuthRequestContext oauthContext)
        {
            httpContext.Items[typeof(OAuthRequestContext)] = oauthContext;
        }

        /// <summary>
        /// Retrieves the OAuth request context from the HTTP context items dictionary.
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <returns>The OAuth request context, or <c>null</c> if it has not been
        /// stored.</returns>
        public static OAuthRequestContext RetrieveOAuthContext(HttpContext httpContext)
        {
            return (OAuthRequestContext) httpContext.Items[typeof(OAuthRequestContext)];
        }

        /// <summary>
        /// Send a OK response with the response parameters in the body.
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <param name="responseParameters">Response parameters</param>
        public static void SendOk(HttpContext context, NameValueCollection responseParameters)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            // There is a problem with the parameters; return 401 Bad Request
            context.Response.StatusCode = (int)HttpStatusCode.OK;

            // Write the response
            if (responseParameters != null && responseParameters.Count > 0)
                context.Response.Write(Rfc3986.EncodeAndJoin(responseParameters));

            context.Response.End();
        }

        /// <summary>
        /// Send a Bad Request response with the OAuthRequestException details in the header
        /// and the response parameters in the body.
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <param name="exception">OAuth exception</param>
        /// <param name="responseParameters">Response parameters</param>
        public static void SendBadRequest(HttpContext context, OAuthRequestException exception, NameValueCollection responseParameters)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (exception == null)
                throw new ArgumentNullException("exception");

            // There is a problem with the parameters; return 400 Bad Request
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            // Add the problem report in the WWW-Authenticate header
            context.Response.AddHeader(
                Constants.WwwAuthenticateHeaderParameter,
                exception.ToHeaderFormat(ServiceProviderContext.Settings.AuthenticationRealm));

            // Write the response
            if (responseParameters != null && responseParameters.Count > 0)
                context.Response.Write(Rfc3986.EncodeAndJoin(responseParameters));
            
            context.Response.End();
        }

        /// <summary>
        /// Send an Unauthorized response with the response parameters in the body.
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <param name="responseParameters">Response parameters</param>
        public static void SendUnauthorized(HttpContext context, NameValueCollection responseParameters)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            // There is a problem with the parameters; return 401 Bad Request
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

            // Write the response
            if (responseParameters != null && responseParameters.Count > 0)
                context.Response.Write(Rfc3986.EncodeAndJoin(responseParameters));

            context.Response.End();
        }

        public static void SetSigningProvider(HttpContext httpContext, OAuthRequestContext context)
        {
            /*
             * Check there is a signing provider for the signature method
             */
            ISigningProvider signingProvider = ServiceProviderContext.GetSigningProvider(context.Parameters.SignatureMethod);

            if (signingProvider == null)
            {
                // There is no signing provider for this signature method
                OAuthRequestException.ThrowSignatureMethodRejected(null);
            }

            // Double check the signing provider declares that it can handle the signature method
            if (!signingProvider.SignatureMethod.Equals(context.Parameters.SignatureMethod))
                OAuthRequestException.ThrowSignatureMethodRejected(null);

            // Ask the signing provider to check the request for pre-conditions
            if (!signingProvider.CheckRequest(httpContext.Request))
                OAuthRequestException.ThrowSignatureMethodRejected(null);

            context.SigningProvider = signingProvider;
        }

        public static void SetConsumer(OAuthRequestContext context)
        {
            /*
             * Get the consumer by its consumer key
             */
            if (ServiceProviderContext.ConsumerStore.Contains(context.Parameters.ConsumerKey))
            {
                IConsumer consumer = ServiceProviderContext.ConsumerStore.GetByKey(context.Parameters.ConsumerKey);

                if (consumer == null)
                    OAuthRequestException.ThrowConsumerKeyUnknown(null);
                else
                    switch (consumer.Status)
                    {
                        case ConsumerStatus.Valid:
                            context.Consumer = consumer;
                            break;

                        case ConsumerStatus.TemporarilyDisabled:
                            OAuthRequestException.ThrowConsumerKeyRefused(null);
                            break;

                        case ConsumerStatus.PermanentlyDisabled:
                            OAuthRequestException.ThrowConsumerKeyRejected(null);
                            break;

                        case ConsumerStatus.Unknown:
                        default:
                            OAuthRequestException.ThrowConsumerKeyUnknown(null);
                            break;
                    }
            }
            else
                OAuthRequestException.ThrowConsumerKeyUnknown(null);
        }
        
        public static void SetRequestId(OAuthRequestContext context)
        {
            /*
             * Check the timestamp and nonce
             */
            context.RequestId = ServiceProviderContext.RequestIdValidator.CheckRequest(
                context.Parameters.Nonce,
                context.Parameters.Timestamp,
                context.Parameters.ConsumerKey);
        }

        public static void SetSignature(HttpContext httpContext, OAuthRequestContext requestContext)
        {
            // Get the token to sign with
            string tokenSecret;
            
            if (requestContext.AccessToken != null)
                tokenSecret = requestContext.AccessToken.Secret;
            else if (requestContext.RequestToken != null)
                tokenSecret = requestContext.RequestToken.Secret;
            else
                tokenSecret = null;

            /*
             * Check the signature
             */
            bool isValid = requestContext.SigningProvider.CheckSignature(
                SignatureBase.Create(
                    httpContext.Request.HttpMethod,
                    httpContext.Request.Url,
                    requestContext.Parameters),
                requestContext.Parameters.Signature,
                requestContext.Consumer.Secret,
                tokenSecret);

            if (!isValid)
                OAuthRequestException.ThrowSignatureInvalid(null);
            else
                requestContext.IsSignatureValid = true;
        }

        public static void AddApplicationResponseParameters(OAuthRequestContext requestContext, NameValueCollection additionalParameters)
        {
            if (additionalParameters == null)
                return;

            // Remove any oauth_ prefixed parameters from the application's additional response
            foreach (string key in additionalParameters.AllKeys)
                if (key.StartsWith(Constants.OAuthParameterPrefix, StringComparison.Ordinal))
                    additionalParameters.Remove(key);

            // Add the application's custom parameters
            requestContext.ResponseParameters.Add(additionalParameters);
        }
    }
}
