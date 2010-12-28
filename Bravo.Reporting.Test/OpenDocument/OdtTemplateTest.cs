using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Xml;

using Xunit;

using Bravo.Reporting.OpenDocument;

namespace Bravo.Reporting.OpenDocument.Test
{
    /// <summary>
    /// ODT 格式模板的测试
    /// </summary>
    public sealed class OdtTemplateTest
    {
        [Fact(DisplayName = "测试 ODF 简单的变量替换")]
        public void TestIdsReplacement()
        {
            var ctx = new Dictionary<string, object>()
            {
                {"name", "John Doe"},
                {"varNull", null },
                {"varEmpty", string.Empty },
            };

            var result = OdfTemplateTestHelper.RenderTemplate(
                @"resources/odf_docs/template_ids.odt", ctx);
            result.ShouldBeWellFormedOdfContent();

            var xmldoc = result.GetContentDocument();
            var paras = xmldoc.GetElementsByTagName("text:p");

            Assert.True(paras.Count >= 1);
            var p = paras[0];
            Assert.Equal("HELLO John Doe WORLDABCABC", p.InnerText);
        }

        [Fact(DisplayName = "测试 ODF 表格行进行循环填充")]
        public void TestRowLoop()
        {
            var ctx = new Dictionary<string, object>()
            {
                {"chars", new char[] {'A', 'B', 'C', 'D', 'E', 'F'} },
            };

            var result = OdfTemplateTestHelper.RenderTemplate(
                @"resources/odf_docs/template_row_loop.odt", ctx);
            result.ShouldBeWellFormedOdfContent();

            var xmldoc = result.GetContentDocument();

            var rows = xmldoc.GetElementsByTagName("table:table-row");

            Assert.Equal(6, rows.Count);
            var row0Col0 = rows[0].ChildNodes[0].InnerText;
            var row5Col0 = rows[5].ChildNodes[0].InnerText;

            Assert.Equal("A", row0Col0);
            Assert.Equal("F", row5Col0);
        }

        [Fact(DisplayName = "测试 ODF 对内容进行转义处理")]
        public void TestEscape()
        {
            var ctx = new Dictionary<string, object>()
            {
                { "gt", ">" },
                { "lt", "<" },
            };

            var result = OdfTemplateTestHelper.RenderTemplate(
                @"resources/odf_docs/template_escape.odt", ctx);
            result.ShouldBeWellFormedOdfContent();

            var xmldoc = result.GetContentDocument();

            var paras = xmldoc.GetElementsByTagName("text:p");

            Assert.True(paras.Count >= 1);
            var p = paras[0];
            Assert.Equal("X > Y < Z &; & #end", p.InnerText);
        }

        [Fact(DisplayName = "测试 ODF 模板中的 VTL 语句")]
        public void TestStatements()
        {
            var ctx = new Dictionary<string, object>()
            {
                { "chars", new char[] {'A', 'B', 'C', 'D'} },
                { "cond1", true },
                { "n", 123 },
            };

            var result = OdfTemplateTestHelper.RenderTemplate(
                @"resources/odf_docs/template_statement.odt", ctx);
            result.ShouldBeWellFormedOdfContent();

            var xmldoc = result.GetContentDocument();

            var paras = xmldoc.GetElementsByTagName("text:p");

            Assert.True(paras.Count >= 1);
            var p = paras[0];
            Assert.Equal("AABBCCDD", p.InnerText);

            p = paras[1];
            Assert.Equal("TRUE", p.InnerText);

            p = paras[2];
            Assert.Equal("TRUETAIL_STRING", p.InnerText);
        }

        [Fact(DisplayName = "测试 ODF 图像标记替换")]
        public void TestImage()
        {
            var ctx = new Dictionary<string, object>()
            {
                { "image1", new Image("png", File.ReadAllBytes("resources/go-home.PNG")) },
            };

            var result = OdfTemplateTestHelper.RenderTemplate(
                @"resources/odf_docs/template_image.odt", ctx);
            result.ShouldBeWellFormedOdfContent();

            var manifestDoc = new XmlDocument();
            using (var s = result.GetEntryInputStream(OdfDocument.ManifestEntryPath))
            {
                manifestDoc.Load(s);
            }

            var mediaType = "manifest:media-type";
            var imageCount = 0;
            foreach (XmlElement e in manifestDoc.GetElementsByTagName("manifest:file-entry"))
            {
                if (e.HasAttribute(mediaType) && e.GetAttribute(mediaType) == "image/png")
                {
                    imageCount++;
                }
            }

            Assert.Equal(2, imageCount);

        }

    }
}
