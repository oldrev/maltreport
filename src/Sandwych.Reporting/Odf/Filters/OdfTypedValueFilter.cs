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
    public class OdfTypedValueFilter : IFluidFilter
    {
        public const string FilterName = "odf_type";

        public string Name => FilterName;

        public ValueTask<FluidValue> InvokeAsync(FluidValue input, FilterArguments arguments, Fluid.TemplateContext context)
        {
            var valueType = arguments.At(0).ToStringValue().ToString();

            return new ValueTask<FluidValue>(new OdfTypedValue(input, valueType));
        }

    }
}
