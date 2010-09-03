using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace Bravo.Reporting.Excel2003Xml
{
    internal class NumberFormatNodeVisitor : INodeVisitor
    {
        #region INodeVisitor 成员
        private const string Key = "esc";

        public void ProcessNode(XmlNode node)
        {
            Debug.Assert(node.NodeType == XmlNodeType.Element);
            Debug.Assert(node.Name == "NumberFormat");

            var nf = (XmlElement)node;
            var attr = nf.GetAttribute(ExcelXmlDocument.FormatAttribute);
            if (!string.IsNullOrEmpty(attr))
            {
                var escapedAttr = VelocityEscapeTool.EscapeVelocity(attr);

                nf.SetAttribute(ExcelXmlDocument.FormatAttribute, escapedAttr);
            }
        }

        #endregion
    }
}
