using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

using XrdsSimple.Net;

namespace XrdsSimple.Net.TestCases
{
    [TestFixture, Ignore("Out of date")]
    public class XRDSDocumentParsingTests
    {
        [Test]
        public void TestOAUTHDiscoveryExampleLoad()
        {
            XRDSDocument document = null;

            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("XrdsSimple.Net.TestCases.OAUTHDiscoveryExample.xml"))
            {
                document = XRDSDocument.Parse(s);
            }

            Assert.That(document.AllXRDElements.Length == 2, "Invalid number of XRD Elements, expected 2");

        }      

        [Test]
        public void TestPlaxoDotCOMXRDSRetrieval()
        {

            XRDSDocument document = XRDSDocument.LoadDocument(new Uri("http://pulse.plaxo.com/pulse/oauth/xrds"));

            Assert.That(document.AllXRDElements.Length  > 0, "Invalid number of XRD Elements, expected 2");
        }
    }
}
