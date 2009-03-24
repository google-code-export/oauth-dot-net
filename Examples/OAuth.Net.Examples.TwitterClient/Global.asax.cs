using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Web;
using Castle.Core;
using Castle.Windsor;
using CommonServiceLocator.WindsorAdapter;
using Microsoft.Practices.ServiceLocation;
using OAuth.Net.Common;
using OAuth.Net.Components;

namespace OAuth.Net.Examples.TwitterClient
{
    public class Global : HttpApplication
    {
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers",
            Justification = "Required to public by ASP.NET")]
        protected void Application_Start(object sender, EventArgs e)
        {
            IWindsorContainer container = new WindsorContainer();

            // Register components in code
            // For XML component registration, see Fire Eagle consumer example
            container.AddComponentWithLifestyle<ISigningProvider, HmacSha1SigningProvider>(
                "signing.provider:HMAC-SHA1",
                LifestyleType.Thread);
            container.AddComponent<INonceProvider, GuidNonceProvider>();

            WindsorServiceLocator injector = new WindsorServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => injector);

            // There is a bug(?) with Twitter's servers at the moment where 
            // POSTing with an Expect header will cause a 417 Expectation Failed

            // The below prevents .NET from sending Expect headers for all POST
            // requests in this AppDomain
            // (see: http://groups.google.com/group/twitter-development-talk/browse_thread/thread/7c67ff1a2407dee7)
            ServicePointManager.Expect100Continue = false;
        }
    }
}