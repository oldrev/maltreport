//作者：李维
//创建时间：2010-09-09

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace Bravo.Reporting.Office2003Xml
{
    public class WordXmlDocument : SingleXmlDocumentBase
    {
        public WordXmlDocument()
        {
        }

        public override ITemplate Compile()
        {
            var t = new WordXmlTemplate();
            t.LoadFromDocument(this);
            var xml = t.GetXmlDocument();

            var nsmanager = new WordXmlNamespaceManager(xml.NameTable);
            nsmanager.LoadOpenDocumentNamespaces();

            //TODO: 这里执行编译
            WriteCompiledMainContent(t, xml);

            return t;
        }

        private static void WriteCompiledMainContent(WordXmlTemplate t, XmlDocument xml)
        {
            using (var ms = new MemoryStream())
            using (var sw = new StreamWriter(ms))
            using (var writer = new XmlTextWriter(sw))
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
