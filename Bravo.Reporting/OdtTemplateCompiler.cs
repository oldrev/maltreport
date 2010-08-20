using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;
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

            //这里把 ODF 模板转换为  StringTemplate
            var placeholders = xml.SelectNodes("//text:placeholder", nsmanager);
            foreach (XmlNode placeholder in placeholders)
            {
                var value = placeholder.InnerText.Trim();
                if (value.Length >= 2 && value[0] == '<')
                {
                    value = value.Substring(1, value.Length - 2).Trim();
                }

                if (value.Length < 1)
                {
                    throw new SyntaxErrorException();
                }

                var tokens = value.Split(' ', '\t');
                switch (tokens[0])
                {
                    case "foreach":
                        var startNode = new ForeachStartElement(xml, value);
                        ReplacePlaceHolderNode(startNode, placeholder);
                        break;

                    case "end":
                        var endNode = new ForeachEndElement(xml);
                        ReplacePlaceHolderNode(endNode, placeholder);
                        break;

                    default:
                        ProcessPlaceHolderNode(xml, placeholder, value);
                        break;

                }
            }

            SaveXml(odfTemplate, xml);

            return odfTemplate;
        }

        private static void ProcessPlaceHolderNode(XmlDocument xml, XmlNode placeholder, string value)
        {
            placeholder.ParentNode.ReplaceChild(
                new PlaceHolderElement(xml, value), placeholder);
        }

        private static void ReplacePlaceHolderNode(XmlNode newNode, XmlNode placeholder)
        {
            //如果上级节点只包含 placeholder 这个节点的话，那么上级也是没用的
            //以此类推，直到上级节点包含其他类型的节点为止

            XmlNode childNode = placeholder;
            while (childNode.ParentNode.ChildNodes.Count == 1)
            {
                childNode = childNode.ParentNode;
            }

            //如果一个 foreach 放在 table-cell 里，那么应该把 foreach 所在的行都替换掉
            //这样只能执行垂直循环，以后考虑水平循环

            if (childNode.Name == "table:table-cell")
            {
                while (childNode.Name != "table:table-row")
                {
                    childNode = childNode.ParentNode;
                }
            }

            childNode.ParentNode.ReplaceChild(
                newNode, childNode);
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
