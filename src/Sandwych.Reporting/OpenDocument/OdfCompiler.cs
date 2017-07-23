using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using System.Diagnostics;

using Sandwych.Reporting.Xml;
using System.Linq;
using Sandwych.Reporting.OpenDocument.Filters;

namespace Sandwych.Reporting.OpenDocument
{
    internal class OdfCompiler
    {
        private const string DtlProtocolPrefix = "dtl://";

        private static readonly Lazy<Regex> PlaceHolderValuePattern =
            new Lazy<Regex>(() => new Regex(@"^<\s*(.*)\s*>$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline), true);

        private static readonly Lazy<Regex> HyperLinkValuePattern =
            new Lazy<Regex>(() => new Regex(@"^dtl://(.*)\s*$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline), true);

        private static readonly Lazy<Regex> ImageBoxPattern =
            new Lazy<Regex>(() => new Regex(@".*\{\{.*\s*\|\s*image\s*\}\}.*", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline), true);

        public static void Compile(OdfDocument template)
        {
            var xml = template.ReadMainContentXml();

            // First pass, process all simple tags
            PreprocessElements(xml);

            // Second pass, process all looping table things
            ProcessTableRows(xml);

            template.WriteMainContentXml(xml);
        }

        private static void ProcessTableRows(OdfContentXmlDocument xml)
        {
            var rowElements = FindAllRowElements(xml).ToArray();

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

            if (rowDirectiveElements.Count() == 1)
            {
                row.ParentNode.ReplaceChild(rowDirectiveElements.First(), row);
            }
        }

        private static IEnumerable<XmlElement> FindAllRowElements(OdfContentXmlDocument xml)
        {
            var nodeList = xml.GetElementsByTagName(OdfDocument.TableRowElement);

            foreach (XmlElement rowEle in nodeList)
            {
                yield return rowEle;
            }
        }

        private static IEnumerable<DirectiveElement> FindDirectiveNodesInRow(XmlElement row)
        {
            foreach (XmlElement subnode in row.ChildNodes)
            {
                var se = subnode as DirectiveElement;
                if (se != null)
                {
                    yield return se;
                }
            }
        }

        private static void PreprocessElements(OdfContentXmlDocument xml)
        {
            var placeholders = FindAllPlaceholderElements(xml).ToArray();

            foreach (XmlNode placeholder in placeholders)
            {
                string value = ExtractTemplateExpression(placeholder);

                if (value.StartsWith("{{") && value.EndsWith("}}"))
                {
                    ProcessIdentifierTag(xml, placeholder, value);

                }
                else if (value.StartsWith("{%") && value.EndsWith("%}"))
                {
                    var directiveNode = new DirectiveElement(xml, value);
                    directiveNode.ReduceTagByCount(placeholder);
                }
                else
                {
                    throw new SyntaxErrorException();
                }
            }

            //处理 draw:frame 包含 draw:image 的情况
            var drawFrameElements = xml.SelectNodes("//" + OdfDocument.DrawFrameElement, xml.NamespaceManager);
            var dtlDrawFrames = new List<XmlNode>();
            foreach (XmlNode node in drawFrameElements)
            {
                var nameAttr = node.Attributes["draw:name"];
                if (nameAttr != null && !string.IsNullOrWhiteSpace(nameAttr.Value) && nameAttr.Value.Trim().StartsWith(DtlProtocolPrefix))
                {
                    dtlDrawFrames.Add(node);
                }
            }

            foreach (var node in dtlDrawFrames)
            {
                ProcessDrawFrameElement(node, xml.NamespaceManager);
            }

        }

        private static IEnumerable<XmlElement> FindAllPlaceholderElements(OdfContentXmlDocument xml)
        {
            var textPlaceholders = xml.GetElementsByTagName(OdfDocument.TextPlaceholderElement);
            foreach (XmlElement tpe in textPlaceholders)
            {
                yield return tpe;
            }

            var textAnchors = xml.GetElementsByTagName(OdfDocument.TextAnchorElement);
            foreach (XmlElement ta in textAnchors)
            {
                var href = ta.GetAttribute("xlink:href");
                if (href != null && href.Trim().StartsWith(DtlProtocolPrefix))
                {
                    yield return ta;
                }
            }
        }

        private static string ExtractTemplateExpression(XmlNode placeholder)
        {
            string value = null;

            Match match = null;

            if (placeholder.Name == "text:placeholder")
            {
                match = PlaceHolderValuePattern.Value.Match(placeholder.InnerText);
            }
            else
            {
                var href = placeholder.Attributes["xlink:href"].Value;
                match = HyperLinkValuePattern.Value.Match(Uri.UnescapeDataString(href));
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

        private static void ProcessIdentifierTag(OdfContentXmlDocument xml, XmlNode placeholder, string value)
        {
            Debug.Assert(xml != null);
            Debug.Assert(placeholder != null);
            Debug.Assert(!string.IsNullOrEmpty(value));

            var ie = new ReferenceElement(xml, value);

            if (placeholder.Name == OdfDocument.TextPlaceholderElement)
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
            var placeholderType = placeholder.Attributes[OdfDocument.TextPlaceholderTypeAttribute]
                .InnerText.Trim().ToLowerInvariant();
            ;
            switch (placeholderType)
            {
                case "text":
                    placeholder.ParentNode.ReplaceChild(ie, placeholder);
                    break;

                default:
                    throw new SyntaxErrorException("Unsupported placeholder type: " + placeholderType);
            }
        }

        private static void ProcessDrawFrameElement(XmlNode drawFrameNode, XmlNamespaceManager nsmanager)
        {
            if (drawFrameNode.Name != OdfDocument.DrawFrameElement)
            {
                throw new InvalidOperationException();
            }

            var nameAttr = drawFrameNode.Attributes["draw:name"];
            var drawImageNode = drawFrameNode.SelectSingleNode("//draw:image", nsmanager);
            drawFrameNode.RemoveChild(drawImageNode);
            var userExpr = nameAttr.Value.Trim().Substring(DtlProtocolPrefix.Length);
            var fluidExpr = "{{ " + userExpr + " }}";
            drawFrameNode.InnerText = fluidExpr;
        }

    }
}
