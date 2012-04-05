using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace Sandwych.Reporting.Xml
{
	internal class VelocityEscapedXmlTextWriter : XmlTextWriter
	{
		public VelocityEscapedXmlTextWriter (Stream inStream)
            : base(inStream, Encoding.UTF8)
		{
		}

		public override void WriteStartElement (string prefix, string localName, string ns)
		{
			base.WriteStartElement (prefix, localName, ns);
		}

		public override void WriteEndElement ()
		{
			base.WriteEndElement ();
		}

		public override void WriteString (string text)
		{
			if (text != null) {
				text = VelocityEscapeTool.EscapeDirective (text);
			}

			base.WriteString (text);
		}
	}
}
