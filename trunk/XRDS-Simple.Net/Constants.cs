using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRDS_Simple.Net
{
    public static class Constants
    {
        public const string XRDS_Version = "2.0";

        public const string XRDS_Namespace = "xri://$xrds";

        public const string XRD_Namespace = "xri://$XRD*($v*2.0)";

        public const string XRDSimple_Namespace = "http://xrds-simple.net/core/1.0";

        public const string ApplicationXRDSXML_ContentType = "application/xrds+xml";

        public const string XRDSLocation_Header = "X-XRDS-Location";
    }
}
