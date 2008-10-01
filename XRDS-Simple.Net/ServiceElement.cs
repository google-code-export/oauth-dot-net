using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace XRDS_Simple.Net
{
    [XmlType(Namespace = Constants.XRD_Namespace)]
    [XmlRoot(ElementName = "Service", Namespace = Constants.XRD_Namespace)]
    public class ServiceElement
    {
        private List<string> typeElements = new List<string>();
        private List<string> mediaTypeElements = new List<string>();
        private List<URIElement> uriElements = new List<URIElement>();
        private List<LocalIDElement> localIDElements = new List<LocalIDElement>();
        private List<string> mustSupportElements = new List<string>();

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
    }
}
