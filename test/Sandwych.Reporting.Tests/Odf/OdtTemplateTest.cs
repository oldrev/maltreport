using System;
using System.Collections.Generic;
using System.IO;
using Sandwych.Reporting.Odf;
using Sandwych.Reporting.Tests.Common;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Linq;
using System.Xml.Linq;
using Sandwych.Reporting.Xml;

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
            var template = await OdfTemplateCompiler.Instance.CompileAsync(odt);
        }

        [Test]
        public async Task RenderSimpleOdtTemplate()
        {
            using var stream = GetTemplate("Odf.Template3.odt");
            var odt = await OdfDocument.LoadFromAsync(stream);
            var template = await OdfTemplateCompiler.Instance.CompileAsync(odt);

            var dataSet = new TestingDataSet();
            var values = new Dictionary<string, object>()
            {
                { "table1", dataSet.Table1 },
                { "so", dataSet.SimpleObject },
            };
            var context = new TemplateContext(values);

            var result = await template.RenderAsync(context) as IZipDocument;

            var xdoc = await result.ReadXDocumentEntryAsync("content.xml");
            Assert.AreEqual(dataSet.SimpleObject.StringValue, xdoc.Root.Value);

            await result.SaveAsync(Path.Combine(TempDirPath, "Template3-rendered.odt"));
        }

        [Test]
        public async Task RenderComplexOdtTemplateShouldBeOk()
        {
            using var stream = GetTemplate("Odf.Template1.odt");
            var odt = await OdfDocument.LoadFromAsync(stream);
            var template = await OdfTemplateCompiler.Instance.CompileAsync(odt);

            var dataSet = new TestingDataSet();
            var values = new Dictionary<string, object>()
            {
                { "table1", dataSet.Table1 },
                { "so", dataSet.SimpleObject },
            };
            var context = new TemplateContext(values);

            var result = await template.RenderAsync(context) as IZipDocument;

            var xdoc = await result.ReadXDocumentEntryAsync("content.xml");
            var tableNS = xdoc.NSManager.GetNamespace("table");
            var officeNS = xdoc.NSManager.GetNamespace("office");
            var textNS = xdoc.NSManager.GetNamespace("text");

            // Test output
            {
                var officeText = xdoc.Root.Descendants(officeNS + "text");
                var firstList = officeText.Elements(textNS + "list").First();
                Assert.AreEqual("String: " + dataSet.SimpleObject.StringValue, firstList.Elements().ElementAt(0).Value);
                Assert.AreEqual("Integer: " + dataSet.SimpleObject.IntegerValue, firstList.Elements().ElementAt(1).Value);
            }

            // Test table1
            {
                var table1 = xdoc.Root
                    .Descendants(tableNS + "table")
                    .Where(e => e.Attribute(tableNS + "name").Value == "TABLE1")
                    .Single();
                var table1Rows = table1.Elements(tableNS + "table-row").ToArray();
                Assert.AreEqual(dataSet.Table1.Rows.Count + 1, table1Rows.Count());
                for (var irow = 0; irow < dataSet.Table1.Rows.Count; irow++)
                {
                    var row = table1Rows[irow + 1];
                    var cols = row.Elements(tableNS + "table-cell").ToArray();
                    Assert.AreEqual(7, cols.Length);
                    Assert.AreEqual(dataSet.Table1.Rows[irow].Text, cols[0].Value);
                    Assert.AreEqual(dataSet.Table1.Rows[irow].Integer, int.Parse(cols[1].Value));
                    Assert.AreEqual(dataSet.Table1.Rows[irow].Decimal, decimal.Parse(cols[2].Value));
                    Assert.AreEqual(dataSet.Table1.Rows[irow].Double, double.Parse(cols[3].Value));
                    Assert.AreEqual(dataSet.Table1.Rows[irow].DateTime.DayOfYear, DateTime.Parse(cols[4].Value).DayOfYear);
                    Assert.AreEqual(dataSet.Table1.Rows[irow].DateTimeOffset.ToUnixTimeSeconds(),
                        DateTimeOffset.Parse(cols[5].Value).ToUnixTimeSeconds());
                    Assert.AreEqual(dataSet.Table1.Rows[irow].TimeSpan, TimeSpan.Parse(cols[6].Value));
                }
            }
            await result.SaveAsync(Path.Combine(TempDirPath, "Template1-rendered.odt"));

        }


    }
}
