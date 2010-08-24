//作者：李维
//创建时间：2010-08-20


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
