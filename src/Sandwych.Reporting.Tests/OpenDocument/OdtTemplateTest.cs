using System;
using System.Collections.Generic;
using System.IO;
using Sandwych.Reporting.OpenDocument;
using Sandwych.Reporting.Tests.Common;
using NUnit.Framework;

namespace Sandwych.Reporting.Tests.OpenDocument
{
    [TestFixture]
    public class OdtTemplateTest : AbstractTest
    {
        private const string Template1OdtName = "Sandwych.Reporting.Tests.OpenDocument.Templates.Template1.odt";

        [Test]
        public void CanCompileOdtDocumentTemplate()
        {
            using (var stream = DocumentTestHelper.GetResource(Template1OdtName))
            {
                var odt = OdfDocument.Load(stream);
                var template = new OdtTemplate(odt);
            }
        }

        [Test]
        public void CanRenderOdtTemplate()
        {
            OdfTemplate template;
            using (var stream = DocumentTestHelper.GetResource(Template1OdtName))
            {
                var odt = OdfDocument.Load(stream);
                template = new OdtTemplate(odt);
            }

            var dataSet = new TestingDataSet();
            var values = new Dictionary<string, object>()
            {
                { "table1", dataSet.Table1 },
                { "so", dataSet.SimpleObject },
            };
            var context = new TemplateContext(values);

            var result = template.Render(context);

            result.Save(Path.Combine(this.TempPath, "odt-out.odt"));
        }

    }
}
