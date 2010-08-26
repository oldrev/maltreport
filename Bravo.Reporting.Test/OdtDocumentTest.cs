using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

namespace Bravo.Reporting.Test
{
    [TestFixture]
    public class OdtDocumentTest
    {

        [Test]
        public void TestReadWriteDocumentEntry()
        {
            var doc = new OdfDocument();
            using (var writer = doc.GetEntryWriter("test-entry"))
            {
                writer.WriteLine("test-content");
            }

            using (var reader = doc.GetEntryReader("test-entry"))
            {
                var content = reader.ReadLine();
                Assert.AreEqual(content, "test-content");
            }
        }
    }
}
