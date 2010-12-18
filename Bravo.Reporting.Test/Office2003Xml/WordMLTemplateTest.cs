using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Schema;

using Xunit;

using Bravo.Reporting.Test;

namespace Bravo.Reporting.OfficeXml.Test
{
    /// <summary>
    /// "Word 2003 XML 格式模板的测试
    /// </summary>
    public sealed class WordMLTemplateTest
    {

        [Fact(DisplayName = "测试 Word 2003 Xml 的简单引用替换")]
        public void TestReferenceReplacement()
        {
            var ctx = new Dictionary<string, object>()
            {
                {"var1", "_HELLO_" },
                {"var2", "_WORLD_" },
            };

            var result = TemplateTestHelper.RenderTemplate<WordMLDocument>(
                @"resources/word2003xml_docs/template_reference_replacement.xml", ctx);

            var xmldoc = TemplateTestHelper.GetlXmlDocument((WordMLDocument)result);
            xmldoc.ShouldBeWellFormedWordML();

            var body = xmldoc.GetElementsByTagName("w:body")[0];
            var bodyText = body.InnerText.Trim();

            Assert.Equal("TEST_HELLO_REFERENCE_WORLD_REPLACEMENT", bodyText);
        }

        [Fact(DisplayName = "测试 Word 2003 Xml 的 RTL:// 链接 URL 转义")]
        public void TestEscapeUrl()
        {
            var ctx = new Dictionary<string, object>()
            {
            };

            var result = TemplateTestHelper.RenderTemplate<WordMLDocument>(
                @"resources/word2003xml_docs/template_escape_url.xml", ctx);

            var xmldoc = TemplateTestHelper.GetlXmlDocument((WordMLDocument)result);
            xmldoc.ShouldBeWellFormedWordML();

            var body = xmldoc.GetElementsByTagName("w:body")[0];
            var bodyText = body.InnerText.Trim();

            Assert.Equal("革命", bodyText);
        }

    }
}
