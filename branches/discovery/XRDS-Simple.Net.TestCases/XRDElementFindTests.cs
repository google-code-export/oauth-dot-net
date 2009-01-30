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
    [TestFixture]
    public class XRDElementFindTests
    {
        [Test]
        public void TestOAUTHDiscoveryExampleMainXRDElementFind()
        {
            XRDSDocument document = null;

            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("XrdsSimple.Net.TestCases.OAUTHDiscoveryExample.xml"))
            {
                document = XRDSDocument.Parse(s);
            }

            XRDElement mainServiceElement = document.FindXRD();

            Assert.That(mainServiceElement != null, "No Main XRDSSimple Element found");            
        }

        [Test]
        public void TestOAUTHDiscoveryExampleNamedXRDElementFind()
        {
            XRDSDocument document = null;

            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("XrdsSimple.Net.TestCases.OAUTHDiscoveryExample.xml"))
            {
                document = XRDSDocument.Parse(s);
            }

            XRDElement mainServiceElement = document.FindXRD("oauth");

            Assert.That(mainServiceElement != null, "No Main XRDSSimple Element found");
        }

        [Test]
        public void TestOAUTHDiscoveryServicesPrioityCheck()
        {
            XRDSDocument document = null;

            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("XrdsSimple.Net.TestCases.OAUTHDiscoveryExample.xml"))
            {
                document = XRDSDocument.Parse(s);
            }

            XRDElement mainServiceElement = document.FindXRD("oauth");

            int lastPriority = Int32.MaxValue;

            foreach (ServiceElement service in mainServiceElement.FindServices())
            {
                int priority;

                if (!Int32.TryParse(service.Priority, out priority))
                    priority = lastPriority;

                Assert.That(priority <= lastPriority, "Incorrectly ordered priority found.");

                lastPriority = priority;
            }
        }
        
    }
}
