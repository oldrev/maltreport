using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Bravo.Reporting.ReportNodes
{
    internal class PlaceHolderElement : XmlElement
    {
        public PlaceHolderElement(XmlDocument doc, string placeHolder)
            : base(string.Empty, "span", "text", doc)
        {
            this.InnerText = placeHolder;
        }

        public override void WriteTo(XmlWriter w)
        {
            w.WriteRaw("$" + this.InnerText + "$");
        }
    }
}
