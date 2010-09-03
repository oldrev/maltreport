using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace Bravo.Reporting.Excel2003Xml
{
    internal class RowNodeVisitor : INodeVisitor
    {
        #region INodeVisitor 成员

        public void ProcessNode(XmlNode node)
        {
            Debug.Assert(node.NodeType == XmlNodeType.Element);
            Debug.Assert(node.Name == "Row");

            var row = (XmlElement)node;

            if (row.HasAttribute(ExcelXmlDocument.IndexAttribute))
            {
                row.RemoveAttribute(ExcelXmlDocument.IndexAttribute);
            }
        }

        #endregion
    }
}
