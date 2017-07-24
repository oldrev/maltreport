using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;

namespace Sandwych.Reporting.OfficeML
{
    public class WordMLCompiler : ICompiler<WordMLDocument, WordMLTemplate>
    {
        public WordMLTemplate Compile(WordMLDocument templateDocument)
        {
            throw new NotImplementedException();
        }

        private static void ProcessPlaceholders(WordMLDocument doc)
        {
            var placeholders = FindAllPlaceholders(doc.XmlDocument);

            foreach (XmlElement phe in placeholders)
            {
                var attr = phe.GetAttribute(WordMLDocument.DestAttribute);
                var value = attr.Replace("dtl://", "").Trim('/', ' ');

                if (phe.HasAttribute(WordMLDocument.BookMarkElement))
                {
                    value = "#" + phe.GetAttribute(WordMLDocument.BookMarkElement);
                }

                value = UrlUtility.UrlDecode(value, Encoding.UTF8);

                if (value.Length < 2)
                {
                    throw new SyntaxErrorException();
                }

                if (value[0] == '#')
                {
                    ProcessDirectiveTag(doc.XmlDocument, phe, value);
                }
                else if (value[0] == '$')
                {
                    ProcessReferenceTag(doc.XmlDocument, phe, value);
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

            /*
            var directive = new DirectiveElement(xml, value);
            directive.ReduceTagByDirective(placeholderNode);
            */
        }

        private static List<XmlElement> FindAllPlaceholders(XmlDocument xml)
        {
            var placeholders = new List<XmlElement>();
            var allNodes = xml.GetElementsByTagName(WordMLDocument.HlinkElement);

            foreach (XmlElement e in allNodes)
            {
                var attr = e.GetAttribute(WordMLDocument.DestAttribute);
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
    }
}
