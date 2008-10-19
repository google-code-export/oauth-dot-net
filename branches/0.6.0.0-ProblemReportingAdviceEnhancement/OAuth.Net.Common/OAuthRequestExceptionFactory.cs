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
using System.Globalization;
using System.Text;
using Microsoft.Practices.ServiceLocation;

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
        /// Creates an OAuthRequestExceptionFactory which creates and
        /// throws instances of <see cref="OAuthRequestException"/>, with
        /// no adviser.
        /// </summary>
        /// <remarks>
        /// Since no adviser is specified, the exceptions thrown by this 
        /// factory will have their <see cref="OAuthRequestException.Advice"/>
        /// properties set to <c>null</c>.
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

        /// <summary>
        /// Creates an OAuthRequestExceptionFactory which creates and throws
        /// instances of <see cref="OAuthRequestException"/>, using the 
        /// supplied <see cref="ProblemReportingAdviserDelegate"/> to generate
        /// user-friendly advice for each exception thrown.
        /// </summary>
        /// <param name="adviser">Problem Reporting adviser delegate</param>
        public OAuthRequestExceptionFactory(ProblemReportingAdviserDelegate adviser)
        {
            this.Adviser = new DelegatedProblemReportingAdviser(adviser);
        }

        // A method which returns an exception
        private delegate TExceptionResult ExceptionBuilder<TExceptionResult>()
            where TExceptionResult : Exception;

        /// <summary>
        /// The <see cref="IProblemReportingAdviser"/> to be called to
        /// generate user-friendly advice for exceptions thrown by this
        /// factory.
        /// </summary>
        public IProblemReportingAdviser Adviser
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a new OAuthRequestExceptionFactory, configured with the 
        /// default adviser from the current service locator.
        /// </summary>
        /// <returns>A new OAuthRequestExceptionFactory</returns>
        public static OAuthRequestExceptionFactory WithInjectedAdviser()
        {
            try
            {
                return new OAuthRequestExceptionFactory(
                    ServiceLocator.Current.GetInstance<IProblemReportingAdviser>());
            }
            catch (ActivationException)
            {
                // If no IProblemReportingAdviser is set up, don't use one
                return new OAuthRequestExceptionFactory();
            }
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
        /// <see cref="ProblemReportingProblemTypes.VersionRejected">version_rejected</see>.
        /// </para>
        /// 
        /// <para>
        /// The <see cref="OAuthRequestException.AdditionalParameter">additional parameter</see>
        /// (<see cref="ProblemReportingParameters.AcceptableVersions">oauth_acceptable_versions</see>)
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
        /// the <see cref="OAuthRequestException.Advice"/> will be <c>null</c>.
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
        /// 
        /// <exception cref="System.ArgumentException">
        /// If <paramref name="minVersion"/> and/or <paramref name="maxVersion"/> 
        /// is null or empty
        /// </exception>
        /// <exception cref="OAuth.Net.Common.OAuthRequestException">
        /// On success
        /// </exception>
        public void ThrowVersionRejected(string minVersion, string maxVersion)
        {
            if (string.IsNullOrEmpty(minVersion))
                throw new ArgumentException("minVersion argument cannot be null or empty", "minVersion");
            if (string.IsNullOrEmpty(maxVersion))
                throw new ArgumentException("maxVersion argument cannot be null or empty", "maxVersion");

            throw this.PostApplyAdviser(() => new OAuthRequestException()
                {
                    Problem = ProblemReportingProblemTypes.VersionRejected,

                    AdditionalParameter = new KeyValuePair<string, string>(
                        ProblemReportingParameters.AcceptableVersions, minVersion + '-' + maxVersion),

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
        /// <see cref="ProblemReportingProblemTypes.ParameterAbsent">parameter_absent</see>.
        /// </para>
        /// 
        /// <para>
        /// The <see cref="OAuthRequestException.AdditionalParameter">additional parameter</see>
        /// (<see cref="ProblemReportingParameters.ParametersAbsent">oauth_parameters_absent</see>)
        /// of the exception indicates the set of parameter names that are absent,
        /// percent-encoded and separated by <c>&amp;</c>.
        /// </para>
        /// 
        /// <para>
        /// If an <see cref="Adviser"/> is set, and it returns non-<c>null</c>
        /// advise for this exception, this advice will be stored in the 
        /// <see cref="OAuthRequestException.Advice"/> property. Otherwise, 
        /// the <see cref="OAuthRequestException.Advice"/> will be <c>null</c>.
        /// </para>
        /// 
        /// <para>
        /// The <see cref="Exception.Source"/> will be <see cref="OAuthRequestExceptionSources.Local">local</see>.
        /// </para>
        /// </remarks>
        /// 
        /// <param name="parameters">The parameters that are absent</param>
        /// 
        /// <exception cref="System.ArgumentException">
        /// If <paramref name="parameters"/> is null or empty
        /// </exception>
        /// <exception cref="OAuth.Net.Common.OAuthRequestException">
        /// On success
        /// </exception>
        public void ThrowParametersAbsent(string[] parameters)
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

            throw this.PostApplyAdviser(() => new OAuthRequestException()
                {
                    Problem = ProblemReportingProblemTypes.ParameterAbsent,

                    AdditionalParameter = new KeyValuePair<string, string>(
                        ProblemReportingParameters.ParametersAbsent, absentParameters.ToString()),

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
        /// <see cref="ProblemReportingProblemTypes.ParameterRejected">parameter_rejected</see>.
        /// </para>
        /// 
        /// <para>
        /// The <see cref="OAuthRequestException.AdditionalParameter">additional parameter</see>
        /// (<see cref="ProblemReportingParameters.ParametersRejected">oauth_parameters_rejected</see>)
        /// of the exception consists of a set of parameters, encoded as they would be 
        /// in a URL query string. These are parameters that the sender recently 
        /// received but doesn't understand. Note that these parameters will be 
        /// percent-encoded twice: once to form a query string and again because 
        /// the query string is the value of <see cref="ProblemReportingParameters.ParametersRejected">oauth_parameters_rejected</see>.
        /// </para>
        /// 
        /// <para>
        /// If an <see cref="Adviser"/> is set, and it returns non-<c>null</c>
        /// advise for this exception, this advice will be stored in the 
        /// <see cref="OAuthRequestException.Advice"/> property. Otherwise, 
        /// the <see cref="OAuthRequestException.Advice"/> will be <c>null</c>.
        /// </para>
        /// 
        /// <para>
        /// The <see cref="Exception.Source"/> will be <see cref="OAuthRequestExceptionSources.Local">local</see>.
        /// </para>
        /// </remarks>
        /// 
        /// <param name="parameters">The parameters that are rejected</param>
        /// 
        /// <exception cref="System.ArgumentException">
        /// If <paramref name="parameters"/> is null or empty.
        /// </exception>
        /// <exception cref="OAuth.Net.Common.OAuthRequestException">
        /// On success
        /// </exception>
        public void ThrowParametersRejected(string[] parameters)
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

            throw this.PostApplyAdviser(() => new OAuthRequestException()
                {
                    Problem = ProblemReportingProblemTypes.ParameterRejected,

                    AdditionalParameter = new KeyValuePair<string, string>(
                        ProblemReportingParameters.ParametersRejected, rejectedParameters.ToString()),

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
        /// <see cref="ProblemReportingProblemTypes.TimestampRefused">timestamp_refused</see>.
        /// </para>
        /// 
        /// <para>
        /// The <see cref="OAuthRequestException.AdditionalParameter">additional parameter</see>
        /// (<see cref="ProblemReportingParameters.AcceptableTimestamps">oauth_acceptable_timestamps</see>)
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
        /// the <see cref="OAuthRequestException.Advice"/> will be <c>null</c>.
        /// </para>
        /// 
        /// <para>
        /// The <see cref="Exception.Source"/> will be <see cref="OAuthRequestExceptionSources.Local">local</see>.
        /// </para>
        /// </remarks>
        /// 
        /// <param name="minTimestamp">The minimum allowable timestamp</param>
        /// <param name="maxTimestamp">The maximum allowable timestamp</param>
        /// 
        /// <exception cref="System.ArgumentException">
        /// If <paramref name="minTimestamp"/> and/or <paramref name="maxTimestamp"/> 
        /// is less than <c>0</c>
        /// </exception>
        /// <exception cref="OAuth.Net.Common.OAuthRequestException">
        /// On success
        /// </exception>
        public void ThrowTimestampRefused(long minTimestamp, long maxTimestamp)
        {
            if (minTimestamp < 0)
                throw new ArgumentException("minTimestamp argument must be greater than or equal to 0", "minTimestamp");
            if (maxTimestamp < 0)
                throw new ArgumentException("maxTimestamp argument must be greater than or equal to 0", "maxTimestamp");

            throw this.PostApplyAdviser(() => new OAuthRequestException()
                {
                    Problem = ProblemReportingProblemTypes.TimestampRefused,

                    AdditionalParameter = new KeyValuePair<string, string>(
                        ProblemReportingParameters.AcceptableTimestamps,
                        string.Format(CultureInfo.InvariantCulture, "{0}-{1}", minTimestamp, maxTimestamp)),

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
        /// <see cref="ProblemReportingProblemTypes.NonceUsed">nonce_used</see>.
        /// </para>
        /// 
        /// <para>
        /// If an <see cref="Adviser"/> is set, and it returns non-<c>null</c>
        /// advise for this exception, this advice will be stored in the 
        /// <see cref="OAuthRequestException.Advice"/> property. Otherwise, 
        /// the <see cref="OAuthRequestException.Advice"/> will be <c>null</c>.
        /// </para>
        /// 
        /// <para>
        /// The <see cref="Exception.Source"/> will be <see cref="OAuthRequestExceptionSources.Local">local</see>.
        /// </para>
        /// </remarks>
        /// 
        /// <exception cref="OAuth.Net.Common.OAuthRequestException">
        /// Always
        /// </exception>
        public void ThrowNonceUsed()
        {
            throw this.PostApplyAdviser(() => BuildSimpleReport(
                ProblemReportingProblemTypes.NonceUsed));
        }

        /// <summary>
        /// Throws an exception indicating the signature method is invalid.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The <see cref="OAuthRequestException.Problem">problem type</see> is 
        /// <see cref="ProblemReportingProblemTypes.SignatureMethodRejected">signature_method_rejected</see>.
        /// </para>
        /// 
        /// <para>
        /// If an <see cref="Adviser"/> is set, and it returns non-<c>null</c>
        /// advise for this exception, this advice will be stored in the 
        /// <see cref="OAuthRequestException.Advice"/> property. Otherwise, 
        /// the <see cref="OAuthRequestException.Advice"/> will be <c>null</c>.
        /// </para>
        /// 
        /// <para>
        /// The <see cref="Exception.Source"/> will be <see cref="OAuthRequestExceptionSources.Local">local</see>.
        /// </para>
        /// </remarks>
        /// 
        /// <exception cref="OAuth.Net.Common.OAuthRequestException">
        /// Always
        /// </exception>
        public void ThrowSignatureMethodRejected()
        {
            throw this.PostApplyAdviser(() => BuildSimpleReport(
                ProblemReportingProblemTypes.SignatureMethodRejected));
        }

        /// <summary>
        /// Throws an exception indicating the consumer key is unknown to the
        /// service provider.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The <see cref="OAuthRequestException.Problem">problem type</see> is 
        /// <see cref="ProblemReportingProblemTypes.ConsumerKeyUnknown">consumer_key_unknown</see>.
        /// </para>
        /// 
        /// <para>
        /// If an <see cref="Adviser"/> is set, and it returns non-<c>null</c>
        /// advise for this exception, this advice will be stored in the 
        /// <see cref="OAuthRequestException.Advice"/> property. Otherwise, 
        /// the <see cref="OAuthRequestException.Advice"/> will be <c>null</c>.
        /// </para>
        /// 
        /// <para>
        /// The <see cref="Exception.Source"/> will be <see cref="OAuthRequestExceptionSources.Local">local</see>.
        /// </para>
        /// </remarks>
        /// 
        /// <exception cref="OAuth.Net.Common.OAuthRequestException">
        /// Always
        /// </exception>
        public void ThrowConsumerKeyUnknown()
        {
            throw this.PostApplyAdviser(() => BuildSimpleReport(
                ProblemReportingProblemTypes.ConsumerKeyUnknown));
        }

        /// <summary>
        /// Throws an exception indicating the consumer key is permanently unacceptable
        /// to the service provider. For example, the consumer may be blacklisted.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The <see cref="OAuthRequestException.Problem">problem type</see> is 
        /// <see cref="ProblemReportingProblemTypes.ConsumerKeyRejected">consumer_key_rejected</see>.
        /// </para>
        /// 
        /// <para>
        /// If an <see cref="Adviser"/> is set, and it returns non-<c>null</c>
        /// advise for this exception, this advice will be stored in the 
        /// <see cref="OAuthRequestException.Advice"/> property. Otherwise, 
        /// the <see cref="OAuthRequestException.Advice"/> will be <c>null</c>.
        /// </para>
        /// 
        /// <para>
        /// The <see cref="Exception.Source"/> will be <see cref="OAuthRequestExceptionSources.Local">local</see>.
        /// </para>
        /// </remarks>
        /// 
        /// <exception cref="OAuth.Net.Common.OAuthRequestException">
        /// Always
        /// </exception>
        public void ThrowConsumerKeyRejected()
        {
            throw this.PostApplyAdviser(() => BuildSimpleReport(
                ProblemReportingProblemTypes.ConsumerKeyRejected));
        }

        /// <summary>
        /// Throws an exception indicating the consumer key is temporarily unacceptable
        /// to the service provider. For example, the consumer may be being being throttled.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The <see cref="OAuthRequestException.Problem">problem type</see> is 
        /// <see cref="ProblemReportingProblemTypes.ConsumerKeyRefused">consumer_key_refused</see>.
        /// </para>
        /// 
        /// <para>
        /// If an <see cref="Adviser"/> is set, and it returns non-<c>null</c>
        /// advise for this exception, this advice will be stored in the 
        /// <see cref="OAuthRequestException.Advice"/> property. Otherwise, 
        /// the <see cref="OAuthRequestException.Advice"/> will be <c>null</c>.
        /// </para>
        /// 
        /// <para>
        /// The <see cref="Exception.Source"/> will be <see cref="OAuthRequestExceptionSources.Local">local</see>.
        /// </para>
        /// </remarks>
        /// 
        /// <exception cref="OAuth.Net.Common.OAuthRequestException">
        /// Always
        /// </exception>
        public void ThrowConsumerKeyRefused()
        {
            throw this.PostApplyAdviser(() => BuildSimpleReport(
                ProblemReportingProblemTypes.ConsumerKeyRefused));
        }

        /// <summary>
        /// Throws an exception indicating the supplied signature is invalid. That is, 
        /// it doesn't match the signature computed by the Service Provider. 
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The <see cref="OAuthRequestException.Problem">problem type</see> is 
        /// <see cref="ProblemReportingProblemTypes.SignatureInvalid">signature_invalid</see>.
        /// </para>
        /// 
        /// <para>
        /// If an <see cref="Adviser"/> is set, and it returns non-<c>null</c>
        /// advise for this exception, this advice will be stored in the 
        /// <see cref="OAuthRequestException.Advice"/> property. Otherwise, 
        /// the <see cref="OAuthRequestException.Advice"/> will be <c>null</c>.
        /// </para>
        /// 
        /// <para>
        /// The <see cref="Exception.Source"/> will be <see cref="OAuthRequestExceptionSources.Local">local</see>.
        /// </para>
        /// </remarks>
        /// 
        /// <exception cref="OAuth.Net.Common.OAuthRequestException">
        /// Always
        /// </exception>
        public void ThrowSignatureInvalid()
        {
            throw this.PostApplyAdviser(() => BuildSimpleReport(
                ProblemReportingProblemTypes.SignatureInvalid));
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
        /// <see cref="ProblemReportingProblemTypes.TokenRejected">token_rejected</see>.
        /// </para>
        /// 
        /// <para>
        /// If an <see cref="Adviser"/> is set, and it returns non-<c>null</c>
        /// advise for this exception, this advice will be stored in the 
        /// <see cref="OAuthRequestException.Advice"/> property. Otherwise, 
        /// the <see cref="OAuthRequestException.Advice"/> will be <c>null</c>.
        /// </para>
        /// 
        /// <para>
        /// The <see cref="Exception.Source"/> will be <see cref="OAuthRequestExceptionSources.Local">local</see>.
        /// </para>
        /// </remarks>
        /// 
        /// <exception cref="OAuth.Net.Common.OAuthRequestException">
        /// Always
        /// </exception>
        public void ThrowTokenRejected()
        {
            throw this.PostApplyAdviser(() => BuildSimpleReport(
                ProblemReportingProblemTypes.TokenRejected));
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
        /// <see cref="ProblemReportingProblemTypes.TokenUsed">token_used</see>.
        /// </para>
        /// 
        /// <para>
        /// If an <see cref="Adviser"/> is set, and it returns non-<c>null</c>
        /// advise for this exception, this advice will be stored in the 
        /// <see cref="OAuthRequestException.Advice"/> property. Otherwise, 
        /// the <see cref="OAuthRequestException.Advice"/> will be <c>null</c>.
        /// </para>
        /// 
        /// <para>
        /// The <see cref="Exception.Source"/> will be <see cref="OAuthRequestExceptionSources.Local">local</see>.
        /// </para>
        /// </remarks>
        /// 
        /// <exception cref="OAuth.Net.Common.OAuthRequestException">
        /// Always
        /// </exception>
        public void ThrowTokenUsed()
        {
            throw this.PostApplyAdviser(() => BuildSimpleReport(
                ProblemReportingProblemTypes.TokenUsed));
        }

        /// <summary>
        /// Throws an exception indicating the supplied token has expired. That 
        /// is, it was issued too long ago to be used now. 
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The <see cref="OAuthRequestException.Problem">problem type</see> is 
        /// <see cref="ProblemReportingProblemTypes.TokenExpired">token_expired</see>.
        /// </para>
        /// 
        /// <para>
        /// If an <see cref="Adviser"/> is set, and it returns non-<c>null</c>
        /// advise for this exception, this advice will be stored in the 
        /// <see cref="OAuthRequestException.Advice"/> property. Otherwise, 
        /// the <see cref="OAuthRequestException.Advice"/> will be <c>null</c>.
        /// </para>
        /// 
        /// <para>
        /// The <see cref="Exception.Source"/> will be <see cref="OAuthRequestExceptionSources.Local">local</see>.
        /// </para>
        /// </remarks>
        /// 
        /// <exception cref="OAuth.Net.Common.OAuthRequestException">
        /// Always
        /// </exception>
        public void ThrowTokenExpired()
        {
            throw this.PostApplyAdviser(() => BuildSimpleReport(
                ProblemReportingProblemTypes.TokenExpired));
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
        /// <see cref="ProblemReportingProblemTypes.TokenRevoked">token_revoked</see>.
        /// </para>
        /// 
        /// <para>v
        /// </para>
        /// 
        /// <para>
        /// The <see cref="Exception.Source"/> will be <see cref="OAuthRequestExceptionSources.Local">local</see>.
        /// </para>
        /// </remarks>
        /// 
        /// <exception cref="OAuth.Net.Common.OAuthRequestException">
        /// Always
        /// </exception>
        public void ThrowTokenRevoked()
        {
            throw this.PostApplyAdviser(() => BuildSimpleReport(
                ProblemReportingProblemTypes.TokenRevoked));
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

            if (!string.IsNullOrEmpty(parameters.AdditionalParameters[ProblemReportingParameters.Problem]))
            {
                OAuthRequestException ex = new OAuthRequestException()
                {
                    Problem = parameters.AdditionalParameters[ProblemReportingParameters.Problem],
                    Advice = parameters.AdditionalParameters[ProblemReportingParameters.ProblemAdvice],
                    Source = OAuthRequestExceptionSources.Remote
                };

                // Load additional parameter for specific types
                switch (parameters.AdditionalParameters[ProblemReportingParameters.Problem])
                {
                    case ProblemReportingProblemTypes.VersionRejected:
                        if (!string.IsNullOrEmpty(parameters.AdditionalParameters[ProblemReportingParameters.AcceptableVersions]))
                            ex.AdditionalParameter = new KeyValuePair<string, string>(
                                ProblemReportingParameters.AcceptableVersions,
                                parameters.AdditionalParameters[ProblemReportingParameters.AcceptableVersions]);

                        break;

                    case ProblemReportingProblemTypes.ParameterAbsent:
                        if (!string.IsNullOrEmpty(parameters.AdditionalParameters[ProblemReportingParameters.ParametersAbsent]))
                            ex.AdditionalParameter = new KeyValuePair<string, string>(
                                ProblemReportingParameters.ParametersAbsent,
                                parameters.AdditionalParameters[ProblemReportingParameters.ParametersAbsent]);

                        break;

                    case ProblemReportingProblemTypes.ParameterRejected:
                        if (!string.IsNullOrEmpty(parameters.AdditionalParameters[ProblemReportingParameters.ParametersRejected]))
                            ex.AdditionalParameter = new KeyValuePair<string, string>(
                                ProblemReportingParameters.ParametersRejected,
                                parameters.AdditionalParameters[ProblemReportingParameters.ParametersRejected]);

                        break;

                    case ProblemReportingProblemTypes.TimestampRefused:
                        if (!string.IsNullOrEmpty(parameters.AdditionalParameters[ProblemReportingParameters.AcceptableTimestamps]))
                            ex.AdditionalParameter = new KeyValuePair<string, string>(
                                ProblemReportingParameters.AcceptableTimestamps,
                                parameters.AdditionalParameters[ProblemReportingParameters.AcceptableTimestamps]);

                        break;
                }

                // Throw the OAuthRequestException
                throw this.PostApplyAdviser(() => ex);
            }
        }

        // Builds OAuthRequestExceptions without additional parameters
        private static OAuthRequestException BuildSimpleReport(string problem)
        {
            return new OAuthRequestException()
            {
                Problem = problem,
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

            ex.Advice = this.Adviser.AdviseUpon(ex);

            return ex;
        }
    }
}
