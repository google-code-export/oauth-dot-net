using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace XRDS_Simple.Net
{
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

        [XmlAttribute(DataType = "nonNegativeInteger", AttributeName = "priority", Namespace = Constants.XRD_Namespace)]
        public string Priority
        {
            get;
            set;
        }

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
