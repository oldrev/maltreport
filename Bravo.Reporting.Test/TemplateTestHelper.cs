using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.IO;

using NUnit.Framework;

using Bravo.Reporting.OpenDocument;

namespace Bravo.Reporting.Test
{
    public static class TemplateTestHelper
    {
        /// <summary>
        /// 一步执行模板编译、渲染并返回结果
        /// </summary>
        /// <param name="odfPath"></param>
        /// <returns></returns>
        public static T RenderTemplate<T>(string tmpPath, IDictionary<string, object> context)
            where T : IDocument, new()
        {
            var t = new T();
            t.Load(tmpPath);
            return (T)t.Compile().Render(context);
        }

        public static XmlDocument GetlXmlDocument<T>(T singleXmlDoc)
            where T : IDocument
        {
            var xmldoc = new XmlDocument();
            using (var ms = new MemoryStream(singleXmlDoc.GetBuffer()))
            {
                xmldoc.Load(ms);
            }

            return xmldoc;
        }

        public static void AddXmlSchema(XmlDocument xml,
            ValidationEventHandler validationEventHandler, string xsdFilePath)
        {
            XmlSchema wordnetSchema = null;
            using (var sr = new StreamReader(xsdFilePath))
            {
                wordnetSchema = XmlSchema.Read(sr, validationEventHandler);
            }
            xml.Schemas.Add(wordnetSchema);
        }

        public static void AssertValidXmlDocument(
            XmlDocument xml, IEnumerable<string> xsdFiles)
        {
            xml.Schemas = new XmlSchemaSet(xml.NameTable);

            var errors = 0;
            var warnings = 0;

            ValidationEventHandler validationEventHandler =
                (object sender, ValidationEventArgs e) =>
                {
                    if (e.Severity == XmlSeverityType.Error)
                    {
                        errors++;
                    }
                    else
                    {
                        warnings++;
                    }
                    Console.WriteLine("XSD Validation Message: [{0}]{1}", e.Severity, e.Message);
                };

            foreach (var xsdFile in xsdFiles)
            {
                TemplateTestHelper.AddXmlSchema(xml, validationEventHandler, xsdFile);
            }

            Assert.AreEqual(0, errors);
            Assert.AreEqual(0, warnings);
        }
    }
}
