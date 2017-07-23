using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sandwych.Reporting.Textilize;

namespace Sandwych.Reporting.OpenDocument
{
    public class OdsTemplate : OdfTemplate
    {
        public OdsTemplate(Stream inStream) : base(inStream)
        {
        }

        public OdsTemplate(OdfDocument templateDocument) : base(templateDocument)
        {
        }

        protected override void SetInternalFilters(OdfDocument outputDocument, FluidTemplateContext templateContext)
        {
            base.SetInternalFilters(outputDocument, templateContext);

            //TODO Add internal filters for cells generation
        }

    }
}
