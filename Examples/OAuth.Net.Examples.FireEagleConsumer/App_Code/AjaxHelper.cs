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
using System.Text;
using System.Web;

namespace OAuth.Net.Examples.FireEagleConsumer
{
    public static class AjaxHelper
    {
        public static void SendLocation(HttpContext context, Location location)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            context.Response.Write(AjaxHelper.GetLocationJsonObject(location));
            context.Response.End();
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Json", Justification = "JSON is a domain term")]
        public static string GetLocationJsonObject(Location location)
        {
            if (location == null)
                throw new ArgumentNullException("location");

            var response = new StringBuilder();
            response.Append("{ ").AppendLine();
            response.Append(@"  ""location"":").AppendLine();
            response.Append(@"  {").AppendLine();

            if (location.Point != null)
            {
                response.Append(@"    ""point"":").AppendLine();
                response.Append(@"    {").AppendLine();
                response.Append(@"      ""latitude"": ").Append(location.Point.Latitude).Append(",").AppendLine();
                response.Append(@"      ""longitude"": ").Append(location.Point.Longitude).AppendLine();
                response.Append(@"    },").AppendLine();
            }

            if (location.BoundingBox != null)
            {
                response.Append(@"    ""boundingbox"":").AppendLine();
                response.Append(@"    {").AppendLine();
                response.Append(@"      ""southwest"": ").AppendLine();
                response.Append(@"      {").AppendLine();
                response.Append(@"        ""latitude"": ").Append(location.BoundingBox.Southwest.Latitude).Append(",").AppendLine();
                response.Append(@"        ""longitude"": ").Append(location.BoundingBox.Southwest.Longitude).AppendLine();
                response.Append(@"      },").AppendLine();
                response.Append(@"      ""northeast"": ").AppendLine();
                response.Append(@"      {").AppendLine();
                response.Append(@"        ""latitude"": ").Append(location.BoundingBox.Northeast.Latitude).Append(",").AppendLine();
                response.Append(@"        ""longitude"": ").Append(location.BoundingBox.Northeast.Longitude).AppendLine();
                response.Append(@"      },").AppendLine();
                response.Append(@"    },").AppendLine();
            }

            response.Append(@"    ""name"": """).Append(JsonEscape(location.Name)).Append(@"""").AppendLine();

            response.Append(@"  }").AppendLine();
            response.Append(@"}").AppendLine();

            return response.ToString();
        }

        public static void SendAuthorizationRequired(HttpContext context, AuthorizationRequiredException exception)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (exception == null)
                throw new ArgumentNullException("exception");

            context.Response.Write(@"{ ""authrequired"": """ + JsonEscape(exception.AuthorizationUri.AbsoluteUri) + @""" }");
            context.Response.End();
        }
        
        public static void SendError(HttpContext context, string error)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (error == null)
                throw new ArgumentNullException("error");

            context.Response.Write(@"{ ""error"": """ + JsonEscape(error) + @""" }");
            context.Response.End();
        }

        public static void SendOk(HttpContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            context.Response.Write(@"{ ""ok"": ""true"" }");
            context.Response.End();
        }

        private static string JsonEscape(string escapee)
        {
            return escapee.Replace(@"\", @"\\")
                            .Replace(@"""", @"\""")
                            .Replace(@"/", @"\/")
                            .Replace(@"\b", @"\\b")
                            .Replace(@"\f", @"\\f")
                            .Replace(@"\n", @"\\n")
                            .Replace(@"\r", @"\\r")
                            .Replace(@"\t", @"\\t");
        }
    }
}
