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
    /// Representation of the LocalID Element in an XRD Service definition.
    /// </summary>
    [XmlType(Namespace = Constants.XRD_Namespace)]
    [XmlRoot(ElementName = "Service", Namespace = Constants.XRD_Namespace)]
    public class LocalIDElement : IPriority
    {
        /// <summary>
        /// The priority this LocalID has over other LocalID's defined in the same service element.
        /// </summary>
        [XmlAttribute(DataType = "nonNegativeInteger", AttributeName = "priority", Namespace = Constants.XRD_Namespace)]
        public string Priority
        {
            get;
            set;
        }       

        /// <summary>
        /// The textual representation of the LocalID.
        /// </summary>
        [XmlText(DataType = "anyURI")]
        public string Text
        {
            get;
            set;
        }

        /// <summary>
        /// Deserializes an XMLNode into an instance of an LocalIDElement.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static LocalIDElement Parse(XmlNode node)
        {
            LocalIDElement localIDElement = new LocalIDElement();

            foreach (XmlAttribute attribute in node.Attributes)
                if (attribute.LocalName.ToLower() == "priority")
                    localIDElement.Priority = attribute.Value;

            localIDElement.Text = node.InnerText;

            return localIDElement;
        }

    }
}
