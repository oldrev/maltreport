using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Xml;

using NUnit.Framework;

using Bravo.Reporting.Test;

namespace Bravo.Reporting.Office2003Xml.Test
{
    [TestFixture(Description = "Word 2003 XML 格式模板的测试")]
    public sealed class WordXmlTemplateTest
    {
        [Test(Description = "测试 Word 2003 Xml 的简单引用替换")]
        public void TestReferenceReplacement()
        {
            var ctx = new Dictionary<string, object>()
            {
                {"var1", "_HELLO_" },
                {"var2", "_WORLD_" },
            };

            var result = TemplateTestHelper.RenderTemplate<WordXmlDocument>(
                @"resources/word2003xml_docs/template_reference_replacement.xml", ctx);

            var xmldoc = TemplateTestHelper.GetlXmlDocument((WordXmlDocument)result);
      
            var body = xmldoc.GetElementsByTagName("w:body")[0];
            var bodyText = body.InnerText.Trim();

            Assert.AreEqual("TEST_HELLO_REFERENCE_WORLD_REPLACEMENT", bodyText);
        }

    }
}
