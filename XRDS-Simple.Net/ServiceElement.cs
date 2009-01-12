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
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace XrdsSimple.Net
{

    /// <summary>
    /// Representation of an XRDSSimple Service definition.
    /// </summary>
    [XmlType(Namespace = Constants.XRD_Namespace)]
    [XmlRoot(ElementName = "Service", Namespace = Constants.XRD_Namespace)]
    public class ServiceElement : IPriority
    {
        private List<string> typeElements = new List<string>();
        private List<string> mediaTypeElements = new List<string>();
        private List<URIElement> uriElements = new List<URIElement>();
        private List<URIElement> uriTemplateElements = new List<URIElement>();
        private List<LocalIDElement> localIDElements = new List<LocalIDElement>();
        private List<string> mustSupportElements = new List<string>();

        #region ServiceElement Properties

        /// <summary>
        /// The priority this service has over other services defined in the same XRD.
        /// </summary>
        [XmlAttribute(DataType = "nonNegativeInteger", AttributeName = "priority", Namespace = Constants.XRD_Namespace)]
        public string Priority
        {
            get;
            set;
        }

        /// <summary>
        /// Array of the type URI's this service element is for.
        /// </summary>
        [XmlElement(DataType = "anyURI", ElementName = "Type", Namespace = Constants.XRD_Namespace)]
        public string[] Types
        {
            get { return typeElements.ToArray(); }
            set
            {
                typeElements.Clear();
                if (value != null)
                    typeElements.AddRange(value);
            }
        }

        /// <summary>
        /// Array of the media types this service element is for.
        /// </summary>
        [XmlElement(DataType = "string", ElementName = "MediaType", Namespace = Constants.XRD_Namespace)]
        public string[] MediaTypes
        {
            get { return mediaTypeElements.ToArray(); }
            set
            {
                mediaTypeElements.Clear();
                if (value != null)
                    mediaTypeElements.AddRange(value);
            }
        }

        /// <summary>
        /// Array of the URI's this service can be obtained at.
        /// </summary>
        [XmlElement(ElementName = "URI", Namespace = Constants.XRD_Namespace)]
        public URIElement[] URIs
        {
            get { return uriElements.ToArray(); }
            set
            {
                uriElements.Clear();
                if (value != null)
                    uriElements.AddRange(value);
            }
        }

        /// <summary>
        /// Array of the URI-Template's this service can be obtained at.
        /// </summary>
        [XmlElement(ElementName = "URI-Template", Namespace = Constants.OpenSocial_Namespace)]
        public URIElement[] URITemplates
        {
            get { return uriTemplateElements.ToArray(); }
            set
            {
                uriTemplateElements.Clear();
                if (value != null)
                    uriTemplateElements.AddRange(value);
            }
        }

        /// <summary>
        /// Array of LocalDs that can be used with the service.
        /// </summary>
        [XmlElement(ElementName = "LocalID", Namespace = Constants.XRD_Namespace)]
        public LocalIDElement[] LocalIDs
        {
            get { return localIDElements.ToArray(); }
            set
            {
                localIDElements.Clear();
                if (value != null)
                    localIDElements.AddRange(value);
            }
        }

        /// <summary>
        /// Array of URI's that identify protocols the consumer must support
        /// to use this service.
        /// </summary>
        [XmlElement(DataType = "anyURI", ElementName = "MustSupport", Namespace = Constants.XRDSimple_Namespace)]
        public string[] MustSupport
        {
            get { return mustSupportElements.ToArray(); }
            set
            {
                mustSupportElements.Clear();
                if (value != null)
                    mustSupportElements.AddRange(value);
            }
        }

        #endregion

        #region XRDSSimple Helper Methods

        /// <summary>
        /// Returns a sorted by priority enumeration of the LocalID elements
        /// </summary>
        /// <returns></returns>
        public IEnumerable<LocalIDElement> GetLocalIDs()
        {
            if (localIDElements.Count > 1)
            {
                List<IPriority> sortedList = localIDElements.ConvertAll<IPriority>(element => (IPriority)element);

                PriorityComparer comparer = new PriorityComparer();
                sortedList.Sort(comparer);

                //Expose this only as an IEnumerable.
                return sortedList.ConvertAll<LocalIDElement>(element => (LocalIDElement)element);
            }
            else
                return localIDElements;
        }


        /// <summary>
        /// Returns a sorted list of URI's by priority enumeration of the URi elements
        /// </summary>
        /// <returns></returns>
        public IEnumerable<URIElement> GetURIs()
        {
            if (uriElements.Count > 1)
            {
                List<IPriority> sortedList = uriElements.ConvertAll<IPriority>(element => (IPriority)element);

                PriorityComparer comparer = new PriorityComparer();
                sortedList.Sort(comparer);

                //Expose this only as an IEnumerable.
                return sortedList.ConvertAll<URIElement>(element => (URIElement)element); ;
            }
            else
                return uriElements;
        }

        /// <summary>
        /// Deserializes an XMLNode into an instance of an ServiceElement.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static ServiceElement Parse(XmlNode node, bool checkNameSpace)
        {
            ServiceElement serviceElement = new ServiceElement();

            foreach (XmlAttribute attribute in node.Attributes)
            {
                if (attribute.LocalName.ToLower() == "priority")
                    serviceElement.Priority = attribute.Value;

            }


            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.LocalName.ToLower() == "type")
                    if ((checkNameSpace && childNode.NamespaceURI.ToLower() == Constants.XRD_Namespace.ToLower()) || (!checkNameSpace))
                        serviceElement.typeElements.Add(childNode.InnerText);

                if (childNode.LocalName.ToLower() == "uri")
                    if ((checkNameSpace && childNode.NamespaceURI.ToLower() == Constants.XRD_Namespace.ToLower()) || (!checkNameSpace))
                        serviceElement.uriElements.Add(URIElement.Parse(childNode, checkNameSpace));

                if (childNode.LocalName.ToLower() == "uri-template")
                    if ((checkNameSpace && childNode.NamespaceURI.ToLower() == Constants.OpenSocial_Namespace.ToLower()) || (!checkNameSpace))
                        serviceElement.uriTemplateElements.Add(URIElement.Parse(childNode, checkNameSpace));

                if (childNode.LocalName.ToLower() == "mediatype")
                    if ((checkNameSpace && childNode.NamespaceURI.ToLower() == Constants.XRD_Namespace.ToLower()) || (!checkNameSpace))
                        serviceElement.mediaTypeElements.Add(childNode.InnerText);

                if (childNode.LocalName.ToLower() == "localid")
                    if ((checkNameSpace && childNode.NamespaceURI.ToLower() == Constants.XRD_Namespace.ToLower()) || (!checkNameSpace))
                        serviceElement.localIDElements.Add(LocalIDElement.Parse(childNode));

                if (childNode.LocalName.ToLower() == "mustsupport")
                    if ((checkNameSpace && childNode.NamespaceURI.ToLower() == Constants.XRDSimple_Namespace.ToLower()) || (!checkNameSpace))
                        serviceElement.mustSupportElements.Add(childNode.InnerText);

            }

            return serviceElement;
        }

        #endregion
    }
}
