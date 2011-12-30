using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Schema;

using NUnit.Framework;

using Malt.Reporting.Test;

namespace Malt.Reporting.OfficeXml
{
    /// <summary>
    /// "Word 2003 XML 格式模板的测试
    /// </summary>
    [TestFixture]
    public sealed class WordMLTemplateTest
    {

        [Ignore]
        //[Test]
        public void TestReferenceReplacement()
        {
            var ctx = new Dictionary<string, object>() {
                {"var1", "_HELLO_" },
                {"var2", "_WORLD_" },
            };

            var result = TemplateTestHelper.RenderTemplate<WordMLTemplate>(
                @"resources/word2003xml_docs/template_reference_replacement.doc", ctx);

            var xmldoc = TemplateTestHelper.GetlXmlDocument((WordMLTemplate)result);
            xmldoc.ShouldBeWellFormedWordML();

            var body = xmldoc.GetElementsByTagName("w:body")[0];
            var bodyText = body.InnerText.Trim();

            Assert.AreEqual("TEST_HELLO_REFERENCE_WORLD_REPLACEMENT", bodyText);
        }


        [Test(Description = "测试 Word 2003 Xml 的 RTL:// 链接 URL 转义")]
        public void TestEscapeUrl()
        {
            var ctx = new Dictionary<string, object>()
            {
            };

            var result = TemplateTestHelper.RenderTemplate<WordMLTemplate>(
                @"resources/word2003xml_docs/template_escape_url.doc", ctx);

            var xmldoc = TemplateTestHelper.GetlXmlDocument((WordMLTemplate)result);
            xmldoc.ShouldBeWellFormedWordML();

            var body = xmldoc.GetElementsByTagName("w:body")[0];
            var bodyText = body.InnerText.Trim();

            Assert.AreEqual("革命", bodyText);
        }

    }
}
