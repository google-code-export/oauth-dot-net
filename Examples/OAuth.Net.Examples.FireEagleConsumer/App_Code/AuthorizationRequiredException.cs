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
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace OAuth.Net.Examples.FireEagleConsumer
{
    [Serializable]
    public class AuthorizationRequiredException : Exception, ISerializable
    {
        public AuthorizationRequiredException()
            : base()
        {
        }

        public AuthorizationRequiredException(string message)
            : base(message)
        {
        }

        public AuthorizationRequiredException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected AuthorizationRequiredException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.AuthorizationUri = (Uri) info.GetValue("AuthorizationUri", typeof(Uri));
        }

        public Uri AuthorizationUri { get; set; }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("AuthorizationUri", this.AuthorizationUri, typeof(Uri));
        }
    }
}
