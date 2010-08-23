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

    public class OdtTemplateCompiler : ITemplateCompiler
    {
        #region ITemplateCompiler 成员

        public OdfArchive Compile(OdfArchive inputOdf)
        {
            var odfTemplate = new OdfArchive();
            inputOdf.CopyTo(odfTemplate);

            var xml = LoadXml(odfTemplate);
            var nsmanager = new XmlNamespaceManager(xml.NameTable);
            nsmanager.AddNamespace("text", "urn:oasis:names:tc:opendocument:xmlns:text:1.0");
            nsmanager.AddNamespace("table", "urn:oasis:names:tc:opendocument:xmlns:table:1.0");

            //第一遍，先处理简单的Tag 替换
            ClearTags(xml, nsmanager);

            //第二遍，处理表格循环
            ProcessRowNodes(xml, nsmanager);

            SaveXml(odfTemplate, xml);

            return odfTemplate;
        }

        private static void ProcessRowNodes(XmlDocument xml, XmlNamespaceManager nsmanager)
        {
            var cellNodes = xml.SelectNodes("//table:table-cell", nsmanager);
            foreach (XmlNode cellNode in cellNodes)
            {
                Console.WriteLine(cellNode.ParentNode.ChildNodes.Count);
                bool isDirectiveNode = IsDirectiveNode(cellNode);

                //如果一个 table-cell 下面的节点全部是 VTL statement，那么我们就把
                //该节点去掉
                if (isDirectiveNode)
                {
                }
            }
        }

        /// <summary>
        /// 确定一个节点的子节点是否全部是 VTL statement
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static bool IsDirectiveNode(XmlNode node)
        {
            bool result = false;
            foreach (XmlNode childNode in node.ChildNodes)
            {
                var sn = childNode as StatementElement;
                if (sn != null && (
                    sn.Directive == "#foreach" ||
                    sn.Directive == "#if" ||
                    sn.Directive == "#end"))
                {
                    result = true;
                }
            }
            return result;
        }

        private static void ClearTags(XmlDocument xml, XmlNamespaceManager nsmanager)
        {
            var tokenPattern = new Regex(@"<\s*(([\$#]\w+).*)\s*>$");
            var placeholders = xml.SelectNodes("//text:placeholder", nsmanager);
            foreach (XmlNode placeholder in placeholders)
            {
                var match = tokenPattern.Match(placeholder.InnerText);
                var value = match.Groups[1].Value;
                var directive = match.Groups[2].Value;

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
                    ProcessSimpleStatementTag(statementNode, placeholder);
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

        private static void ProcessSimpleStatementTag(XmlNode newNode, XmlNode placeholder)
        {
            //如果上级节点只包含 placeholder 这个节点的话，那么上级也是没用的
            //以此类推，直到上级节点包含其他类型的节点为止

            XmlNode ancestor = placeholder;
            while (ancestor.ParentNode.ChildNodes.Count == 1)
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
