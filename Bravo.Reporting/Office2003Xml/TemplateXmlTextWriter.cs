using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace Bravo.Reporting.Office2003Xml
{
    internal class TemplateXmlTextWriter : XmlTextWriter
    {
        private string currentElementName;

        public TemplateXmlTextWriter(Stream inStream)
            : base(inStream, Encoding.UTF8)
        {
        }

        public override void WriteStartElement(string prefix, string localName, string ns)
        {
            currentElementName = localName;
            base.WriteStartElement(prefix, localName, ns);
        }

        /// <summary>
        /// 只允许 Cell 或 Data 中存在 Velocity 的 #directive
        /// </summary>
        /// <param name="text"></param>
        public override void WriteString(string text)
        {
            if (text != null &&
                this.currentElementName != "Cell" &&
                this.currentElementName != "Data")
            {
                text = VelocityEscapeTool.EscapeDirective(text);
            }

            base.WriteString(text);
        }
    }
}
