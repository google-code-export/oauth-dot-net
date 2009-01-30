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
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace OAuth.Net.Common
{
    /// <summary>
    /// Flags enumeration to indicate which sources should be used to collate OAuth parameters from. As this is a flags enumeration,
    /// different sources can be combined.
    /// </summary>
    [Flags]
    public enum OAuthParameterSources
    {
        /// <summary>
        /// The parameters should be collated from none
        /// of the available sources. This should value not 
        /// be used.        
        /// </summary>
        None = 0,

        /// <summary>
        /// The HTTP Authorization header should be used
        /// to collate parameters from.     
        /// </summary>
        HttpAuthorizationHeader = 1,

        /// <summary>
        /// The HTTP POST body should be used to collate
        /// parameters from (with a content-type of
        /// application/x-www-form-urlencoded)     
        /// </summary>        
        HttpPostBody = 2,

        /// <summary>
        /// The HTTP query string should be used to 
        /// collate parameters from.     
        /// </summary>       
        HttpQueryString = 4,

        /// <summary>
        /// The HTTP WWW-Authenticate header should be used to 
        /// collate parameters from.     
        /// </summary>       
        HttpWwwAuthenticateHeader = 8,

        /// <summary>
        /// Default sources for service providers are: the 
        /// Authorization header, POST body and query string.     
        /// </summary>        
        ServiceProviderDefault = HttpAuthorizationHeader | HttpPostBody | HttpQueryString,

        /// <summary>
        /// Default sources for consumers are: the 
        /// WWW-Authenticate header and the POST body.
        /// </summary>        
        ConsumerDefault = HttpWwwAuthenticateHeader | HttpPostBody
    }

    [DebuggerDisplay("[AdditionalParameters: {AdditionalParameters.Count} Callback: {Callback} Consumer Key: {ConsumerKey} Nonce: {Nonce} Realm: {Realm} Signature: {Signature} Signature Method: {SignatureMethod} Timestamp: {Timestamp} Token: {Token} Token Secret: {TokenSecret} Version: {Version}")]        
    public class OAuthParameters
    {
        private readonly IDictionary<string, string> parameters;

        /// <summary>
        /// Create a new empty OAuthParameters.
        /// </summary>
        public OAuthParameters()
        {
            this.parameters = new Dictionary<string, string>();

            this.Callback = null;
            this.ConsumerKey = null;
            this.Nonce = null;
            this.Realm = null;
            this.Signature = null;
            this.SignatureMethod = null;
            this.Timestamp = null;
            this.Token = null;
            this.TokenSecret = null;
            this.Version = null;
            this.AdditionalParameters = new NameValueCollection();
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Callback is a domain term")]
        public string Callback
        {
            get { return this.parameters[Constants.CallbackParameter]; }
            set { this.parameters[Constants.CallbackParameter] = value; }
        }

        public string ConsumerKey
        {
            get { return this.parameters[Constants.ConsumerKeyParameter]; }
            set { this.parameters[Constants.ConsumerKeyParameter] = value; }
        }

        public string Nonce
        {
            get { return this.parameters[Constants.NonceParameter]; }
            set { this.parameters[Constants.NonceParameter] = value; }
        }

        public string Realm
        {
            get { return this.parameters[Constants.RealmParameter]; }
            set { this.parameters[Constants.RealmParameter] = value; }
        }

        public string Signature
        {
            get { return this.parameters[Constants.SignatureParameter]; }
            set { this.parameters[Constants.SignatureParameter] = value; }
        }

        public string SignatureMethod
        {
            get { return this.parameters[Constants.SignatureMethodParameter]; }
            set { this.parameters[Constants.SignatureMethodParameter] = value; }
        }

        public string Timestamp
        {
            get { return this.parameters[Constants.TimestampParameter]; }
            set { this.parameters[Constants.TimestampParameter] = value; }
        }

        public string Token
        {
            get { return this.parameters[Constants.TokenParameter]; }
            set { this.parameters[Constants.TokenParameter] = value; }
        }

        public string TokenSecret
        {
            get { return this.parameters[Constants.TokenSecretParameter]; }
            set { this.parameters[Constants.TokenSecretParameter] = value; }
        }

        public string Version
        {
            get { return this.parameters[Constants.VersionParameter]; }
            set { this.parameters[Constants.VersionParameter] = value; }
        }

        public NameValueCollection AdditionalParameters
        {
            get;
            private set;
        }

        /// <summary>
        /// Parses the OAuth parameters from the HTTP request, sourcing
        /// parameters from all 3 of:
        /// <list>
        ///   <item>The HTTP Authorization header, if present</item>
        ///   <item>The POST body, if present and if the content-type is 
        ///         <c>application/x-www-form-urlencoded</c></item>
        ///   <item>The query string, if present</item>
        /// </list>
        /// TODO: Mention validation
        /// </summary>
        /// 
        /// <param name="request">The HTTP request</param>
        /// 
        /// <returns>
        /// An OAuthParameters object containing the parsed reserved OAuth
        /// parameters and any additional, valid parameters
        /// </returns>
        public static OAuthParameters Parse(HttpRequest request)
        {
            return OAuthParameters.Parse(request, OAuthParameterSources.ServiceProviderDefault);
        }

        /// <summary>
        /// Parses the OAuth parameters from the HTTP request, sourcing 
        /// parameters from the sources specified by <paramref name="sources"/>.
        ///
        /// TODO: Mention validation
        /// </summary>
        /// 
        /// <param name="request">The HTTP request</param>
        /// <param name="sources">The sources to collate parameters from 
        /// (must not be <c>OAuthParameterSources.None</c>)</param>
        /// 
        /// <returns>
        /// An OAuthParameters object containing the parsed reserved OAuth
        /// parameters and any additional, valid parameters
        /// </returns>
        public static OAuthParameters Parse(HttpRequest request, OAuthParameterSources sources)
        {
            return OAuthParameters.DoParse(request.Headers[Constants.AuthorizationHeaderParameter], request.Headers[Constants.WwwAuthenticateHeaderParameter], request.Form, request.QueryString, sources, true);
        }

        /// <summary>
        /// Parses the OAuth parameters from a HttpWebResponse.
        /// </summary>
        /// <param name="response">The Http response</param>
        /// <returns>
        /// An OAuthParameters object containing the parsed reserved OAuth parameters and any additional, valid parameters.</returns>
        public static OAuthParameters Parse(HttpWebResponse response)
        {
            if (response == null)
                return null;

            NameValueCollection bodyParams = new NameValueCollection();

            using (MemoryStream ms = new MemoryStream())
            {
                System.IO.Stream stream = response.GetResponseStream();
                byte[] buffer = new byte[32768];

                int read;

                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }

                Encoding bodyEncoding = Encoding.ASCII;
                if (!String.IsNullOrEmpty(response.ContentEncoding))
                    bodyEncoding = Encoding.GetEncoding(response.ContentEncoding);

                string responseBody = bodyEncoding.GetString(ms.ToArray());

                string[] nameValuePairs = responseBody.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string nameValuePair in nameValuePairs)
                {
                    string[] nameValuePairParts = nameValuePair.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    if (nameValuePairParts.Length == 2)
                        bodyParams.Add(HttpUtility.UrlDecode(nameValuePairParts[0]), HttpUtility.UrlDecode(nameValuePairParts[1]));
                }
            }

            return OAuthParameters.DoParse(null, response.Headers[Constants.WwwAuthenticateHeaderParameter], bodyParams, null, OAuthParameterSources.ConsumerDefault, false);
        }

        /// <summary>
        /// Parses the OAuth parameters from a OAuthProtectedResponse.
        /// </summary>
        /// <param name="response">The OAuthProtectedResponse</param>
        /// <returns>
        /// An OAuthParameters object containing the parsed reserved OAuth parameters and any additional, valid parameters.</returns>
        public static OAuthParameters Parse(OAuthResource response)
        {
            if (response == null)
                return null;

            NameValueCollection bodyParams = new NameValueCollection();

            using (MemoryStream ms = new MemoryStream())
            {
                System.IO.Stream stream = response.GetResponseStream();
                byte[] buffer = new byte[32768];

                int read;

                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }

                Encoding bodyEncoding = Encoding.ASCII;
                if (!String.IsNullOrEmpty(response.ContentEncoding))
                    bodyEncoding = Encoding.GetEncoding(response.ContentEncoding);

                string responseBody = bodyEncoding.GetString(ms.ToArray());

                string[] nameValuePairs = responseBody.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string nameValuePair in nameValuePairs)
                {
                    string[] nameValuePairParts = nameValuePair.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    if (nameValuePairParts.Length == 2)
                        bodyParams.Add(HttpUtility.UrlDecode(nameValuePairParts[0]), HttpUtility.UrlDecode(nameValuePairParts[1]));
                }

                // Reset the stream
                stream.Position = 0;
            }

            return OAuthParameters.DoParse(null, response.Headers[Constants.WwwAuthenticateHeaderParameter], bodyParams, null, OAuthParameterSources.ConsumerDefault, false);
        }

        /// <summary>
        /// Gets additional non-OAuth parameters from the specified name value collections.
        /// </summary>
        /// <param name="parameterCollections">The name value collections to source from</param>
        /// <returns>A NameValueCollection containing all the additional parameters from the 
        /// specified name value collections</returns>
        public static NameValueCollection GetNonOAuthParameters(params NameValueCollection[] parameterCollections)
        {
            NameValueCollection @params = new NameValueCollection();

            foreach (NameValueCollection paramCollection in parameterCollections)
                if (paramCollection != null)
                    foreach (string key in paramCollection.AllKeys)
                        if (Constants.ReservedParameterNames.IndexOf(key) < 0)
                            foreach (string value in paramCollection.GetValues(key))
                                @params.Add(key, value);

            return @params;
        }

        public string ToHeaderFormat()
        {
            string[] excludedParameters = 
            {
                Constants.RealmParameter,
                Constants.TokenSecretParameter
            };

            StringBuilder refAuthHeader = new StringBuilder(Constants.OAuthAuthScheme);
            refAuthHeader.Append(" ");

            bool first = true;

            if (!String.IsNullOrEmpty(this.Realm))
            {
                OAuthParameters.EncodeHeaderValue(refAuthHeader, Constants.RealmParameter, this.Realm, first ? string.Empty : ", ", true);
                first = false;
            }

            foreach (string key in this.parameters.Keys)
            {
                if (!String.IsNullOrEmpty(this.parameters[key]) && Array.IndexOf<string>(excludedParameters, key) < 0)
                {
                    OAuthParameters.EncodeHeaderValue(refAuthHeader, key, this.parameters[key], first ? string.Empty : ", ", true);
                    first = false;
                }
            }

            return refAuthHeader.ToString();
        }

        public string ToQueryStringFormat()
        {
            string[] excludedParameters = 
            {
                Constants.RealmParameter,
                Constants.TokenSecretParameter
            };

            StringBuilder queryString = new StringBuilder();

            bool first = true;

            foreach (string key in this.parameters.Keys)
            {
                if (!String.IsNullOrEmpty(this.parameters[key]) && Array.IndexOf<string>(excludedParameters, key) < 0)
                {
                    OAuthParameters.EncodeHeaderValue(queryString, key, this.parameters[key], first ? string.Empty : "&", false);
                    first = false;
                }
            }

            foreach (string key in this.AdditionalParameters.Keys)
            {
                foreach (string value in this.AdditionalParameters.GetValues(key))
                {
                    OAuthParameters.EncodeHeaderValue(queryString, key, value, first ? string.Empty : "&", false);
                    first = false;
                }
            }

            return queryString.ToString();
        }

        /// <summary>
        /// Validates that all the specified parameters are present and non-empty.
        /// </summary>
        /// 
        /// <param name="requiredParameters">The required parameters</param>
        /// 
        /// <exception cref="OAuth.Net.Common.OAuthRequestException">
        /// If any of the required parameters are missing
        /// </exception>
        public void RequireAllOf(params string[] requiredParameters)
        {
            List<string> missing = new List<string>();

            foreach (string requiredParameter in requiredParameters)
                if (string.IsNullOrEmpty(this.parameters[requiredParameter]))
                    missing.Add(requiredParameter);

            if (missing.Count > 0)
                OAuthRequestException.ThrowParametersAbsent(missing.ToArray(), null);
        }

        /// <summary>
        /// Validates that no parameters are present except those specified.
        /// The specified parameters do not have to exist (they are optional); 
        /// but other parameters MUST not exist.
        /// </summary>
        /// 
        /// <param name="allowedParameters">The allowed parameters</param>
        /// 
        /// <exception cref="OAuth.Net.Common.OAuthRequestException">
        /// If any parameters are not in the set of allowed parameters
        /// </exception>
        public void AllowOnly(params string[] allowedParameters)
        {
            List<string> invalid = new List<string>();

            foreach (var parameter in this.parameters.Keys)
                if (!string.IsNullOrEmpty(this.parameters[parameter]))
                    if (Array.IndexOf<string>(allowedParameters, parameter) < 0)
                        invalid.Add(parameter);

            foreach (var parameter in this.AdditionalParameters.AllKeys)
                if (!string.IsNullOrEmpty(this.AdditionalParameters[parameter]))
                    if (Array.IndexOf<string>(allowedParameters, parameter) < 0)
                        invalid.Add(parameter);

            if (invalid.Count > 0)
                OAuthRequestException.ThrowParametersRejected(invalid.ToArray(), null);
        }

        /// <summary>
        /// Validates that the version matches one of the allowed versions.
        /// </summary>
        /// 
        /// <param name="allowedVersions">The allowed version strings, in order
        /// from earliest to latest</param>
        /// 
        /// <exception cref="OAuth.Net.Common.OAuthRequestException">
        /// If the version does not match any of the allowed versions
        /// </exception>
        public void RequireVersion(params string[] allowedVersions)
        {
            if (allowedVersions == null)
                throw new ArgumentNullException("allowedVersions");

            if (allowedVersions.Length < 1)
                throw new ArgumentException("allowedVersions argument is mandatory", "allowedVersions");

            if (!string.IsNullOrEmpty(this.parameters[Constants.VersionParameter]))
                foreach (string allowedVersion in allowedVersions)
                    if (allowedVersion.Equals(this.parameters[Constants.VersionParameter]))
                        return;

            OAuthRequestException.ThrowVersionRejected(allowedVersions[0], allowedVersions[allowedVersions.Length - 1], null);
        }

        /// <summary>
        /// Creates a normalized representation of the parameters for use in the signature base string.
        /// </summary>
        /// <param name="excludedParameters">Names of parameters to exclude from the normalized string.</param>
        /// <returns>The signature-base normalized representation of the parameters.</returns>
        public string ToNormalizedString(params string[] excludedParameters)
        {
            var @params = new List<KeyValuePair<string, string>>();

            // Add OAuth parameters whose values are not null except excluded parameters
            foreach (var param in this.parameters.Keys)
                if (this.parameters[param] != null && Array.IndexOf<string>(excludedParameters, param) < 0)
                    @params.Add(new KeyValuePair<string, string>(Rfc3986.Encode(param), Rfc3986.Encode(this.parameters[param])));

            // Add all additional parameters
            foreach (var param in this.AdditionalParameters.AllKeys)
                foreach (var value in this.AdditionalParameters.GetValues(param) ?? new string[] { })
                    @params.Add(new KeyValuePair<string, string>(Rfc3986.Encode(param), Rfc3986.Encode(value)));

            // Sort parameters into lexicographic order (by key and value)
            @params.Sort(
                (left, right) =>
                    left.Key.Equals(right.Key, StringComparison.Ordinal)
                        ? string.Compare(left.Value, right.Value, StringComparison.Ordinal)
                        : string.Compare(left.Key, right.Key, StringComparison.Ordinal));

            // Concatenate and encode
            string equals = "=";
            string ampersand = "&";

            StringBuilder parms = new StringBuilder();
            bool first = true;
            foreach (var pair in @params)
            {
                if (first)
                    first = false;
                else
                    parms.Append(ampersand);

                parms.Append(pair.Key).Append(equals).Append(pair.Value);
            }

            return parms.ToString();
        }
                
        internal static OAuthParameters DoParse(string authHeader, string wwwAuthHeader, NameValueCollection form, NameValueCollection queryString, OAuthParameterSources sources, bool validateParameters)
        {
            if (sources == OAuthParameterSources.None)
                throw new ArgumentException("sources must not be OAuthParameterSources.None", "sources");

            bool useAuthHeader = ((sources & OAuthParameterSources.HttpAuthorizationHeader) == OAuthParameterSources.HttpAuthorizationHeader);
            bool useWwwAuthHeader = ((sources & OAuthParameterSources.HttpWwwAuthenticateHeader) == OAuthParameterSources.HttpWwwAuthenticateHeader);
            bool usePost = (sources & OAuthParameterSources.HttpPostBody) == OAuthParameterSources.HttpPostBody;
            bool useQueryString = ((sources & OAuthParameterSources.HttpQueryString) == OAuthParameterSources.HttpQueryString);

            NameValueCollection authHeaderParams = useAuthHeader ? ParseAuthHeader(authHeader) : null;
            NameValueCollection wwwAuthHeaderParams = useWwwAuthHeader ? ParseAuthHeader(wwwAuthHeader) : null;
            NameValueCollection postParams = usePost ? form : null;
            NameValueCollection queryStringParams = useQueryString ? queryString : null;

            // Do validation if required
            if (validateParameters)
            {
                /*
                 * Check for any duplicated OAuth parameters
                 */
                ResultInfo<string[]> result = CheckForDuplicateReservedParameters(
                        authHeaderParams, wwwAuthHeaderParams, postParams, queryStringParams);

                if (!result)
                    OAuthRequestException.ThrowParametersRejected(result, null);

                /*
                 * Check for non-reserved parameters prefixed with oauth_
                 */
                result = CheckForInvalidParameterNames(authHeaderParams, wwwAuthHeaderParams, postParams, queryStringParams);

                if (!result)
                    OAuthRequestException.ThrowParametersRejected(result, null);
            }

            return new OAuthParameters()
            {
                Callback = GetParam(Constants.CallbackParameter, authHeaderParams, wwwAuthHeaderParams, postParams, queryStringParams),
                ConsumerKey = GetParam(Constants.ConsumerKeyParameter, authHeaderParams, wwwAuthHeaderParams, postParams, queryStringParams),
                Nonce = GetParam(Constants.NonceParameter, authHeaderParams, postParams, wwwAuthHeaderParams, queryStringParams),
                Realm = authHeaderParams != null ? authHeaderParams[Constants.RealmParameter] : null,
                Signature = GetParam(Constants.SignatureParameter, authHeaderParams, wwwAuthHeaderParams, postParams, queryStringParams),
                SignatureMethod = GetParam(Constants.SignatureMethodParameter, wwwAuthHeaderParams, authHeaderParams, postParams, queryStringParams),
                Timestamp = GetParam(Constants.TimestampParameter, authHeaderParams, wwwAuthHeaderParams, postParams, queryStringParams),
                Token = GetParam(Constants.TokenParameter, authHeaderParams, wwwAuthHeaderParams, postParams, queryStringParams),
                TokenSecret = GetParam(Constants.TokenSecretParameter, authHeaderParams, wwwAuthHeaderParams, postParams, queryStringParams),
                Version = GetParam(Constants.VersionParameter, authHeaderParams, wwwAuthHeaderParams, postParams, queryStringParams),

                AdditionalParameters = GetNonOAuthParameters(wwwAuthHeaderParams, postParams, queryStringParams)
            };
        }

        private static void EncodeHeaderValue(StringBuilder buffer, string key, string value, string separator, bool quote)
        {
            buffer.Append(separator);
            buffer.Append(Rfc3986.Encode(key));
            buffer.Append("=");

            if (quote)
                buffer.Append('"');

            buffer.Append(Rfc3986.Encode(value));

            if (quote)
                buffer.Append('"');
        }

        /*
         * Check for an OAuth Authorization HTTP header and, if present, parse it
         * and add it to the collection
         */
        private static NameValueCollection ParseAuthHeader(string authHeader)
        {
            if (!string.IsNullOrEmpty(authHeader))
            {
                NameValueCollection @params = new NameValueCollection();

                // Check for OAuth auth-scheme
                Match authSchemeMatch = Constants.OAuthCredentialsRegex.Match(authHeader);
                if (authSchemeMatch.Success)
                {
                    // We have OAuth credentials in the Authorization header; parse the parts
                    // Sad-to-say, but this code is much simpler than the regex for it!
                    string[] authParameterValuePairs = authHeader.Substring(authSchemeMatch.Length)
                        .Split(',');

                    foreach (string authParameterValuePair in authParameterValuePairs)
                    {
                        string[] parts = authParameterValuePair.Trim().Split('=');

                        if (parts.Length == 2)
                        {
                            string parameter = parts[0];
                            string value = parts[1];

                            if (value.StartsWith("\"", StringComparison.Ordinal) && value.EndsWith("\"", StringComparison.Ordinal))
                            {
                                value = value.Substring(1, value.Length - 2);

                                try
                                {
                                    value = Constants.StringEscapeSequence.Replace(
                                        value,
                                        (Match match) =>
                                        {
                                            Group group = match.Groups[1];
                                            if (group.Length == 1)
                                            {
                                                switch (group.Value)
                                                {
                                                    case "\"": return "\"";
                                                    case "'": return "'";
                                                    case "\\": return "\\";
                                                    case "0": return "\0";
                                                    case "a": return "\a";
                                                    case "b": return "\b";
                                                    case "f": return "\f";
                                                    case "n": return "\n";
                                                    case "r": return "\r";
                                                    case "t": return "\t";
                                                    case "v": return "\v";
                                                }
                                            }

                                            return string.Format( 
                                                CultureInfo.InvariantCulture,
                                                "{0}", 
                                                char.Parse(group.Value));
                                        });
                                }
                                catch (FormatException)
                                {
                                    continue;
                                }

                                // Add the parameter and value
                                @params.Add(parameter, value);
                            }
                        }
                    }
                }
                
                return @params;
            }

            return null;
        }
        
        /*
         * Checks for duplicated reserved parameters.
         */
        private static ResultInfo<string[]> CheckForDuplicateReservedParameters(
                params NameValueCollection[] paramCollections)
        {
            List<string> duplicated = new List<string>();
            int count;

            foreach (string param in Constants.ReservedParameterNames)
            {
                count = 0;

                foreach (NameValueCollection paramCollection in paramCollections)
                    if (paramCollection != null)
                    {
                        string[] values = paramCollection.GetValues(param);
                        if (values != null)
                            count += values.Length;
                    }

                if (count > 1)
                    duplicated.Add(param);
            }

            return duplicated.Count > 0
                ? new ResultInfo<string[]>(false, duplicated.ToArray())
                : new ResultInfo<string[]>(true, null);
        }

        /*
         * Checks for non-reserved parameter names starting with oauth_.
         */
        private static ResultInfo<string[]> CheckForInvalidParameterNames(
                params NameValueCollection[] paramCollections)
        {
            List<string> invalid = new List<string>();

            foreach (NameValueCollection paramCollection in paramCollections)
                if (paramCollection != null)
                    foreach (string param in paramCollection.Keys)
                        if (param.StartsWith(Constants.OAuthParameterPrefix, StringComparison.Ordinal)
                                && Constants.ReservedParameterNames.IndexOf(param) < 0)
                            invalid.Add(param);

            return invalid.Count > 0
                ? new ResultInfo<string[]>(false, invalid.ToArray())
                : new ResultInfo<string[]>(true, null);
        }

        /// <summary>
        /// Gets the specified parameter from one of the specified name value collections. The first value
        /// encountered is returned.
        /// </summary>
        /// <param name="param">The parameter to get</param>
        /// <param name="paramCollections">The name value collections to source from</param>
        /// <returns>The first value for the parameter or <c>null</c> if it is not contained in
        /// any of the sources</returns>
        private static string GetParam(string param, params NameValueCollection[] paramCollections)
        {
            foreach (NameValueCollection paramCollection in paramCollections)
                if (paramCollection != null && !string.IsNullOrEmpty(paramCollection[param]))
                    return paramCollection[param];

            return null;
        }
    }
}
