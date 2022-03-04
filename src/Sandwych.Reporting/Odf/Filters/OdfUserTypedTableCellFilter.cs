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

        private FluidValue CreateTableCellValue(string valueType, FluidValue input, Fluid.TemplateContext context) => valueType switch
        {
            OdfStringTableCellValue.OdfValueType => new OdfStringTableCellValue(input),
            OdfFloatTableCellValue.OdfValueType => new OdfFloatTableCellValue(input),
            OdfPercentageTableCellValue.OdfValueType => new OdfPercentageTableCellValue(input),
            OdfBooleanTableCellValue.OdfValueType => new OdfBooleanTableCellValue(input),
            OdfTimeTableCellValue.OdfValueType => input.TryGetDateTimeInput(context, out var dto) ? 
                                                    new OdfTimeTableCellValue(dto) : NilValue.Instance,
            OdfDateTableCellValue.OdfValueType => input.TryGetDateTimeInput(context, out var dto) ? 
                                                    new OdfDateTableCellValue(dto) : NilValue.Instance,
            _ => throw new NotSupportedException($"Not supported 'office:value-type': '{valueType}'"),
        };

    }
}
