using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;

using Sandwych.Reporting.Xml;

namespace Sandwych.Reporting.OfficeXml
{
	internal class ExcelMLlRowNodeProcessor : IXmlNodeProcessor
	{
        #region INodeVisitor 成员

		public void ProcessNode (XmlNode node)
		{
			Debug.Assert (node.NodeType == XmlNodeType.Element);
			Debug.Assert (node.Name == "Row");

			var row = (XmlElement)node;

			if (row.HasAttribute (ExcelMLTemplate.IndexAttribute)) {
				row.RemoveAttribute (ExcelMLTemplate.IndexAttribute);
			}
		}

        #endregion
	}
}
