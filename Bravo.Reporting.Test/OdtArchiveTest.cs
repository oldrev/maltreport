using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

namespace Bravo.Reporting.Test
{
    [TestFixture]
    public class OdtArchiveTest
    {

        [Test]
        public void TestWriteStream()
        {
            var arc = new OdfArchive();
            using (var writer = arc.GetContentWriter("test-entry"))
            {
                writer.WriteLine("test-content");
            }

            using (var reader = arc.GetContentReader("test-entry"))
            {
                var content = reader.ReadLine();
                Assert.AreEqual(content, "test-content");
            }
        }
    }
}
