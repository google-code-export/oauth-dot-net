using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace OAuth.Net.Common
{
    /// <summary>
    /// Builds and throws <see cref="OAuthRequestException"/> instances
    /// for particular scenarios.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    /// To intercept the user-friendly advice message passed to the consumer
    /// with this exception, set the <see cref="Adviser"/> property to an 
    /// instance of the <see cref="IProblemReportingAdviser"/> interface.
    /// </para>
    /// </remarks>
    public class OAuthRequestExceptionFactory
    {
        /// <summary>
        /// The <see cref="IProblemReportingAdviser"/> to be called to
        /// generate user-friendly advice for exceptions thrown by this
        /// factory.
        /// </summary>
        public IProblemReportingAdviser Adviser
        {
            get; set;
        }

        /// <summary>
        /// Creates an OAuthRequestExceptionFactory which creates and
        /// throws instances of <see cref="OAuthRequestException"/>, with
        /// no adviser.
        /// </summary>
        /// <remarks>
        /// Since no adviser is specified, the exceptions thrown by this 
        /// factory will have their <see cref="OAuthRequestException.Advice"/>
        /// properties set to the advice passed in to the Throw... methods.
        /// </remarks>
        public OAuthRequestExceptionFactory()
        {
        }

        /// <summary>
        /// Creates an OAuthRequestExceptionFactory which creates and throws
        /// instances of <see cref="OAuthRequestException"/>, using the 
        /// supplied <see cref="IProblemReportingAdviser"/> to generate
        /// user-friendly advice for each exception thrown.
        /// </summary>
        /// <param name="adviser">Problem Reporting adviser</param>
        public OAuthRequestExceptionFactory(IProblemReportingAdviser adviser)
        {
            this.Adviser = adviser;
        }

        /// <overloads>
        /// Throws an exception indicating that the 
        /// <see cref="Constants.VersionParameter">oauth_version</see> isn't 
        /// supported. 
        /// </overloads>
        /// 
        /// <remarks>
        /// <para>
        /// The <see cref="OAuthRequestException.Problem">problem type</see> is 
        /// <see cref="OAuthRequestExceptionProblemTypes.VersionRejected">version_rejected</see>.
        /// </para>
        /// 
        /// <para>
        /// The <see cref="OAuthRequestException.AdditionalParameter">additional parameter</see>
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
        /// If an <see cref="Adviser"/> is set, and it returns non-<c>null</c>
        /// advise for this exception, this advice will be stored in the 
        /// <see cref="OAuthRequestException.Advice"/> property. Otherwise, 
        /// the <paramref name="advice"/> supplied will be stored.
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
        public void ThrowVersionRejected(string minVersion, string maxVersion, string advice)
        {
            if (string.IsNullOrEmpty(minVersion))
                throw new ArgumentException("minVersion argument cannot be null or empty", "minVersion");
            if (string.IsNullOrEmpty(maxVersion))
                throw new ArgumentException("maxVersion argument cannot be null or empty", "maxVersion");

            throw PostApplyAdviser(() => new OAuthRequestException()
                {
                    Problem = OAuthRequestExceptionProblemTypes.VersionRejected,

                    AdditionalParameter = new KeyValuePair<string, string>(
                        OAuthRequestExceptionParameters.AcceptableVersions, minVersion + '-' + maxVersion),

                    Advice = advice,

                    Source = OAuthRequestExceptionSources.Local
                });
        }

        /// <summary>
        /// Throws an exception indicating that required parameter(s) weren't received.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The <see cref="OAuthRequestException.Problem">problem type</see> is 
        /// <see cref="OAuthRequestExceptionProblemTypes.ParameterAbsent">parameter_absent</see>.
        /// </para>
        /// 
        /// <para>
        /// The <see cref="OAuthRequestException.AdditionalParameter">additional parameter</see>
        /// (<see cref="OAuthRequestExceptionParameters.ParametersAbsent">oauth_parameters_absent</see>)
        /// of the exception indicates the set of parameter names that are absent,
        /// percent-encoded and separated by <c>&amp;</c>.
        /// </para>
        /// 
        /// <para>
        /// If an <see cref="Adviser"/> is set, and it returns non-<c>null</c>
        /// advise for this exception, this advice will be stored in the 
        /// <see cref="OAuthRequestException.Advice"/> property. Otherwise, 
        /// the <paramref name="advice"/> supplied will be stored.
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
        public void ThrowParametersAbsent(string[] parameters, string advice)
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

            throw PostApplyAdviser(() => new OAuthRequestException()
                {
                    Problem = OAuthRequestExceptionProblemTypes.ParameterAbsent,

                    AdditionalParameter = new KeyValuePair<string, string>(
                        OAuthRequestExceptionParameters.ParametersAbsent, absentParameters.ToString()),

                    Advice = advice,

                    Source = OAuthRequestExceptionSources.Local
                });
        }

        /// <summary>
        /// Throws an exception indicating unexpected parameter(s) were received.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The <see cref="OAuthRequestException.Problem">problem type</see> is 
        /// <see cref="OAuthRequestExceptionProblemTypes.ParameterRejected">parameter_rejected</see>.
        /// </para>
        /// 
        /// <para>
        /// The <see cref="OAuthRequestException.AdditionalParameter">additional parameter</see>
        /// (<see cref="OAuthRequestExceptionParameters.ParametersRejected">oauth_parameters_rejected</see>)
        /// of the exception consists of a set of parameters, encoded as they would be 
        /// in a URL query string. These are parameters that the sender recently 
        /// received but doesn't understand. Note that these parameters will be 
        /// percent-encoded twice: once to form a query string and again because 
        /// the query string is the value of <see cref="OAuthRequestExceptionParameters.ParametersRejected">oauth_parameters_rejected</see>.
        /// </para>
        /// 
        /// <para>
        /// If an <see cref="Adviser"/> is set, and it returns non-<c>null</c>
        /// advise for this exception, this advice will be stored in the 
        /// <see cref="OAuthRequestException.Advice"/> property. Otherwise, 
        /// the <paramref name="advice"/> supplied will be stored.
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
        public void ThrowParametersRejected(string[] parameters, string advice)
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

            throw PostApplyAdviser(() => new OAuthRequestException()
                {
                    Problem = OAuthRequestExceptionProblemTypes.ParameterRejected,

                    AdditionalParameter = new KeyValuePair<string, string>(
                        OAuthRequestExceptionParameters.ParametersRejected, rejectedParameters.ToString()),

                    Advice = advice,

                    Source = OAuthRequestExceptionSources.Local
                });
        }

        /// <summary>
        /// Throws an exception indicating that the timestamp was refused.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The <see cref="OAuthRequestException.Problem">problem type</see> is 
        /// <see cref="OAuthRequestExceptionProblemTypes.TimestampRefused">timestamp_refused</see>.
        /// </para>
        /// 
        /// <para>
        /// The <see cref="OAuthRequestException.AdditionalParameter">additional parameter</see>
        /// (<see cref="OAuthRequestExceptionParameters.AcceptableTimestamps">oauth_acceptable_timestamps</see>)
        /// of the exception consists of two numbers in decimal notation, separated by '-' 
        /// (hyphen). It's the range of timestamps acceptable to the sender. That is, 
        /// it means the sender will currently accept an 
        /// <see cref="Constants.TimestampParameter">oauth_timestamp</see> that's not less 
        /// than the first number and not greater than the second number.
        /// </para>
        /// 
        /// <para>
        /// If an <see cref="Adviser"/> is set, and it returns non-<c>null</c>
        /// advise for this exception, this advice will be stored in the 
        /// <see cref="OAuthRequestException.Advice"/> property. Otherwise, 
        /// the <paramref name="advice"/> supplied will be stored.
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
        public void ThrowTimestampRefused(long minTimestamp, long maxTimestamp, string advice)
        {
            if (minTimestamp < 0)
                throw new ArgumentException("minTimestamp argument must be greater than or equal to 0", "minTimestamp");
            if (maxTimestamp < 0)
                throw new ArgumentException("maxTimestamp argument must be greater than or equal to 0", "maxTimestamp");

            throw PostApplyAdviser(() => new OAuthRequestException()
                {
                    Problem = OAuthRequestExceptionProblemTypes.TimestampRefused,

                    AdditionalParameter = new KeyValuePair<string, string>(
                        OAuthRequestExceptionParameters.AcceptableTimestamps,
                        string.Format(CultureInfo.InvariantCulture, "{0}-{1}", minTimestamp, maxTimestamp)),

                    Advice = advice,

                    Source = OAuthRequestExceptionSources.Local
                });
        }

        /// <summary>
        /// Throws an exception indicating the nonce was used in a previous request, 
        /// and consequently can't be used now
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The <see cref="OAuthRequestException.Problem">problem type</see> is 
        /// <see cref="OAuthRequestExceptionProblemTypes.NonceUsed">nonce_used</see>.
        /// </para>
        /// 
        /// <para>
        /// If an <see cref="Adviser"/> is set, and it returns non-<c>null</c>
        /// advise for this exception, this advice will be stored in the 
        /// <see cref="OAuthRequestException.Advice"/> property. Otherwise, 
        /// the <paramref name="advice"/> supplied will be stored.
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
        public void ThrowNonceUsed(string advice)
        {
            throw PostApplyAdviser(() => BuildSimpleReport(
                OAuthRequestExceptionProblemTypes.NonceUsed, 
                advice));
        }

        /// <summary>
        /// Throws an exception indicating the signature method is invalid.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The <see cref="OAuthRequestException.Problem">problem type</see> is 
        /// <see cref="OAuthRequestExceptionProblemTypes.SignatureMethodRejected">signature_method_rejected</see>.
        /// </para>
        /// 
        /// <para>
        /// If an <see cref="Adviser"/> is set, and it returns non-<c>null</c>
        /// advise for this exception, this advice will be stored in the 
        /// <see cref="OAuthRequestException.Advice"/> property. Otherwise, 
        /// the <paramref name="advice"/> supplied will be stored.
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
        public void ThrowSignatureMethodRejected(string advice)
        {
            throw PostApplyAdviser(() => BuildSimpleReport(
                OAuthRequestExceptionProblemTypes.SignatureMethodRejected, 
                advice));
        }

        /// <summary>
        /// Throws an exception indicating the consumer key is unknown to the
        /// service provider.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The <see cref="OAuthRequestException.Problem">problem type</see> is 
        /// <see cref="OAuthRequestExceptionProblemTypes.ConsumerKeyUnknown">consumer_key_unknown</see>.
        /// </para>
        /// 
        /// <para>
        /// If an <see cref="Adviser"/> is set, and it returns non-<c>null</c>
        /// advise for this exception, this advice will be stored in the 
        /// <see cref="OAuthRequestException.Advice"/> property. Otherwise, 
        /// the <paramref name="advice"/> supplied will be stored.
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
        public void ThrowConsumerKeyUnknown(string advice)
        {
            throw PostApplyAdviser(() => BuildSimpleReport(
                OAuthRequestExceptionProblemTypes.ConsumerKeyUnknown, 
                advice));
        }

        /// <summary>
        /// Throws an exception indicating the consumer key is permanently unacceptable
        /// to the service provider. For example, the consumer may be blacklisted.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The <see cref="OAuthRequestException.Problem">problem type</see> is 
        /// <see cref="OAuthRequestExceptionProblemTypes.ConsumerKeyRejected">consumer_key_rejected</see>.
        /// </para>
        /// 
        /// <para>
        /// If an <see cref="Adviser"/> is set, and it returns non-<c>null</c>
        /// advise for this exception, this advice will be stored in the 
        /// <see cref="OAuthRequestException.Advice"/> property. Otherwise, 
        /// the <paramref name="advice"/> supplied will be stored.
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
        public void ThrowConsumerKeyRejected(string advice)
        {
            throw PostApplyAdviser(() => BuildSimpleReport(
                OAuthRequestExceptionProblemTypes.ConsumerKeyRejected, 
                advice));
        }

        /// <summary>
        /// Throws an exception indicating the consumer key is temporarily unacceptable
        /// to the service provider. For example, the consumer may be being being throttled.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The <see cref="OAuthRequestException.Problem">problem type</see> is 
        /// <see cref="OAuthRequestExceptionProblemTypes.ConsumerKeyRefused">consumer_key_refused</see>.
        /// </para>
        /// 
        /// <para>
        /// If an <see cref="Adviser"/> is set, and it returns non-<c>null</c>
        /// advise for this exception, this advice will be stored in the 
        /// <see cref="OAuthRequestException.Advice"/> property. Otherwise, 
        /// the <paramref name="advice"/> supplied will be stored.
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
        public void ThrowConsumerKeyRefused(string advice)
        {
            throw PostApplyAdviser(() => BuildSimpleReport(
                OAuthRequestExceptionProblemTypes.ConsumerKeyRefused, 
                advice));
        }

        /// <summary>
        /// Throws an exception indicating the supplied signature is invalid. That is, 
        /// it doesn't match the signature computed by the Service Provider. 
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The <see cref="OAuthRequestException.Problem">problem type</see> is 
        /// <see cref="OAuthRequestExceptionProblemTypes.SignatureInvalid">signature_invalid</see>.
        /// </para>
        /// 
        /// <para>
        /// If an <see cref="Adviser"/> is set, and it returns non-<c>null</c>
        /// advise for this exception, this advice will be stored in the 
        /// <see cref="OAuthRequestException.Advice"/> property. Otherwise, 
        /// the <paramref name="advice"/> supplied will be stored.
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
        public void ThrowSignatureInvalid(string advice)
        {
            throw PostApplyAdviser(() => BuildSimpleReport(
                OAuthRequestExceptionProblemTypes.SignatureInvalid, 
                advice));
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
        /// The <see cref="OAuthRequestException.Problem">problem type</see> is 
        /// <see cref="OAuthRequestExceptionProblemTypes.TokenRejected">token_rejected</see>.
        /// </para>
        /// 
        /// <para>
        /// If an <see cref="Adviser"/> is set, and it returns non-<c>null</c>
        /// advise for this exception, this advice will be stored in the 
        /// <see cref="OAuthRequestException.Advice"/> property. Otherwise, 
        /// the <paramref name="advice"/> supplied will be stored.
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
        public void ThrowTokenRejected(string advice)
        {
            throw PostApplyAdviser(() => BuildSimpleReport(
                OAuthRequestExceptionProblemTypes.TokenRejected, 
                advice));
        }

        /// <summary>
        /// Throws an exception indicating the supplied token has been consumed. 
        /// That is, it can't be used any more because it has already been used 
        /// in a previous request or requests. 
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The <see cref="OAuthRequestException.Problem">problem type</see> is 
        /// <see cref="OAuthRequestExceptionProblemTypes.TokenUsed">token_used</see>.
        /// </para>
        /// 
        /// <para>
        /// If an <see cref="Adviser"/> is set, and it returns non-<c>null</c>
        /// advise for this exception, this advice will be stored in the 
        /// <see cref="OAuthRequestException.Advice"/> property. Otherwise, 
        /// the <paramref name="advice"/> supplied will be stored.
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
        public void ThrowTokenUsed(string advice)
        {
            throw PostApplyAdviser(() => BuildSimpleReport(
                OAuthRequestExceptionProblemTypes.TokenUsed, 
                advice));
        }

        /// <summary>
        /// Throws an exception indicating the supplied token has expired. That 
        /// is, it was issued too long ago to be used now. 
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The <see cref="OAuthRequestException.Problem">problem type</see> is 
        /// <see cref="OAuthRequestExceptionProblemTypes.TokenExpired">token_expired</see>.
        /// </para>
        /// 
        /// <para>
        /// If an <see cref="Adviser"/> is set, and it returns non-<c>null</c>
        /// advise for this exception, this advice will be stored in the 
        /// <see cref="OAuthRequestException.Advice"/> property. Otherwise, 
        /// the <paramref name="advice"/> supplied will be stored.
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
        public void ThrowTokenExpired(string advice)
        {
            throw PostApplyAdviser(() => BuildSimpleReport(
                OAuthRequestExceptionProblemTypes.TokenExpired, 
                advice));
        }

        /// <summary>
        /// Throws an exception indicating the supplied token has been revoked. 
        /// That is, the service provider has unilaterally decided it will never 
        /// accept this token.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The <see cref="OAuthRequestException.Problem">problem type</see> is 
        /// <see cref="OAuthRequestExceptionProblemTypes.TokenRevoked">token_revoked</see>.
        /// </para>
        /// 
        /// <para>
        /// If an <see cref="Adviser"/> is set, and it returns non-<c>null</c>
        /// advise for this exception, this advice will be stored in the 
        /// <see cref="OAuthRequestException.Advice"/> property. Otherwise, 
        /// the <paramref name="advice"/> supplied will be stored.
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
        public void ThrowTokenRevoked(string advice)
        {
            throw PostApplyAdviser(() => BuildSimpleReport(
                OAuthRequestExceptionProblemTypes.TokenRevoked, 
                advice));
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
        public void TryRethrow(OAuthParameters parameters)
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
                throw PostApplyAdviser(() => ex);
            }
        }

        // Builds OAuthRequestExceptions without additional parameters
        private static OAuthRequestException BuildSimpleReport(
            string problem, 
            string advice)
        {
            return new OAuthRequestException()
            {
                Problem = problem,
                Advice = advice,
                Source = OAuthRequestExceptionSources.Local
            };
        }

        // Applys the adviser (if not null) to the exception
        private OAuthRequestException PostApplyAdviser(
            ExceptionBuilder<OAuthRequestException> builder)
        {
            OAuthRequestException ex = builder();

            if (this.Adviser == null)
                return ex;

            string advice = this.Adviser.AdviseUpon(ex);

            if (advice != null)
                ex.Advice = advice;

            return ex;
        }

        // A method which returns an exception
        private delegate TExceptionResult ExceptionBuilder<TExceptionResult>()
            where TExceptionResult : Exception;
    }
}
