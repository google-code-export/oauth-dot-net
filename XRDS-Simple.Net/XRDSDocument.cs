using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace XRDS_Simple.Net
{
    //The XRDSDocument should hold the location which should be used to find the XRDElement
    //if it has a fragement!

    [XmlType(Namespace = Constants.XRDS_Namespace)]
    [XmlRoot(ElementName = "XRDS", Namespace = Constants.XRDS_Namespace)]
    public class XRDSDocument
    {
        #region XRDSdocument Instance Properties
        List<XRDElement> xrdElements = new List<XRDElement>();

        /// <summary>
        /// Holds the URI used of the document location as this may hold a fragment
        /// which will identify what XRD element to return.
        /// </summary>
        private Uri DocumentLocation
        {
            get;
            set;
        }

        [XmlElementAttribute(ElementName = "XRD", Namespace = Constants.XRD_Namespace)]
        public XRDElement[] AllXRDElements
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

        /// <summary>
        /// Returns a list of all those XRDElements that have 
        /// not expired.
        /// </summary>
        public List<XRDElement> ValidXRDElements
        {
            get
            {  
                return xrdElements.FindAll(element => !element.Expires.HasValue || element.Expires.Value > DateTime.Now);
            }
        }

        #endregion

        #region XRDElement Find Methods

        /// <summary>
        /// Finds the default XRDSimple Element for this document by using the Fragment of the document location to 
        /// identify the XRD by it's ID attribute; or returning the last XRD element in the filtered collection.        
        /// </summary>
        /// <returns></returns>
        public XRDElement FindXRD()
        {
            return FindXRD(true);
        }

        /// <summary>
        /// Finds the default XRD Element for this document by using the Fragment of the document location to 
        /// identify the XRD by it's ID attribute; or returning the last XRD element in the collection.        
        /// </summary>
        /// <returns></returns>
        public XRDElement FindXRD(bool requireXRDSSimpleType)
        {
            XRDElement foundElement = null;
            //7.1 XRD Identification
            //From all the non-Expired XRD Elements
            //If the document location includes a URI fragment then we look to see if there is a matching CRD identifier.
            //If none found then DiscoveryException thrown.
            if (DocumentLocation != null && !String.IsNullOrEmpty(DocumentLocation.Fragment))
            {
                foundElement = FindXRD(DocumentLocation.Fragment, requireXRDSSimpleType);
            }
            else
            {
                //If No ID element found then return the last element in the document (have to assume that the deserializaiton puts
                //them in the correct order.
                List<XRDElement> filteredElements = ValidXRDElements;

                if (requireXRDSSimpleType)
                {
                    filteredElements = FilterXRDSimpleTypes(filteredElements);
                }

                if (filteredElements.Count > 0)
                {                    
                    foundElement = filteredElements[filteredElements.Count - 1];
                }                                    
            }
            
            return foundElement;
        }

        /// <summary>
        /// Searches the valid XRDSimple Elements to find one with a specific XML ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public XRDElement FindXRD(string id)
        {
            return FindXRD(id, true);
        }

        /// <summary>
        /// Searches the valid XRD Elements to find one with a specific XML ID, allows an additional filter
        /// to specify if this element must be of an XRDS Simple Type.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public XRDElement FindXRD(string id, bool requireXRDSSimpleType)
        {
            List<XRDElement> filteredElements = ValidXRDElements;
            
            if (requireXRDSSimpleType)
            {
                filteredElements = FilterXRDSimpleTypes(filteredElements);
            }

            return filteredElements.Find(delegate(XRDElement element)
            {
                return element.ID == id;
            });
        }

        /// <summary>
        /// Filters the list of XRDElements for those which match the XRDSimple XRI
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        private List<XRDElement> FilterXRDSimpleTypes(List<XRDElement> elements)
        {
            return elements.FindAll(delegate(XRDElement element)
                    {
                        return Array.Exists<string>(element.Types, delegate(string type)
                        {
                            return type == Constants.XRDSimple_XRI;
                        });
                    });
        }

        #endregion

        #region XRDSDocument Static Method
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
            Uri cleanEndPointUri = XRDSDocument.CleanUriFragment(endPointUri);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(cleanEndPointUri);
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
                    xrdsDocument.DocumentLocation = endPointUri;

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
                Uri cleanNewLocationUri = XRDSDocument.CleanUriFragment(newLocationUri);
                if ( cleanNewLocationUri != cleanEndPointUri
                    && !routeStack.Contains(cleanNewLocationUri))
                {
                    //The old URI is retained in a list of URI's visited to ensure
                    //that we don't go back to somewhere we have already been.
                    routeStack.Add(cleanEndPointUri);

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

        #endregion
    }
}
