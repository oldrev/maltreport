using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace Bravo.Reporting
{
    internal class OpeningTag
    {
        public OpeningTag(XmlNode startNode, XmlNode endNode)
        {
            Debug.Assert(startNode != null);
            Debug.Assert(endNode != null);

            this.StartNode = startNode;
            this.EndNode = endNode;
        }

        public XmlNode StartNode { get; private set; }

        public XmlNode EndNode { get; private set; }
    }
}
