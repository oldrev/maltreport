using System;
using System.Collections.Generic;
using Sandwych.Reporting.OfficeML;
using Sandwych.Reporting.OpenDocument;
using Sandwych.Reporting.Tests.Common;
using Xunit;

namespace Sandwych.Reporting.Tests.OfficeML
{

    public class WordMLDocumentTest
    {
        private const string Template1OdtName = "Sandwych.Reporting.Tests.OfficeML.Templates.Template1.doc.xml";

        [Fact]
        public void CanRenderWordMLTemplate()
        {
            WordMLTemplate template;
            using (var stream = DocumentTestHelper.GetResource(Template1OdtName))
            {
                var templateDocument = WordMLDocument.Load(stream);
                template = new WordMLTemplate(templateDocument);
            }

            var dataSet = new TestingDataSet();
            var values = new Dictionary<string, object>()
            {
                { "table1", dataSet.Table1 },
                { "so", dataSet.SimpleObject },
            };
            var context = new TemplateContext(values);

            var result = template.Render(context);

            result.Save(@"c:\tmp\out.doc.xml");
        }


    }
}
