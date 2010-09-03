using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Bravo.Reporting.OpenDocument;

namespace Bravo.Reporting.OpenDocument.Test
{
    [TestFixture]
    public class OdtDocumentTest
    {

        [Test(Description = "测试读写 ODT 文档的内容")]
        public void TestReadWriteDocumentEntry()
        {
            var doc = new OdfDocument();
            using (var writer = doc.GetEntryTextWriter("test-entry"))
            {
                writer.WriteLine("test-content");
            }

            using (var reader = doc.GetEntryTextReader("test-entry"))
            {
                var content = reader.ReadLine();
                Assert.AreEqual(content, "test-content");
            }
        }
    }
}
