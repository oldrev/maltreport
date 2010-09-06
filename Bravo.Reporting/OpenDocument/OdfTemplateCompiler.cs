//作者：李维
//创建时间：2010-08-20
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Bravo.Reporting.Xml;

namespace Bravo.Reporting.OpenDocument
{
    /// <summary>
    /// ODF 模板编译器
    /// 把用户创建的 ODF 文档中的 content.xml 转换为合适的 NVelocity 模板格式文件
    /// </summary>
    internal class OdfTemplateCompiler 
    {
        public const string PlaceHolderPattern =
            @"//text:placeholder | //text:a[starts-with(@xlink:href, 'rtl://')]";

        public ITemplate Compile(IDocument doc)
        {
            if (doc.GetType() != typeof(OdfDocument))
            {
                throw new ArgumentException("只支持编译 OdfDocument 类型", "doc");
            }

            var t = new OdfTemplate();
            doc.CopyTo(t);

            var xml = t.ReadMainContentXml();
            var nsmanager = new OdfNamespaceManager(xml.NameTable);
            nsmanager.LoadOpenDocumentNamespaces();

            //第1遍，先处理简单的Tag 替换
            ClearTextTags(xml, nsmanager);

            //第2遍，处理表格循环
            ProcessTableRowNodes(xml, nsmanager);

            //把编译后的 XmlDocument 写入
            using (var cos = t.GetEntryOutputStream(t.MainContentEntryPath))
            using (var writer = new VelocityEscapedXmlTextWriter(cos))
            {
                writer.Formatting = Formatting.Indented; //对于 Velocity 模板，最好格式化
                xml.WriteTo(writer);
            }

            return t;
        }

        private static void ProcessTableRowNodes(XmlDocument xml, XmlNamespaceManager nsmanager)
        {
            var rowNodes = xml.SelectNodes("//table:table-row", nsmanager);
            var rowStatementNodes = new List<StatementElement>(5);
            foreach (XmlElement row in rowNodes)
            {
                rowStatementNodes.Clear();

                //检测一个行中的 table-cell 是否只包含 table:table-cell 和 report-statement 元素
                //把其中的 cell 都去掉
                foreach (XmlElement subnode in row.ChildNodes)
                {
                    var se = subnode as StatementElement;
                    if (se != null)
                    {
                        rowStatementNodes.Add(se);
                    }
                }

                if (rowStatementNodes.Count == 1)
                {
                    row.ParentNode.ReplaceChild(rowStatementNodes[0], row);
                }
            }
        }

        private static void ClearTextTags(XmlDocument xml, XmlNamespaceManager nsmanager)
        {
            var placeHolderPattern = new Regex(@"<\s*(([\$#]\w+).*)\s*>$");
            var linkPattern = new Regex(@"^rtl://(([\$#]\w+).*)\s*$");
            var placeholders = xml.SelectNodes(PlaceHolderPattern, nsmanager);
            foreach (XmlNode placeholder in placeholders)
            {
                string value = null;
                string directive = null;

                Match match = null;

                if (placeholder.Name == "text:placeholder")
                {
                    match = placeHolderPattern.Match(placeholder.InnerText);
                }
                else
                {
                    var href = placeholder.Attributes["xlink:href"].Value;
                    match = linkPattern.Match(Uri.UnescapeDataString(href));
                }

                value = match.Groups[1].Value;
                directive = match.Groups[2].Value;

                if (match.Groups.Count != 3)
                {
                    throw new SyntaxErrorException("Syntax Error: " + placeholder.InnerText);
                }

                if (value.Length < 1)
                {
                    throw new SyntaxErrorException();
                }

                if (value[0] == '$')
                {
                    ProcessIdentifierTag(xml, placeholder, value);

                }
                else if (value[0] == '#')
                {
                    var statementNode = new StatementElement(xml, value, directive);
                    ReduceTag(statementNode, placeholder);
                }
                else
                {
                    throw new SyntaxErrorException();
                }
            }
        }

        private static void ProcessIdentifierTag(XmlDocument xml, XmlNode placeholder, string value)
        {
            Debug.Assert(xml != null);
            Debug.Assert(placeholder != null);
            Debug.Assert(!string.IsNullOrEmpty(value));

            var ie = new ReferenceElement(xml, value);

            if (placeholder.Name == "text:placeholder")
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
            var placeholderType = placeholder.Attributes["text:placeholder-type"]
                .InnerText.Trim().ToLowerInvariant(); ;
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
            var drawboxNode = LookupAncestor(placeholder, "draw:text-box");
            if (drawboxNode.Name != "draw:text-box")
            {
                throw new SyntaxErrorException("图像类型的占位符必须放在图文框中");
            }

            drawboxNode.ParentNode.ReplaceChild(ie, drawboxNode);
        }

        /// <summary>
        /// 化简 Tag
        /// </summary>
        /// <param name="newNode"></param>
        /// <param name="placeholder"></param>
        private static void ReduceTag(XmlNode newNode, XmlNode placeholder)
        {
            //如果上级节点只包含 placeholder 这个节点的话，那么上级也是没用的
            //以此类推，直到上级节点包含其他类型的节点或者上级节点是单元格为止

            XmlNode ancestor = placeholder;
            while (ancestor.ParentNode.ChildNodes.Count == 1)
            {
                ancestor = ancestor.ParentNode;
            }

            ancestor.ParentNode.ReplaceChild(newNode, ancestor);
        }

        /// <summary>
        /// 查找祖先元素
        /// </summary>
        /// <param name="ancestorName"></param>
        /// <param name="node"></param>
        private static XmlNode LookupAncestor(XmlNode node, string ancestorName)
        {
            XmlNode ancestor = node;
            while (ancestor.ParentNode.ChildNodes.Count == 1 &&
                ancestor.Name != ancestorName)
            {
                ancestor = ancestor.ParentNode;
            }

            return ancestor;
        }
    }
}
