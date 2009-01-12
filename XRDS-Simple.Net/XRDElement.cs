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
    /// An XRD element in an XRDS document, with the elements supported
    /// by XRDSSimple.
    /// </summary>
    [XmlType(Namespace = Constants.XRD_Namespace)]
    [XmlRoot(ElementName = "XRD", Namespace = Constants.XRD_Namespace)]
    public class XRDElement
    {
        private List<string> typeElements = new List<string>();
        private List<ServiceElement> serviceElements = new List<ServiceElement>();

        private string xrdsVersion = Constants.XRDS_Version;

        #region XRDElement Properties
        
        /// <summary>
        /// An identifier used to distinguish different XRD elements in a
        /// single XRDS document.
        /// </summary>
        [XmlAttribute(AttributeName = "id", Namespace="http://www.w3.org/XML/1998/namespace")]
        public string ID
        {
            get;
            set;
        }        

        /// <summary>
        /// The XRDS version this XRD complies to.
        /// </summary>
        [XmlAttribute(AttributeName = "version", Namespace = Constants.XRD_Namespace)]
        public string Version
        {
            get { return xrdsVersion; }
            set 
            {
                xrdsVersion = value;
            }
        }

        /// <summary>
        /// An array of types that this XRD document represents.  For XRDSSimple 
        /// only one type element needs to be defined.
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
        /// The date and time that this XRD is no longer valid from.
        /// </summary>
        [XmlElement(DataType = "dateTime", ElementName = "Expires", IsNullable = true, Namespace = Constants.XRD_Namespace)]
        public DateTime? Expires
        {
            get;
            set;
        }

        /// <summary>
        /// An array of service elements associated with this XRD.
        /// </summary>
        [XmlElement(ElementName = "Service", Namespace = Constants.XRD_Namespace)]
        public ServiceElement[] Services
        {
            get { return serviceElements.ToArray(); }
            set
            {
                serviceElements.Clear();
                if (value != null)
                    serviceElements.AddRange( value );
            }
        }

        #endregion

        /// <summary>
        /// Returns an enumeration of services for this XRD Element in 
        /// a valid order based on each services priority.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ServiceElement> FindServices()
        {
            return FindServices((Predicate<ServiceElement>)null);
        }

        /// <summary>
        /// Returns an enumeration of services for this XRD Element in 
        /// a valid order based on each services priority.
        /// </summary>
        /// <returns></returns
        public IEnumerable<ServiceElement> FindServices(string type)
        {
            return FindServices(service => Array.IndexOf<string>(service.Types, type) > -1);
        }        

        /// <summary>
        /// Returns an enumeration of service for this XRD Element filtered by
        /// the supplied Predicate.  The returned elements are ordered based on their 
        /// priority.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IEnumerable<ServiceElement> FindServices(Predicate<ServiceElement> filter)
        {
          
            List<ServiceElement> filteredList = serviceElements;

            if (filter != null)
                filteredList = serviceElements.FindAll(filter);

            if (filteredList.Count > 1)
            {
                List<IPriority> sortedList = filteredList.ConvertAll<IPriority>(element => (IPriority)element);

                PriorityComparer comparer = new PriorityComparer();
                sortedList.Sort(comparer);

                //Expose this only as an IEnumerable.
                return sortedList.ConvertAll<ServiceElement>(element => (ServiceElement)element); ;
            }
            else
                return filteredList;
        }

        /// <summary>
        /// Deserializes an XMLNode into an instance of an XRDElement.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static XRDElement Parse(XmlNode node, bool checkNameSpace)
        {
            XRDElement xrdElement = new XRDElement();

            foreach (XmlAttribute attribute in node.Attributes)
            {
                if (attribute.LocalName.ToLower() == "id")
                    if ((checkNameSpace && attribute.NamespaceURI.ToLower() == Constants.XML_Namespace.ToLower()) || (!checkNameSpace))
                        xrdElement.ID = attribute.Value;

                if (attribute.LocalName.ToLower() == "version")
                    xrdElement.Version = attribute.Value;
            }

            foreach (XmlNode childNode in node.ChildNodes)
            {

                if (childNode.LocalName.ToLower() == "expires")
                    if ((checkNameSpace && childNode.NamespaceURI.ToLower() == Constants.XRD_Namespace.ToLower()) || (!checkNameSpace))
                        xrdElement.Expires = Convert.ToDateTime(childNode.InnerText);

                if (childNode.LocalName.ToLower() == "type")
                    if ((checkNameSpace && childNode.NamespaceURI.ToLower() == Constants.XRD_Namespace.ToLower()) || (!checkNameSpace))
                        xrdElement.typeElements.Add(childNode.InnerText);

                if (childNode.LocalName.ToLower() == "service")
                    if ((checkNameSpace && childNode.NamespaceURI.ToLower() == Constants.XRD_Namespace.ToLower()) || (!checkNameSpace))
                        xrdElement.serviceElements.Add(ServiceElement.Parse(childNode, checkNameSpace));

            }

            return xrdElement;
        }

    }
}
