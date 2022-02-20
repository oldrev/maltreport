using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Sandwych.Reporting.Xml
{
    public class RawXText : XText
    {
        public RawXText(string value) : base(value) { }

        public override void WriteTo(System.Xml.XmlWriter writer)
        {
            writer.WriteRaw(this.Value);
        }
    }
}
