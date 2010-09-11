using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;

using Bravo.Reporting.Xml;

namespace Bravo.Reporting.Office2003Xml
{
    internal class ExcelMLTableNodeProcessor : IXmlNodeProcessor
    {
        #region INodeVisitor 成员

        public void ProcessNode(XmlNode node)
        {
            Debug.Assert(node.NodeType == XmlNodeType.Element);
            Debug.Assert(node.Name == "Table");

            var table = (XmlElement)node;
            if (table.HasAttribute(ExcelMLDocument.ExpandedRowCountAttribute))
            {
                table.RemoveAttribute(ExcelMLDocument.ExpandedRowCountAttribute);
            }

            if (table.HasAttribute(ExcelMLDocument.ExpandedColumnCountAttribute))
            {
                table.RemoveAttribute(ExcelMLDocument.ExpandedColumnCountAttribute);
            }
        }

        #endregion
    }
}
