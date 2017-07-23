using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
