namespace OAuth.Net.Common
{
    /// <summary>
    /// Provides user-friendly advice for errors covered by the <see 
    /// href="http://wiki.oauth.net/ProblemReporting">Problem Reporting 
    /// extension</see>.
    /// </summary>
    public interface IProblemReportingAdviser
    {
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
        string AdviseUpon(OAuthRequestException exception);
    }
}
