using System;
using System.IO;
using System.Collections.Generic;
using Sandwych.Reporting.OfficeML;
using Sandwych.Reporting.OpenDocument;
using Sandwych.Reporting.Tests.Common;
using NUnit.Framework;

namespace Sandwych.Reporting.Tests.OfficeML
{

    [TestFixture]
    public class WordMLDocumentTest : AbstractTest
    {
        private const string Template1OdtName = "Sandwych.Reporting.Tests.OfficeML.Templates.Template1.doc.xml";

        [Test]
        public void CanRenderWordMLTemplate()
        {
            WordMLTemplate template;
            using (var stream = DocumentTestHelper.GetResource(Template1OdtName))
            {
                var templateDocument = WordMLDocument.LoadFrom(stream);
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

            result.Save(Path.Combine(this.TempPath, "wordml-out.doc.xml"));
        }


    }
}
