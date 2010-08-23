using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;

namespace Bravo.Reporting.ReportNodes
{
    internal class StatementElement : XmlElement
    {
        public StatementElement(XmlDocument doc, string exp, string directive)
            : base(string.Empty, "StatementElement", "Bravo-Reporting", doc)
        {
            this.InnerText = exp;
            this.Directive = directive;
        }

        public override void WriteTo(XmlWriter w)
        {
            w.WriteRaw(this.InnerText + "\n");
        }

        public string Directive { get; private set; }
    }
}
