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

namespace OAuth.Net.Common
{
    /// <summary>
    /// The parameters used by the <a href="http://wiki.oauth.net/ProblemReporting">
    /// Problem Reporting extension</a>. These are used by 
    /// <see cref="OAuthRequestException">OAuthRequestException</see>.
    /// </summary>
    public static class OAuthRequestExceptionParameters
    {
        /// <summary>
        /// <c>oauth_problem</c>:
        /// Parameter for the problem type.
        /// </summary>
        /// 
        /// <remarks>
        /// The value must be one of the predefined problem types, although a 
        /// Consumer SHOULD be prepared to receive other values, from a Service 
        /// Provider that implements a future version of problem reporting.
        /// </remarks>
        /// 
        /// <value>oauth_problem</value>
        public const string Problem = "oauth_problem";

        /// <summary>
        /// <c>oauth_acceptable_versions</c>:
        /// Parameter for the range of acceptable versions. 
        /// </summary>
        /// 
        /// <remarks>
        /// Applicable to the <see cref="OAuthRequestExceptionProblemTypes.VersionRejected">version_rejected</see> 
        /// problem type.
        /// </remarks>
        /// 
        /// <value>oauth_acceptable_versions</value>
        public const string AcceptableVersions = "oauth_acceptable_versions";

        /// <summary>
        /// <c>oauth_acceptable_timestamps</c>:
        /// Parameter for the range of acceptable timestamps.
        /// </summary>
        /// 
        /// <remarks>
        /// Applicable to the <see cref="OAuthRequestExceptionProblemTypes.TimestampRefused">timestamp_refused</see>
        /// problem type.
        /// </remarks>
        /// 
        /// <value>oauth_acceptable_timestamps</value>
        public const string AcceptableTimestamps = "oauth_acceptable_timestamps";

        /// <summary>
        /// <c>oauth_parameters_absent</c>:
        /// Parameter for the OAuth parameters that were absent from the request.
        /// </summary>
        /// 
        /// <remarks>
        /// Applicable to the <see cref="OAuthRequestExceptionProblemTypes.ParameterAbsent">parameter_absent</see>
        /// problem type.
        /// </remarks>
        /// 
        /// <value>oauth_parameters_absent</value>
        public const string ParametersAbsent = "oauth_parameters_absent";

        /// <summary>
        /// <c>oauth_parameters_rejected</c>:
        /// Parameter for the OAuth parameters whose values were rejected.
        /// </summary>
        /// 
        /// <remarks>
        /// Applicable to the <see cref="OAuthRequestExceptionProblemTypes.ParameterRejected">parameter_rejected</see>
        /// problem type.
        /// </remarks>
        /// 
        /// <value>oauth_parameters_rejected</value>
        public const string ParametersRejected = "oauth_parameters_rejected";

        /// <summary>
        /// <c>oauth_problem_advice</c>:
        /// Parameter for advice about the problem to be displayed to the user.
        /// </summary>
        /// 
        /// <remarks>
        /// It SHOULD describe the problem in a way that's meaningful to the 
        /// User, and suggest how to resolve the problem. The Consumer MUST NOT 
        /// interpret it as HTML or any other markup language; all the characters 
        /// MUST be displayed to the user. To indicate a line break, the single 
        /// character LF (Unicode U+000A) MUST be used.
        /// </remarks>
        /// 
        /// <value>oauth_problem_advice</value>
        public const string ProblemAdvice = "oauth_problem_advice";
    }
}
