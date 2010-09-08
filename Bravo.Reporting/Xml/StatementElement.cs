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

            //WORKAROUND:
            //由于 NVelocity 1.1 不支持 #{end} 形式的 Directive，因此遇到类似 #endWHATEVER_STRING
            //的 Directive 会造成解析错误，这里我们在后边加上一个 VTL 注释隔开 #end#**#WHATEVER_STRING 

            if (exp[exp.Length - 1] == ')')
            {
                this.statement = exp;
            }
            else
            {
                this.statement = exp + "#**#";
            }
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
