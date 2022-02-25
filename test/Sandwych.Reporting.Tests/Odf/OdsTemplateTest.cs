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

        [Test]
        public async Task CanCompileOdsDocumentTemplate()
        {
            using var stream = GetTemplate("Odf.Template2.ods");
            var ods = await OdfDocument.LoadFromAsync(stream);
            var template = await OdfTemplateCompiler.Instance.CompileAsync(ods);
        }

        [Test]
        public async Task CanRenderOdsTemplate()
        {
            using var stream = GetTemplate("Odf.Template2.ods");
            var ods = await OdfDocument.LoadFromAsync(stream);
            var template = await OdfTemplateCompiler.Instance.CompileAsync(ods);

            var dataSet = new TestingDataSet();
            var values = new 
            {
                table1 = dataSet.Table1,
                so = dataSet.SimpleObject,
            };
            var context = new TemplateContext(values);
            context.Options.AllowUnsafeAccess = true;

            var result = await template.RenderAsync(context);

            await result.SaveAsync(Path.Combine(TempDirPath, "ods-out.ods"));
        }

    }
}
