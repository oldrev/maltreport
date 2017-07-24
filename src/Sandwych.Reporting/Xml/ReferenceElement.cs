//Creation Time: 2010-08-20
using System.Diagnostics;
using System.Xml;

namespace Sandwych.Reporting.Xml
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

            this.expression = exp;
        }

        public override void WriteTo(XmlWriter w)
        {
            Debug.Assert(this.expression != null);
            Debug.Assert(w != null);

            w.WriteRaw("{{");
            w.WriteRaw(this.expression);
            w.WriteRaw("}}");
        }
    }
}