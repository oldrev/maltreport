using System;
using System.Collections.Generic;
using System.IO;
using Sandwych.Reporting.OpenDocument;
using Sandwych.Reporting.Tests.Common;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Sandwych.Reporting.Tests.OpenDocument
{

    [TestFixture]
    public class OdsTemplateTest : AbstractTest
    {
        private const string Template2OdsName = "Sandwych.Reporting.Tests.OpenDocument.Templates.Template2.ods";

        [Test]
        public async Task CanCompileOdsDocumentTemplate()
        {
            using var stream = DocumentTestHelper.GetResource(Template2OdsName);
            var ods = await OdfDocument.LoadFromAsync(stream);
            var template = new OdsTemplate(ods);
        }

        [Test]
        public async Task CanRenderOdsTemplate()
        {
            OdfTemplate template;
            using var stream = DocumentTestHelper.GetResource(Template2OdsName);
            var ods = await OdfDocument.LoadFromAsync(stream);
            template = new OdsTemplate(ods);

            var dataSet = new TestingDataSet();
            var values = new Dictionary<string, object>()
            {
                { "table1", dataSet.Table1 },
                { "so", dataSet.SimpleObject },
            };
            var context = new TemplateContext(values);

            var result = await template.RenderAsync(context);

            await result.SaveAsync(Path.Combine(this.TempPath, "ods-out.ods"));
        }

    }
}
