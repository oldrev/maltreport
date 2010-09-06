//作者：李维
//创建时间：2010-09-03
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Bravo.Reporting.Xml;

namespace Bravo.Reporting.Excel2003Xml
{
    /// <summary>
    /// ODF 模板编译器
    /// 把用户创建的 ODF 文档中的 content.xml 转换为合适的 NVelocity 模板格式文件
    /// </summary>
    public class ExcelXmlTemplateCompiler : ITemplateCompiler
    {
        private const string HRefAttribute = "ss:HRef";

        private static readonly Dictionary<string, IXmlNodeVisitor> visitors =
            new Dictionary<string, IXmlNodeVisitor>()
            {
                { "Table", new TableNodeVisitor() },
                { "Row", new RowNodeVisitor() },
                { "Column", new ColumnNodeVisitor() },
                { "Cell", new CellNodeVisitor() },
                //{ "NumberFormat", new NumberFormatNodeVisitor() },

            };

        public ITemplate Compile(IDocument doc)
        {
            if (doc.GetType() != typeof(ExcelXmlDocument))
            {
                throw new ArgumentException("只支持编译 Microsoft Excel 2003 XML 类型", "doc");
            }

            var t = new ExcelXmlTemplate();
            doc.CopyTo(t);

            var xml = t.ReadMainContentXml();
            var nsmanager = new ExcelXmlNamespaceManager(xml.NameTable);
            nsmanager.LoadOpenDocumentNamespaces();

            ClearTemplate(xml);

            ProcessPlaceHolders(xml);

            //把编译后的 XmlDocument 写入
            using (var cos = t.GetEntryOutputStream(t.MainContentEntryPath))
            using (var writer = new TemplateXmlTextWriter(cos))
            {
                writer.Formatting = Formatting.Indented; //对于 Velocity 模板，最好格式化
                xml.WriteTo(writer);
            }

            return t;
        }

        private static void ProcessPlaceHolders(XmlDocument xml)
        {
            var workbookNode = FindFirstChildNode(xml, "Workbook");

            if (workbookNode == null)
            {
                throw new TemplateException("无效的 Excel 2003 Xml 文件格式");
            }

            var placeholders = FindAllPlaceholders(workbookNode);

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
                    ProcessStatementTag(xml, phe, value);
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

        private static void ClearTemplate(XmlNode doc)
        {
            //把所有的行数和列数设置去掉, Excel 会自动计算的
            //ss:ExpandedColumnCount="5" ss:ExpandedRowCount="3"
            var nodes = doc.SelectNodes("//*");
            foreach (XmlElement e in nodes)
            {
                IXmlNodeVisitor visitor = null;
                if (visitors.TryGetValue(e.Name, out visitor))
                {
                    visitor.ProcessNode(e);
                }
            }
        }

        private static void ProcessReferenceTag(XmlElement phe, string value)
        {
            phe.RemoveAttribute(HRefAttribute);
            phe.InnerText = value;
        }

        private static void ProcessStatementTag(XmlDocument xml, XmlElement phe, string value)
        {
            var se = new StatementElement(xml, value, "null");
            if (phe.ParentNode.ChildNodes.Count == 1)
            {
                phe.ParentNode.ParentNode.ReplaceChild(se, phe.ParentNode);
            }
            else
            {
                phe.ParentNode.ReplaceChild(se, phe);
            }
        }

        private static List<XmlElement> FindAllPlaceholders(XmlNode doc)
        {
            var placeholders = new List<XmlElement>();
            var allNodes = doc.SelectNodes("//*");

            foreach (XmlElement e in allNodes)
            {
                if (e.Name == "Cell" && e.HasAttribute(HRefAttribute))
                {
                    var attr = e.GetAttribute(HRefAttribute);
                    if (attr.StartsWith("rtl://", StringComparison.InvariantCulture))
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
