using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;

namespace Bravo.Reporting.ReportNodes
{
    internal class ForeachStartElement : XmlElement
    {
        private static readonly Regex Pattern = 
            new Regex(@"^\s*foreach\s+(\w+)\s+in\s+([\w\.]+)\s*$");

        public ForeachStartElement(XmlDocument doc, string exp)
            : base(string.Empty, "span", "text", doc)
        {
            var exps = Pattern.Split(exp);

            //匹配不够两个就是语法错误
            if (exps.Length < 2)
            {
                throw new SyntaxErrorException();
            }

            //转换
            var stExp = string.Format("${0}: {{ {1}|",
                exps[2],
                exps[1]);

            this.InnerText = stExp;
        }

        public override void WriteTo(XmlWriter w)
        {
            w.WriteRaw(this.InnerText);
        }
    }
}
