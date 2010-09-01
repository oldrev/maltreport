using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

using NUnit.Framework;


namespace Bravo.Reporting.Test
{
    [TestFixture(Description = "ODT 格式模板的测试")]
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

            Assert.GreaterOrEqual(1, paras.Count);
            var p = paras[0];
            Assert.AreEqual("HELLO John Doe WORLD", p.InnerText);
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

            Assert.AreEqual(6, rows.Count);
            var row0Col0 = rows[0].ChildNodes[0].InnerText;
            var row5Col0 = rows[5].ChildNodes[0].InnerText;

            Assert.AreEqual("A", row0Col0);
            Assert.AreEqual("F", row5Col0);
        }

        [Test(Description = "测试对内容进行转义处理")]
        public void TestEscape()
        {
            var ctx = new Dictionary<string, object>()
            {
                { "gt", ">" },
                { "lt", "<" },
            };

            var result = OdfTemplateTestHelper.RenderTemplate(
                @"odf_docs/template_escape.odt", ctx);

            var xmldoc = OdfTemplateTestHelper.GetContentDocument(result);

            var paras = xmldoc.GetElementsByTagName("text:p");

            Assert.GreaterOrEqual(paras.Count, 1);
            var p = paras[0];
            Assert.AreEqual("X > Y < Z &; &", p.InnerText);
        }

        [Test(Description = "测试模板中的 VTL 语句")]
        public void TestStatements()
        {
            var ctx = new Dictionary<string, object>()
            {
                { "chars", new char[] {'A', 'B', 'C', 'D'} },
                { "cond1", true },
                { "n", 123 },
            };

            var result = OdfTemplateTestHelper.RenderTemplate(
                @"odf_docs/template_statement.odt", ctx);

            var xmldoc = OdfTemplateTestHelper.GetContentDocument(result);

            var paras = xmldoc.GetElementsByTagName("text:p");

            Assert.GreaterOrEqual(paras.Count, 1);
            var p = paras[0];
            Assert.AreEqual("AABBCCDD", p.InnerText);

            p = paras[1];
            Assert.AreEqual("TRUE", p.InnerText);

            p = paras[2];
            Assert.AreEqual("TRUE", p.InnerText);
        }

        [Test(Description = "测试图像标记替换")]
        public void TestImage()
        {
            var ctx = new Dictionary<string, object>()
            {
                { "image1", new Image("png", File.ReadAllBytes("odf_docs/go-home.PNG")) },
            };

            var result = OdfTemplateTestHelper.RenderTemplate(
                @"odf_docs/template_image.odt", ctx);

            var xmldoc = OdfTemplateTestHelper.GetContentDocument(result);

        }

    }
}
