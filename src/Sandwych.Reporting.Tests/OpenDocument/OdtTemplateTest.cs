using System;
using System.Collections.Generic;
using Sandwych.Reporting.OpenDocument;
using Sandwych.Reporting.Tests.Common;
using Xunit;

namespace Sandwych.Reporting.Tests.OpenDocument
{

    public class OdtTemplateTest
    {
        private const string Template1OdtName = "Sandwych.Reporting.Tests.OpenDocument.Templates.Template1.odt";

        [Fact]
        public void CanCompileOdtDocumentTemplate()
        {
            using (var stream = DocumentTestHelper.GetResource(Template1OdtName))
            {
                var odt = new OdfDocument();
                odt.Load(stream);
                var template = new OdfTemplate(odt);
            }
        }

        [Fact]
        public void CanRenderOdtTemplate()
        {
            OdfDocument odt = new OdfDocument();
            using (var stream = DocumentTestHelper.GetResource(Template1OdtName))
            {
                odt.Load(stream);
            }
            var template = new OdfTemplate(odt);

            var dataSet = new TestingDataSet();
            var context = new Dictionary<string, object>()
            {
                { "table1", dataSet.Table1 },
                { "so", dataSet.SimpleObject },
            };

            var result = template.Render(context);
            result.Save(@"c:\tmp\out.odt");
        }

    }
}
