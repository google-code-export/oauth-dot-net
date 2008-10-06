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
using System.Xml.Serialization;

namespace XRDS_Simple.Net
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
        IEnumerable<LocalIDElement> GetLocalIDs()
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
        IEnumerable<URIElement> GetURIs()
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

        #endregion
    }
}
