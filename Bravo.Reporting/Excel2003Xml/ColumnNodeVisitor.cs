using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;

using Bravo.Reporting.Xml;

namespace Bravo.Reporting.Excel2003Xml
{
    internal class ColumnNodeVisitor : IXmlNodeVisitor
    {
        #region INodeVisitor 成员

        public void ProcessNode(XmlNode node)
        {
            Debug.Assert(node.NodeType == XmlNodeType.Element);
            Debug.Assert(node.Name == "Column");

            var column = (XmlElement)node ;

            if (column.HasAttribute(ExcelXmlDocument.IndexAttribute))
            {
                column.RemoveAttribute(ExcelXmlDocument.IndexAttribute);
            }
        }

        #endregion
    }
}
