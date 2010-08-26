//作者：李维
//创建时间：2010-08-20


using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Bravo.Reporting.ReportNodes;

namespace Bravo.Reporting
{
    /// <summary>
    /// ODF 编译器
    /// 把用户创建的 ODF 文档中的 content.xml 转换为合适的 NVelocity 模板格式文件
    /// </summary>
    public class OdfTemplateCompiler : ITemplateCompiler
    {
        public const string PlaceHolderPattern = "//text:placeholder | //text:a[starts-with(@xlink:href, 'rtl://')]";

        #region ITemplateCompiler 成员

        public OdfDocument Compile(OdfDocument inputOdf)
        {
            var odfTemplate = new OdfDocument();
            inputOdf.CopyTo(odfTemplate);

            var xml = LoadXml(odfTemplate);
            var nsmanager = CreateContentNamespaceManager(xml);

            //第一遍，先处理简单的Tag 替换
            ClearTags(xml, nsmanager);

            //第二遍，处理表格循环
            ProcessTableRowNodes(xml, nsmanager);

            SaveXml(odfTemplate, xml);

            return odfTemplate;
        }

        private static XmlNamespaceManager CreateContentNamespaceManager(XmlDocument xml)
        {
            var nsmanager = new XmlNamespaceManager(xml.NameTable);
            nsmanager.AddNamespace("text", @"urn:oasis:names:tc:opendocument:xmlns:text:1.0");
            nsmanager.AddNamespace("table", @"urn:oasis:names:tc:opendocument:xmlns:table:1.0");
            nsmanager.AddNamespace("xlink", @"http://www.w3.org/1999/xlink");

            //注册编译器用到的命名空间
            nsmanager.AddNamespace("bravo", @"urn:bravo:reporting");
            return nsmanager;
        }

        private static void ProcessTableRowNodes(XmlDocument xml, XmlNamespaceManager nsmanager)
        {
            var rowNodes = xml.SelectNodes("//table:table-row", nsmanager);
            foreach (XmlNode row in rowNodes)
            {
                var rowStatementNodes = new List<XmlNode>();

                //检测一个行中的 table-cell 是否只包含 table:table-cell 和 bravo:statement 元素
                if (IsStatementRow(row))
                {
                    //把其中的 cell 都去掉
                    foreach (XmlNode subnode in row.ChildNodes)
                    {
                        if (subnode is StatementElement)
                        {
                            rowStatementNodes.Add(subnode);
                        }
                    }

                    if (row.ParentNode == null || row.ParentNode.Name != "table:table")
                    {
                        throw new TemplateException("Invalid template");
                    }

                    foreach (var sn in rowStatementNodes)
                    {
                        row.ParentNode.InsertAfter(sn, row);
                    }

                    row.ParentNode.RemoveChild(row);
                }
            }
        }

        /// <summary>
        /// 确定一个 table-row 下面只包含 table-cell 和 bravo-statement 元素
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private static bool IsStatementRow(XmlNode row)
        {
            foreach (XmlNode node in row.ChildNodes)
            {
                //一个 row 下面的子节点都必须是 bravo:statement 和 空的 celltable 而且必须是 #if #foreach #end 等
                if (!(
                    node is StatementElement ||
                    (node.InnerText == null || node.InnerText.Length <= 0)))
                {
                    return false;
                }
            }

            return true;
        }

        private static void ClearTags(XmlDocument xml, XmlNamespaceManager nsmanager)
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
                    ReduceStatementTag(statementNode, placeholder);
                }
                else
                {
                    throw new SyntaxErrorException();
                }
            }
        }

        private static void ProcessIdentifierTag(XmlDocument xml, XmlNode placeholder, string value)
        {
            placeholder.ParentNode.ReplaceChild(
                new IdentifierElement(xml, value), placeholder);
        }

        /// <summary>
        /// 化简语句 Tag
        /// </summary>
        /// <param name="newNode"></param>
        /// <param name="placeholder"></param>
        private static void ReduceStatementTag(XmlNode newNode, XmlNode placeholder)
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

        private static void SaveXml(OdfDocument odfTemplate, XmlDocument xml)
        {
            using (var cos = odfTemplate.GetEntryOutputStream(OdfDocument.ENTRY_CONTENT))
            using (var writer = new XmlTextWriter(cos, Encoding.UTF8))
            {
                xml.WriteTo(writer);
            }
        }

        private static XmlDocument LoadXml(OdfDocument odfTemplate)
        {
            var xml = new XmlDocument();
            using (var contentStream = odfTemplate.GetEntryInputStream(OdfDocument.ENTRY_CONTENT))
            {
                xml.Load(contentStream);
            }
            return xml;
        }

        #endregion
    }
}
