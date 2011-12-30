using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Xml;

using NUnit.Framework;

using Malt.Reporting.Test;

namespace Malt.Reporting.OfficeXml
{
    //"Excel 2003 XML 格式模板的测试"
    [TestFixture]
    public sealed class ExcelMLTemplateTest
    {
		public sealed class FunctionsProvider
		{
			public string ToUpper(string s)
			{
				return s.ToUpper();
			}
		}


        [Test(Description = "测试 Excel 2003 XML 的简单行循环")]
        public void TestSimpleRowLoop()
        {
            var ctx = new Dictionary<string, object>()
            {
                {"chars", new char[] {'A', 'B', 'C', 'D', 'E', 'F'} },
            };

            var result = TemplateTestHelper.RenderTemplate<ExcelMLTemplate>(
                @"resources/excel2003xml_docs/template_row_loop.xls", ctx);

            var xmldoc = TemplateTestHelper.GetlXmlDocument((ExcelMLTemplate)result);

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

            var result = TemplateTestHelper.RenderTemplate<ExcelMLTemplate>(
                @"resources/excel2003xml_docs/template_column_loop.xls", ctx);

            var xmldoc = TemplateTestHelper.GetlXmlDocument(result);

            var table = xmldoc.GetElementsByTagName("Table")[0];

            Assert.AreEqual("JJJXYABCDEFZKKK", table.InnerText);

            var rows = xmldoc.GetElementsByTagName("Row");

            Assert.AreEqual("JJJ", rows[0].InnerText);
            Assert.AreEqual("XYABCDEFZ", rows[1].InnerText);
            Assert.AreEqual("KKK", rows[2].InnerText);

        }

        [Test(Description = "测试 Excel 2003 Xml 复杂模板的编译渲染")]
        public void TestComplexTemplate()
        {
            var ctx = ContextDataFactory.CreateComplexDataContext();

            var result = TemplateTestHelper.RenderTemplate<ExcelMLTemplate>(
                @"resources/excel2003xml_docs/template_complex.xls", ctx);

            var xmldoc = TemplateTestHelper.GetlXmlDocument(result);
            xmldoc.ShouldBeWellFormedExcelML();

        }

        [Test(Description = "测试执行提供的函数")]
        public void TestUserFunction()
        {					
            var ctx = new Dictionary<string, object>()
            {
				{ "functions", new FunctionsProvider() },
            };

            var result = TemplateTestHelper.RenderTemplate<ExcelMLTemplate>(
                @"resources/excel2003xml_docs/function.xls", ctx);

            var xmldoc = TemplateTestHelper.GetlXmlDocument(result);
            var table = xmldoc.GetElementsByTagName("Table")[0];

            Assert.AreEqual("ABCD", table.InnerText);
        }

    }
}
