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
    public class OdtTemplateTest : AbstractTest
    {
        private const string Template1OdtName = "Sandwych.Reporting.Tests.OpenDocument.Templates.Template1.odt";
        private const string Template3OdtName = "Sandwych.Reporting.Tests.OpenDocument.Templates.Template3.odt";

        [Test]
        public async Task CanCompileOdtDocumentTemplate()
        {
            using var stream = GetTemplate("Odf.Template1.odt");
            var odt = await OdfDocument.LoadFromAsync(stream);
            var template = new OdtTemplate(odt);
        }

        [Test]
        public async Task CanRenderOdtTemplate()
        {
            using var stream = GetTemplate("Odf.Template1.odt");
            var odt = await OdfDocument.LoadFromAsync(stream);
            var template = new OdtTemplate(odt);

            var dataSet = new TestingDataSet();
            var values = new Dictionary<string, object>()
            {
                { "table1", dataSet.Table1 },
                { "so", dataSet.SimpleObject },
            };
            var context = new TemplateContext(values);

            var result = await template.RenderAsync(context);

            await result.SaveAsync(Path.Combine(this.TempPath, "odt-out.odt"));
        }


        [Test]
        public async Task CanRenderOdt3Template()
        {
            using var stream = GetTemplate("Odf.Template3.odt");
            var odt = await OdfDocument.LoadFromAsync(stream);
            var template = new OdtTemplate(odt);

            var dataSet = new TestingDataSet();
            var values = new Dictionary<string, object>()
            {
                { "table1", dataSet.Table1 },
                { "so", dataSet.SimpleObject },
            };
            var context = new TemplateContext(values);

            var result = await template.RenderAsync(context);

            await result.SaveAsync(Path.Combine(this.TempPath, "odt-out.odt"));
        }
    }
}
