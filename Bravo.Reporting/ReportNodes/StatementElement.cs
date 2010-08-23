using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;

namespace Bravo.Reporting.ReportNodes
{
    internal class StatementElement : XmlElement
    {
        private static readonly List<string> OpeningOrClosingTags = new List<string>()
        {
            "#foreach",
            "#if",
            "#end",
        };

        public StatementElement(XmlDocument doc, string exp, string directive)
            : base(string.Empty, "statement", "urn:bravo:reporting", doc)
        {
            this.InnerText = exp;
            this.Directive = directive;
        }

        public override void WriteTo(XmlWriter w)
        {
            w.WriteRaw("\n" + this.InnerText + "\n");
        }

        public string Directive { get; private set; }

        public bool IsOpeningOrClosingTag
        {
            get
            {
                return OpeningOrClosingTags.Contains(this.Directive);
            }
        }
    }
}
