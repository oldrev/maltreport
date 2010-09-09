//作者：李维
//创建时间：2010-08-20


using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace Bravo.Reporting.Xml
{
    internal sealed class ReferenceElement : XmlElement
    {
        public const string ElementName = "report-reference";
        private string expression = null;

        public ReferenceElement(XmlDocument doc, string exp)
            : base(string.Empty, ElementName, string.Empty, doc)
        {
            Debug.Assert(doc != null);
            Debug.Assert(exp != null);

            if (exp[1] == '{')
            {
                this.expression = exp;
            }
            else
            {
                this.expression = "${" + exp.Substring(1) + "}";
            }
        }

        public override void WriteTo(XmlWriter w)
        {
            Debug.Assert(this.expression != null);
            Debug.Assert(w != null);

            w.WriteRaw(this.expression);
        }
    }
}
