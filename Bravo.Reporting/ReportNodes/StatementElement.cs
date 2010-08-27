//作者：李维
//创建时间：2010-08-20


using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;

namespace Bravo.Reporting.ReportNodes
{
    /// <summary>
    /// VTL Statement XML Element
    /// </summary>
    internal class StatementElement : XmlElement
    {
        public StatementElement(XmlDocument doc, string exp, string directive)
            : base(string.Empty, "statement", "urn:bravo:reporting", doc)
        {
            this.InnerText = exp;
            this.Directive = directive;
        }

        /// <summary>
        /// Write to writer
        /// </summary>
        /// <param name="w"></param>
        public override void WriteTo(XmlWriter w)
        {
            w.WriteRaw(this.InnerText);
        }

        public string Directive { get; private set; }
    }
}
