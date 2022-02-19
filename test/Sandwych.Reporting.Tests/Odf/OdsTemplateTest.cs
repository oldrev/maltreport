using System;
using System.Collections.Generic;
using System.IO;
using Sandwych.Reporting.Odf;
using Sandwych.Reporting.Tests.Common;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Sandwych.Reporting.Tests.Odf
{

    [TestFixture]
    public class OdsTemplateTest : AbstractTest
    {
        private const string Template2OdsName = "Sandwych.Reporting.Tests.OpenDocument.Templates.Template2.ods";

        [Test]
        public async Task CanCompileOdsDocumentTemplate()
        {
            using var stream = GetTemplate("Odf.Template2.ods");
            var ods = await OdfDocument.LoadFromAsync(stream);
            var template = new OdsTemplate(ods);
        }

        [Test]
        public async Task CanRenderOdsTemplate()
        {
            using var stream = GetTemplate("Odf.Template2.ods");
            var ods = await OdfDocument.LoadFromAsync(stream);
            var template = new OdsTemplate(ods);

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
