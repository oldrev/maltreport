using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;

using Bravo.Reporting.Xml;

namespace Bravo.Reporting.Excel2003Xml
{
    internal class TableNodeVisitor : IXmlNodeVisitor
    {
        #region INodeVisitor 成员

        public void ProcessNode(XmlNode node)
        {
            Debug.Assert(node.NodeType == XmlNodeType.Element);
            Debug.Assert(node.Name == "Table");

            var table = (XmlElement)node;
            if (table.HasAttribute(ExcelXmlDocument.ExpandedRowCountAttribute))
            {
                table.RemoveAttribute(ExcelXmlDocument.ExpandedRowCountAttribute);
            }

            if (table.HasAttribute(ExcelXmlDocument.ExpandedColumnCountAttribute))
            {
                table.RemoveAttribute(ExcelXmlDocument.ExpandedColumnCountAttribute);
            }
        }

        #endregion
    }
}
