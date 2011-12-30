//作者：李维
//创建时间：2010-09-03
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;

using Malt.Reporting.Xml;

namespace Malt.Reporting.OfficeXml
{
    internal static class ExcelMLCompiler
    {
        private const string HRefAttribute = "ss:HRef";
        private static readonly Dictionary<string, IXmlNodeProcessor> xmlNodeProcessors;

        static ExcelMLCompiler()
        {
            xmlNodeProcessors = new Dictionary<string, IXmlNodeProcessor>()
            {
                { "Table", new ExcelMLTableNodeProcessor() },
                { "Row", new ExcelMLlRowNodeProcessor() },
                { "Column", new ExcelMLColumnNodeProcessor() },
                { "Cell", new ExcelMLCellNodeProcessor() },
            };

        }

        public static void Compile(ExcelMLTemplate doc)
        {
            if (doc == null)
            {
                throw new ArgumentNullException("template");
            }

            var xml = doc.GetXmlDocument();

            var nsmanager = new ExcelMLNamespaceManager(xml.NameTable);
            nsmanager.LoadOpenDocumentNamespaces();

            ClearTemplate(xml);

            ProcessPlaceHolders(xml);

            WriteCompiledMainContent(doc, xml);
        }

        private static void WriteCompiledMainContent(ExcelMLTemplate t, XmlDocument xml)
        {
            Debug.Assert(t != null);
            Debug.Assert(xml != null);

            using (var ms = new MemoryStream())
            using (var writer = new ExcelMLTextWriter(ms))
            {
                writer.Formatting = Formatting.None; //对于 Velocity 模板，最好格式化
                xml.WriteTo(writer);
                writer.Flush();
                ms.Flush();
                t.PutBuffer(ms.ToArray());
            }
        }

        private static void ProcessPlaceHolders(XmlDocument xml)
        {
            Debug.Assert(xml != null);

            var workbookNode = FindFirstChildNode(xml, "Workbook");

            if (workbookNode == null)
            {
                throw new TemplateException("Invalid document format of Excel 2003 Xml");
            }

            var placeholders = FindAllPlaceholders(xml);

            foreach (XmlElement phe in placeholders)
            {
                var attr = phe.GetAttribute(HRefAttribute);
                var value = attr.Substring(5).Trim('/', ' ');

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
                    ProcessReferenceTag(phe, value);
                }
                else
                {
                    throw new SyntaxErrorException(attr);
                }

            }
        }

        private static void ClearTemplate(XmlNode node)
        {
            Debug.Assert(node != null);

            IXmlNodeProcessor visitor = null;
            if (xmlNodeProcessors.TryGetValue(node.Name, out visitor))
            {
                visitor.ProcessNode(node);
            }

            foreach (XmlNode child in node.ChildNodes)
            {
                ClearTemplate(child);
            }

        }

        private static void ProcessReferenceTag(XmlElement phe, string value)
        {
            Debug.Assert(phe != null);
            Debug.Assert(!string.IsNullOrEmpty(value));

            phe.RemoveAttribute(HRefAttribute);
            phe.InnerText = value;
        }

        private static void ProcessDirectiveTag(XmlDocument xml, XmlElement phe, string value)
        {
            Debug.Assert(xml != null);
            Debug.Assert(phe != null);
            Debug.Assert(!string.IsNullOrEmpty(value));

            var se = new DirectiveElement(xml, value);
            if (phe.ParentNode.ChildNodes.Count == 1)
            {
                phe.ParentNode.ParentNode.ReplaceChild(se, phe.ParentNode);
            }
            else
            {
                phe.ParentNode.ReplaceChild(se, phe);
            }
        }

        private static List<XmlElement> FindAllPlaceholders(XmlDocument doc)
        {
            Debug.Assert(doc != null);

            var placeholders = new List<XmlElement>(50);
            var allNodes = doc.GetElementsByTagName("Cell");

            foreach (XmlElement e in allNodes)
            {
                if (e.HasAttribute(HRefAttribute))
                {
                    var attr = e.GetAttribute(HRefAttribute);
                    if (attr.StartsWith("rtl://", StringComparison.Ordinal))
                    {
                        placeholders.Add(e);
                    }
                }
            }

            return placeholders;
        }

        private static XmlNode FindFirstChildNode(XmlNode parent, string childName)
        {
            Debug.Assert(parent != null);
            Debug.Assert(!string.IsNullOrEmpty(childName));
            foreach (XmlNode n in parent.ChildNodes)
            {
                if (n.Name == childName)
                {
                    return n;
                }
            }

            return null;
        }

    }
}
