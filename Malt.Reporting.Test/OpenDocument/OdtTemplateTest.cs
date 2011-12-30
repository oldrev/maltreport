using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Xml;

using NUnit.Framework;

using Malt.Reporting.OpenDocument;

namespace Malt.Reporting.OpenDocument
{
    /// <summary>
    /// ODT 格式模板的测试
    /// </summary>
    [TestFixture]
    public sealed class OdtTemplateTest
    {
        [Test(Description = "测试 ODF 简单的变量替换")]
        public void TestIdsReplacement()
        {
            var ctx = new Dictionary<string, object>() {
                {"name", "John Doe"},
                {"varNull", null },
                {"varEmpty", string.Empty },
            };

            var result = OdfTemplateTestHelper.RenderTemplate(
                @"resources/odf_docs/template_ids.odt", ctx);
            result.ShouldBeWellFormedOdfContent();

            var xmldoc = result.GetContentDocument();
            var paras = xmldoc.GetElementsByTagName("text:p");

            Assert.IsTrue(paras.Count >= 1);
            var p = paras[0];
            Assert.AreEqual("HELLO John Doe WORLDABCABC", p.InnerText);
        }

        [Test(Description = "测试 ODF 表格行进行循环填充")]
        public void TestRowLoop()
        {
            var ctx = new Dictionary<string, object>() {
                {"chars", new char[] {'A', 'B', 'C', 'D', 'E', 'F'} },
            };

            var result = OdfTemplateTestHelper.RenderTemplate(
                @"resources/odf_docs/template_row_loop.odt", ctx);
            result.ShouldBeWellFormedOdfContent();

            var xmldoc = result.GetContentDocument();

            var rows = xmldoc.GetElementsByTagName("table:table-row");

            Assert.AreEqual(6, rows.Count);
            var row0Col0 = rows[0].ChildNodes[0].InnerText;
            var row5Col0 = rows[5].ChildNodes[0].InnerText;

            Assert.AreEqual("A", row0Col0);
            Assert.AreEqual("F", row5Col0);
        }

        [Test(Description = "测试 ODF 对内容进行转义处理")]
        public void TestEscape()
        {
            var ctx = new Dictionary<string, object>() {
                { "gt", ">" },
                { "lt", "<" },
            };

            var result = OdfTemplateTestHelper.RenderTemplate(
                @"resources/odf_docs/template_escape.odt", ctx);
            result.ShouldBeWellFormedOdfContent();

            var xmldoc = result.GetContentDocument();

            var paras = xmldoc.GetElementsByTagName("text:p");

            Assert.IsTrue(paras.Count >= 1);
            var p = paras[0];
            Assert.AreEqual("X > Y < Z &; & #end", p.InnerText);
        }

        [Test(Description = "测试 ODF 模板中的 VTL 语句")]
        public void TestStatements()
        {
            var ctx = new Dictionary<string, object>() {
                { "chars", new char[] {'A', 'B', 'C', 'D'} },
                { "cond1", true },
                { "n", 123 },
            };

            var result = OdfTemplateTestHelper.RenderTemplate(
                @"resources/odf_docs/template_statement.odt", ctx);
            result.ShouldBeWellFormedOdfContent();

            var xmldoc = result.GetContentDocument();

            var paras = xmldoc.GetElementsByTagName("text:p");

            Assert.IsTrue(paras.Count >= 1);
            var p = paras[0];
            Assert.AreEqual("AABBCCDD", p.InnerText);

            p = paras[1];
            Assert.AreEqual("TRUE", p.InnerText);

            p = paras[2];
            Assert.AreEqual("TRUETAIL_STRING", p.InnerText);
        }

        [Test]
        public void UseImageShouldBeOk()
        {
            var ctx = new Dictionary<string, object>() {
                { "image1", new Image("png", File.ReadAllBytes("resources/go-home.PNG")) },
            };

            var result = OdfTemplateTestHelper.RenderTemplate(
                @"resources/odf_docs/template_image.odt", ctx);
            result.ShouldBeWellFormedOdfContent();

            var manifestDoc = new XmlDocument();
            using (var s = result.GetEntryInputStream(OdfTemplate.ManifestEntryPath)) {
                manifestDoc.Load(s);
            }

            var mediaType = "manifest:media-type";
            var imageCount = 0;
            foreach (XmlElement e in manifestDoc.GetElementsByTagName("manifest:file-entry")) {
                if (e.HasAttribute(mediaType) && e.GetAttribute(mediaType) == "image/png") {
                    imageCount++;
                }
            }

            Assert.AreEqual(2, imageCount);
        }

    }
}
