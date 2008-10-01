using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

using XRDS_Simple.Net;

namespace XRDS_Simple.Net.TestCases
{
    [TestFixture]
    public class XRDSDocumentParsingTests
    {
        [Test]
        public void TestOAUTHDiscoveryExampleLoad()
        {
            XRDSDocument document = null;

            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("XRDS_Simple.Net.TestCases.OAUTHDiscoveryExample.xml"))
            {
                document = XRDSDocument.Parse(s);
            }

            Assert.That(document.XRDElements.Length == 2, "Invalid number of XRD Elements, expected 2");

        }

        [Test]
        public void TestPlaxoDotCOMXRDSRetrieval()
        {

            XRDSDocument document = XRDSDocument.LoadDocument(new Uri("http://pulse.plaxo.com/pulse/oauth/xrds"));

            Assert.That(document.XRDElements.Length  > 2, "Invalid number of XRD Elements, expected 2");
        }
    }
}
