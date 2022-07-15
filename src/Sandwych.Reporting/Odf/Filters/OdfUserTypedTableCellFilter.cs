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
    public class OdfUserTypedTableCellFilter : IFluidFilter
    {
        public const string FilterName = "cell";

        public string Name => FilterName;

        public ValueTask<FluidValue> InvokeAsync(FluidValue input, FilterArguments arguments, Fluid.TemplateContext context)
        {
            var valueType = arguments.HasNamed("type") ? arguments["type"].ToStringValue().ToString() : "string";

            var cellValue = this.CreateTableCellValue(valueType, input, context);

            return new ValueTask<FluidValue>(cellValue);
        }

        private OdfTableCellValue CreateTableCellValue(string valueType, FluidValue input, Fluid.TemplateContext context) => valueType switch
        {
            OdfStringTableCellValue.OdfValueType => new OdfStringTableCellValue(input),
            OdfFloatTableCellValue.OdfValueType => new OdfFloatTableCellValue(input),
            OdfPercentageTableCellValue.OdfValueType => new OdfPercentageTableCellValue(input),
            OdfTimeTableCellValue.OdfValueType => new OdfTimeTableCellValue(input, context),
            _ => throw new NotSupportedException($"Not supported 'office:value-type': '{valueType}'"),
        };

    }
}
