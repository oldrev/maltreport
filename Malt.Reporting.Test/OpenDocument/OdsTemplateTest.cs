using System;
using System.Collections.Generic;
using System.Xml;
using System.Diagnostics;

using System.Text;

using NUnit.Framework;

namespace Malt.Reporting.OpenDocument.Test
{

    public class OdsTemplateTest
    {

        [Test(Description = "测试 ODF 表格列进行循环填充")]
        public void TestColumnLoop()
        {
            var ctx = new Dictionary<string, object>()
            {
                {"chars", new char[] {'A', 'B', 'C', 'D', 'E', 'F'} },
                {"numbers", new int[] {1, 2, 3, } },
            };

            var result = OdfTemplateTestHelper.RenderTemplate(
                @"resources/odf_docs/template_column_loop.ods", ctx);
            OdfTemplateTestHelper.ShouldBeWellFormedOdfContent(result);

            var xmldoc = OdfTemplateTestHelper.GetContentDocument(result);

            var rows = xmldoc.GetElementsByTagName("table:table-row");
            var row = rows[0];

            Assert.AreEqual("ABCDEF", row.InnerText);

            Debug.WriteLine(string.Format(
                "'template_column_loop.ods' 文件单元格数量：{0}",
                row.ChildNodes.Count));

            Assert.AreEqual(6, row.ChildNodes.Count);

            var row2 = rows[2];
            Assert.AreEqual(3, row2.ChildNodes.Count);

            var row4 = rows[4];
            Assert.AreEqual(4, row4.ChildNodes.Count);
            Assert.AreEqual("1234", row4.InnerText);

        }
    }
}
