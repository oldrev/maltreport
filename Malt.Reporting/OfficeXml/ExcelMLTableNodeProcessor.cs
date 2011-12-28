using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;

using Malt.Reporting.Xml;

namespace Malt.Reporting.OfficeXml
{
	internal class ExcelMLTableNodeProcessor : IXmlNodeProcessor
	{
        #region INodeVisitor 成员

		public void ProcessNode (XmlNode node)
		{
			Debug.Assert (node.NodeType == XmlNodeType.Element);
			Debug.Assert (node.Name == "Table");

			var table = (XmlElement)node;
			if (table.HasAttribute (ExcelMLTemplate.ExpandedRowCountAttribute)) {
				table.RemoveAttribute (ExcelMLTemplate.ExpandedRowCountAttribute);
			}

			if (table.HasAttribute (ExcelMLTemplate.ExpandedColumnCountAttribute)) {
				table.RemoveAttribute (ExcelMLTemplate.ExpandedColumnCountAttribute);
			}
		}

        #endregion
	}
}
