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
    /* 支持一下几个语法
     * 
     * placeholder
     * 
     * foreach item in items
     * end
     * 
     * if xxxxx then
     * elseif xxxxxxxx
     * else
     * end
     * 
     * 
     * */

    public class OdfTemplateCompiler : ITemplateCompiler
    {
        public const string PlaceHolderPattern = "//text:placeholder | //text:a[starts-with(@xlink:href, 'rtl://')]";

        #region ITemplateCompiler 成员

        public OdfArchive Compile(OdfArchive inputOdf)
        {
            var odfTemplate = new OdfArchive();
            inputOdf.CopyTo(odfTemplate);

            var xml = LoadXml(odfTemplate);
            var nsmanager = new XmlNamespaceManager(xml.NameTable);
            nsmanager.AddNamespace("text", @"urn:oasis:names:tc:opendocument:xmlns:text:1.0");
            nsmanager.AddNamespace("table", @"urn:oasis:names:tc:opendocument:xmlns:table:1.0");
            nsmanager.AddNamespace("xlink", @"http://www.w3.org/1999/xlink");
            nsmanager.AddNamespace("bravo", @"urn:bravo:reporting");

            //第一遍，先处理简单的Tag 替换
            ClearTags(xml, nsmanager);

            //第二遍，处理表格循环
            ProcessRowNodes(xml, nsmanager);

            SaveXml(odfTemplate, xml);

            return odfTemplate;
        }

        private static void ProcessRowNodes(XmlDocument xml, XmlNamespaceManager nsmanager)
        {
            var statementNodes = xml.SelectNodes("//bravo:statement", nsmanager);
            foreach (XmlNode node in statementNodes)
            {
                var statementNode = node as StatementElement;
                var parent = node.ParentNode;

                //如果一行里面有一个单元格只有 opening_tag 一个元素，那么可以认为该行是一个
                //指令行，可以替换掉
                if (parent.Name == "table:table-cell" &&
                    parent.ChildNodes.Count == 1 &&
                    statementNode.IsOpeningOrClosingTag)
                {
                    var rowNode = parent.ParentNode;
                    rowNode.ParentNode.ReplaceChild(node, rowNode);
                }
            }
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
            while (ancestor.ParentNode.ChildNodes.Count == 1 &&
                ancestor.ParentNode.Name != "table:table-cell")
            {
                ancestor = ancestor.ParentNode;
            }

            ancestor.ParentNode.ReplaceChild(newNode, ancestor);
        }

        private static void SaveXml(OdfArchive odfTemplate, XmlDocument xml)
        {
            using (var cos = odfTemplate.GetContentOutputStream(OdfArchive.ENTRY_CONTENT))
            using (var writer = new XmlTextWriter(cos, Encoding.UTF8))
            {
                xml.WriteTo(writer);
            }
        }

        private static XmlDocument LoadXml(OdfArchive odfTemplate)
        {
            var xml = new XmlDocument();
            using (var contentStream = odfTemplate.GetContentInputStream(OdfArchive.ENTRY_CONTENT))
            {
                xml.Load(contentStream);
            }
            return xml;
        }

        #endregion
    }
}
