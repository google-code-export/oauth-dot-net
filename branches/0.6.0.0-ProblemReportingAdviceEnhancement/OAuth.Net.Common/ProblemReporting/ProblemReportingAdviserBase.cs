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

using System.Collections.Generic;

namespace OAuth.Net.Common
{
    /// <summary>
    /// A base class for implementations of 
    /// <see cref="IProblemReportingAdviser"/> which map different problem
    /// types to different adviser methods.
    /// </summary>
    /// <remarks>
    /// <para>
    /// By default, the known problem types are mapped to the AdviseUpon...
    /// methods of this base class, which return <c>null</c>. You should
    /// override the methods for problem types for which you want to provide
    /// advice.
    /// </para>
    /// 
    /// <para>
    /// If no delegate is mapped to a problem type, 
    /// <see cref="AdviseWhenNoMappingFound"/> is used. You can override this
    /// method too.
    /// </para>
    /// </remarks>
    public abstract class ProblemReportingAdviserBase : IProblemReportingAdviser
    {
        /// <summary>
        /// Provides the default mapping of AdviseUpon... methods to their
        /// respective problem types.
        /// </summary>
        protected ProblemReportingAdviserBase()
        {
            this.ProblemAdvisers = new Dictionary<string, ProblemReportingAdviserDelegate>
            {
                {
                    ProblemReportingProblemTypes.ConsumerKeyRefused,
                    this.AdviseUponConsumerKeyRefused
                },
                {
                    ProblemReportingProblemTypes.ConsumerKeyRejected,
                    this.AdviseUponConsumerKeyRejected
                },
                {
                    ProblemReportingProblemTypes.ConsumerKeyUnknown,
                    this.AdviseUponConsumerKeyUnknown
                },
                {
                    ProblemReportingProblemTypes.NonceUsed,
                    this.AdviseUponNonceUsed
                },
                {
                    ProblemReportingProblemTypes.ParameterAbsent,
                    this.AdviseUponParameterAbsent
                },
                {
                    ProblemReportingProblemTypes.ParameterRejected,
                    this.AdviseUponParameterRejected
                },
                {
                    ProblemReportingProblemTypes.PermissionDenied,
                    this.AdviseUponPermissionDenied
                },
                {
                    ProblemReportingProblemTypes.PermissionUnknown,
                    this.AdviseUponPermissionUnknown
                },
                {
                    ProblemReportingProblemTypes.SignatureInvalid,
                    this.AdviseUponSignatureInvalid
                },
                {
                    ProblemReportingProblemTypes.SignatureMethodRejected,
                    this.AdviseUponSignatureMethodRejected
                },
                {
                    ProblemReportingProblemTypes.TimestampRefused,
                    this.AdviseUponTimestampRefused
                },
                {
                    ProblemReportingProblemTypes.TokenExpired,
                    this.AdviseUponTokenExpired
                },
                {
                    ProblemReportingProblemTypes.TokenRejected,
                    this.AdviseUponTokenRejected
                },
                {
                    ProblemReportingProblemTypes.TokenRevoked,
                    this.AdviseUponTokenRevoked
                },
                {
                    ProblemReportingProblemTypes.TokenUsed,
                    this.AdviseUponTokenUsed
                },
                {
                    ProblemReportingProblemTypes.VersionRejected,
                    this.AdviseUponVersionRejected
                }
            };
        }

        /// <summary>
        /// Map of Problem Reporting problem types to adviser delegates.
        /// </summary>
        /// <remarks>
        /// By default, the AdviseUpon... methods of this base class
        /// are mapped to their respective problem types.
        /// </remarks>
        protected IDictionary<string, ProblemReportingAdviserDelegate> ProblemAdvisers { get; set; }

        /// <summary>
        /// Provides user-friendly advice for the Problem Reporting error
        /// described by the specified <see cref="OAuthRequestException"/>.
        /// This is transmitted to the consumer, which may display it verbatim
        /// to the end-user.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This implementation dispatches to the methods mapped in the 
        /// <see cref="ProblemAdvisers"/> dictionary, which by default 
        /// maps the AdviseUpon... methods to their respective problem types.
        /// If no mapping exists for a problem type, 
        /// <see cref="AdviseWhenNoMappingFound"/> is invoked.
        /// </para>
        /// 
        /// <para>
        /// The default implementations of the Advise... methods return
        /// <c>null</c> as the advice. You should override methods for whose 
        /// problem types you want to provide advice.
        /// </para>
        /// </remarks>
        /// <param name="exception">Exception describing a problem reporting
        /// error</param>
        /// <returns>A string containing user-friendly advise about this
        /// exception, or <c>null</c> if no advice is available.</returns>
        public virtual string AdviseUpon(OAuthRequestException exception)
        {
            if (exception == null)
                return null;

            ProblemReportingAdviserDelegate adviser 
                = this.ProblemAdvisers[exception.Problem];

            return (adviser ?? this.AdviseWhenNoMappingFound)(exception);
        }

        /// <summary>
        /// Provides advice when the problem type does not match
        /// a mapped adviser method.
        /// </summary>
        /// <param name="exception">Exception describing a problem reporting
        /// error</param>
        /// <returns>A string containing user-friendly advise about this
        /// exception, or <c>null</c> if no advice is available.</returns>
        protected virtual string AdviseWhenNoMappingFound(
            OAuthRequestException exception)
        {
            return null;
        }

        /// <summary>
        /// Provides advice when the consumer key has been refused.
        /// </summary>
        /// <param name="exception">Exception describing a problem reporting
        /// error</param>
        /// <returns>A string containing user-friendly advise about this
        /// exception, or <c>null</c> if no advice is available.</returns>
        protected virtual string AdviseUponConsumerKeyRefused(
            OAuthRequestException exception)
        {
            return null;
        }

        /// <summary>
        /// Provides advice when the consumer key has been rejected.
        /// </summary>
        /// <param name="exception">Exception describing a problem reporting
        /// error</param>
        /// <returns>A string containing user-friendly advise about this
        /// exception, or <c>null</c> if no advice is available.</returns>
        protected virtual string AdviseUponConsumerKeyRejected(
            OAuthRequestException exception)
        {
            return null;
        }

        /// <summary>
        /// Provides advice when the consumer key is unknown.
        /// </summary>
        /// <param name="exception">Exception describing a problem reporting
        /// error</param>
        /// <returns>A string containing user-friendly advise about this
        /// exception, or <c>null</c> if no advice is available.</returns>
        protected virtual string AdviseUponConsumerKeyUnknown(
            OAuthRequestException exception)
        {
            return null;
        }

        /// <summary>
        /// Provides advice when the nonce has been used.
        /// </summary>
        /// <param name="exception">Exception describing a problem reporting
        /// error</param>
        /// <returns>A string containing user-friendly advise about this
        /// exception, or <c>null</c> if no advice is available.</returns>
        protected virtual string AdviseUponNonceUsed(
            OAuthRequestException exception)
        {
            return null;
        }

        /// <summary>
        /// Provides advice when some parameter(s) are absent.
        /// </summary>
        /// <param name="exception">Exception describing a problem reporting
        /// error</param>
        /// <returns>A string containing user-friendly advise about this
        /// exception, or <c>null</c> if no advice is available.</returns>
        protected virtual string AdviseUponParameterAbsent(
            OAuthRequestException exception)
        {
            return null;
        }

        /// <summary>
        /// Provides advice when some parameter(s) have been rejected.
        /// </summary>
        /// <param name="exception">Exception describing a problem reporting
        /// error</param>
        /// <returns>A string containing user-friendly advise about this
        /// exception, or <c>null</c> if no advice is available.</returns>
        protected virtual string AdviseUponParameterRejected(
            OAuthRequestException exception)
        {
            return null;
        }

        /// <summary>
        /// Provides advice when permission is denied.
        /// </summary>
        /// <param name="exception">Exception describing a problem reporting
        /// error</param>
        /// <returns>A string containing user-friendly advise about this
        /// exception, or <c>null</c> if no advice is available.</returns>
        protected virtual string AdviseUponPermissionDenied(
            OAuthRequestException exception)
        {
            return null;
        }

        /// <summary>
        /// Provides advice when it is not known whether permission has been
        /// granted.
        /// </summary>
        /// <param name="exception">Exception describing a problem reporting
        /// error</param>
        /// <returns>A string containing user-friendly advise about this
        /// exception, or <c>null</c> if no advice is available.</returns>
        protected virtual string AdviseUponPermissionUnknown(
            OAuthRequestException exception)
        {
            return null;
        }

        /// <summary>
        /// Provides advice when the signature is invalid.
        /// </summary>
        /// <param name="exception">Exception describing a problem reporting
        /// error</param>
        /// <returns>A string containing user-friendly advise about this
        /// exception, or <c>null</c> if no advice is available.</returns>
        protected virtual string AdviseUponSignatureInvalid(
            OAuthRequestException exception)
        {
            return null;
        }

        /// <summary>
        /// Provides advice when the signature method is rejected.
        /// </summary>
        /// <param name="exception">Exception describing a problem reporting
        /// error</param>
        /// <returns>A string containing user-friendly advise about this
        /// exception, or <c>null</c> if no advice is available.</returns>
        protected virtual string AdviseUponSignatureMethodRejected(
            OAuthRequestException exception)
        {
            return null;
        }

        /// <summary>
        /// Provides advice when the timestamp is refused.
        /// </summary>
        /// <param name="exception">Exception describing a problem reporting
        /// error</param>
        /// <returns>A string containing user-friendly advise about this
        /// exception, or <c>null</c> if no advice is available.</returns>
        protected virtual string AdviseUponTimestampRefused(
            OAuthRequestException exception)
        {
            return null;
        }

        /// <summary>
        /// Provides advice when a token has expired.
        /// </summary>
        /// <param name="exception">Exception describing a problem reporting
        /// error</param>
        /// <returns>A string containing user-friendly advise about this
        /// exception, or <c>null</c> if no advice is available.</returns>
        protected virtual string AdviseUponTokenExpired(
            OAuthRequestException exception)
        {
            return null;
        }

        /// <summary>
        /// Provides advice when a token is rejected.
        /// </summary>
        /// <param name="exception">Exception describing a problem reporting
        /// error</param>
        /// <returns>A string containing user-friendly advise about this
        /// exception, or <c>null</c> if no advice is available.</returns>
        protected virtual string AdviseUponTokenRejected(
            OAuthRequestException exception)
        {
            return null;
        }

        /// <summary>
        /// Provides advice when a token has been revoked.
        /// </summary>
        /// <param name="exception">Exception describing a problem reporting
        /// error</param>
        /// <returns>A string containing user-friendly advise about this
        /// exception, or <c>null</c> if no advice is available.</returns>
        protected virtual string AdviseUponTokenRevoked(
            OAuthRequestException exception)
        {
            return null;
        }

        /// <summary>
        /// Provides advice when a token has already been used.
        /// </summary>
        /// <param name="exception">Exception describing a problem reporting
        /// error</param>
        /// <returns>A string containing user-friendly advise about this
        /// exception, or <c>null</c> if no advice is available.</returns>
        protected virtual string AdviseUponTokenUsed(
            OAuthRequestException exception)
        {
            return null;
        }

        /// <summary>
        /// Provides advice when the version is rejected.
        /// </summary>
        /// <param name="exception">Exception describing a problem reporting
        /// error</param>
        /// <returns>A string containing user-friendly advise about this
        /// exception, or <c>null</c> if no advice is available.</returns>
        protected virtual string AdviseUponVersionRejected(
            OAuthRequestException exception)
        {
            return null;
        }
    }
}
