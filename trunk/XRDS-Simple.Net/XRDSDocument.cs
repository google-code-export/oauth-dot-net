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
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace XrdsSimple.Net
{
    /// <summary>
    /// Representation of an XRDSSimple Document that is used to discover services
    /// provided by service providers.
    /// </summary>
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

        /// <summary>
        /// All the XRD Elements in the XRDSDocument whether valid elements
        /// or not.
        /// </summary>
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

        /// <summary>
        /// For a given URI attempts to download the XRDS document, following
        /// andy XRDS-Location redirects.
        /// </summary>
        /// <param name="endPointUri"></param>
        /// <returns></returns>
        public static XRDSDocument LoadDocument(Uri endPointUri)
        {
            return XRDSDocument.LoadDocument(endPointUri, new List<Uri>());
        }

        /// <summary>
        /// Private helper method that performs the XRDS download.  Keeps 
        /// a list of the route followed to avoid an endless loop.
        /// </summary>
        /// <param name="endPointUri"></param>
        /// <param name="routeStack"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Removes any fragement from a URI.
        /// </summary>
        /// <param name="endPointUri"></param>
        /// <returns></returns>
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
            return XRDSDocument.Parse(stream, true);
        }

        /// <summary>
        /// Deserializes an XML stream into an instance of an XRDSDocument.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static XRDSDocument Parse(System.IO.Stream stream, bool checkNameSpace)
        {
            XRDSDocument xrdsDocument = new XRDSDocument();

            XmlDocument document = new XmlDocument();
            document.Load(stream);
            XmlNode baseNode = document.DocumentElement;

            if (baseNode.LocalName.ToLower() == "xrds")
            {
                foreach (XmlNode node in baseNode.ChildNodes)
                    if (node.LocalName.ToLower() == "xrd")
                        if ((checkNameSpace && node.NamespaceURI.ToLower() == Constants.XRD_Namespace.ToLower()) || (!checkNameSpace))
                            xrdsDocument.xrdElements.Add(XRDElement.Parse(node, checkNameSpace));

            }

            return xrdsDocument;

            //// Old method did not work with some valid XML structures
            // XmlSerializer serializer = new XmlSerializer(typeof(XRDSDocument));
            // return (XRDSDocument)serializer.Deserialize(stream);
        }

        /// <summary>
        /// Serializes an instance of an XRDSDocument into a XML string
        /// </summary>
        /// <param name="xrdsDocument"></param>
        /// <returns></returns>
        public static string ToXmlString(XRDSDocument xrdsDocument)
        {

            String XmlizedString = null;
            MemoryStream memoryStream = new MemoryStream();
            XmlSerializer xs = new XmlSerializer(typeof(XRDSDocument));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
            xmlTextWriter.Formatting = Formatting.Indented;
            xmlTextWriter.IndentChar = ' ';
            xmlTextWriter.Indentation = 4;

            xs.Serialize(xmlTextWriter, xrdsDocument);
            memoryStream = (MemoryStream)xmlTextWriter.BaseStream;

            UTF8Encoding encoding = new UTF8Encoding();
            XmlizedString = encoding.GetString(memoryStream.ToArray());

            // XML corrections
            XmlizedString = XmlizedString.Replace(" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "");
            XmlizedString = XmlizedString.Replace("        <Expires xsi:nil=\"true\" />\r\n", "");
            XmlizedString = XmlizedString.Replace(" xmlns=\"xri://$XRD*($v*2.0)\"", " xmlns=\"xri://$XRD*($v*2.0)\" xmlns:simple=\"http://xrds-simple.net/core/1.0\"");

            return XmlizedString;

        }

        #endregion
    }
}
