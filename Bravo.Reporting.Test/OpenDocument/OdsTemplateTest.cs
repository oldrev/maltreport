using System;
using System.Collections.Generic;
using System.Xml;
using System.Diagnostics;

using System.Text;

using NUnit.Framework;

namespace Bravo.Reporting.OpenDocument.Test
{

    [TestFixture]
    public class OdsTemplateTest
    {

        [Test(Description = "测试表格列进行循环填充")]
        public void TestColumnLoop()
        {
            var ctx = new Dictionary<string, object>()
            {
                {"chars", new char[] {'A', 'B', 'C', 'D', 'E', 'F'} },
                {"numbers", new int[] {1, 2, 3, } },
            };

            var result = OdfTemplateTestHelper.RenderTemplate(
                @"resources/odf_docs/template_column_loop.ods", ctx);

            var xmldoc = OdfTemplateTestHelper.GetContentDocument(result);

            var rows = xmldoc.GetElementsByTagName("table:table-row");
            var row = rows[0];

            Assert.IsTrue(row.InnerText == "ABCDEF");

            Debug.WriteLine(string.Format(
                "'template_column_loop.ods' 文件单元格数量：{0}",
                row.ChildNodes.Count));

            Assert.AreEqual(row.ChildNodes.Count, 6);

            var row2 = rows[2];
            Assert.AreEqual(row2.ChildNodes.Count, 3);

        }
    }
}
