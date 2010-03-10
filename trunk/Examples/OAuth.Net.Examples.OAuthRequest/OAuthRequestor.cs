using System;
using System.Collections.Generic;
using System.Text;

using OAuth.Net.Consumer;

using Castle.Core.Resource;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using CommonServiceLocator.WindsorAdapter;
using Microsoft.Practices.ServiceLocation;

namespace OAuth.Net.Examples.OAuthRequest
{
    class OAuthRequestor
    {
        private static readonly string HELP = "/?";
        private static readonly Uri NullUri = new Uri("http://empty");

        public static void Main(string[] args)
        {
            if (args.Length == 0
                || args.Length != 3
                || HELP.Equals(args[0]))
            {
                RenderHelp();
                return;
            }

            Uri protectedResource = null;

            if(!Uri.TryCreate( args[0], UriKind.Absolute, out protectedResource))
            {
                RenderHelp();
                return;
            }

            string consumerKey = args[1];
            string consumerSecret = args[2];

            SetupServiceLocator();

            DoConsumerRequest(protectedResource, consumerKey, consumerSecret);
        }

        private static void SetupServiceLocator()
        {
            IServiceLocator injector =
                new WindsorServiceLocator(
                    new WindsorContainer(
                        new XmlInterpreter(
                            new ConfigResource("oauth.net.components"))));

            ServiceLocator.SetLocatorProvider(() => injector);
        }

        private static void DoConsumerRequest(Uri protectedResource, string consumerKey, string consumerSecret)
        {

            var request = OAuthConsumerRequest.Create(
                new EndPoint(protectedResource),
                OAuthService.Create(
                    new EndPoint(),
                    NullUri,
                    new EndPoint(),
                    new OAuth.Net.Components.OAuthConsumer(consumerKey, consumerSecret)));


            OAuthResponse response = request.GetResource();
            if (!response.HasProtectedResource)
                Console.Write("OAuth request did not respond with a resource");
            else
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(response.ProtectedResource.GetResponseStream()))
                {
                    Console.Write(reader.ReadToEnd());
                }
            }
        }

        private static void RenderHelp()
        {
            string help = @"Performs an OAuth Request for a protected resource.

OAuthRequest [protected resource] [consumerkey] [consumersecret]
   [protected resource]    
        Specifies the URI to the OAuth Protected Resource to obtain   
   [consumerkey] 
        Specifies the consumer key to use with the request
   [consumersecret]
        Specifies the consumer secret to use with the request

Press any key to continue.....";

            Console.Write(help);
            Console.ReadKey(false);
        }
    }
}
