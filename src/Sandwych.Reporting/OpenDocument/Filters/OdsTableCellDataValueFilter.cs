using System;
using System.Collections.Generic;
using System.Text;
using Fluid;
using Fluid.Values;
using Sandwych.Reporting.OpenDocument.Values;

namespace Sandwych.Reporting.OpenDocument.Filters
{
    public class OdsTableCellDataValueFilter : ISyncFilter
    {
        public string Name => "_rtl_ods_table_cell_datavalue";

        public FluidValue Execute(FluidValue input, FilterArguments arguments, Fluid.TemplateContext context)
        {
            //TODO check arguments
            var cellType = arguments.At(0);
            var formatType = arguments.At(1);


            return new OdsTableCellDataValue("123-123-123", null);
        }
    }
}
