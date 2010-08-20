using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Bravo.Reporting.ReportNodes
{
    internal class ForeachEndElement : XmlElement
    {
        public ForeachEndElement(XmlDocument doc)
            : base(string.Empty, "span", "text", doc)
        {
        }

        public override void WriteTo(XmlWriter w)
        {
            w.WriteRaw("}$");
        }
    }
}
