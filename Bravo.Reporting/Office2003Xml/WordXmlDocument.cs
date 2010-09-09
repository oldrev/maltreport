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
        private const string DestAttribute = "w:dest";
        private const string HlinkElement = "w:hlink";

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
            ProcessPlaceHolders(xml);


            WriteCompiledMainContent(t, xml);

            return t;
        }

        private static void WriteCompiledMainContent(WordXmlTemplate t, XmlDocument xml)
        {
            using (var ms = new MemoryStream())
            using (var writer = new XmlTextWriter(ms, Encoding.UTF8))
            {
                writer.Formatting = Formatting.None;
                xml.WriteTo(writer);
                writer.Flush();
                ms.Flush();
                t.PutBuffer(ms.ToArray());
            }
        }

        private static void ProcessPlaceHolders(XmlDocument xml)
        {
            var placeholders = FindAllPlaceholders(xml);

            foreach (XmlElement phe in placeholders)
            {
                var attr = phe.GetAttribute(DestAttribute);
                var value = attr.Substring(5).Trim('/', ' ');

                if (value.Length < 2)
                {
                    throw new SyntaxErrorException();
                }

                //TODO: 测试中文
                value = UrlUtility.UrlDecode(value, Encoding.UTF8);

                if (value[0] == '#')
                {
                    //ProcessStatementTag(xml, phe, value);
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

        private static List<XmlElement> FindAllPlaceholders(XmlNode doc)
        {
            var placeholders = new List<XmlElement>();
            var allNodes = doc.SelectNodes("//*");

            foreach (XmlElement e in allNodes)
            {
                if (e.Name == HlinkElement)
                {
                    var attr = e.GetAttribute(DestAttribute);
                    if (attr.StartsWith("rtl://", StringComparison.InvariantCulture))
                    {
                        placeholders.Add(e);
                    }
                }
            }

            return placeholders;
        }

        private static void ProcessReferenceTag(
            XmlDocument xml, XmlElement placeholderElement, string value)
        {
            //这里我们强制替换成 w:t 元素，因为 Word2003 xml 不支持图片
            var refEle = new Xml.ReferenceElement(xml, value);
            var rEle = xml.CreateElement("w:r", WordXmlNamespaceManager.WNamespace);
            var tEle = xml.CreateElement("w:t", WordXmlNamespaceManager.WNamespace);
            rEle.AppendChild(tEle);
            tEle.AppendChild(refEle);
            placeholderElement.ParentNode.ReplaceChild(rEle, placeholderElement);
        }
    }
}
