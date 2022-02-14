using System;
using Sandwych.Reporting.OpenDocument;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Sandwych.Reporting.Tests.OpenDocument
{

    [TestFixture]
    public class OdtDocumentTest
    {
        private const string Template1OdtName = "Sandwych.Reporting.Tests.OpenDocument.Templates.Template1.odt";


        [Test]
        public async Task CanLoadOdfDocumentAsTemplate()
        {
            using (var stream = DocumentTestHelper.GetResource(Template1OdtName))
            {
                var odt = await OdfDocument.LoadFromAsync(stream);
                var template = new OdtTemplate(odt);
            }

        }


    }
}
