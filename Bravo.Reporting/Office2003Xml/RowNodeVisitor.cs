using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;

using Bravo.Reporting.Xml;

namespace Bravo.Reporting.Office2003Xml
{
    internal class RowNodeVisitor : IXmlNodeVisitor
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
