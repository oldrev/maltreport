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
    internal sealed class DirectiveElement : XmlElement
    {
        private string directive;
        public DirectiveElement(XmlDocument doc, string directive)
            : base(string.Empty, "report-statement", string.Empty, doc)
        {
            Debug.Assert(doc != null);
            Debug.Assert(directive != null);

            //WORKAROUND:
            //由于 NVelocity 1.1 不支持 #{end} 形式的 Directive，因此遇到类似 #endWHATEVER_STRING
            //的 Directive 会造成解析错误，这里我们在后边加上一个 VTL 注释隔开 #end#**#WHATEVER_STRING 

            if (directive[directive.Length - 1] == ')')
            {
                this.directive = directive;
            }
            else
            {
                this.directive = directive + "#**#";
            }
        }

        /// <summary>
        /// Write to writer
        /// </summary>
        /// <param name="w"></param>
        public override void WriteTo(XmlWriter w)
        {
            Debug.Assert(directive != null);
            Debug.Assert(w != null);

            w.WriteRaw(this.directive);
        }
    }
}
