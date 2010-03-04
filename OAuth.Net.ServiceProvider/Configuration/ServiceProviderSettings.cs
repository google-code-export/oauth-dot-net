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

using System.Configuration;
using OAuth.Net.Common;

namespace OAuth.Net.ServiceProvider
{
    public sealed class ServiceProviderSettings
        : ConfigurationSection
    {
        [ConfigurationProperty("AuthenticationRealm", IsRequired = true)]
        public string AuthenticationRealm
        {
            get { return (string)this["AuthenticationRealm"]; }
            set { this["AuthenticationRealm"] = value; }
        }

        /// <summary>
        /// Defines if the Pipeline module checks for OAuth authentication parameters on inbound requests.
        /// The &lt;location&gt;&lt;/location&gt tag of the web.config file can be used to switch this settings on/off for
        /// different urls.
        /// </summary>
        [ConfigurationProperty("AuthenticateRequests", IsRequired = false, DefaultValue = true)]
        public bool AuthenticateRequests
        {
            get { return (bool)this["AuthenticateRequests"]; }
            set { this["AuthenticateRequests"] = value; }
        }

        #warning TODO: This is completely ignored at the moment! (never advertised)
        [ConfigurationProperty("AdvertiseOAuthSupport", IsRequired = false, DefaultValue = true)]
        public bool AdvertiseOAuthSupport
        {
            get { return (bool)this["AdvertiseOAuthSupport"]; }
            set { this["AdvertiseOAuthSupport"] = value; }
        }

        [ConfigurationProperty("AllowOutOfBandCallback", IsRequired = false, DefaultValue = true)]
        public bool AllowOutOfBandCallback
        {
            get { return (bool)this["AllowOutOfBandCallback"]; }
            set { this["AllowOutOfBandCallback"] = value; }
        }

        [ConfigurationProperty("ParameterSources", IsRequired = false, DefaultValue = OAuthParameterSources.ServiceProviderDefault)]
        public OAuthParameterSources ParameterSources
        {
            get { return (OAuthParameterSources)this["ParameterSources"]; }
            set { this["ParameterSources"] = value; }
        }

        [ConfigurationProperty("ImpersonateUserForProtectedResources", IsRequired = false, DefaultValue = true)]
        public bool ImpersonateUserForProtectedResources
        {
            get { return (bool)this["ImpersonateUserForProtectedResources"]; }
            set { this["ImpersonateUserForProtectedResources"] = value; }
        }

        [ConfigurationProperty("DummyIdentity", IsRequired = false, DefaultValue = "OAuthDummyIdentity")]
        public string DummyIdentity
        {
            get { return (string)this["DummyIdentity"]; }
            set { this["DummyIdentity"] = value; }
        }

        [ConfigurationProperty("AllowConsumerRequests", IsRequired = false, DefaultValue = false)]
        public bool AllowConsumerRequests
        {
            get { return (bool)this["AllowConsumerRequests"]; }
            set { this["AllowConsumerRequests"] = value; }
        }

        [ConfigurationProperty("ConsumerRequestRoles", IsRequired = false, DefaultValue = new string[]{})]
        public string[] ConsumerRequestRoles
        {
            get { return (string[])this["ConsumerRequestRoles"]; }
            set { this["ConsumerRequestRoless"] = value;}
        }

    }
}
