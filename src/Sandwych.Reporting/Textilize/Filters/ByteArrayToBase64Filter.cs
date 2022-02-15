using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Fluid;
using Fluid.Values;

namespace Sandwych.Reporting.Textilize.Filters
{
    public struct ByteArrayToBase64Filter : IFluidFilter
    {
        public string Name => "base64";

        public ValueTask<FluidValue> InvokeAsync(FluidValue input, FilterArguments arguments, Fluid.TemplateContext context)
        {
            var buf = input.ToObjectValue() as byte[];

            if (buf == null)
            {
                throw new NotSupportedException($"The type of input value must be 'byte[]'");
            }

            var base64Str = Convert.ToBase64String(buf);
            return new ValueTask<FluidValue>(new StringValue(base64Str));
        }

    }
}
