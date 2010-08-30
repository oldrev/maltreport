//作者：李维
//创建时间：2010-08-20


using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace Bravo.Reporting.ReportNodes
{
    internal class IdentifierElement : XmlElement
    {

        private string expression = null;

        public IdentifierElement(XmlDocument doc, string exp)
            : base(string.Empty, "identifier", "urn:bravo:reporting", doc)
        {
            Debug.Assert(doc != null);
            Debug.Assert(exp != null);

            this.expression = exp;
        }

        public override void WriteTo(XmlWriter w)
        {
            Debug.Assert(this.expression != null);
            Debug.Assert(w != null);

            w.WriteRaw(this.expression);
        }
    }
}
