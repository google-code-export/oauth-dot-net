using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace XRDS_Simple.Net
{

    [XmlType(Namespace = Constants.XRD_Namespace)]
    [XmlRoot(ElementName = "Service", Namespace = Constants.XRD_Namespace)]
    public class LocalIDElement
    {
        [XmlAttribute(DataType = "nonNegativeInteger", AttributeName = "priority", Namespace = Constants.XRD_Namespace)]
        public string Priority
        {
            get;
            set;
        }       

        [XmlText(DataType = "anyURI")]
        public string Text
        {
            get;
            set;
        }

    }
}
