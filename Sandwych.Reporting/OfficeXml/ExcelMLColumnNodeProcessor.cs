using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;

using Sandwych.Reporting.Xml;

namespace Sandwych.Reporting.OfficeXml
{
	internal class ExcelMLColumnNodeProcessor : IXmlNodeProcessor
	{
        #region INodeVisitor 成员

		public void ProcessNode (XmlNode node)
		{
			Debug.Assert (node.NodeType == XmlNodeType.Element);
			Debug.Assert (node.Name == "Column");

			var column = (XmlElement)node;

			if (column.HasAttribute (ExcelMLTemplate.IndexAttribute)) {
				column.RemoveAttribute (ExcelMLTemplate.IndexAttribute);
			}
		}

        #endregion
	}
}
