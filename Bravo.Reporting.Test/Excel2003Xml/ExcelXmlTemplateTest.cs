using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Xml;

using NUnit.Framework;

namespace Bravo.Reporting.Excel2003Xml.Test
{
    [TestFixture(Description = "ODT 格式模板的测试")]
    public sealed class ExcelXmlTemplateTest
    {
        [Test(Description = "测试 Excel 2003 Xml 的简单行循环")]
        public void TestSimpleRowLoop()
        {
            var ctx = new Dictionary<string, object>()
            {
                {"chars", new char[] {'A', 'B', 'C', 'D', 'E', 'F'} },
            };

            var result = ExcelXmlTemplateTestHelper.RenderTemplate(
                @"resources/excel2003xml_docs/template_row_loop.xml", ctx);

            var xmldoc = new XmlDocument();
            using (var ins = result.GetEntryInputStream(result.MainContentEntryPath))
            {
                xmldoc.Load(ins);
            }

            var rows = xmldoc.GetElementsByTagName("Row");

            Assert.AreEqual(6, rows.Count);
            var row0 = rows[0].InnerText;
            var row5 = rows[5].InnerText;

            Assert.AreEqual("AAAAA", row0);
            Assert.AreEqual("FFFFF", row5);
        }

        [Test(Description = "测试 Excel 2003 Xml 的简单列循环")]
        public void TestSimpleColumnLoop()
        {
            var ctx = new Dictionary<string, object>()
            {
                {"chars", new char[] {'A', 'B', 'C', 'D', 'E', 'F'} },
            };

            var result = ExcelXmlTemplateTestHelper.RenderTemplate(
                @"resources/excel2003xml_docs/template_column_loop.xml", ctx);

            var xmldoc = new XmlDocument();
            using (var ins = result.GetEntryInputStream(result.MainContentEntryPath))
            {
                xmldoc.Load(ins);
            }

            var table = xmldoc.GetElementsByTagName("Table")[0];

            xmldoc.Save("d:\\test_loop_col.xml");

            Assert.AreEqual("JJJXYABCDEFZKKK", table.InnerText);
        }

    }
}
