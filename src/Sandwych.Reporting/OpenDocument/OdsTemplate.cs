using Sandwych.Reporting.Textilize;
using System.IO;

namespace Sandwych.Reporting.OpenDocument
{
    public class OdsTemplate : OdfTemplate
    {
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