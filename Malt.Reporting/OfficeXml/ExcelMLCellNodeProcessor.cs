using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;

using Malt.Reporting.Xml;

namespace Malt.Reporting.OfficeXml
{
    internal class ExcelMLCellNodeProcessor : IXmlNodeProcessor
    {
        #region INodeVisitor 成员

        public void ProcessNode(XmlNode node)
        {
            Debug.Assert(node.NodeType == XmlNodeType.Element);
            Debug.Assert(node.Name == "Cell");

            var cell = (XmlElement)node;

            if (cell.ChildNodes.Count == 1 &&
                cell.FirstChild.Name == "Data" &&
                cell.FirstChild.InnerText == "#VALUE!")
            {
                var data = (XmlElement)cell.FirstChild;
                data.SetAttribute(ExcelMLDocument.TypeAttribute, "String");
                data.InnerText = "";
            }

        }

        #endregion
    }
}
