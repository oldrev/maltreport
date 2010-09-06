using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace Bravo.Reporting.Xml
{
    internal class VelocityEscapedXmlTextWriter : XmlTextWriter
    {
        public VelocityEscapedXmlTextWriter(Stream inStream)
            : base(inStream, Encoding.UTF8)
        {
        }

        /// <summary>
        /// 写入文本内容
        /// 我们不能容忍纯文本内容里包含 '#'
        /// </summary>
        /// <param name="text"></param>
        public override void WriteString(string text)
        {
            if (text != null)
            {
                text = VelocityEscapeTool.EscapeVelocity(text);
            }

            base.WriteString(text);
        }
    }
}
