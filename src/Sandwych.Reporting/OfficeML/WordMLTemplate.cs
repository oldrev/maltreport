using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Threading.Tasks;
using Sandwych.Reporting.Xml;

namespace Sandwych.Reporting.OfficeML
{
    public class WordMLTemplate : AbstractXmlTemplate<WordMLDocument>
    {
        public WordMLTemplate()
        {
        }

        private void Compile()
        {
            /*
            var nsmanager = new WordMLNamespaceManager(xml.NameTable);
            nsmanager.LoadOpenDocumentNamespaces();

            ProcessPlaceholders(xml);

            WriteCompiledMainContent(this, xml);
            */
        }

        private static void WriteCompiledMainContent(WordMLTemplate t, XmlDocument xml)
        {
            throw new NotImplementedException();
            using (var ms = new MemoryStream())
            using (var writer = XmlWriter.Create(ms))
            {
                xml.WriteTo(writer);
                writer.Flush();
                ms.Flush();
            }
        }


        public override Task<WordMLDocument> RenderAsync(TemplateContext context)
        {
            throw new NotImplementedException();
        }

        public override WordMLDocument Render(TemplateContext context)
        {
            throw new NotImplementedException();
        }
    }
}
