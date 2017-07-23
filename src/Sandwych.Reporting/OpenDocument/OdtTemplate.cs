using System.IO;

namespace Sandwych.Reporting.OpenDocument
{
    public class OdtTemplate : OdfTemplate
    {
        public OdtTemplate(Stream inStream) : base(inStream)
        {
        }

        public OdtTemplate(OdfDocument templateDocument) : base(templateDocument)
        {
        }
    }
}