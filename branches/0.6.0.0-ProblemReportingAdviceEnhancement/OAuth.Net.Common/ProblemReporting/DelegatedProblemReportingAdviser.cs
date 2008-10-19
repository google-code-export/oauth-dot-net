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

namespace OAuth.Net.Common
{
    /// <summary>
    /// A delegate which supplies user-friendly advice for the supplied
    /// exception.
    /// </summary>
    /// <param name="exception">Exception describing a problem reporting
    /// error</param>
    /// <returns>A string containing user-friendly advise about this
    /// exception, or <c>null</c> if no advice is available.</returns>
    public delegate string ProblemReportingAdviserDelegate(OAuthRequestException exception);

    /// <summary>
    /// An implementation of <see cref="IProblemReportingAdviser"/>
    /// which uses a delegate to provide user-friendly advice.
    /// </summary>
    public class DelegatedProblemReportingAdviser : IProblemReportingAdviser
    {
        /// <summary>
        /// Creates a DelegatedProblemReportingAdviser using the supplied
        /// delegate.
        /// </summary>
        /// <param name="adviser">Adviser delegate</param>
        public DelegatedProblemReportingAdviser(
            ProblemReportingAdviserDelegate adviser)
        {
            this.Adviser = adviser;
        }

        /// <summary>
        /// The delegate to supply advice
        /// </summary>
        public ProblemReportingAdviserDelegate Adviser { get; set; }

        /// <summary>
        /// Provides user-friendly advice for the Problem Reporting error
        /// described by the specified <see cref="OAuthRequestException"/>.
        /// This is transmitted to the consumer, which may display it verbatim
        /// to the end-user.
        /// </summary>
        /// <param name="exception">Exception describing a problem reporting
        /// error</param>
        /// <returns>A string containing user-friendly advise about this
        /// exception, or <c>null</c> if no advice is available.</returns>
        public string AdviseUpon(OAuthRequestException exception)
        {
            return this.Adviser != null
                ? this.Adviser(exception)
                : null;
        }
    }
}
