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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;

namespace OAuth.Net.Examples.FireEagleConsumer
{
    public class Location
    {
        public bool BestGuess { get; set; }

        public string Id { get; set; }

        public GeoRssPoint Point { get; set; }

        public GeoRssBox BoundingBox { get; set; }

        public string Label { get; set; }

        public int Level { get; set; }

        public string LevelName { get; set; }

        public DateTime LocatedAt { get; set; }

        public string Name { get; set; }

        public string NormalName { get; set; }

        public string PlaceId { get; set; }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Woeid", Justification = "WOEID is a domain term")]
        public string Woeid { get; set; }

        public static Location Parse(IXPathNavigable node)
        {
            XPathNavigator navigator = node.CreateNavigator();
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(navigator.NameTable);
            namespaceManager.AddNamespace("georss", "http://www.georss.org/georss");

            return new Location()
            {
                BestGuess = Try.Do<bool>(() => bool.Parse(navigator.SelectSingleNode("@best-guess").Value)),
                Id = Try.Do<string>(() => navigator.SelectSingleNode("id/text()").Value),
                Point = Try.Do<GeoRssPoint>(() => GeoRssPoint.Parse(navigator.SelectSingleNode("georss:point/text()", namespaceManager).Value)),
                BoundingBox = Try.Do<GeoRssBox>(() => GeoRssBox.Parse(navigator.SelectSingleNode("georss:box/text()", namespaceManager).Value)),
                Label = Try.Do<string>(() => navigator.SelectSingleNode("label/text()").Value),
                Level = Try.Do<int>(() => int.Parse(navigator.SelectSingleNode("level/text()").Value, CultureInfo.InvariantCulture)),
                LevelName = Try.Do<string>(() => navigator.SelectSingleNode("level-name/text()").Value),
                LocatedAt = Try.Do<DateTime>(() => DateTime.Parse(navigator.SelectSingleNode("located-at/text()").Value, CultureInfo.InvariantCulture)),
                Name = Try.Do<string>(() => navigator.SelectSingleNode("name/text()").Value),
                NormalName = Try.Do<string>(() => navigator.SelectSingleNode("normal-name/text()").Value),
                PlaceId = Try.Do<string>(() => navigator.SelectSingleNode("place-id/text()").Value),
                Woeid = Try.Do<string>(() => navigator.SelectSingleNode("woeid/text()").Value)
            };
        }
    }
}
