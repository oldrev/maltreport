using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Sandwych.Reporting.Textilize;
using Sandwych.Reporting.Odf.Filters;

namespace Sandwych.Reporting.Odf
{
    public class OdsTemplate : OdfTemplate
    {
        public OdsTemplate(OdfDocument templateDocument) : base(templateDocument)
        {
        }

        protected override IEnumerable<IFluidFilter> GetInternalFiltersToRegister(OdfDocument document)
        {
            return base.GetInternalFiltersToRegister(document)
                .Append(new OdfUserTypedTableCellFilter())
                .Append(new OdfInternalTypedTableCellFilter());
        }
    }
}
