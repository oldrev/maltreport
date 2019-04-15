using System;
using Sandwych.Reporting.OpenDocument;
using NUnit.Framework;

namespace Sandwych.Reporting.Tests.OpenDocument
{

    [TestFixture]
    public class OdtDocumentTest
    {
        private const string Template1OdtName = "Sandwych.Reporting.Tests.OpenDocument.Templates.Template1.odt";


        [Test]
        public void CanLoadOdfDocumentAsTemplate()
        {
            using (var stream = DocumentTestHelper.GetResource(Template1OdtName))
            {
                var odt = OdfDocument.LoadFrom(stream);
                var template = new OdtTemplate(odt);
            }

        }


    }
}
