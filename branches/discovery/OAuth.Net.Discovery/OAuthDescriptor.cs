using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OAuth.Net.Common;
using OAuth.Net.Components;
using OAuth.Net.Consumer;
using Microsoft.Practices.ServiceLocation;
using XrdsSimple.Net;


namespace OAuth.Net.Discovery
{
    public class OAuthDescriptor
    {
        Uri resourceUri;

        XRDElement oAuthDescriptorXRD;

        private OAuthDescriptor(Uri resourceUri, XRDElement oAuthDescriptorXRD)
        {
            this.resourceUri = resourceUri;
            this.oAuthDescriptorXRD = oAuthDescriptorXRD;
        }

        public OAuthEndpoint RequestTokenEndPoint
        {
            get
            {
                return GetEndPoint(Constants.OAuthEndPointRequestServiceTypeURI, "POST");
            }
        }

        public OAuthEndpoint AuthorizationTokenEndPoint
        {
            get
            {
                return GetEndPoint(Constants.OAuthEndPointAuthorizeServiceTypeURI, "GET");
            }
        }

        public OAuthEndpoint AccessTokenEndPoint
        {
            get
            {
                return GetEndPoint(Constants.OAuthEndPointAccessServiceTypeURI, "POST");
            }
        }

        public OAuthEndpoint ResourceEndPoint
        {
            get
            {
                return GetEndPoint(Constants.OAuthEndPointResourceServiceTypeURI, "GET");
            }
        }

        public IConsumer Consumer
        {
            get
            {               
                IEnumerator<ServiceElement> services = oAuthDescriptorXRD.FindServices(Constants.OAuthConsumerIdentityStaticServiceTypeURI).GetEnumerator();

                if (services.MoveNext())
                {
                    //A Static Consumer only has an ID.
                    IEnumerator<LocalIDElement> localID = services.Current.GetLocalIDs().GetEnumerator();
                    if (localID.MoveNext())
                    {
                       OAuthConsumer consumer = new OAuth.Net.Components.OAuthConsumer(localID.Current.Text, String.Empty);
                       return consumer;
                    }
                    else
                        throw new Exception("Invalid static consumer declaration in OAuth Descriptor");
                }     
           
                //No static provider found so we need to find the URL where a manual one can be created from

                services = oAuthDescriptorXRD.FindServices(Constants.OAuthConsumerIdentityOutOfBandServiceTypeURI).GetEnumerator();

                if (services.MoveNext())
                {
                    IEnumerator<URIElement> uriElements = services.Current.GetURIs().GetEnumerator();
                    if (uriElements.MoveNext())
                    {
                        IConsumerStore store = ServiceLocator.Current.GetInstance<IConsumerStore>();

                        throw new Exception("Need to through Out Of Band Exception with URI to say where to go and get it");
                    }
                    else
                        throw new Exception("Invalid out of band consumer declaration in OAuth Descriptor.  No URI declared");
                }
                else
                    throw new Exception("Could not find consumer service declaration in OAuth Descriptor");
            }
        }

        public OAuthEndpoint GetEndPoint(string endpointType, string defaultHttpMethod)
        {
            IEnumerator<ServiceElement> services = oAuthDescriptorXRD.FindServices(endpointType).GetEnumerator();

            if (services.MoveNext())
            {
                ServiceElement service = services.Current;

                OAuthParameterSources acceptedParameters = OAuthParameterSources.None;

                //Work out what request parameter methods are supported.
                foreach (string type in service.Types)
                {
                    switch (type)
                    {
                        case Constants.OAuthRequestParameterAuthHeaderServiceTypeURI:
                            acceptedParameters = acceptedParameters | OAuthParameterSources.HttpAuthorizationHeader;
                            break;
                        case Constants.OAuthRequestParameterPostBodyServiceTypeURI:
                            acceptedParameters = acceptedParameters | OAuthParameterSources.HttpPostBody;
                            break;
                        case Constants.OAuthRequestParameterURIQueryServiceTypeURI:
                            acceptedParameters = acceptedParameters | OAuthParameterSources.HttpQueryString;
                            break;
                    }
                }

                string signatureMethod = String.Empty;

                //Work out what signature should be used
                foreach (string type in service.Types)
                {
                    if (type.StartsWith(Constants.OAuthSignatureServiceTypeURI))
                    {
                        signatureMethod = type.Substring(Constants.OAuthSignatureServiceTypeURI.Length - 1);
                        break;
                    }
                }


                Uri url = this.resourceUri;
                string httpMethod = defaultHttpMethod;

                //Get the first URL for this endpoint - again we should cache these so we can loop round them

                IEnumerator<URIElement> urls = service.GetURIs().GetEnumerator();
                if (urls.MoveNext())
                {
                    url = new Uri(urls.Current.Text);
                    if (!String.IsNullOrEmpty(urls.Current.HttpMethod))
                        httpMethod = urls.Current.HttpMethod;
                }

                return new OAuthEndpoint(url, httpMethod, signatureMethod, acceptedParameters);
            }
            else
                throw new OAuthDiscoveryException(String.Format("{0} did not contain valid OAuth Descriptor service", resourceUri.AbsoluteUri ));
        }

        /// <summary>
        /// Performs the Configuration Retrieval to return the OAuth Descriptor
        /// from the XRDS Simple document returned for the resource URI.
        /// </summary>
        /// <param name="resourceUri"></param>
        /// <returns></returns>
        public static OAuthDescriptor LocateOAuthDescriptor(Uri resourceUri)
        {
            XRDElement xrd = LocateOAuthDescriptorXRD(resourceUri);

            return new OAuthDescriptor(resourceUri, xrd);
        }

        private static XRDElement LocateOAuthDescriptorXRD(Uri xrdsLocationUri)
        {
            XRDSDocument xrdsDocument = XRDSDocument.LoadDocument(xrdsLocationUri);

            if (String.IsNullOrEmpty(xrdsLocationUri.Fragment))
            {
                //The URI doesn't have a fragment so we need to find the OAuth Discovery Service
                //type to identify where the Descriptor can be found.
                XRDElement defaultXRD = xrdsDocument.FindXRD();

                IEnumerator<ServiceElement> foundServices = defaultXRD.FindServices(Constants.OAuthDiscoveryServiceTypeURI).GetEnumerator();

                if (foundServices.MoveNext())
                {
                    ServiceElement oauthService = foundServices.Current;

                    XRDElement oAuthDescriptor = null;

                    //The service element should only contain one.  If it doesn't we try all until 
                    //we get an XRDElement.
                    foreach (URIElement element in oauthService.GetURIs())
                    {
                        //If the Uri is a fragment we look in the existing document.
                        if (element.Text.StartsWith("#"))
                        {
                            oAuthDescriptor = xrdsDocument.FindXRD(element.Text.Substring(1));
                        }
                        else
                        {
                            //If the configuration is located else where - I'm assuming that we have to start the process again.
                            //It would be better if I could add this as some kind of redirect for the current XRDS document
                            oAuthDescriptor = LocateOAuthDescriptorXRD(new Uri(element.Text));
                        }

                        if (oAuthDescriptor != null)
                            break;
                    }

                    return oAuthDescriptor;
                }
                else
                    throw new Exception("No valid OAuth Descriptor could be found");
            }
            else
                return xrdsDocument.FindXRD(); //The xrdsLocation already has a fragment which must be the OAuth descriptor.            
        }
    }
}
