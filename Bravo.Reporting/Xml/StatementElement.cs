//作者：李维
//创建时间：2010-08-20


using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Bravo.Reporting.Xml
{
    /// <summary>
    /// VTL Statement XML Element
    /// </summary>
    internal sealed class StatementElement : XmlElement
    {
        private string statement;
        public StatementElement(XmlDocument doc, string exp)
            : base(string.Empty, "report-statement", string.Empty, doc)
        {
            Debug.Assert(doc != null);
            Debug.Assert(exp != null);

            this.statement = exp;
        }

        /// <summary>
        /// Write to writer
        /// </summary>
        /// <param name="w"></param>
        public override void WriteTo(XmlWriter w)
        {
            Debug.Assert(statement != null);
            Debug.Assert(w != null);
            
            w.WriteRaw(this.statement);
        }
    }
}
