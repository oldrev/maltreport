using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Bravo.Reporting.ReportNodes
{
    internal class IdentifierElement : XmlElement
    {
        public IdentifierElement(XmlDocument doc, string identifier)
            : base(string.Empty, "identifier", "urn:bravo:reporting", doc)
        {
            this.InnerText = identifier;
        }

        public override void WriteTo(XmlWriter w)
        {
            w.WriteRaw(this.InnerText);
        }
    }
}
