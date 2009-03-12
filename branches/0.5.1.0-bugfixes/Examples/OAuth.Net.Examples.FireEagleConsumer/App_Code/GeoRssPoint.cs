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
// Authors: Bruce Boughton, Chris Adams
// Website: http://lab.madgex.com/oauth-net/
// Email:   oauth-dot-net@madgex.com

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace OAuth.Net.Examples.FireEagleConsumer
{
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Rss", Justification = "RSS is a domain term")]
    public class GeoRssPoint
    {
        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public static GeoRssPoint Parse(string simpleEncoding)
        {
            string[] parts = simpleEncoding.Split(
                new[] { ' ' }, 
                StringSplitOptions.RemoveEmptyEntries);

            return new GeoRssPoint()
            {
                Latitude = decimal.Parse(parts[0], CultureInfo.InvariantCulture),
                Longitude = decimal.Parse(parts[1], CultureInfo.InvariantCulture)
            };
        }

        public static GeoRssPoint Parse(string latitude, string longitude)
        {
            return new GeoRssPoint()
            {
                Latitude = decimal.Parse(latitude, CultureInfo.InvariantCulture),
                Longitude = decimal.Parse(longitude, CultureInfo.InvariantCulture)
            };
        }
    }
}
