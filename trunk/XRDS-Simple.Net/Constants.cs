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
// OAuth.net uses the Windsor Container from the Castle Project. See "Castle 
// Project License.txt" in the Licenses folder.
//
// XRDS-Simple.net uses the HTMLAgility Pack. See "HTML Agility Pack License.txt"
// in the Licenses folder.
// 
// Authors: Chris Adams, Bruce Boughton
// Website: http://lab.madgex.com/oauth-net/
// Email:   oauth-dot-net@madgex.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRDS_Simple.Net
{
    /// <summary>
    /// Constants used by the XRDS-Simple.Net libraries.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// The XRDS Version supported by this class library.
        /// </summary>
        public const string XRDS_Version = "2.0";

        /// <summary>
        /// The XML namespace of the XRDS document node.
        /// </summary>
        public const string XRDS_Namespace = "xri://$xrds";

        /// <summary>
        /// THE XML namespace of the XRD element.
        /// </summary>
        public const string XRD_Namespace = "xri://$XRD*($v*2.0)";

        /// <summary>
        /// The XML XRDSimple Namespace extension
        /// </summary>
        public const string XRDSimple_Namespace = "http://xrds-simple.net/core/1.0";

        /// <summary>
        /// XRDSimple XRI Used in the Type declaration.
        /// </summary>
        public const string XRDSimple_XRI = "xri://$xrds*simple";

        /// <summary>
        /// XRDS+XML HTTP ContentType
        /// </summary>
        public const string ApplicationXRDSXML_ContentType = "application/xrds+xml";

        /// <summary>
        /// HTTP Header that identifies the location of the XRDS discovery document.
        /// </summary>
        public const string XRDSLocation_Header = "X-XRDS-Location";
    }
}
