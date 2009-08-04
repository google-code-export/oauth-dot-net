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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Practices.ServiceLocation;
using OAuth.Net.Common;

namespace OAuth.Net.Consumer
{
    /// <summary>
    /// An OAuthService encapsulates protocol information about a set of OAuth endpoints.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>An OAuthService contains the following protocol information:</para>
    /// 
    /// <list type="bullet">
    /// <item><description>The URL for obtaining Request Tokens</description></item>
    /// <item><description>The URL to send users to for authorization</description></item>
    /// <item><description>The URL for obtaining Access Tokens</description></item>
    /// <item><description>The HTTP method to use (GET or POST)</description></item>
    /// <item><description>Whether Authorization HTTP headers should be used</description></item>
    /// <item><description>The authorization Realm</description></item>
    /// <item><description>The signature method to use</description></item>
    /// <item><description>The OAuth specification version</description></item>
    /// <item><description>The consumer details (key and secret)</description></item>
    /// </list>
    /// 
    /// <para>In future, it should be possible to perform automatic discovery of
    /// the protocol information (a discovery specification is being worked on), but
    /// for now this information must be supplied.</para>
    /// </remarks>
    [DebuggerDisplay("Request Token URL: {RequestTokenUrl} Authorization URL: {AuthorizationUrl} Access Token URL: {AccessTokenUrl} Http Method: {HttpMethod} Use Authorization Header: {UseAuthorizationHeader} Realm: {Realm} Signature Method: {SignatureMethod} OAuth Version: {OAuthVersion} Consumer: {Consumer}")]
    public class OAuthService
    {
        // Uninstantiable except via factory methods
        private OAuthService() 
        {
        }

        /// <summary>
        /// The component locator provides component instances for this service.
        /// These components are defined in the config section associated with
        /// this service.
        /// </summary>
        public IServiceLocator ComponentLocator
        {
            get;
            private set;
        }

        /// <summary>
        /// The EndPoint for obtaining request tokens
        /// </summary>
        public EndPoint RequestTokenEndPoint
        {
            get;
            private set;
        }

        /// <summary>
        /// The URL for obtaining request tokens
        /// </summary>
        public Uri RequestTokenUrl
        {
            get
            {
                return RequestTokenEndPoint.Uri;
            }
        }        

        /// <summary>
        /// The URL to send users to for authorization
        /// </summary>
        public Uri AuthorizationUrl
        {
            get;
            private set;
        }

        /// <summary>
        /// The EndPoint for obtaining access tokens
        /// </summary>
        public EndPoint AccessTokenEndPoint
        {
            get;
            private set;
        }

        /// <summary>
        /// The URL for obtaining access tokens
        /// </summary>
        public Uri AccessTokenUrl
        {
            get
            {
                return AccessTokenEndPoint.Uri;
            }
        }

        /// <summary>
        /// The URL that the user is redirected to by the Service Provider.  If no Uri is defined 
        /// the request is defined as out-of-band and the call back has been defined elsewhere or will
        /// be performed manually.
        /// <remarks>This value can be dynamically updated during the request by handling the OnBeforeGetRequestToken 
        /// event in OauthRequest</remarks>
        /// </summary>
        public Uri CallbackUrl
        {
            get;
            private set;
        }        
        
        /// <summary>
        /// Whether to use HTTP Authorization headers (or query string / post body instead)
        /// </summary>
        public bool UseAuthorizationHeader
        {
            get;
            private set;
        }

        /// <summary>
        /// The HTTP authorization realm
        /// </summary>
        public string Realm
        {
            get;
            private set;
        }

        /// <summary>
        /// The signature method to use
        /// </summary>
        public string SignatureMethod
        {
            get;
            private set;
        }

        /// <summary>
        /// The OAuth specification version
        /// </summary>
        /// <value>Must be <c>1.0</c></value>
        public string OAuthVersion
        {
            get;
            private set;
        }

        /// <summary>
        /// The consumer credentials
        /// </summary>
        public IConsumer Consumer
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates an OAuthService using defaults for most parameters, loading components
        /// from the current global service locator.
        /// </summary>
        /// <remarks>
        /// <para>The OAuthService created will have the following defaults:</para>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Property</term>    
        ///         <description>Value</description>
        ///     </listheader>
        ///     <item>
        ///         <term>HttpMethod</term>
        ///         <description><c>"POST"</c></description>
        ///     </item>
        ///     <item>
        ///         <term>UseAuthorizationHeader</term>
        ///         <description><c>true</c></description>
        ///     </item>
        ///     <item>
        ///         <term>Realm</term>
        ///         <description><c>null</c></description>
        ///     </item>
        ///     <item>
        ///         <term>SignatureMethod</term>
        ///         <description><c>"HMAC-SHA1"</c></description>
        ///     </item>
        ///     <item>
        ///         <term>OAuthVersion</term>
        ///         <description><c>"1.0"</c></description>
        ///     </item>
        ///     <item>
        ///         <term>ConfigSection</term>
        ///         <description><c>"oauth.net.consumer"</c></description>
        ///     </item>
        /// </list>
        /// </remarks>
        /// <param name="requestTokenUrl">URL for obtaining request tokens</param>
        /// <param name="authorizationUrl">URL to send users to for authorization</param>
        /// <param name="accessTokenUrl">URL for obtaining access tokens</param>
        /// <param name="consumer">Consumer credentials</param>
        /// <returns>An OAuthService</returns>
        [Obsolete("This method has been superseded by the version implementing EndPoint arguments.")]
        public static OAuthService Create(
            Uri requestTokenUrl, 
            Uri authorizationUrl,
            Uri accessTokenUrl, 
            IConsumer consumer)
        {
            return OAuthService.Create(
                requestTokenUrl, 
                authorizationUrl,
                accessTokenUrl, 
                "POST", 
                true, 
                null, 
                "HMAC-SHA1", 
                Constants.Version1_0,
                consumer,
                () => ServiceLocator.Current);
        }

        /// <summary>
        /// Creates an OAuthService using defaults for most parameters, loading components
        /// from the current global service locator.
        /// </summary>
        /// <remarks>
        /// <para>The OAuthService created will have the following defaults:</para>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Property</term>    
        ///         <description>Value</description>
        ///     </listheader>
        ///     <item>
        ///         <term>HttpMethod</term>
        ///         <description><c>"POST"</c></description>
        ///     </item>
        ///     <item>
        ///         <term>UseAuthorizationHeader</term>
        ///         <description><c>true</c></description>
        ///     </item>
        ///     <item>
        ///         <term>Realm</term>
        ///         <description><c>null</c></description>
        ///     </item>
        ///     <item>
        ///         <term>SignatureMethod</term>
        ///         <description><c>"HMAC-SHA1"</c></description>
        ///     </item>
        ///     <item>
        ///         <term>OAuthVersion</term>
        ///         <description><c>"1.0"</c></description>
        ///     </item>
        ///     <item>
        ///         <term>ConfigSection</term>
        ///         <description><c>"oauth.net.consumer"</c></description>
        ///     </item>
        /// </list>
        /// </remarks>
        /// <param name="requestTokenEndPoint">EndPoint for obtaining request tokens</param>
        /// <param name="authorizationUrl">URL to send users to for authorization</param>
        /// <param name="accessTokenEndPoint">EndPoint for obtaining access tokens</param>
        /// <param name="consumer">Consumer credentials</param>
        /// <returns>An OAuthService</returns>
        public static OAuthService Create(
            EndPoint requestTokenEndPoint,
            Uri authorizationUrl,
            EndPoint accessTokenEndPoint,
            IConsumer consumer)
        {
            return OAuthService.Create(
                requestTokenEndPoint,
                authorizationUrl,
                accessTokenEndPoint,                
                true,
                null,
                "HMAC-SHA1",
                Constants.Version1_0,
                consumer,
                () => ServiceLocator.Current);
        }

        /// <summary>
        /// Creates an OAuthService using defaults for most parameters, loading components
        /// from the service locator provided by the supplied provider.
        /// </summary>
        /// <remarks>
        /// <para>The OAuthService created will have the following defaults:</para>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Property</term>    
        ///         <description>Value</description>
        ///     </listheader>
        ///     <item>
        ///         <term>HttpMethod</term>
        ///         <description><c>"POST"</c></description>
        ///     </item>
        ///     <item>
        ///         <term>UseAuthorizationHeader</term>
        ///         <description><c>true</c></description>
        ///     </item>
        ///     <item>
        ///         <term>Realm</term>
        ///         <description><c>null</c></description>
        ///     </item>
        ///     <item>
        ///         <term>SignatureMethod</term>
        ///         <description><c>"HMAC-SHA1"</c></description>
        ///     </item>
        ///     <item>
        ///         <term>OAuthVersion</term>
        ///         <description><c>"1.0"</c></description>
        ///     </item>
        /// </list>
        /// </remarks>
        /// <param name="requestTokenUrl">URL for obtaining request tokens</param>
        /// <param name="authorizationUrl">URL to send users to for authorization</param>
        /// <param name="accessTokenUrl">URL for obtaining access tokens</param>
        /// <param name="consumer">Consumer credentials</param>
        /// <param name="serviceLocatorProvider">Service locator provider which provides a service locator for components</param>
        /// <returns>An OAuthService</returns>        
        [Obsolete("This method has been superseded by the version implementing EndPoint arguments.")]
        public static OAuthService Create(
            Uri requestTokenUrl,
            Uri authorizationUrl,
            Uri accessTokenUrl,
            IConsumer consumer,
            ServiceLocatorProvider serviceLocatorProvider)
        {
            return OAuthService.Create(
                requestTokenUrl,
                authorizationUrl,
                accessTokenUrl,
                "POST",
                true,
                null,
                "HMAC-SHA1",
                Constants.Version1_0,
                consumer,
                serviceLocatorProvider);
        }

        /// <summary>
        /// Creates an OAuthService using defaults for most parameters, loading components
        /// from the service locator provided by the supplied provider.
        /// </summary>
        /// <remarks>
        /// <para>The OAuthService created will have the following defaults:</para>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Property</term>    
        ///         <description>Value</description>
        ///     </listheader>
        ///     <item>
        ///         <term>HttpMethod</term>
        ///         <description><c>"POST"</c></description>
        ///     </item>
        ///     <item>
        ///         <term>UseAuthorizationHeader</term>
        ///         <description><c>true</c></description>
        ///     </item>
        ///     <item>
        ///         <term>Realm</term>
        ///         <description><c>null</c></description>
        ///     </item>
        ///     <item>
        ///         <term>SignatureMethod</term>
        ///         <description><c>"HMAC-SHA1"</c></description>
        ///     </item>
        ///     <item>
        ///         <term>OAuthVersion</term>
        ///         <description><c>"1.0"</c></description>
        ///     </item>
        /// </list>
        /// </remarks>
        /// <param name="requestTokenEndPoint">EndPoint for obtaining request tokens</param>
        /// <param name="authorizationUrl">URL to send users to for authorization</param>
        /// <param name="accessTokenEndPoint">EndPoint for obtaining access tokens</param>
        /// <param name="consumer">Consumer credentials</param>
        /// <param name="serviceLocatorProvider">Service locator provider which provides a service locator for components</param>
        /// <returns>An OAuthService</returns>        
        public static OAuthService Create(
            EndPoint requestTokenEndPoint,
            Uri authorizationUrl,
            EndPoint accessTokenEndPoint,
            IConsumer consumer,
            ServiceLocatorProvider serviceLocatorProvider)
        {
            return OAuthService.Create(
                requestTokenEndPoint,
                authorizationUrl,
                accessTokenEndPoint,                
                true,
                null,
                "HMAC-SHA1",
                Constants.Version1_0,
                consumer,
                serviceLocatorProvider);
        }

        /// <summary>
        /// Creates an OAuthService using defaults for most parameters, loading components
        /// from the current global service locator.
        /// </summary>
        /// <remarks>
        /// <para>The OAuthService created will have the following defaults:</para>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Property</term>    
        ///         <description>Value</description>
        ///     </listheader>
        ///     <item>
        ///         <term>HttpMethod</term>
        ///         <description><c>"POST"</c></description>
        ///     </item>
        ///     <item>
        ///         <term>UseAuthorizationHeader</term>
        ///         <description><c>true</c></description>
        ///     </item>
        ///     <item>
        ///         <term>Realm</term>
        ///         <description><c>null</c></description>
        ///     </item>
        ///     <item>
        ///         <term>OAuthVersion</term>
        ///         <description><c>"1.0"</c></description>
        ///     </item>
        ///     <item>
        ///         <term>ConfigSection</term>
        ///         <description><c>"oauth.net.consumer"</c></description>
        ///     </item>
        /// </list>
        /// </remarks>
        /// <param name="requestTokenUrl">URL for obtaining request tokens</param>
        /// <param name="authorizationUrl">URL to send users to for authorization</param>
        /// <param name="accessTokenUrl">URL for obtaining access tokens</param>
        /// <param name="signatureMethod">Signature method to use</param>
        /// <param name="consumer">Consumer credentials</param>
        /// <returns>An OAuthService</returns>        
        [Obsolete("This method has been superseded by the version implementing EndPoint arguments.")]
        public static OAuthService Create(
            Uri requestTokenUrl,
            Uri authorizationUrl,
            Uri accessTokenUrl,
            string signatureMethod,
            IConsumer consumer)
        {
            return OAuthService.Create(
                requestTokenUrl,
                authorizationUrl,
                accessTokenUrl,
                "POST",
                true,
                null,
                signatureMethod,
                Constants.Version1_0,
                consumer,
                () => ServiceLocator.Current);
        }

        /// <summary>
        /// Creates an OAuthService using defaults for most parameters, loading components
        /// from the current global service locator.
        /// </summary>
        /// <remarks>
        /// <para>The OAuthService created will have the following defaults:</para>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Property</term>    
        ///         <description>Value</description>
        ///     </listheader>
        ///     <item>
        ///         <term>HttpMethod</term>
        ///         <description><c>"POST"</c></description>
        ///     </item>
        ///     <item>
        ///         <term>UseAuthorizationHeader</term>
        ///         <description><c>true</c></description>
        ///     </item>
        ///     <item>
        ///         <term>Realm</term>
        ///         <description><c>null</c></description>
        ///     </item>
        ///     <item>
        ///         <term>OAuthVersion</term>
        ///         <description><c>"1.0"</c></description>
        ///     </item>
        ///     <item>
        ///         <term>ConfigSection</term>
        ///         <description><c>"oauth.net.consumer"</c></description>
        ///     </item>
        /// </list>
        /// </remarks>
        /// <param name="requestTokenEndPoint">EndPoint for obtaining request tokens</param>
        /// <param name="authorizationUrl">URL to send users to for authorization</param>
        /// <param name="accessTokenEndPoint">EndPoint for obtaining access tokens</param>
        /// <param name="signatureMethod">Signature method to use</param>
        /// <param name="consumer">Consumer credentials</param>
        /// <returns>An OAuthService</returns>
        public static OAuthService Create(
            EndPoint requestTokenEndPoint,
            Uri authorizationUrl,
            EndPoint accessTokenEndPoint,
            string signatureMethod,
            IConsumer consumer)
        {
            return OAuthService.Create(
                requestTokenEndPoint,
                authorizationUrl,
                accessTokenEndPoint,                
                true,
                null,
                signatureMethod,
                Constants.Version1_0,
                consumer,
                () => ServiceLocator.Current);
        }

        /// <summary>
        /// Creates an OAuthService using defaults for most parameters, loading components
        /// from the service locator provided by the supplied provider.
        /// </summary>
        /// <remarks>
        /// <para>The OAuthService created will have the following defaults:</para>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Property</term>    
        ///         <description>Value</description>
        ///     </listheader>
        ///     <item>
        ///         <term>HttpMethod</term>
        ///         <description><c>"POST"</c></description>
        ///     </item>
        ///     <item>
        ///         <term>UseAuthorizationHeader</term>
        ///         <description><c>true</c></description>
        ///     </item>
        ///     <item>
        ///         <term>Realm</term>
        ///         <description><c>null</c></description>
        ///     </item>
        ///     <item>
        ///         <term>OAuthVersion</term>
        ///         <description><c>"1.0"</c></description>
        ///     </item>
        /// </list>
        /// </remarks>
        /// <param name="requestTokenUrl">URL for obtaining request tokens</param>
        /// <param name="authorizationUrl">URL to send users to for authorization</param>
        /// <param name="accessTokenUrl">URL for obtaining access tokens</param>
        /// <param name="signatureMethod">Signature method to use</param>
        /// <param name="consumer">Consumer credentials</param>
        /// <param name="serviceLocatorProvider">Service locator provider which provides 
        /// a service locator for components</param>
        /// <returns>An OAuthService</returns>        
        [Obsolete("This method has been superseded by the version implementing EndPoint arguments.")]
        public static OAuthService Create(
            Uri requestTokenUrl,
            Uri authorizationUrl,
            Uri accessTokenUrl,
            string signatureMethod,
            IConsumer consumer,
            ServiceLocatorProvider serviceLocatorProvider)
        {
            return OAuthService.Create(
                requestTokenUrl,
                authorizationUrl,
                accessTokenUrl,
                "POST",
                true,
                null,
                signatureMethod,
                Constants.Version1_0,
                consumer,
                serviceLocatorProvider);
        }

        /// <summary>
        /// Creates an OAuthService using defaults for most parameters, loading components
        /// from the service locator provided by the supplied provider.
        /// </summary>
        /// <remarks>
        /// <para>The OAuthService created will have the following defaults:</para>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Property</term>    
        ///         <description>Value</description>
        ///     </listheader>
        ///     <item>
        ///         <term>HttpMethod</term>
        ///         <description><c>"POST"</c></description>
        ///     </item>
        ///     <item>
        ///         <term>UseAuthorizationHeader</term>
        ///         <description><c>true</c></description>
        ///     </item>
        ///     <item>
        ///         <term>Realm</term>
        ///         <description><c>null</c></description>
        ///     </item>
        ///     <item>
        ///         <term>OAuthVersion</term>
        ///         <description><c>"1.0"</c></description>
        ///     </item>
        /// </list>
        /// </remarks>
        /// <param name="requestTokenEndPoint">EndPoint for obtaining request tokens</param>
        /// <param name="authorizationUrl">URL to send users to for authorization</param>
        /// <param name="accessTokenEndPoint">EndPoint for obtaining access tokens</param>
        /// <param name="signatureMethod">Signature method to use</param>
        /// <param name="consumer">Consumer credentials</param>
        /// <param name="serviceLocatorProvider">Service locator provider which provides 
        /// a service locator for components</param>
        /// <returns>An OAuthService</returns>        
        public static OAuthService Create(
            EndPoint requestTokenEndPoint,
            Uri authorizationUrl,
            EndPoint accessTokenEndPoint,
            string signatureMethod,
            IConsumer consumer,
            ServiceLocatorProvider serviceLocatorProvider)
        {
            return OAuthService.Create(
                requestTokenEndPoint,
                authorizationUrl,
                accessTokenEndPoint,                
                true,
                null,
                signatureMethod,
                Constants.Version1_0,
                consumer,
                serviceLocatorProvider);
        }

        /// <summary>
        /// Creates an OAuthService, with each parameter specified, loading components
        /// from the current global service locator.
        /// </summary>
        /// <param name="requestTokenUrl">URL for obtaining request tokens</param>
        /// <param name="authorizationUrl">URL to send users to for authorization</param>
        /// <param name="accessTokenUrl">URL for obtaining access tokens</param>
        /// <param name="httpMethod">HTTP method to use</param>
        /// <param name="useAuthorizationHeader">Whether to use HTTP Authorization headers</param>
        /// <param name="realm">HTTP authorization realm</param>
        /// <param name="signatureMethod">Signature method to use</param>
        /// <param name="oauthVersion">OAuth specification version</param>
        /// <param name="consumer">Consumer credentials</param>
        /// <returns>An OAuthService</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "oauth", Justification = "OAuth is a domain term")]
        [Obsolete("This method has been superseded by the version implementing EndPoint arguments.")]
        public static OAuthService Create(
            Uri requestTokenUrl,
            Uri authorizationUrl, 
            Uri accessTokenUrl, 
            string httpMethod,
            bool useAuthorizationHeader, 
            string realm, 
            string signatureMethod, 
            string oauthVersion,
            IConsumer consumer)
        {
            return OAuthService.Create(
                requestTokenUrl,
                authorizationUrl,
                accessTokenUrl,
                httpMethod,
                useAuthorizationHeader,
                realm,
                signatureMethod,
                oauthVersion,
                consumer,
                () => ServiceLocator.Current);
        }

        /// <summary>
        /// Creates an OAuthService, with each parameter specified, loading components
        /// from the current global service locator.
        /// </summary>
        /// <param name="requestTokenEndPoint">EndPoint for obtaining request tokens</param>
        /// <param name="authorizationUrl">URL to send users to for authorization</param>
        /// <param name="accessTokenEndPoint">EndPoint for obtaining access tokens</param>        
        /// <param name="useAuthorizationHeader">Whether to use HTTP Authorization headers</param>
        /// <param name="realm">HTTP authorization realm</param>
        /// <param name="signatureMethod">Signature method to use</param>
        /// <param name="oauthVersion">OAuth specification version</param>
        /// <param name="consumer">Consumer credentials</param>
        /// <returns>An OAuthService</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "oauth", Justification = "OAuth is a domain term")]
        public static OAuthService Create(
            EndPoint requestTokenEndPoint,
            Uri authorizationUrl,
            EndPoint accessTokenEndPoint,            
            bool useAuthorizationHeader,
            string realm,
            string signatureMethod,
            string oauthVersion,
            IConsumer consumer)
        {
            return OAuthService.Create(
                requestTokenEndPoint,
                authorizationUrl,
                accessTokenEndPoint,                
                useAuthorizationHeader,
                realm,
                signatureMethod,
                oauthVersion,
                consumer,
                () => ServiceLocator.Current);
        }


        /// <summary>
        /// Creates an OAuthService, with each parameter specified, loading components
        /// from the service locator provided by the supplied provider.
        /// </summary>
        /// <param name="requestTokenUrl">URL for obtaining request tokens</param>
        /// <param name="authorizationUrl">URL to send users to for authorization</param>
        /// <param name="accessTokenUrl">URL for obtaining access tokens</param>
        /// <param name="httpMethod">HTTP method to use</param>
        /// <param name="useAuthorizationHeader">Whether to use HTTP Authorization headers</param>
        /// <param name="realm">HTTP authorization realm</param>
        /// <param name="signatureMethod">Signature method to use</param>
        /// <param name="oauthVersion">OAuth specification version</param>
        /// <param name="consumer">Consumer credentials</param>
        /// <param name="serviceLocatorProvider">Service locator provider which provides a service locator for components</param>
        /// <returns>An OAuthService</returns>
        /// 
        [Obsolete("This method has been superseded by the version implementing EndPoint arguments.")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "oauth", Justification = "OAuth is a domain term")]
        public static OAuthService Create(
            Uri requestTokenUrl,
            Uri authorizationUrl,
            Uri accessTokenUrl,
            string httpMethod,
            bool useAuthorizationHeader,
            string realm,
            string signatureMethod,
            string oauthVersion,
            IConsumer consumer,
            ServiceLocatorProvider serviceLocatorProvider)
        {
            return new OAuthService()
            {
                ComponentLocator = serviceLocatorProvider(),
                RequestTokenEndPoint = new EndPoint(requestTokenUrl,httpMethod),
                AuthorizationUrl = authorizationUrl,
                AccessTokenEndPoint = new EndPoint(accessTokenUrl,httpMethod),
                UseAuthorizationHeader = useAuthorizationHeader,
                Realm = realm,
                SignatureMethod = signatureMethod,
                OAuthVersion = oauthVersion,
                Consumer = consumer
            };
        }

        /// <summary>
        /// Creates an OAuthService, with each parameter specified, loading components
        /// from the service locator provided by the supplied provider.
        /// </summary>
        /// <param name="requestTokenEndPoint">EndPoint for obtaining request tokens</param>
        /// <param name="authorizationUrl">URL to send users to for authorization</param>
        /// <param name="accessTokenEndPoint">EndPoint for obtaining access tokens</param>        
        /// <param name="useAuthorizationHeader">Whether to use HTTP Authorization headers</param>
        /// <param name="realm">HTTP authorization realm</param>
        /// <param name="signatureMethod">Signature method to use</param>
        /// <param name="oauthVersion">OAuth specification version</param>
        /// <param name="consumer">Consumer credentials</param>
        /// <param name="serviceLocatorProvider">Service locator provider which provides a service locator for components</param>
        /// <returns>An OAuthService</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "oauth", Justification = "OAuth is a domain term")]
        public static OAuthService Create(
           EndPoint requestTokenEndPoint,
           Uri authorizationUrl,
           EndPoint accessTokenEndPoint,           
           bool useAuthorizationHeader,
           string realm,
           string signatureMethod,
           string oauthVersion,
           IConsumer consumer,
           ServiceLocatorProvider serviceLocatorProvider)
        {
            return new OAuthService()
            {
                ComponentLocator = serviceLocatorProvider(),
                RequestTokenEndPoint = requestTokenEndPoint,
                AuthorizationUrl = authorizationUrl,
                AccessTokenEndPoint = accessTokenEndPoint,
                UseAuthorizationHeader = useAuthorizationHeader,
                Realm = realm,
                SignatureMethod = signatureMethod,
                OAuthVersion = oauthVersion,
                Consumer = consumer
            };
        }

        /// <summary>
        /// Checks if two OAuthServices are equal
        /// </summary>
        /// <param name="left">Left-hand-side OAuthService</param>
        /// <param name="right">Right-hand-side OAuthService</param>
        /// <returns><c>true</c>, if they are equal; otherwise <c>false</c></returns>
        public static bool operator ==(OAuthService left, OAuthService right)
        {
            if (object.ReferenceEquals(left, right))
                return true;

            if (((object)left) == null && ((object)right) == null)
                return true;

            return left.Equals(right);
        }

        /// <summary>
        /// Checks if two OAuthServices are not equal
        /// </summary>
        /// <param name="left">Left-hand-side OAuthService</param>
        /// <param name="right">Right-hand-side OAuthService</param>
        /// <returns><c>true</c>, if they are not equal; otherwise <c>false</c></returns>
        public static bool operator !=(OAuthService left, OAuthService right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Builds the authorization URL to send a user to for a particular request token.  As no callback
        /// URL is defined calling this method will force the OAuth Callback to be out-of-band.
        /// </summary>
        /// <remarks>
        /// If an <see cref="OAuthRequest"/> requires authorization, the URL to send the user
        /// to should be built by calling this method, passing in the 
        /// <see cref="OAuthRequest.RequestToken"/> value.
        /// </remarks>
        /// <param name="token">Request token that requires authorization</param>        
        /// <returns>The Uri to send the user to (with the OAuth token parameters encoded)</returns>
        public Uri BuildAuthorizationUrl(IToken token)
        {
            return this.BuildAuthorizationUrl(token, null);
        }   

        /// <summary>
        /// Builds the authorization URL to send a user to for a particular request token, 
        /// including the additional parameters supplied and the URI for the service provider 
        /// to return the user to.
        /// </summary>
        /// <remarks>
        /// If an <see cref="OAuthRequest"/> requires authorization, the URL to send the user
        /// to should be built by calling this method, passing in the 
        /// <see cref="OAuthRequest.RequestToken"/> value.
        /// </remarks>
        /// <param name="token">Request token that requires authorization</param>        
        /// <param name="additionalParameters">Additional parameters to add to the query 
        /// string</param>
        /// <returns>The Uri to send the user to (with callback URL &amp; additional parameters 
        /// added and the OAuth token parameters encoded)</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "callback", Justification = "Callback is a domain term")]
        public Uri BuildAuthorizationUrl(
            IToken token,             
            NameValueCollection additionalParameters)
        {
            if (token.Type == TokenType.Request)
            {                
                OAuthParameters authParameters = new OAuthParameters()
                {
                    Token = token.Token
                };                

                if (additionalParameters != null)
                    authParameters.AdditionalParameters.Add(additionalParameters);

                // Construct final authorization Uri (HTTP method must be GET)
                string query = authParameters.ToQueryStringFormat();

                UriBuilder authUri = new UriBuilder(this.AuthorizationUrl);

                if (String.IsNullOrEmpty(authUri.Query))
                    authUri.Query = query;
                else
                    authUri.Query = authUri.Query.Substring(1) + "&" + query;

                return authUri.Uri;
            }
            else
                throw new ArgumentException("Invalid token type for Authorization");
        }

        /// <summary>
        /// Checks whether the supplied object is equal to this OAuthService object.
        /// </summary>
        /// <remarks>OAuthServices are compared property by property.</remarks>
        /// <param name="obj">Object</param>
        /// <returns><c>true</c> if the objects' properties have the same values;
        /// <c>false</c> otherwise</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (System.Object.ReferenceEquals(this, obj))
                return true;

            if (this.GetType() != obj.GetType())
                return false;

            return this.Equals(obj as OAuthService);
        }

        /// <summary>
        /// Checks whether the supplied OAuthService is equal to this OAuthService object.
        /// </summary>
        /// <remarks>OAuthServices are compared property by property (excluding 
        /// ComponentLocator).</remarks>
        /// <param name="other">Other OAuthService</param>
        /// <returns><c>true</c> if the OAuthServices' properties have the same values;
        /// <c>false</c> otherwise</returns>
        private bool Equals(OAuthService other)
        {
            return other != null
                && this.RequestTokenEndPoint.Equals(other.RequestTokenUrl)
                && this.AuthorizationUrl.Equals(other.AuthorizationUrl)
                && this.AccessTokenEndPoint.Equals(other.AccessTokenUrl)                
                && this.UseAuthorizationHeader == other.UseAuthorizationHeader
                && string.Equals(this.Realm, other.Realm)
                && string.Equals(this.SignatureMethod, other.SignatureMethod)
                && string.Equals(this.OAuthVersion, other.OAuthVersion)
                && this.Consumer.Equals(other.Consumer);
        }

        /// <summary>
        /// Gets a hash code for the OAuthService.
        /// </summary>
        /// <returns>A hash code, computed from the hash codes of the properties</returns>
        public override int GetHashCode()
        {
            return this.RequestTokenEndPoint.GetHashCode() 
                ^ this.AuthorizationUrl.GetHashCode() ^ this.AccessTokenEndPoint.GetHashCode()                 
                ^ this.Realm.GetHashCode() ^ this.SignatureMethod.GetHashCode() 
                ^ this.OAuthVersion.GetHashCode() ^ this.Consumer.GetHashCode();
        }
    }
}
