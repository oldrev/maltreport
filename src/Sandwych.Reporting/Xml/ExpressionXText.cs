using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Sandwych.Reporting.Xml
{
    public class ExpressionXText : XText
    {
        public string Expression => this.Value;

        public ExpressionXText(string expression) : base(expression)
        {
        }

        public ExpressionXText(XText text) : base(text)
        {
        }

        public override void WriteTo(System.Xml.XmlWriter writer)
        {
            writer.WriteRaw(@"{{ ");
            writer.WriteRaw(this.Value);
            writer.WriteRaw(@" }}");
        }
    }
}
