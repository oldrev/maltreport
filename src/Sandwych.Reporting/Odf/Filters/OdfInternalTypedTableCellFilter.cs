using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Fluid;
using Fluid.Values;
using Sandwych.Reporting.Odf.Values;
using Sandwych.Reporting.Textilize;

namespace Sandwych.Reporting.Odf.Filters
{
    public class OdfInternalTypedTableCellFilter : IFluidFilter
    {
        public const string FilterName = "__ODF_Filters_TypedTableCell";

        public string Name => FilterName;

        public ValueTask<FluidValue> InvokeAsync(FluidValue input, FilterArguments arguments, Fluid.TemplateContext context)
        {
            var cellValue = input as OdfTableCellValue ?? new OdfStringTableCellValue(input);
            cellValue.AddAttribute("table:style-name", arguments.At(0).ToStringValue().ToString());

            return new ValueTask<FluidValue>(cellValue);
        }

    }
}
