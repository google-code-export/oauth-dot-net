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
using System.Xml.Serialization;

namespace OAuth.Net.Examples.TwitterClient.Api
{
    [XmlRoot(ElementName = "user")]
    public class BasicUser
    {
        [XmlElement(ElementName = "id")]
        public string Id { get; set; }

        [XmlElement(ElementName = "name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "screen_name")]
        public string ScreenName { get; set; }

        [XmlElement(ElementName = "location")]
        public string Location { get; set; }

        [XmlElement(ElementName = "description")]
        public string Description { get; set; }

        [XmlElement(ElementName = "profile_image_url")]
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores",
            Justification = "Shim for XML serialization. Not intended to be used by callers")]
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings",
            Justification = "XML Serialization does not support Uri. Have to use string shim.")]
        public string __ProfileImageUrl
        {
            get
            {
                return this.ProfileImageUrl != null
                    ? this.ProfileImageUrl.AbsoluteUri
                    : null;
            }

            set
            {
                Uri uri;
                Uri.TryCreate(value, UriKind.Absolute, out uri);
                this.ProfileImageUrl = uri;
            }
        }

        [XmlIgnore]
        public Uri ProfileImageUrl { get; set; }

        [XmlElement(ElementName = "url")]
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores",
            Justification = "Shim for XML serialization. Not intended to be used by callers")]
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings",
            Justification = "XML Serialization does not support Uri. Have to use string shim.")]
        public string __Url
        {
            get
            {
                return this.Url != null
                    ? this.Url.AbsoluteUri
                    : null;
            }

            set
            {
                Uri uri;
                Uri.TryCreate(value, UriKind.Absolute, out uri);
                this.Url = uri;
            }
        }

        [XmlIgnore]
        public Uri Url { get; set; }

        [XmlElement(ElementName = "protected")]
        public bool IsProtected { get; set; }

        [XmlElement(ElementName = "followers_count")]
        public int FollowersCount { get; set; }

        [XmlElement(ElementName = "status")]
        public Status LastStatus { get; set; }
    }
}