using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace Bravo.Reporting.Office2003Xml
{
    internal static class TemplateExtensions
    {

        public static void WriteCompiledMainContent<T>(T t, XmlDocument xml)
            where T : SingleXmlDocumentBase
        {
            using (var ms = new MemoryStream())
            using (var writer = new TemplateXmlTextWriter(ms))
            {
                writer.Formatting = Formatting.Indented; //对于 Velocity 模板，最好格式化
                xml.WriteTo(writer);
                writer.Flush();
                ms.Flush();
                t.PutBuffer(ms.GetBuffer());
            }
        }
    }
}
