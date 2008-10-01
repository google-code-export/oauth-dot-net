using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace XRDS_Simple.Net
{
    [XmlType(Namespace = Constants.XRDS_Namespace)]
    [XmlRoot(ElementName = "XRDS", Namespace = Constants.XRDS_Namespace)]
    public class XRDSDocument
    {
        List<XRDElement> xrdElements = new List<XRDElement>();

        [XmlElementAttribute(ElementName = "XRD", Namespace = Constants.XRD_Namespace)]
        public XRDElement[] XRDElements
        {
            get
            {
                return xrdElements.ToArray();
            }

            set
            {
                xrdElements.Clear();
                if( value != null )
                    xrdElements.AddRange(value);
            }
        }


        public static XRDSDocument LoadDocument(Uri endPointUri)
        {
            return XRDSDocument.LoadDocument(endPointUri, new List<Uri>());
        }

        private static XRDSDocument LoadDocument(Uri endPointUri, List<Uri>routeStack)
        {
            //Under phase 1 we are going to ignore section 5.1.1 and only look at using the GET protocol
            //to load the document.

            //We issue a HTTP(S) GET Request to the Endpoint.  The request SHOULD include an Accept header 
            //specifying type application/xrds+xml

            //5.1 If the endpoint contains a fragment, it MUST be removed prior to querying for the document's location.           
            endPointUri = XRDSDocument.CleanUriFragment(endPointUri);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endPointUri);
            request.Method = "GET";
            request.Accept = Constants.ApplicationXRDSXML_ContentType;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();


            //The response Must be one of three options:
            // 1. A Valid XRDS document with the Content-Type header of application/xrds+xml.                       
            if (!String.IsNullOrEmpty(response.ContentType) && 
                response.ContentType.ToLower().Contains(Constants.ApplicationXRDSXML_ContentType))
            {                
                try
                {
                    XRDSDocument xrdsDocument = XRDSDocument.Parse(response.GetResponseStream());

                    return xrdsDocument;
                }
                catch (Exception e)
                {
                    throw new DiscoveryException("Error parsing XRDS-simple XML stream", e);
                }
            }

            
            Uri newLocationUri = endPointUri;            
            bool newLocationFound = false;

            // 2. A X-XRDS-Location header with a URI of the correct location.
            if (!String.IsNullOrEmpty( response.Headers[Constants.XRDSLocation_Header] ))
            {
                try
                {
                    newLocationUri = new Uri(response.Headers[Constants.XRDSLocation_Header]);
                    newLocationFound = true;
                }
                catch (Exception e)
                {
                    throw new DiscoveryException(String.Format("X-XRDS-Location {0} is not a valid URI", response.Headers[Constants.XRDSLocation_Header]), e);
                }
            }
            else
            {
                // 3. A HTML document with a meta element that has a http-equiv attribute of X-XRDS-Location.            
                HtmlAgilityPack.HtmlNodeNavigator htmlNavigator = new HtmlAgilityPack.HtmlNodeNavigator(response.GetResponseStream());

                XPathNavigator metaNode = htmlNavigator.SelectSingleNode(
                    @"//meta[translate(@http-equiv,'abcdefghijklmnopqrstuvwxyz','ABCDEFGHIJKLMNOPQRSTUVWXYZ')='X-XRDS-LOCATION']"
                    );

                if (metaNode != null)
                {
                    string metaContent = metaNode.GetAttribute("content", String.Empty);

                    if (!String.IsNullOrEmpty(metaContent))
                    {
                        try
                        {
                            newLocationUri = new Uri(metaContent);
                            newLocationFound = true;
                        }
                        catch (Exception e)
                        {
                            throw new DiscoveryException(String.Format("X-XRDS-Location {0} is not a valid URI", metaContent), e);
                        }
                    }                    
                }

                
            }

            if (newLocationFound)
            {
                if (XRDSDocument.CleanUriFragment(newLocationUri) != endPointUri 
                    && !routeStack.Contains(newLocationUri))
                {
                    //The old URI is retained in a list of URI's visited to ensure
                    //that we don't go back to somewhere we have already been.
                    routeStack.Add(endPointUri);

                    return XRDSDocument.LoadDocument(newLocationUri, routeStack);
                }
                else
                    throw new DiscoveryException("X-XRDS-Location found is same as end point");
            }
            
            
            throw new DiscoveryException( String.Format("Discovery failed for endpoint {0}.", endPointUri.AbsoluteUri));
        }




        private static Uri CleanUriFragment(Uri endPointUri)
        {
            Uri cleanedUri = endPointUri;

            if (!String.IsNullOrEmpty(cleanedUri.Fragment))
            {
                UriBuilder builder = new UriBuilder(cleanedUri);
                builder.Fragment = String.Empty;
                cleanedUri = builder.Uri;
            }

            return cleanedUri;
        }

        /// <summary>
        /// Deserializes an XML stream into an instance of an XRDSDocument.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static XRDSDocument Parse(System.IO.Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(XRDSDocument));

            return (XRDSDocument)serializer.Deserialize(stream);
        }
    }
}
