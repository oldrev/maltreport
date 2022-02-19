using System;
using Sandwych.Reporting.Odf;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Sandwych.Reporting.Tests.Odf
{

    [TestFixture]
    public class OdtDocumentTest : AbstractTest
    {
        [Test]
        public async Task CanLoadOdfDocumentAsTemplate()
        {
            using var stream = GetTemplate("Odf.Template1.odt");
            var odt = await OdfDocument.LoadFromAsync(stream);
            var template = new OdtTemplate(odt);

        }


    }
}
