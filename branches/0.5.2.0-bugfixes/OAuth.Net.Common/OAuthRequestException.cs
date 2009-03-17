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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace OAuth.Net.Common
{
    /// <summary>
    /// An OAuthRequestException represents an OAuth protocol error. It is implemented
    /// using the <a href="http://wiki.oauth.net/ProblemReporting">Problem Reporting 
    /// extension</a>.
    /// </summary>
    /// 
    /// <remarks>
    /// <p>
    /// An OAuth exception can be serialized to a HTTP response using the parameters
    /// in the nested <see cref="OAuthRequestExceptionParameters">Parameters</see> class.
    /// </p>
    /// 
    /// <p>
    /// OAuth exceptions that are sent with a HTTP response can be parsed and rethrown
    /// using the <see cref="TryRethrow"/> method.
    /// </p>
    /// </remarks>
    [Serializable]
    [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "Exceptions represent certain defined error messages. Static Throw... methods are provided.")]
    public class OAuthRequestException
        : OAuthException
    {
        protected OAuthRequestException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.Problem = info.GetString("Problem");
            this.Advice = info.GetString("Advice");
            this.AdditionalParameter = (KeyValuePair<string, string>)info.GetValue("AdditionalParameter", typeof(KeyValuePair<string, string>));
        }

        [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "Exceptions represent certain defined error messages. Static Throw... methods are provided.")]
        private OAuthRequestException()
            : base()
        {
        }

        /// <summary>
        /// The problem type.
        /// </summary>
        /// 
        /// <remarks>
        /// The value SHOULD be one of the problem types described in the 
        /// <a href="http://wiki.oauth.net/ProblemReporting">Problem 
        /// Reporting extension</a>.
        /// </remarks>
        public string Problem
        {
            get;
            private set;
        }

        /// <summary>
        /// An optional additional key-value pair with
        /// extra problem reporting information.
        /// </summary>
        /// 
        /// <remarks>This should 
        /// be one of the extra parameters specified in the
        /// <a href="http://wiki.oauth.net/ProblemReporting">Problem 
        /// Reporting extension</a>.
        /// </remarks>
        public KeyValuePair<string, string> AdditionalParameter
        {
            get;
            private set;
        }

        /// <summary>
        /// An optional free-form text value to be shown to the User. 
        /// </summary>
        /// 
        /// <remarks>
        /// It SHOULD describe the problem in a way that's meaningful 
        /// to the User, and suggest how to resolve the problem. 
        /// The Consumer MUST NOT interpret it as HTML or any other 
        /// markup language; all the characters MUST be displayed to 
        /// the user. To indicate a line break, the single character 
        /// LF (Unicode U+000A) MUST be used.
        /// </remarks>
        public string Advice
        {
            get;
            private set;
        }

        /// <summary>
        /// Throws an exception indicating that the 
        /// <see cref="Constants.VersionParameter">oauth_version</see> isn't 
        /// supported. 
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The <see cref="Problem">problem type</see> is 
        /// <see cref="OAuthRequestExceptionProblemTypes.VersionRejected">version_rejected</see>.
        /// </para>
        /// 
        /// <para>
        /// The <see cref="AdditionalParameter">additional parameter</see>
        /// (<see cref="OAuthRequestExceptionParameters.AcceptableVersions">oauth_acceptable_versions</see>)
        /// of the exception indicates the range of versions acceptable to the 
        /// sender. That is, it means the sender will currently accept an 
        /// <see cref="Constants.VersionParameter">oauth_version</see> that's not 
        /// less than the first number and not greater than the second number. 
        /// A version <c>A.B</c> is considered greater than <c>C.D</c> if 
        /// either <c>A &gt; C</c>, or <c>A = C</c> and <c>B &gt; D</c>.
        /// </para>
        /// 
        /// <para>
        /// The <paramref name="advice"/> parameter, if supplied, will be
        /// stored in the <see cref="Advice"/> property.
        /// </para>
        /// 
        /// <para>
        /// The <see cref="Exception.Source"/> will be <see cref="OAuthRequestExceptionSources.Local">local</see>.
        /// </para>
        /// </remarks>
        /// 
        /// <param name="minVersion">The minimum version of the OAuth protocol
        /// that is accepted</param>
        /// <param name="maxVersion">The maximum version of the OAuth protocol
        /// that is accepted</param>
        /// <param name="advice">(Optional) Plain text advice for the user 
        /// of the consumer</param>
        /// 
        /// <exception cref="System.ArgumentException">
        /// If <paramref name="minVersion"/> and/or <paramref name="maxVersion"/> 
        /// is null or empty
        /// </exception>
        /// <exception cref="OAuth.Net.Common.OAuthRequestException">
        /// On success
        /// </exception>
        public static void ThrowVersionRejected(string minVersion, string maxVersion, string advice)
        {
            if (string.IsNullOrEmpty(minVersion))
                throw new ArgumentException("minVersion argument cannot be null or empty", "minVersion");
            if (string.IsNullOrEmpty(maxVersion))
                throw new ArgumentException("maxVersion argument cannot be null or empty", "maxVersion");

            throw new OAuthRequestException()
            {
                Problem = OAuthRequestExceptionProblemTypes.VersionRejected,

                AdditionalParameter = new KeyValuePair<string, string>(
                    OAuthRequestExceptionParameters.AcceptableVersions, minVersion + '-' + maxVersion),

                Advice = advice,

                Source = OAuthRequestExceptionSources.Local
            };
        }

        /// <summary>
        /// Throws an exception indicating that required parameter(s) weren't received.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The <see cref="Problem">problem type</see> is 
        /// <see cref="OAuthRequestExceptionProblemTypes.ParameterAbsent">parameter_absent</see>.
        /// </para>
        /// 
        /// <para>
        /// The <see cref="AdditionalParameter">additional parameter</see>
        /// (<see cref="OAuthRequestExceptionParameters.ParametersAbsent">oauth_parameters_absent</see>)
        /// of the exception indicates the set of parameter names that are absent,
        /// percent-encoded and separated by <c>&amp;</c>.
        /// </para>
        /// 
        /// <para>
        /// The <paramref name="advice"/> parameter, if supplied, will be
        /// stored in the <see cref="Advice"/> property.
        /// </para>
        /// 
        /// <para>
        /// The <see cref="Exception.Source"/> will be <see cref="OAuthRequestExceptionSources.Local">local</see>.
        /// </para>
        /// </remarks>
        /// 
        /// <param name="parameters">The parameters that are absent</param>
        /// <param name="advice">(Optional) Plain text advice for the user 
        /// of the consumer</param>
        /// 
        /// <exception cref="System.ArgumentException">
        /// If <paramref name="parameters"/> is null or empty
        /// </exception>
        /// <exception cref="OAuth.Net.Common.OAuthRequestException">
        /// On success
        /// </exception>
        public static void ThrowParametersAbsent(string[] parameters, string advice)
        {
            if (parameters == null || parameters.Length == 0)
                throw new ArgumentException("parameters argument cannot be null or of length 0", "parameters");

            StringBuilder absentParameters = new StringBuilder();

            bool first = true;
            foreach (string parameter in parameters)
            {
                if (!first)
                    absentParameters.Append("&");
                else
                    first = false;

                absentParameters.Append(Rfc3986.Encode(parameter));
            }

            throw new OAuthRequestException()
            {
                Problem = OAuthRequestExceptionProblemTypes.ParameterAbsent,

                AdditionalParameter = new KeyValuePair<string, string>(
                    OAuthRequestExceptionParameters.ParametersAbsent, absentParameters.ToString()),

                Advice = advice,

                Source = OAuthRequestExceptionSources.Local
            };
        }

        /// <summary>
        /// Throws an exception indicating unexpected parameter(s) were received.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The <see cref="Problem">problem type</see> is 
        /// <see cref="OAuthRequestExceptionProblemTypes.ParameterRejected">parameter_rejected</see>.
        /// </para>
        /// 
        /// <para>
        /// The <see cref="AdditionalParameter">additional parameter</see>
        /// (<see cref="OAuthRequestExceptionParameters.ParametersRejected">oauth_parameters_rejected</see>)
        /// of the exception consists of a set of parameters, encoded as they would be 
        /// in a URL query string. These are parameters that the sender recently 
        /// received but doesn't understand. Note that these parameters will be 
        /// percent-encoded twice: once to form a query string and again because 
        /// the query string is the value of <see cref="OAuthRequestExceptionParameters.ParametersRejected">oauth_parameters_rejected</see>.
        /// </para>
        /// 
        /// <para>
        /// The <paramref name="advice"/> parameter, if supplied, will be
        /// stored in the <see cref="Advice"/> property.
        /// </para>
        /// 
        /// <para>
        /// The <see cref="Exception.Source"/> will be <see cref="OAuthRequestExceptionSources.Local">local</see>.
        /// </para>
        /// </remarks>
        /// 
        /// <param name="parameters">The parameters that are rejected</param>
        /// <param name="advice">(Optional) Plain text advice for the user 
        /// of the consumer</param>
        /// 
        /// <exception cref="System.ArgumentException">
        /// If <paramref name="parameters"/> is null or empty.
        /// </exception>
        /// <exception cref="OAuth.Net.Common.OAuthRequestException">
        /// On success
        /// </exception>
        public static void ThrowParametersRejected(string[] parameters, string advice)
        {
            if (parameters == null || parameters.Length == 0)
                throw new ArgumentException("parameters argument cannot be null or of length 0", "parameters");

            StringBuilder rejectedParameters = new StringBuilder();

            bool first = true;
            foreach (string parameter in parameters)
            {
                if (!first)
                    rejectedParameters.Append("&");
                else
                    first = false;

                rejectedParameters.Append(Rfc3986.Encode(parameter));
            }

            throw new OAuthRequestException()
            {
                Problem = OAuthRequestExceptionProblemTypes.ParameterRejected,

                AdditionalParameter = new KeyValuePair<string, string>(
                    OAuthRequestExceptionParameters.ParametersRejected, rejectedParameters.ToString()),

                Advice = advice,

                Source = OAuthRequestExceptionSources.Local
            };
        }

        /// <summary>
        /// Throws an exception indicating that the timestamp was refused.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The <see cref="Problem">problem type</see> is 
        /// <see cref="OAuthRequestExceptionProblemTypes.TimestampRefused">timestamp_refused</see>.
        /// </para>
        /// 
        /// <para>
        /// The <see cref="AdditionalParameter">additional parameter</see>
        /// (<see cref="OAuthRequestExceptionParameters.AcceptableTimestamps">oauth_acceptable_timestamps</see>)
        /// of the exception consists of two numbers in decimal notation, separated by '-' 
        /// (hyphen). It's the range of timestamps acceptable to the sender. That is, 
        /// it means the sender will currently accept an 
        /// <see cref="Constants.TimestampParameter">oauth_timestamp</see> that's not less 
        /// than the first number and not greater than the second number.
        /// </para>
        /// 
        /// <para>
        /// The <paramref name="advice"/> parameter, if supplied, will be
        /// stored in the <see cref="Advice"/> property.
        /// </para>
        /// 
        /// <para>
        /// The <see cref="Exception.Source"/> will be <see cref="OAuthRequestExceptionSources.Local">local</see>.
        /// </para>
        /// </remarks>
        /// 
        /// <param name="minTimestamp">The minimum allowable timestamp</param>
        /// <param name="maxTimestamp">The maximum allowable timestamp</param>
        /// <param name="advice">(Optional) Plain text advice for the user 
        /// of the consumer</param>
        /// 
        /// <exception cref="System.ArgumentException">
        /// If <paramref name="minTimestamp"/> and/or <paramref name="maxTimestamp"/> 
        /// is less than <c>0</c>
        /// </exception>
        /// <exception cref="OAuth.Net.Common.OAuthRequestException">
        /// On success
        /// </exception>
        public static void ThrowTimestampRefused(long minTimestamp, long maxTimestamp, string advice)
        {
            if (minTimestamp < 0)
                throw new ArgumentException("minTimestamp argument must be greater than or equal to 0", "minTimestamp");
            if (maxTimestamp < 0)
                throw new ArgumentException("maxTimestamp argument must be greater than or equal to 0", "maxTimestamp");

            throw new OAuthRequestException()
            {
                Problem = OAuthRequestExceptionProblemTypes.TimestampRefused,

                AdditionalParameter = new KeyValuePair<string, string>(
                    OAuthRequestExceptionParameters.AcceptableTimestamps, 
                    string.Format(CultureInfo.InvariantCulture, "{0}-{1}", minTimestamp, maxTimestamp)),

                Advice = advice,

                Source = OAuthRequestExceptionSources.Local
            };
        }

        /// <summary>
        /// Throws an exception indicating the nonce was used in a previous request, 
        /// and consequently can't be used now
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The <see cref="Problem">problem type</see> is 
        /// <see cref="OAuthRequestExceptionProblemTypes.NonceUsed">nonce_used</see>.
        /// </para>
        /// 
        /// <para>
        /// The <paramref name="advice"/> parameter, if supplied, will be
        /// stored in the <see cref="Advice"/> property.
        /// </para>
        /// 
        /// <para>
        /// The <see cref="Exception.Source"/> will be <see cref="OAuthRequestExceptionSources.Local">local</see>.
        /// </para>
        /// </remarks>
        /// 
        /// <param name="advice">(Optional) Plain text advice for the user 
        /// of the consumer</param>
        /// 
        /// <exception cref="OAuth.Net.Common.OAuthRequestException">
        /// Always
        /// </exception>
        public static void ThrowNonceUsed(string advice)
        {
            OAuthRequestException.ThrowSimpleReport(OAuthRequestExceptionProblemTypes.NonceUsed, advice);
        }

        /// <summary>
        /// Throws an exception indicating the signature method is invalid.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The <see cref="Problem">problem type</see> is 
        /// <see cref="OAuthRequestExceptionProblemTypes.SignatureMethodRejected">signature_method_rejected</see>.
        /// </para>
        /// 
        /// <para>
        /// The <paramref name="advice"/> parameter, if supplied, will be
        /// stored in the <see cref="Advice"/> property.
        /// </para>
        /// 
        /// <para>
        /// The <see cref="Exception.Source"/> will be <see cref="OAuthRequestExceptionSources.Local">local</see>.
        /// </para>
        /// </remarks>
        /// 
        /// <param name="advice">(Optional) Plain text advice for the user 
        /// of the consumer</param>
        /// 
        /// <exception cref="OAuth.Net.Common.OAuthRequestException">
        /// Always
        /// </exception>
        public static void ThrowSignatureMethodRejected(string advice)
        {
            OAuthRequestException.ThrowSimpleReport(OAuthRequestExceptionProblemTypes.SignatureMethodRejected, advice);
        }

        /// <summary>
        /// Throws an exception indicating the consumer key is unknown to the
        /// service provider.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The <see cref="Problem">problem type</see> is 
        /// <see cref="OAuthRequestExceptionProblemTypes.ConsumerKeyUnknown">consumer_key_unknown</see>.
        /// </para>
        /// 
        /// <para>
        /// The <paramref name="advice"/> parameter, if supplied, will be
        /// stored in the <see cref="Advice"/> property.
        /// </para>
        /// 
        /// <para>
        /// The <see cref="Exception.Source"/> will be <see cref="OAuthRequestExceptionSources.Local">local</see>.
        /// </para>
        /// </remarks>
        /// 
        /// <param name="advice">(Optional) Plain text advice for the user 
        /// of the consumer</param>
        /// 
        /// <exception cref="OAuth.Net.Common.OAuthRequestException">
        /// Always
        /// </exception>
        public static void ThrowConsumerKeyUnknown(string advice)
        {
            OAuthRequestException.ThrowSimpleReport(OAuthRequestExceptionProblemTypes.ConsumerKeyUnknown, advice);
        }

        /// <summary>
        /// Throws an exception indicating the consumer key is permanently unacceptable
        /// to the service provider. For example, the consumer may be blacklisted.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The <see cref="Problem">problem type</see> is 
        /// <see cref="OAuthRequestExceptionProblemTypes.ConsumerKeyRejected">consumer_key_rejected</see>.
        /// </para>
        /// 
        /// <para>
        /// The <paramref name="advice"/> parameter, if supplied, will be
        /// stored in the <see cref="Advice"/> property.
        /// </para>
        /// 
        /// <para>
        /// The <see cref="Exception.Source"/> will be <see cref="OAuthRequestExceptionSources.Local">local</see>.
        /// </para>
        /// </remarks>
        /// 
        /// <param name="advice">(Optional) Plain text advice for the user 
        /// of the consumer</param>
        /// 
        /// <exception cref="OAuth.Net.Common.OAuthRequestException">
        /// Always
        /// </exception>
        public static void ThrowConsumerKeyRejected(string advice)
        {
            OAuthRequestException.ThrowSimpleReport(OAuthRequestExceptionProblemTypes.ConsumerKeyRejected, advice);
        }

        /// <summary>
        /// Throws an exception indicating the consumer key is temporarily unacceptable
        /// to the service provider. For example, the consumer may be being being throttled.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The <see cref="Problem">problem type</see> is 
        /// <see cref="OAuthRequestExceptionProblemTypes.ConsumerKeyRefused">consumer_key_refused</see>.
        /// </para>
        /// 
        /// <para>
        /// The <paramref name="advice"/> parameter, if supplied, will be
        /// stored in the <see cref="Advice"/> property.
        /// </para>
        /// 
        /// <para>
        /// The <see cref="Exception.Source"/> will be <see cref="OAuthRequestExceptionSources.Local">local</see>.
        /// </para>
        /// </remarks>
        /// 
        /// <param name="advice">(Optional) Plain text advice for the user 
        /// of the consumer</param>
        /// 
        /// <exception cref="OAuth.Net.Common.OAuthRequestException">
        /// Always
        /// </exception>
        public static void ThrowConsumerKeyRefused(string advice)
        {
            OAuthRequestException.ThrowSimpleReport(OAuthRequestExceptionProblemTypes.ConsumerKeyRefused, advice);
        }

        /// <summary>
        /// Throws an exception indicating the supplied signature is invalid. That is, 
        /// it doesn't match the signature computed by the Service Provider. 
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The <see cref="Problem">problem type</see> is 
        /// <see cref="OAuthRequestExceptionProblemTypes.SignatureInvalid">signature_invalid</see>.
        /// </para>
        /// 
        /// <para>
        /// The <paramref name="advice"/> parameter, if supplied, will be
        /// stored in the <see cref="Advice"/> property.
        /// </para>
        /// 
        /// <para>
        /// The <see cref="Exception.Source"/> will be <see cref="OAuthRequestExceptionSources.Local">local</see>.
        /// </para>
        /// </remarks>
        /// 
        /// <param name="advice">(Optional) Plain text advice for the user 
        /// of the consumer</param>
        /// 
        /// <exception cref="OAuth.Net.Common.OAuthRequestException">
        /// Always
        /// </exception>
        public static void ThrowSignatureInvalid(string advice)
        {
            OAuthRequestException.ThrowSimpleReport(OAuthRequestExceptionProblemTypes.SignatureInvalid, advice);
        }

        /// <summary>
        /// Throws an exception indicating the supplied token is unacceptable to 
        /// the service provider. The reason is unspecified. It might mean that 
        /// the token was never issued, or consumed or expired and then subsequently 
        /// forgotten by the service provider. 
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The <see cref="Problem">problem type</see> is 
        /// <see cref="OAuthRequestExceptionProblemTypes.TokenRejected">token_rejected</see>.
        /// </para>
        /// 
        /// <para>
        /// The <paramref name="advice"/> parameter, if supplied, will be
        /// stored in the <see cref="Advice"/> property.
        /// </para>
        /// 
        /// <para>
        /// The <see cref="Exception.Source"/> will be <see cref="OAuthRequestExceptionSources.Local">local</see>.
        /// </para>
        /// </remarks>
        /// 
        /// <param name="advice">(Optional) Plain text advice for the user 
        /// of the consumer</param>
        /// 
        /// <exception cref="OAuth.Net.Common.OAuthRequestException">
        /// Always
        /// </exception>
        public static void ThrowTokenRejected(string advice)
        {
            OAuthRequestException.ThrowSimpleReport(OAuthRequestExceptionProblemTypes.TokenRejected, advice);
        }

        /// <summary>
        /// Throws an exception indicating the supplied token has been consumed. 
        /// That is, it can't be used any more because it has already been used 
        /// in a previous request or requests. 
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The <see cref="Problem">problem type</see> is 
        /// <see cref="OAuthRequestExceptionProblemTypes.TokenUsed">token_used</see>.
        /// </para>
        /// 
        /// <para>
        /// The <paramref name="advice"/> parameter, if supplied, will be
        /// stored in the <see cref="Advice"/> property.
        /// </para>
        /// 
        /// <para>
        /// The <see cref="Exception.Source"/> will be <see cref="OAuthRequestExceptionSources.Local">local</see>.
        /// </para>
        /// </remarks>
        /// 
        /// <param name="advice">(Optional) Plain text advice for the user 
        /// of the consumer</param>
        /// 
        /// <exception cref="OAuth.Net.Common.OAuthRequestException">
        /// Always
        /// </exception>
        public static void ThrowTokenUsed(string advice)
        {
            OAuthRequestException.ThrowSimpleReport(OAuthRequestExceptionProblemTypes.TokenUsed, advice);
        }

        /// <summary>
        /// Throws an exception indicating the supplied token has expired. That 
        /// is, it was issued too long ago to be used now. 
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The <see cref="Problem">problem type</see> is 
        /// <see cref="OAuthRequestExceptionProblemTypes.TokenExpired">token_expired</see>.
        /// </para>
        /// 
        /// <para>
        /// The <paramref name="advice"/> parameter, if supplied, will be
        /// stored in the <see cref="Advice"/> property.
        /// </para>
        /// 
        /// <para>
        /// The <see cref="Exception.Source"/> will be <see cref="OAuthRequestExceptionSources.Local">local</see>.
        /// </para>
        /// </remarks>
        /// 
        /// <param name="advice">(Optional) Plain text advice for the user 
        /// of the consumer</param>
        /// 
        /// <exception cref="OAuth.Net.Common.OAuthRequestException">
        /// Always
        /// </exception>
        public static void ThrowTokenExpired(string advice)
        {
            OAuthRequestException.ThrowSimpleReport(OAuthRequestExceptionProblemTypes.TokenExpired, advice);
        }

        /// <summary>
        /// Throws an exception indicating the supplied token has been revoked. 
        /// That is, the service provider has unilaterally decided it will never 
        /// accept this token.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The <see cref="Problem">problem type</see> is 
        /// <see cref="OAuthRequestExceptionProblemTypes.TokenRevoked">token_revoked</see>.
        /// </para>
        /// 
        /// <para>
        /// The <paramref name="advice"/> parameter, if supplied, will be
        /// stored in the <see cref="Advice"/> property.
        /// </para>
        /// 
        /// <para>
        /// The <see cref="Exception.Source"/> will be <see cref="OAuthRequestExceptionSources.Local">local</see>.
        /// </para>
        /// </remarks>
        /// 
        /// <param name="advice">(Optional) Plain text advice for the user 
        /// of the consumer</param>
        /// 
        /// <exception cref="OAuth.Net.Common.OAuthRequestException">
        /// Always
        /// </exception>
        public static void ThrowTokenRevoked(string advice)
        {
            OAuthRequestException.ThrowSimpleReport(OAuthRequestExceptionProblemTypes.TokenRevoked, advice);
        }

        //// TODO: ThrowAdditionalAuthorizationRequired
        //// TODO: ThrowPermissionUnknown
        //// TODO: ThrowPermissionDenied
        //// TODO: ThrowUserRefused

        /// <summary>
        /// Tries to parse an OAuthRequestException from some OAuth parameters. If an exception
        /// is indicated, the exception will be re-thrown.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// If no exception is indicated, this method will return without throwing.
        /// </para>
        /// 
        /// <para>
        /// This will handle future <a href="http://wiki.oauth.net/ProblemReporting">Problem 
        /// Reporting</a> problem types, but not future additional parameters.
        /// </para>
        /// </remarks>
        /// 
        /// <example>
        /// This example shows how to rethrow an OAuth exception transmitted in a HTTP response.
        /// 
        /// <code lang="C#" numberLines="true">
        /// HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        /// OAuthParameters responseParameters = OAuthParameters.Parse(response);
        /// OAuthRequestException.TryRethrow(responseParameters);
        /// </code>
        /// </example>
        /// 
        /// <param name="parameters">The OAuth parameters</param>
        /// 
        /// <exception cref="OAuth.Net.Common.OAuthRequestException">
        /// If the OAuth parameters indicate an OAuth exception
        /// </exception>
        public static void TryRethrow(OAuthParameters parameters)
        {
            if (parameters == null || parameters.AdditionalParameters == null)
                return;

            if (!string.IsNullOrEmpty(parameters.AdditionalParameters[OAuthRequestExceptionParameters.Problem]))
            {
                OAuthRequestException ex = new OAuthRequestException()
                {
                    Problem = parameters.AdditionalParameters[OAuthRequestExceptionParameters.Problem],
                    Advice = parameters.AdditionalParameters[OAuthRequestExceptionParameters.ProblemAdvice],
                    Source = OAuthRequestExceptionSources.Remote
                };

                // Load additional parameter for specific types
                switch (parameters.AdditionalParameters[OAuthRequestExceptionParameters.Problem])
                {
                    case OAuthRequestExceptionProblemTypes.VersionRejected:
                        if (!string.IsNullOrEmpty(parameters.AdditionalParameters[OAuthRequestExceptionParameters.AcceptableVersions]))
                            ex.AdditionalParameter = new KeyValuePair<string, string>(
                                OAuthRequestExceptionParameters.AcceptableVersions,
                                parameters.AdditionalParameters[OAuthRequestExceptionParameters.AcceptableVersions]);

                        break;

                    case OAuthRequestExceptionProblemTypes.ParameterAbsent:
                        if (!string.IsNullOrEmpty(parameters.AdditionalParameters[OAuthRequestExceptionParameters.ParametersAbsent]))
                            ex.AdditionalParameter = new KeyValuePair<string, string>(
                                OAuthRequestExceptionParameters.ParametersAbsent,
                                parameters.AdditionalParameters[OAuthRequestExceptionParameters.ParametersAbsent]);

                        break;

                    case OAuthRequestExceptionProblemTypes.ParameterRejected:
                        if (!string.IsNullOrEmpty(parameters.AdditionalParameters[OAuthRequestExceptionParameters.ParametersRejected]))
                            ex.AdditionalParameter = new KeyValuePair<string, string>(
                                OAuthRequestExceptionParameters.ParametersRejected,
                                parameters.AdditionalParameters[OAuthRequestExceptionParameters.ParametersRejected]);

                        break;

                    case OAuthRequestExceptionProblemTypes.TimestampRefused:
                        if (!string.IsNullOrEmpty(parameters.AdditionalParameters[OAuthRequestExceptionParameters.AcceptableTimestamps]))
                            ex.AdditionalParameter = new KeyValuePair<string, string>(
                                OAuthRequestExceptionParameters.AcceptableTimestamps,
                                parameters.AdditionalParameters[OAuthRequestExceptionParameters.AcceptableTimestamps]);

                        break;
                }

                // Throw the OAuthRequestException
                throw ex;
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Problem", this.Problem, typeof(string));
            info.AddValue("Advice", this.Advice, typeof(string));
            info.AddValue("AdditionalParameter", this.AdditionalParameter, typeof(KeyValuePair<string, string>));
        }

        /// <summary>
        /// Produces a string representation of the exception for use in the
        /// HTTP WWW-Authenticate header.
        /// </summary>
        /// 
        /// <remarks>
        /// The format is similar to:
        /// 
        /// <code>OAuth realm="...", oauth_problem="..", oauth_parameters_absent=".."</code>
        /// </remarks>
        /// 
        /// <param name="realm">The realm to put in the header</param>
        /// 
        /// <returns>The exception in WWW-Authenticate header format</returns>
        public string ToHeaderFormat(string realm)
        {
            var header = new StringBuilder();

            // Auth-scheme
            header.Append(OAuth.Net.Common.Constants.OAuthAuthScheme).Append(" ");

            // Realm
            header.Append(OAuth.Net.Common.Constants.RealmParameter).Append("=\"").Append(realm.Replace("\"", "\\\"")).Append("\"");

            // Problem
            header.Append(", ").Append(OAuthRequestExceptionParameters.Problem).Append("=\"").Append(Problem).Append("\"");

            // Additional parameter
            if (!string.IsNullOrEmpty(this.AdditionalParameter.Key)
                    && !string.IsNullOrEmpty(this.AdditionalParameter.Value))
                header.Append(", ").Append(this.AdditionalParameter.Key).Append("=\"").Append(this.AdditionalParameter.Value).Append("\"");

            // Advice
            if (!string.IsNullOrEmpty(this.Advice))
                header.Append(", ").Append(OAuthRequestExceptionParameters.ProblemAdvice).Append("=\"").Append(Advice.Replace("\"", "\\\"").Replace("\n", "\\n")).Append("\"");

            return header.ToString();
        }

        private static void ThrowSimpleReport(string problem, string advice)
        {
            throw new OAuthRequestException()
            {
                Problem = problem,
                Advice = advice,
                Source = OAuthRequestExceptionSources.Local
            };
        }
    }
}
