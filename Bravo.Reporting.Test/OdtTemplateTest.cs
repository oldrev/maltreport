using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using NUnit.Framework;


namespace Bravo.Reporting.Test
{
    [TestFixture]
    public sealed class OdtTemplateTest
    {

        [Test(Description = "测试简单的变量替换")]
        public void TestIdsReplacement()
        {
            var ctx = new Dictionary<string, object>()
            {
                {"name", "John Doe"},
            };

            var result = OdfTemplateTestHelper.RenderTemplate(
                @"odf_docs/template_ids.odt", ctx);

            var xmldoc = OdfTemplateTestHelper.GetContentDocument(result);
            var paras = xmldoc.GetElementsByTagName("text:p");

            Assert.GreaterOrEqual(paras.Count, 1);
            var p = paras[0];
            Assert.IsTrue(p.InnerText == "HELLO John Doe WORLD");
        }

        [Test(Description = "测试表格行进行循环填充")]
        public void TestRowLoop()
        {
            var ctx = new Dictionary<string, object>()
            {
                {"chars", new char[] {'A', 'B', 'C', 'D', 'E', 'F'} },
            };

            var result = OdfTemplateTestHelper.RenderTemplate(
                @"odf_docs/template_row_loop.odt", ctx);

            var xmldoc = OdfTemplateTestHelper.GetContentDocument(result);

            var rows = xmldoc.GetElementsByTagName("table:table-row");

            Assert.AreEqual(rows.Count, 6);
            var row0Col0 = rows[0].ChildNodes[0].InnerText;
            var row5Col0 = rows[5].ChildNodes[0].InnerText;

            Assert.IsTrue(row0Col0 == "A");
            Assert.IsTrue(row5Col0 == "F");
        }

    }
}
