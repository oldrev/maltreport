using System;
using Sandwych.Reporting.OpenDocument;
using Xunit;

namespace Sandwych.Reporting.Tests.OpenDocument
{

    public class OdtDocumentTest
    {
        private const string Template1OdtName = "Sandwych.Reporting.Tests.OpenDocument.Templates.Template1.odt";


        [Fact]
        public void CanLoadOdfDocumentAsTemplate()
        {
            using (var stream = DocumentTestHelper.GetResource(Template1OdtName))
            {
                var odt = OdfDocument.Load(stream);
                var template = new OdtTemplate(odt);
            }

        }


    }
}
