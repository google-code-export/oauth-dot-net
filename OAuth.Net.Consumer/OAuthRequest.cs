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
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using OAuth.Net.Common;

namespace OAuth.Net.Consumer
{
    public class OAuthRequest
    {
        /// <summary>
        /// This event is fired before the request to get a request token is created. You may
        /// modify the request Uri and HTTP method, as well as add additional request 
        /// parameters to be sent with the request in the query string or post body.
        /// </summary>
        public event EventHandler<PreRequestEventArgs> OnBeforeGetRequestToken;

        /// <summary>
        /// This event is fired after the request token has been received. If an 
        /// exception occurs receiving the response, this event will not fire.
        /// </summary>
        public event EventHandler<RequestTokenReceivedEventArgs> OnReceiveRequestToken;

        /// <summary>
        /// This event is fired before the request to get an access token is created. You may
        /// modify the request Uri and HTTP method.
        /// </summary>
        public event EventHandler<PreAccessTokenRequestEventArgs> OnBeforeGetAccessToken;

        /// <summary>
        /// This event is fired after the access token has been received. If an 
        /// exception occurs receiving the response, this event will not fire.
        /// </summary>
        public event EventHandler<AccessTokenReceivedEventArgs> OnReceiveAccessToken;

        /// <summary>
        /// This event is fired before the request the to get the protected resource is created.
        /// You may modify the request Uri and HTTP method, as well as add additional request 
        /// parameters to be sent with the request in the query string or post body.
        /// </summary>
        public event EventHandler<PreProtectedResourceRequestEventArgs> OnBeforeGetProtectedResource;

        /// <summary>
        /// The protected resource Uri
        /// </summary>
        public Uri ResourceUri
        {
            get;
            private set;
        }

        /// <summary>
        /// The OAuth service
        /// </summary>
        public OAuthService Service
        {
            get;
            private set;
        }

        /// <summary>
        /// The request token
        /// </summary>
        public IToken RequestToken
        {
            get;
            private set;
        }

        /// <summary>
        /// The access token
        /// </summary>
        public IToken AccessToken
        {
            get;
            private set;
        }

        /// <summary>
        /// This delegate is called when the request token requires authorization.
        /// </summary>
        public EventHandler<AuthorizationEventArgs> AuthorizationHandler
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a new OAuth protected request.
        /// </summary>
        /// <param name="resourceUri">Protected resource URI</param>
        /// <param name="settings">Endpoint settings</param>
        /// <returns>OAuth protected request</returns>
        public static OAuthRequest Create(Uri resourceUri, OAuthService settings)
        {
            return new OAuthRequest()
            {
                ResourceUri = resourceUri,
                Service = settings
            };
        }

        /// <summary>
        /// Creates a new OAuth protected request, initialised with a previously
        /// retrieved request token. This token may or may not have been authorized.
        /// </summary>
        /// <param name="resourceUri">Protected resource URI</param>
        /// <param name="settings">Endpoint settings</param>
        /// <param name="requestToken">Request token</param>
        /// <returns>OAuth protected request</returns>
        public static OAuthRequest Create(Uri resourceUri, OAuthService settings, IToken requestToken)
        {
            return new OAuthRequest()
            {
                ResourceUri = resourceUri,
                Service = settings,
                RequestToken = requestToken
            };
        }

        /// <summary>
        /// Creates a new OAuth protected request, initialised with a previously
        /// retrieved access token. 
        /// </summary>
        /// <param name="resourceUri">Protected resource URI</param>
        /// <param name="settings">Endpoint settings</param>
        /// <param name="requestToken">Request token</param>
        /// <param name="accessToken">Access token</param>
        /// <returns>OAuth protected request</returns>
        public static OAuthRequest Create(Uri resourceUri, OAuthService settings, IToken requestToken, IToken accessToken)
        {
            return new OAuthRequest()
            {
                ResourceUri = resourceUri,
                Service = settings,
                RequestToken = requestToken,
                AccessToken = accessToken
            };
        }

        /// <exception cref="OAuth.Net.Common.OAuthRequestException">
        /// <list>
        /// <item>If the server responds with an OAuthRequestException</item>
        /// <item>If the server's responds unexpectedly</item>
        /// <item>If the requests to the server cannot be signed</item>
        /// </list>
        /// </exception>
        public OAuthResponse GetResource()
        {
            return this.GetResource(null);
        }

        /// <param name="parameters">Additional parameters to send with the protected resource request</param>
        /// <exception cref="OAuth.Net.Common.OAuthRequestException">
        /// <list>
        /// <item>If the server responds with an OAuthRequestException</item>
        /// <item>If the server's responds unexpectedly</item>
        /// <item>If the requests to the server cannot be signed</item>
        /// </list>
        /// </exception>
        public OAuthResponse GetResource(NameValueCollection parameters)
        {
            OAuthResponse response;

            HttpWebRequest request = this.PrepareProtectedResourceRequest(parameters);

            // A null value for the HttpWebRequest is returned when a ResponseToken is returned
            // and no one has returned in the AuthorizationHandler continue with getting an AccessToken
            // or an RequestToken exists but the AccessToken request was refused.
            if (request == null)
                response = new OAuthResponse(this.RequestToken);
            else
            {
                OAuthResource resource;
                OAuthParameters responseParameters;

                try
                {
                    resource = new OAuthResource((HttpWebResponse)request.GetResponse());

                    // Parse the parameters and re-throw any OAuthRequestException from the service provider
                    responseParameters = OAuthParameters.Parse(resource);
                    OAuthRequestException.TryRethrow(responseParameters);

                    // If nothing is thrown then we should have a valid resource.
                    response = new OAuthResponse(this.AccessToken ?? this.RequestToken, resource);
                }
                catch (WebException e)
                {
                    // Parse the parameters and re-throw any OAuthRequestException from the service provider
                    responseParameters = OAuthParameters.Parse(e.Response as HttpWebResponse);
                    OAuthRequestException.TryRethrow(responseParameters);

                    // If no OAuthRequestException, rethrow the WebException
                    #warning TODO: We have consumer the WebException's body so rethrowing it is pretty pointless; wrap the WebException in an OAuthProtocolException and store the body (create an OAuthResource before parsing parameters)
                    throw;
                }
            }

            return response;
        }

        /// <exception cref="OAuth.Net.Common.OAuthRequestException">
        /// <list>
        /// <item>If the server responds with an OAuthRequestException</item>
        /// <item>If the server's responds unexpectedly</item>
        /// <item>If the requests to the server cannot be signed</item>
        /// </list>
        /// </exception>
        protected virtual HttpWebRequest PrepareProtectedResourceRequest(NameValueCollection parameters)
        {
            if (this.AccessToken == null || this.RequestToken == null)
            {
                if (this.RequestToken == null)
                {
                    // Get a request token
                    this.DoGetRequestToken();
                }

                if (this.RequestToken == null)
                    throw new InvalidOperationException("Request token was not received.");

                if (this.RequestToken.Status != TokenStatus.Authorized)
                {
                    // Get the authorization handler to authorize the request token
                    // Halt processing if the authorization handler is out-of-band
                    if (!this.DoAuthorizeRequestToken())
                        return null;
                }

                if (this.RequestToken == null || this.RequestToken.Status != TokenStatus.Authorized)
                    throw new InvalidOperationException("Request token was not authorized.");

                // Get an access token
                this.DoGetAccessToken();
            }

            if (this.AccessToken == null)
                throw new InvalidOperationException("Access token was not received.");

            return this.DoPrepareProtectedResourceRequest(parameters);
        }

        protected virtual void DoGetRequestToken()
        {
            // Fire the OnBeforeGetRequestToken event
            PreRequestEventArgs args = new PreRequestEventArgs(
                this.Service.RequestTokenUrl, 
                this.Service.HttpMethod,
                new NameValueCollection());

            if (this.OnBeforeGetRequestToken != null)
                this.OnBeforeGetRequestToken(this, args);

            // Create and sign the request
            HttpWebRequest request = this.CreateAndSignRequest(
                args.RequestUri, 
                args.HttpMethod, 
                args.AdditionalParameters, 
                null);

            HttpWebResponse response = null;
            OAuthParameters responseParameters = null;

            // Get the service provider response
            try
            {
                response = (HttpWebResponse)request.GetResponse();

                // Parse the parameters and re-throw any OAuthRequestException from the service provider
                responseParameters = OAuthParameters.Parse(response);
                OAuthRequestException.TryRethrow(responseParameters);
            }
            catch (WebException e)
            {
                // Parse the parameters and re-throw any OAuthRequestException from the service provider
                responseParameters = OAuthParameters.Parse(e.Response as HttpWebResponse);
                OAuthRequestException.TryRethrow(responseParameters);

                // If no OAuthRequestException, rethrow the WebException
                throw;
            }
            
            // Store the request token
            this.RequestToken = new OAuthToken(
                TokenType.Request, 
                responseParameters.Token, 
                responseParameters.TokenSecret, 
                this.Service.Consumer);            

            // Fire the OnReceiveRequestToken event
            RequestTokenReceivedEventArgs responseArgs = new RequestTokenReceivedEventArgs(
                this.RequestToken, 
                responseParameters.AdditionalParameters);

            if (this.OnReceiveRequestToken != null)
                this.OnReceiveRequestToken(this, responseArgs);
        }

        /// <summary>
        /// Raises the AuthorizationEventArgs that allows a Consumer to determine 
        /// if the request should stop and return the RequestToken or 
        /// continue and request the access token.  This allow the a Consumer desktop 
        /// app to sleep the thread whilst the consumer goes elsewhere to perform
        /// the authorization.
        /// </summary>
        /// <returns></returns>
        protected virtual bool DoAuthorizeRequestToken()
        {
            if (this.RequestToken == null)
                throw new InvalidOperationException("Request token must be present");

            // There is no feedback from the SP as to whether the user granted/denied access so we assume they authorized the request token
            this.RequestToken.Status = TokenStatus.Authorized;
            
            // Invoke the authorization handler
            AuthorizationEventArgs authArgs = new AuthorizationEventArgs(this.RequestToken);

            if (this.AuthorizationHandler != null)                                            
                this.AuthorizationHandler(this, authArgs);                      

            return authArgs.ContinueOnReturn;
        }

        protected virtual void DoGetAccessToken()
        {
            // Fire the OnBeforeGetAccessToken event
            PreAccessTokenRequestEventArgs preArgs = new PreAccessTokenRequestEventArgs(
                this.Service.AccessTokenUrl,
                this.Service.HttpMethod, 
                this.RequestToken);

            if (this.OnBeforeGetAccessToken != null)
                this.OnBeforeGetAccessToken(this, preArgs);

            // Create and sign the request
            HttpWebRequest request = this.CreateAndSignRequest(
                preArgs.RequestUri, 
                preArgs.HttpMethod, 
                null, 
                this.RequestToken);

            HttpWebResponse response = null;
            OAuthParameters responseParameters = null;

            // Get the service provider response
            try
            {
                response = (HttpWebResponse)request.GetResponse();

                // Parse the parameters and re-throw any OAuthRequestException from the service provider
                responseParameters = OAuthParameters.Parse(response);
                OAuthRequestException.TryRethrow(responseParameters);
            }
            catch (WebException e)
            {
                // Parse the parameters and re-throw any OAuthRequestException from the service provider
                responseParameters = OAuthParameters.Parse(e.Response as HttpWebResponse);
                OAuthRequestException.TryRethrow(responseParameters);

                // If no OAuthRequestException, rethrow the WebException
                throw;
            }

            // Store the access token
            this.AccessToken = new OAuthToken(
                TokenType.Access, 
                responseParameters.Token,
                responseParameters.TokenSecret, 
                this.Service.Consumer);

            // Fire the OnReceiveAccessToken event
            AccessTokenReceivedEventArgs responseArgs = new AccessTokenReceivedEventArgs(
                this.RequestToken,
                this.AccessToken, 
                responseParameters.AdditionalParameters);

            if (this.OnReceiveAccessToken != null)
                this.OnReceiveAccessToken(this, responseArgs);
        }

        protected virtual HttpWebRequest DoPrepareProtectedResourceRequest(NameValueCollection parameters)
        {
            // Fire the OnBeforeGetProtectedResource event
            PreProtectedResourceRequestEventArgs preArgs = new PreProtectedResourceRequestEventArgs(
                this.ResourceUri, 
                this.Service.HttpMethod,
                parameters ?? new NameValueCollection(), 
                this.RequestToken, 
                this.AccessToken);

            if (this.OnBeforeGetProtectedResource != null)
                this.OnBeforeGetProtectedResource(this, preArgs);

            // Prepare request
            return this.CreateAndSignRequest(
                preArgs.RequestUri, 
                preArgs.HttpMethod, 
                preArgs.AdditionalParameters, 
                this.AccessToken);
        }

        protected virtual HttpWebRequest CreateAndSignRequest(Uri requestUri, string httpMethod, NameValueCollection additionalParameters, IToken token)
        {
            int timestamp = UnixTime.ToUnixTime(DateTime.Now);

            OAuthParameters authParameters = new OAuthParameters()
            {
                ConsumerKey = this.Service.Consumer.Key,
                Realm = this.Service.Realm,
                SignatureMethod = this.Service.SignatureMethod,
                Timestamp = timestamp.ToString(CultureInfo.InvariantCulture),
                Nonce = this.Service.ComponentLocator.GetInstance<INonceProvider>().GenerateNonce(timestamp),
                Version = this.Service.OAuthVersion
            };

            if (token != null)
                authParameters.Token = token.Token;

            if (additionalParameters != null && additionalParameters.Count > 0)
                authParameters.AdditionalParameters.Add(additionalParameters);

            // Normalize the request uri for signing
            if (!string.IsNullOrEmpty(requestUri.Query))
            {
                UriBuilder mutableRequestUri = new UriBuilder(requestUri);

                // TODO: Will the parameters necessarily be Rfc3698 encoded here? If not, then Rfc3968.SplitAndDecode will throw FormatException
                authParameters.AdditionalParameters.Add(Rfc3986.SplitAndDecode(mutableRequestUri.Query.Substring(1)));

                mutableRequestUri.Query = null;
                requestUri = mutableRequestUri.Uri;
            }

            // Check there is a signing provider for the signature method
            ISigningProvider signingProvider = this.Service.ComponentLocator.GetInstance<ISigningProvider>(Constants.SigningProviderIdPrefix + this.Service.SignatureMethod);

            if (signingProvider == null)
            {
                // There is no signing provider for this signature method
                OAuthRequestException.ThrowSignatureMethodRejected(null);
            }

            // Double check the signing provider declares that it can handle the signature method
            if (!signingProvider.SignatureMethod.Equals(this.Service.SignatureMethod))
                OAuthRequestException.ThrowSignatureMethodRejected(null);

            // Compute the signature
            authParameters.Signature = signingProvider.ComputeSignature(
                SignatureBase.Create(httpMethod, requestUri, authParameters),
                this.Service.Consumer.Secret,
                (token != null && token.Secret != null) ? token.Secret : null);

            // Create the request, attaching the OAuth parameters and additional parameters
            switch (httpMethod)
            {
                case "GET":
                    if (this.Service.UseAuthorizationHeader)
                    {
                        // Put the OAuth parameters in the header and the additional parameters in the query string
                        string authHeader = authParameters.ToHeaderFormat();
                        string query = Rfc3986.EncodeAndJoin(authParameters.AdditionalParameters);

                        if (!string.IsNullOrEmpty(query))
                        {
                            UriBuilder mutableRequestUri = new UriBuilder(requestUri);
                            if (string.IsNullOrEmpty(mutableRequestUri.Query))
                                mutableRequestUri.Query = query;
                            else
                                mutableRequestUri.Query = mutableRequestUri.Query.Substring(1) + "&" + query;

                            requestUri = mutableRequestUri.Uri;
                        }

                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUri);
                        request.Method = httpMethod;
                        request.Headers.Add(HttpRequestHeader.Authorization, authHeader);
                        return request;
                    }
                    else 
                    {
                        string query = authParameters.ToQueryStringFormat();

                        UriBuilder mutableRequestUri = new UriBuilder(requestUri);
                        if (string.IsNullOrEmpty(mutableRequestUri.Query))
                            mutableRequestUri.Query = query;
                        else
                            mutableRequestUri.Query = mutableRequestUri.Query.Substring(1) + "&" + query;

                        requestUri = mutableRequestUri.Uri;

                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUri);
                        request.Method = httpMethod;
                        return request;
                    }

                case "POST":
                    if (this.Service.UseAuthorizationHeader)
                    {
                        // Put the OAuth parameters in the header and the additional parameters in the post body
                        string authHeader = authParameters.ToHeaderFormat();
                        string body = Rfc3986.EncodeAndJoin(authParameters.AdditionalParameters);

                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUri);
                        request.Method = httpMethod;

                        request.Headers.Add(HttpRequestHeader.Authorization, authHeader);

                        byte[] bodyBytes = Encoding.ASCII.GetBytes(body);
                        request.ContentType = Constants.HttpPostUrlEncodedContentType;
                        request.ContentLength = bodyBytes.Length;

                        Stream requestStream = request.GetRequestStream();
                        requestStream.Write(bodyBytes, 0, bodyBytes.Length);

                        return request;
                    }
                    else
                    {
                        string body = authParameters.ToNormalizedString(
                            Constants.RealmParameter,
                            Constants.TokenSecretParameter);

                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUri);
                        request.Method = httpMethod;

                        byte[] bodyBytes = Encoding.ASCII.GetBytes(body);
                        request.ContentType = Constants.HttpPostUrlEncodedContentType;
                        request.ContentLength = bodyBytes.Length;

                        Stream requestStream = request.GetRequestStream();
                        requestStream.Write(bodyBytes, 0, bodyBytes.Length);

                        return request;
                    }

                default:
                    throw new ArgumentException("httpMethod argument must be GET or POST", "httpMethod");
            }
        }
    }
}
