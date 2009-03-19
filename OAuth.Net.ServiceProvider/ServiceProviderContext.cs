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
using System.Configuration;
using System.Security.Principal;
using Microsoft.Practices.ServiceLocation;
using OAuth.Net.Common;

namespace OAuth.Net.ServiceProvider
{
    public static class ServiceProviderContext
    {
        static ServiceProviderContext()
        {
            Settings = ConfigurationManager.GetSection("oauth.net.serviceprovider") as ServiceProviderSettings;

            if (!string.IsNullOrEmpty(Settings.DummyIdentity))
                DummyIdentity = new GenericIdentity(Settings.DummyIdentity);
        }

        /// <summary>
        /// The service provider settings from config file
        /// </summary>
        public static ServiceProviderSettings Settings
        {
            get;
            private set;
        }

        public static IIdentity DummyIdentity
        {
            get;
            private set;
        }

        /// <summary>
        /// An instance of a consumer store. 
        /// </summary>
        /// <value>
        /// <c>null</c> if no consumer store component is registered.
        /// </value>
        public static IConsumerStore ConsumerStore
        {
            get
            {
                try
                {
                    return ServiceLocator.Current.GetInstance<IConsumerStore>();
                }
                catch (ActivationException)
                {
                    return null;
                }
                catch (NullReferenceException)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// An instance of a request ID validator. 
        /// </summary>
        /// <value>
        /// <c>null</c> if no request ID validator component is registered.
        /// </value>
        public static IRequestIdValidator RequestIdValidator
        {
            get
            {
                try
                {
                    return ServiceLocator.Current.GetInstance<IRequestIdValidator>();
                }
                catch (ActivationException)
                {
                    return null;
                }
                catch (NullReferenceException)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// An instance of a token generator. 
        /// </summary>
        /// <value>
        /// <c>null</c> if no token generator component is registered.
        /// </value>
        public static ITokenGenerator TokenGenerator
        {
            get
            {
                try
                {
                    return ServiceLocator.Current.GetInstance<ITokenGenerator>();
                }
                catch (ActivationException)
                {
                    return null;
                }
                catch (NullReferenceException)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// An instance of a token store. 
        /// </summary>
        /// <value>
        /// <c>null</c> if no token store component is registered.
        /// </value>
        public static ITokenStore TokenStore
        {
            get
            {
                try
                {
                    return ServiceLocator.Current.GetInstance<ITokenStore>();
                }
                catch (ActivationException)
                {
                    return null;
                }
                catch (NullReferenceException)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Get a signing provider for the given signature method. Returns 
        /// <c>null</c> if no signing provider can be found for the 
        /// given signature method.
        /// </summary>
        /// <param name="signatureMethod">The signature method</param>
        /// <returns>The signing provider or <c>null</c> if no signing 
        /// provider can be found for the given signature method.</returns>
        public static ISigningProvider GetSigningProvider(string signatureMethod)
        {
            try
            {
                return ServiceLocator.Current.GetInstance<ISigningProvider>(Constants.SigningProviderIdPrefix + signatureMethod);
            }
            catch (ActivationException)
            {
                return null;
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }

        /// <summary>
        /// Authorize the request token for the specified user.
        /// </summary>
        /// <param name="token">The request token to authorize</param>
        /// <param name="authenticatedUser">The user (must be authenticated)</param>
        /// <returns><c>true</c>, if the token was authorized; 
        /// <c>false</c>, if the token could not be authorized</returns>
        /// <exception cref="OAuth.Net.Common.OAuthRequestException">
        /// If there is a problem with the request that prevents access being granted.
        /// </exception>
        public static bool GrantAccess(IRequestToken token, IIdentity authenticatedUser)
        {
            if (token == null)
                throw new ArgumentNullException("token");

            if (authenticatedUser == null)
                throw new ArgumentNullException("authenticatedUser");

            if (!authenticatedUser.IsAuthenticated)
                throw new ArgumentException("User must be authenticated", "authenticatedUser");

            switch (token.Status)
            {
                case TokenStatus.Authorized:
                    break;

                case TokenStatus.Expired:
                    OAuthRequestException.ThrowTokenExpired(null);
                    break;

                case TokenStatus.Revoked:
                    OAuthRequestException.ThrowTokenRevoked(null);
                    break;

                case TokenStatus.Used:
                    OAuthRequestException.ThrowTokenUsed(null);
                    break;

                case TokenStatus.Unknown:
                    OAuthRequestException.ThrowTokenRejected(null);
                    break;

                case TokenStatus.Unauthorized:
                    break;

                default:
                    throw new ArgumentException("Token status is invalid", "token");
            }

            if (token.ConsumerKey == null)
                OAuthRequestException.ThrowTokenRejected("Token must have a consumer key");

            IConsumer consumer = ServiceProviderContext.ConsumerStore.GetByKey(token.ConsumerKey);

            if (consumer == null)
                OAuthRequestException.ThrowConsumerKeyUnknown("Consumer is unknown");

            switch (consumer.Status)
            {
                case ConsumerStatus.TemporarilyDisabled:
                    OAuthRequestException.ThrowConsumerKeyRefused("Consumer is temporarily disabled");
                    break;

                case ConsumerStatus.PermanentlyDisabled:
                    OAuthRequestException.ThrowConsumerKeyRejected("Consumer is permanently disabled");
                    break;

                case ConsumerStatus.Valid:
                    break;

                case ConsumerStatus.Unknown:
                    OAuthRequestException.ThrowConsumerKeyUnknown("Consumer is unknown");
                    break;

                default:
                    throw new ArgumentException("Token consumer's status is invalid", "token");
            }

            token.Status = TokenStatus.Authorized;
            token.AuthenticatedUser = authenticatedUser;

            if (ServiceProviderContext.TokenStore.Update(token))
                return true;
            else
            {
                token.Status = TokenStatus.Unauthorized;
                token.AuthenticatedUser = null;

                return false;
            }
        }

        /// <summary>
        /// Revoke the request token.
        /// </summary>
        /// <param name="token">The request token to revoke</param>
        /// <returns><c>true</c>, if the token was revoked; 
        /// <c>false</c>, if the token could not be revoked</returns>
        public static bool RevokeToken(IRequestToken token)
        {
            token.Status = TokenStatus.Revoked;

            if (ServiceProviderContext.TokenStore.Update(token))
                return true;
            else
            {
                token.Status = TokenStatus.Unauthorized;
                return false;
            }
        }
    }
}
