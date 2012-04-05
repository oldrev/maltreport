using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using System.Diagnostics;

using Sandwych.Reporting.Xml;

namespace Sandwych.Reporting.OpenDocument
{
    public class OdfCompiler
    {
        private const string TextPlaceholderElement = @"text:placeholder";
        private const string DrawTextBoxElement = @"draw:text-box";
        private const string TextAnchorElement = @"text:a";
        private const string TextPlaceholderTypeAttribute = @"text:placeholder-type";
        private const string TableRowElement = @"table:table-row";
        private static readonly Regex PlaceHolderValuePattern =
            new Regex(@"^<\s*(.*)\s*>$");
        private static readonly Regex HyperLinkValuePattern =
            new Regex(@"^rtl://(.*)\s*$");

        public static void Compile(OdfTemplate template)
        {
            var xml = new XmlDocument();
            template.ReadMainContentXml(xml);
            var nsmanager = new OdfNamespaceManager(xml.NameTable);
            nsmanager.LoadOpenDocumentNamespaces();

            //第1遍，先处理简单的Tag 替换
            PreprocessElements(xml, nsmanager);

            //第2遍，处理表格循环
            ProcessTableRows(xml, nsmanager);

            template.WriteXmlContent(xml);
        }

        #region Compiler Methods

        private static void ProcessTableRows(XmlDocument xml, XmlNamespaceManager nsmanager)
        {
            var rowElements = FindAllRowElements(xml);

            foreach (XmlElement row in rowElements)
            {
                //检测一个行中的 table-cell 是否只包含唯一的 report-directive 元素
                //把其中的 cell 都去掉
                ProcessSingleTableRowElement(row);
            }
        }

        private static void ProcessSingleTableRowElement(XmlElement row)
        {
            var rowDirectiveElements = FindDirectiveNodesInRow(row);

            if (rowDirectiveElements.Count == 1)
            {
                row.ParentNode.ReplaceChild(rowDirectiveElements[0], row);
            }
        }

        private static List<XmlElement> FindAllRowElements(XmlDocument xml)
        {
            var nodeList = xml.GetElementsByTagName(TableRowElement);
            var rowNodes = new List<XmlElement>();

            foreach (XmlElement rowEle in nodeList)
            {
                rowNodes.Add(rowEle);
            }
            return rowNodes;
        }

        private static List<DirectiveElement> FindDirectiveNodesInRow(
            XmlElement row)
        {
            var rowDirectiveNodes = new List<DirectiveElement>(50);
            foreach (XmlElement subnode in row.ChildNodes)
            {
                var se = subnode as DirectiveElement;
                if (se != null)
                {
                    rowDirectiveNodes.Add(se);
                }
            }
            return rowDirectiveNodes;
        }

        private static void PreprocessElements(XmlDocument xml, XmlNamespaceManager nsmanager)
        {
            var placeholders = FindAllRtlElements(xml);

            foreach (XmlNode placeholder in placeholders)
            {
                string value = ExtractTemplateExpression(placeholder);

                if (value[0] == '$')
                {
                    ProcessIdentifierTag(xml, placeholder, value);

                }
                else if (value[0] == '#')
                {
                    var directiveNode = new DirectiveElement(xml, value);
                    directiveNode.ReduceTagByCount(placeholder);                        
                    //ReduceTag(directiveNode, placeholder);
                }
                else
                {
                    throw new SyntaxErrorException();
                }
            }
        }

        private static List<XmlElement> FindAllRtlElements(XmlDocument xml)
        {
            var placeholders = new List<XmlElement>();

            var textPlaceholders = xml.GetElementsByTagName(TextPlaceholderElement);
            foreach (XmlElement tpe in textPlaceholders)
            {
                placeholders.Add(tpe);
            }

            var textAnchors = xml.GetElementsByTagName(TextAnchorElement);
            foreach (XmlElement ta in textAnchors)
            {
                var href = ta.GetAttribute("xlink:href");
                if (href != null && href.Trim().StartsWith("rtl://", StringComparison.Ordinal))
                {
                    placeholders.Add(ta);
                }
            }
            return placeholders;
        }

        private static string ExtractTemplateExpression(XmlNode placeholder)
        {
            string value = null;

            Match match = null;

            if (placeholder.Name == "text:placeholder")
            {
                match = PlaceHolderValuePattern.Match(placeholder.InnerText);
            }
            else
            {
                var href = placeholder.Attributes["xlink:href"].Value;
                match = HyperLinkValuePattern.Match(Uri.UnescapeDataString(href));
            }

            value = match.Groups[1].Value;
            CheckTemplateExpression(placeholder, value, match);

            return value;
        }

        private static void CheckTemplateExpression(XmlNode placeholder, string value, Match match)
        {

            if (match.Groups.Count != 2)
            {
                throw new SyntaxErrorException("Syntax Error: " + placeholder.InnerText);
            }

            if (value.Length < 1)
            {
                throw new SyntaxErrorException();
            }
        }

        private static void ProcessIdentifierTag(XmlDocument xml, XmlNode placeholder, string value)
        {
            Debug.Assert(xml != null);
            Debug.Assert(placeholder != null);
            Debug.Assert(!string.IsNullOrEmpty(value));

            var ie = new ReferenceElement(xml, value);

            if (placeholder.Name == TextPlaceholderElement)
            {
                ProcessPlaceHolderElement(placeholder, ie);
            }
            else
            {
                placeholder.ParentNode.ReplaceChild(ie, placeholder);
            }
        }

        private static void ProcessPlaceHolderElement(XmlNode placeholder, ReferenceElement ie)
        {
            var placeholderType = placeholder.Attributes[TextPlaceholderTypeAttribute]
                .InnerText.Trim().ToLowerInvariant();
            ;
            //处理图像占位符

            switch (placeholderType)
            {
                case "image":
                    ProcessImageTag(placeholder, ie);
                    break;

                case "text":
                    placeholder.ParentNode.ReplaceChild(ie, placeholder);
                    break;

                default:
                    throw new SyntaxErrorException("Unsupported placeholder type: " + placeholderType);
            }
        }

        private static void ProcessImageTag(XmlNode placeholder, ReferenceElement ie)
        {
            Debug.Assert(placeholder != null);
            Debug.Assert(ie != null);

            //向上查找 drawbox
            var drawboxNode = placeholder.LookupAncestor(DrawTextBoxElement);
            if (drawboxNode.Name != DrawTextBoxElement)
            {
                throw new TemplateException("The placeholder of image must be in a 'frame'");
            }

            drawboxNode.ParentNode.ReplaceChild(ie, drawboxNode);
        }

    

        #endregion
    }
}
