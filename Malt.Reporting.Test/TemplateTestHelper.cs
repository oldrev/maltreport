using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.IO;

using Commons.Xml.Relaxng;
using NUnit.Framework;

using Malt.Reporting;
using Malt.Reporting.OpenDocument;

namespace Malt.Reporting.Test
{
    public static class TemplateTestHelper
    {
        /// <summary>
        /// 一步执行模板编译、渲染并返回结果
        /// </summary>
        /// <param name="odfPath"></param>
        /// <returns></returns>
        public static IDocument RenderTemplate<T>(string tmpPath, IDictionary<string, object> context)
            where T : ITemplate, new()
        {
            var t = new T();
            t.Load(tmpPath);
            t.Compile();
            var resultDoc = t.Render(context);
            return resultDoc;
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

        private static void AddXmlSchema(XmlDocument xml,
            ValidationEventHandler validationEventHandler, string xsdFilePath)
        {
            XmlSchema wordnetSchema = null;
            using (var sr = new StreamReader(xsdFilePath))
            {
                wordnetSchema = XmlSchema.Read(sr, validationEventHandler);
            }
            xml.Schemas.Add(wordnetSchema);
        }

        public static void ShouldWellFormed(
            this XmlDocument xml, IEnumerable<string> xsdFiles)
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
                AddXmlSchema(xml, validationEventHandler, xsdFile);
            }

            Assert.AreEqual(0, errors);
            Assert.AreEqual(0, warnings);
        }

        public static void ShouldWellFormed(this Stream xmlStream, string rngFile)
        {
            Assert.IsTrue(RelaxngValidate(xmlStream, rngFile));
        }

        private static bool RelaxngValidate(Stream xmlStream, string rngFile)
        {
            // Grammar.
            RelaxngPattern p = null;
            using (XmlTextReader xtrRng = new XmlTextReader(rngFile))
            {
                p = RelaxngPattern.Read(xtrRng);
                p.Compile();
            }

            // Validate instance.
            using (XmlTextReader xtrXml = new XmlTextReader(xmlStream))
            using (RelaxngValidatingReader vr = new RelaxngValidatingReader(xtrXml, p))
            {
                try
                {
                    while (!vr.EOF)
                    {
                        vr.Read();
                    }
                    // XML file is valid.
                    return true;
                }
                catch (RelaxngException rex)
                {
                    // XML file not valid.
                    Console.WriteLine("RELAX NG Validation: " + rex.Message);
                    return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Bad XML file: " + ex.Message);
                    return false;
                }
            }
        }
    }
}
