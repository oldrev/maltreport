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
    public class WordMLTemplate : AbstractXmlTemplate<IDocument>
    {
        private const string DestAttribute = "w:dest";
        private const string HlinkElement = "w:hlink";
        private const string BookMarkElement = "w:bookmark";

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

        private static void ProcessPlaceholders(XmlDocument xml)
        {
            var placeholders = FindAllPlaceholders(xml);

            foreach (XmlElement phe in placeholders)
            {
                var attr = phe.GetAttribute(DestAttribute);
                var value = attr.Replace("dtl://", "").Trim('/', ' ');

                if (phe.HasAttribute(BookMarkElement))
                {
                    value = "#" + phe.GetAttribute(BookMarkElement);
                }

                value = UrlUtility.UrlDecode(value, Encoding.UTF8);

                if (value.Length < 2)
                {
                    throw new SyntaxErrorException();
                }

                if (value[0] == '#')
                {
                    ProcessDirectiveTag(xml, phe, value);
                }
                else if (value[0] == '$')
                {
                    ProcessReferenceTag(xml, phe, value);
                }
                else
                {
                    throw new SyntaxErrorException(attr);
                }

            }
        }

        private static void ProcessDirectiveTag(
            XmlDocument xml, XmlElement placeholderNode, string value)
        {
            Debug.Assert(xml != null);
            Debug.Assert(placeholderNode != null);
            Debug.Assert(!string.IsNullOrEmpty(value));

            var directive = new DirectiveElement(xml, value);
            directive.ReduceTagByDirective(placeholderNode);
        }

        private static List<XmlElement> FindAllPlaceholders(XmlDocument xml)
        {
            var placeholders = new List<XmlElement>();
            var allNodes = xml.GetElementsByTagName(HlinkElement);

            foreach (XmlElement e in allNodes)
            {
                var attr = e.GetAttribute(DestAttribute);
                if (attr.StartsWith("dtl://", StringComparison.Ordinal))
                {
                    placeholders.Add(e);
                }
            }

            return placeholders;
        }

        private static void ProcessReferenceTag(
            XmlDocument xml, XmlElement placeholderElement, string value)
        {
            var refEle = new Xml.ReferenceElement(xml, value);
            var rEle = xml.CreateElement("w:r", WordMLNamespaceManager.WNamespace);
            var tEle = xml.CreateElement("w:t", WordMLNamespaceManager.WNamespace);
            rEle.AppendChild(tEle);
            tEle.AppendChild(refEle);
            placeholderElement.ParentNode.ReplaceChild(rEle, placeholderElement);
        }

        public override Task<IDocument> RenderAsync(TemplateContext context)
        {
            throw new NotImplementedException();
        }

        public override IDocument Render(TemplateContext context)
        {
            throw new NotImplementedException();
        }
    }
}
