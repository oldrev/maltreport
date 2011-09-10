using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;

using Malt.Reporting.Xml;

namespace Malt.Reporting.OfficeXml
{
	internal class ExcelMLlRowNodeProcessor : IXmlNodeProcessor
	{
        #region INodeVisitor 成员

		public void ProcessNode (XmlNode node)
		{
			Debug.Assert (node.NodeType == XmlNodeType.Element);
			Debug.Assert (node.Name == "Row");

			var row = (XmlElement)node;

			if (row.HasAttribute (ExcelMLDocument.IndexAttribute)) {
				row.RemoveAttribute (ExcelMLDocument.IndexAttribute);
			}
		}

        #endregion
	}
}
