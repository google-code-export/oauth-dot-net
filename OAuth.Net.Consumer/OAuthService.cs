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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Castle.Core.Resource;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
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
        /// <summary>
        /// The default config section to use when no other is specified.
        /// </summary>
        public const string DefaultConfigSection = "oauth.net.consumer";

        // Uninstantiable except via factory methods
        private OAuthService() 
        {
        }

        public string ConfigSection
        {
            get;
            private set;
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
        /// The URL for obtaining request tokens
        /// </summary>
        public Uri RequestTokenUrl
        {
            get;
            private set;
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
        /// The URL for obtaining access tokens
        /// </summary>
        public Uri AccessTokenUrl
        {
            get;
            private set;
        }

        /// <summary>
        /// The HTTP method to use
        /// </summary>
        /// <value>Only <c>GET</c> and <c>POST</c> are supported</value>
        public string HttpMethod
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
        /// from the default config section..
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
                OAuthService.DefaultConfigSection);
        }

        /// <summary>
        /// Creates an OAuthService using defaults for most parameters, loading components
        /// from the specified config section.
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
        /// <param name="configSection">Name of the config section to load components from</param>
        /// <returns>An OAuthService</returns>
        public static OAuthService Create(
            Uri requestTokenUrl,
            Uri authorizationUrl,
            Uri accessTokenUrl,
            IConsumer consumer,
            string configSection)
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
                configSection);
        }

        /// <summary>
        /// Creates an OAuthService using defaults for most parameters, loading components
        /// from the default config section.
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
                OAuthService.DefaultConfigSection);
        }

        /// <summary>
        /// Creates an OAuthService using defaults for most parameters, loading components
        /// from the specified config section.
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
        /// <param name="configSection">Name of the config section to load components from</param>
        /// <returns>An OAuthService</returns>
        public static OAuthService Create(
            Uri requestTokenUrl,
            Uri authorizationUrl,
            Uri accessTokenUrl,
            string signatureMethod,
            IConsumer consumer,
            string configSection)
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
                configSection);
        }

        /// <summary>
        /// Creates an OAuthService, with each parameter specified, loading components
        /// from the default config section (<c>"oauth.net.consumer"</c>).
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
                OAuthService.DefaultConfigSection);
        }

        /// <summary>
        /// Creates an OAuthService, with each parameter specified, loading components
        /// from the specified config section.
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
        /// <param name="configSection">Name of the config section to load components from</param>
        /// <returns>An OAuthService</returns>
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
            string configSection)
        {
            return new OAuthService()
            {
                ConfigSection = configSection,
                ComponentLocator = OAuthService.CreateServiceLocator(configSection),
                RequestTokenUrl = requestTokenUrl,
                AuthorizationUrl = authorizationUrl,
                AccessTokenUrl = accessTokenUrl,
                HttpMethod = httpMethod,
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
            return !left.Equals(right);
        }

        /// <summary>
        /// Builds the authorization URL to send a user to for a particular request token.
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
            return this.BuildAuthorizationUrl(token, null, null);
        }

        /// <summary>
        /// Builds the authorization URL to send a user to for a particular request token, 
        /// with the URI for the service provider to return the user to.
        /// </summary>
        /// <remarks>
        /// If an <see cref="OAuthRequest"/> requires authorization, the URL to send the user
        /// to should be built by calling this method, passing in the 
        /// <see cref="OAuthRequest.RequestToken"/> value.
        /// </remarks>
        /// <param name="token">Request token that requires authorization</param>  
        /// <param name="callbackUrl">URL to have user redirected to after authorization</param>
        /// <returns>The Uri to send the user to (with the callback URL and the OAuth 
        /// token parameters encoded)</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "callback", Justification = "Callback is a domain term")]
        public Uri BuildAuthorizationUrl(IToken token, Uri callbackUrl)
        {
            return this.BuildAuthorizationUrl(token, callbackUrl, null);
        }

        /// <summary>
        /// Builds the authorization URL to send a user to for a particular request token, 
        /// including the additional parameters supplied.
        /// </summary>
        /// <remarks>
        /// If an <see cref="OAuthRequest"/> requires authorization, the URL to send the user
        /// to should be built by calling this method, passing in the 
        /// <see cref="OAuthRequest.RequestToken"/> value.
        /// </remarks>
        /// <param name="token">Request token that requires authorization</param> 
        /// <param name="additionalParameters">Additional parameters to add to the query 
        /// string</param>
        /// <returns>The Uri to send the user to (with the additional parameters added and 
        /// the OAuth token parameters encoded)</returns>
        public Uri BuildAuthorizationUrl(IToken token, NameValueCollection additionalParameters)
        {
            return this.BuildAuthorizationUrl(token, null, additionalParameters);
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
        /// <param name="callbackUrl">URL to have user redirected to after authorization</param>
        /// <param name="additionalParameters">Additional parameters to add to the query 
        /// string</param>
        /// <returns>The Uri to send the user to (with callback URL &amp; additional parameters 
        /// added and the OAuth token parameters encoded)</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "callback", Justification = "Callback is a domain term")]
        public Uri BuildAuthorizationUrl(
            IToken token, 
            Uri callbackUrl, 
            NameValueCollection additionalParameters)
        {
            if (token.Type == TokenType.Request)
            {                
                OAuthParameters authParameters = new OAuthParameters()
                {
                    Token = token.Token
                };

                if (callbackUrl != null)
                    authParameters.Callback = callbackUrl.AbsoluteUri;

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
        public bool Equals(OAuthService other)
        {
            return other != null
                && string.Equals(this.ConfigSection, other.ConfigSection)
                && this.RequestTokenUrl.Equals(other.RequestTokenUrl)
                && this.AuthorizationUrl.Equals(other.AuthorizationUrl)
                && this.AccessTokenUrl.Equals(other.AccessTokenUrl)
                && this.HttpMethod.Equals(other.HttpMethod)
                && this.UseAuthorizationHeader.Equals(other.UseAuthorizationHeader)
                && string.Equals(this.Realm, other.Realm)
                && this.SignatureMethod.Equals(other.SignatureMethod)
                && string.Equals(this.OAuthVersion, other.OAuthVersion)
                && this.Consumer.Equals(other.Consumer);
        }

        /// <summary>
        /// Gets a hash code for the OAuthService.
        /// </summary>
        /// <returns>A hash code, computed from the hash codes of the properties</returns>
        public override int GetHashCode()
        {
            return this.ConfigSection.GetHashCode() ^ this.RequestTokenUrl.GetHashCode() 
                ^ this.AuthorizationUrl.GetHashCode() ^ this.AccessTokenUrl.GetHashCode() 
                ^ this.HttpMethod.GetHashCode() ^ this.UseAuthorizationHeader.GetHashCode() 
                ^ this.Realm.GetHashCode() ^ this.SignatureMethod.GetHashCode() 
                ^ this.OAuthVersion.GetHashCode() ^ this.Consumer.GetHashCode();
        }

        /// <summary>
        /// Creates a service locator instance configured from the config section
        /// specified. If the config section is null or empty then the service 
        /// locator created will not have loaded any components.
        /// </summary>
        /// <param name="configSection">Config section to load components from</param>
        /// <returns>A service locator</returns>
        private static IServiceLocator CreateServiceLocator(string configSection)
        {
            if (!string.IsNullOrEmpty(configSection))
                return new WindsorAdaptor(
                    new WindsorContainer(
                        new XmlInterpreter(
                            new ConfigResource(configSection))));
            else
                return new WindsorAdaptor(new WindsorContainer());
        }
    }
}
